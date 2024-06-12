using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Supadari;
using SceneManager = Supadari.SceneManager;


public class InvManager : MonoBehaviour
{

    /// <summary>探索パート～調査パート間のみ取得できるインスタンス</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "シーンが違うのでインスタンスを取得できません。"); return m_instance; } }

   
    
    /// <summary>この調査パートに配置されたアイテムの中で取得しているアイテム数</summary>
    public int GetItemNum { get { return m_getItemNum; } set { m_getItemNum = value; if (m_getItemNum >= MapManager.Instance.TotalItem) { ClearInv(); } } }

    /// <summary>警戒度上昇フラグを設定します。</summary>
    public bool VigilanceFlag { get { return m_vigilanceFlag; } set { m_vigilanceFlag = value; } }

    /// <summary>調査パートを開きます</summary>
    public void Open(InvType type) {
        //プレイヤーを行動不可にする
        Player.Instance.MoveFlag = false;
        //シーンを開く
        m_currentInvType = type;
        int index = (int)m_currentInvType;
        m_invParts[index].gameObject.SetActive(true);
        m_invParts[index].Open();
        //ボタンをアクティブにする
        m_backBtn.SetActive(true);       
        //カーソルを変更
        SetCursor(false);
    }

    /// <summary>調査パートを閉じ、探索パートにもどります(ボタンにアタッチしてます)</summary>
    public void Close() {
        if (VigilanceFlag) {
            //シーンを閉じる
            int index = (int)m_currentInvType;
            m_invParts[index].Close();
            m_invParts[index].gameObject.SetActive(false);
            //ボタンを非表示に
            m_backBtn.SetActive(false);
            //フラグ設定
            VigilanceFlag = false; // 岬追記　警戒度はリセットしないようにしています
            var ins= Player.Instance;
            ins.MoveFlag = true;
            ins.VisibilityImage = true;
            m_currentInvType = InvType.None;
            //カーソルを元に戻す
            SetCursor(null);
        }
    }

    /// <summary>マウスアイコンを設定します<br />
    /// targetがnullの場合;デフォルトのマウスカーソルになります<br />
    /// targetがtrueの場合:カーソルがターゲットカーソルになります<br />
    /// targetがfalseの場合:カーソルが非ターゲットカーソルになります</summary>
    public void SetMouseIcon(bool? target) {
        SetCursor(target);
    }

    /// <summary>現在の調査パートの警戒度をリセット(0)します</summary>
    public void ResetVigilance() { m_invParts[(int)m_currentInvType].ResetVigilance(); }

    /// <summary>指定した調査パートの警戒度をリセット(0)します</summary>
    public void ResetVigilance(InvType type) { 
        foreach (var inv in m_invParts) {
            if (inv.MyIntType == type) {
                inv.ResetVigilance();
                return;
            }
        }
        UsefulSystem.LogWarning($"指定された調査パートが見つかりませんでした : { type }");
    }

    //全ての調査パートの警戒度をリセット(0)にします
    public void AllResetVigilance() { foreach (var inv in m_invParts) { inv.ResetVigilance(); } }

    //  アタッチ用   //------------------------------------------------------------------------------
    #region Attach
    [SerializeField, Header("戻るボタン")]
    private GameObject m_backBtn;
    [SerializeField, Header("警戒度ゲージ")]
    private InvGauge m_gauge;
    [SerializeField, Header("危険メッセージボックス")]
    private GameObject m_warningMessage;
    [SerializeField, Header("マウスカーソル画像1")]
    Texture2D m_cursor;
    [SerializeField, Header("マウスカーソル画像1")]
    Texture2D m_cursorTaget;
    [SerializeField, Header("探索パート用キャンバス")]
    private GameObject m_invCanvas;
    [SerializeField, Header("探索パート背景用ImageObjct")]
    private GameObject m_baseInvObject;
    [SerializeField, Header("探索パートのアイテムの当たり判定用オブジェクト")]
    private GameObject m_baseInvItem;
    #endregion



    //-----------------------------------------------------------------------------------------------
    //自身のインスタンス
    private static InvManager m_instance;
    private InvManager() { }
    private bool m_vigilanceFlag;                //警戒度上昇フラグ
    private InvType m_currentInvType;            //現在の調査パート状態
    private int m_getItemNum;                    //現在取得しているアイテム数
    private InvPart[] m_invParts;                //探索パート配列

    private void Awake() {
        m_instance = GetComponent<InvManager>();
        VigilanceFlag = false;
        m_gauge.CloseGauge();
        m_gauge.SetRate(0);
        m_currentInvType = InvType.None;
        m_getItemNum = 0;
    }

    /// <summary>MapManagerから調査パートのセットアップを行います</summary>
    public void SetUpInv(in InvType[] type) {
        if(m_invParts == null) {
            m_invParts = new InvPart[type.Length];
            for (int i = 0; i < type.Length; i++) {
                var obj = Instantiate(m_baseInvObject);
                obj.transform.SetParent(m_invCanvas.transform,false);
                m_invParts[i] = obj.AddComponent<InvPart>();
                m_invParts[i].SetUpInvPart(type[i],m_baseInvItem,m_gauge,m_warningMessage);
            }
            m_gauge.gameObject.transform.SetAsLastSibling();
        }
    }

   


    /// <summary>全てのアイテムを取得した際の処理を記述します</summary>
    private void ClearInv() {
        TimerManager timerManager=TimerManager.Instance;
        SceneManager sceneManager = SceneManager.Instance;
        JsonSettings<SettingsGetItemFlags> saveItemData = new JsonSettings<SettingsGetItemFlags>(string.Format("Data{0}", sceneManager.saveSlot), "JsonSaveFile", "ItemGetFlags");
        // 警戒ゲージと制限時間を止める　岬追記
        VigilanceFlag = false;
        timerManager.TimerFlag = false;
        m_gauge.StopWave();

        // アイテム所持フラグを保存し、シーン遷移
        saveItemData = ItemManager.Instance.UsingItemFlag;
        saveItemData.Save();
        SceneManager.Instance.SceneChange(SCENENAME.SolveScene, () => { UIManager.Instance.CloseUI(UIType.Timer); SetCursor(null); });
    }

    /// <summary>カーソルを設定します</summary>
    /// <param name="target"></param>
    private void SetCursor(bool? target) {
        Texture2D tex = target == null ? null : (bool)target ? m_cursorTaget : m_cursor;
        Vector2 vec = tex  == null ? Vector2.zero : new Vector2(tex.width/2,tex.height/2);
        Cursor.SetCursor(tex, vec, CursorMode.Auto);
    }   

}
