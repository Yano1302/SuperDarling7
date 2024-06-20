using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Supadari;
using SceneManager = Supadari.SceneManager;

/// <summary>
/// MapManagerがマップ生成のついでにステージ情報を全て取得する
///                     ↓
///  MapManagerがInvManagerを作成し、ステージ情報にある探索パート情報を渡す
///                     ↓
///  InvManagerが渡された情報を元にInvPartクラスを作成し、InvTypeを指定する。
///  この際に作成したInvPart情報を保管しておき、Open()関数でこの作成したInvPartを開けるようにする
///                     ↓
///  InvTypeは指定されたInvTypeに対応するテクスチャをInvTextureholderから取得すると同時に、
///  対応するCSVファイルを読み込み、アイテムオブジェクトを子として作成する
///                     ↓
///  この際にItemObjectにInvPart参照を渡し、ItemObjectからInvPartにアイテムが取得された事を報告する
///  
/// 
///   管理者  /管理オブジェクト
///     
/// InvManager/警戒度フラグ ->　InvPart毎にフラグを設定する訳ではないので外部からもフラグが設定しやすいInvManagerに設置
/// InvManager/マウスカーソル-> これも単一であるのが保証されているのでInvManagerに設置しItemObject等からも呼び出しやすいようにしている
///  
/// InvPart   / 警戒度ゲージ -> 調査パート毎に警戒度の設定が違うのでゲージクラスの管理はInvPartに委譲(SetUpInv()でゲージクラスを渡している)
/// InvPart   / 危険メッセージ-> 最初はInvManagerで保管するつもりだったが、ステージによって危険メッセージが出てからの猶予を変える可能性があるためInvPartに委譲
/// 
/// アイテムについて
/// 
///                     全て取得している場合は推理パートに移行する
///                                         ↑
///             InvManagerが全てのInvPartを確認し、アイテムを全て取得しているか確認する　→　取得していない場合は現在のInvPartを閉じるだけ
///                                         ↑
///             InvPartが自身のパートのアイテムが全て無くなった(取得済み)なのを判断し、対応するゴールオブジェクトを破棄したのちInvManagerに報告する
///                                         ↑
///             InvPartが残りのアイテム数を確認する(自身の子の残数がそのまま残り個数) <----------------------------------------------------------------|
///                                         ↑                                                                                                         |
///             ItemObjectが自身を取得された場合にItemManagerへアイテムを追加しつつ、InvPartに報告する                                                 |
///                                         ↑                                                                                                         |
///             InvPartが開かれたタイミングでItemObjectが自身のアイテムを既に取得しているかどうかItemManagerに確認しに行く　→　既に持っていた場合には重複を防ぐため破壊しInvPartに報告する
///                                         ↑
/// //探索パートを開くまでの流れ            ↑
///             ゴールオブジェクトに当たった際にInvManagerのOpenからゴールオブジェクトが持っているInvTypeの調査パートを呼び出す。      
///                                         ↑
///              MapManagerがマップ生成する際にゴールオブジェクトに対応するInvTypeを登録しておく。
///   
/// </summary>




public class InvManager : MonoBehaviour
{

    /// <summary>探索パート～調査パート間のみ取得できるインスタンス</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "シーンが違うのでインスタンスを取得できません。"); return m_instance; } }

    /// <summary>警戒度上昇フラグを設定します。</summary>
    public bool VigilanceFlag { get { return m_vigilanceFlag; } set { m_vigilanceFlag = value; } }

    ///
    public InvPart CurrentPart { get { return m_currentPart; } }

    /// <summary>MapManagerから調査パートのセットアップを行います</summary>
    public void SetUpInv(in InvType[] type,List<Goal> goalList) {
        if (m_invParts == null) {
            m_invParts = new InvPart[type.Length];
            for (int i = 0; i < type.Length; i++) {
                //InvPart用オブジェクトを作成し、このManagerクラスの子にする
                var obj = Instantiate(m_baseInvObject);
                obj.transform.SetParent(m_invCanvas.transform, false);
                //InvPartクラスをアタッチし、セットアップ関数を呼び出す
                m_invParts[i] = obj.AddComponent<InvPart>();
                m_invParts[i].SetUpInvPart(type[i], m_baseInvItem, m_gauge, m_warningMessage);
                //InvPartに対応するゴールを検索し格納する
                foreach(var g in goalList) {
                    if(g.InvType == m_invParts[i].InvPartType) {
                        m_invParts[i].SetGoal(g);
                    }
                }
            }
            //生成したアイテムで埋もれない様にUIを一番上に持ってくる
            m_backBtn.gameObject.transform.SetAsLastSibling();
            m_gauge.gameObject.transform.SetAsLastSibling();
            m_warningMessage.gameObject.transform.SetAsLastSibling();

        }
    }

    /// <summary>調査パートを開きます</summary>
    public void Open(InvType type) {
        //プレイヤーを行動不可にする
        Player.Instance.MoveFlag = false;
        //シーンを開く
        m_currentInvType = type;
        int index = (int)m_currentInvType;
        m_currentPart = m_invParts[index];
        m_invParts[index].gameObject.SetActive(true);
        m_invParts[index].Open();
        //ボタンをアクティブにする
        m_backBtn.SetActive(true);       
        //カーソルを変更
        SetCursor(false);
        VigilanceFlag = true;
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

    /// <summary>マウスアイコンを設定します</summary>
    ///  <param name="target">
    ///  targetがnullの場合 ;デフォルトのマウスカーソルになります          <br />
    ///  targetがtrueの場合 :カーソルがターゲットカーソルになります        <br />
    ///  targetがfalseの場合:カーソルが非ターゲットカーソルになります      </param>
    public void SetMouseIcon(bool? target) {
        SetCursor(target);
    }

    /// <summary>現在の調査パートの警戒度をリセット(0)します</summary>
    public void ResetVigilance() { m_invParts[(int)m_currentInvType].ResetVigilance(); }

    /// <summary>指定した調査パートの警戒度をリセット(0)します</summary>
    public void ResetVigilance(InvType type) { 
        foreach (var inv in m_invParts) {
            if (inv.InvPartType == type) {
                inv.ResetVigilance();
                return;
            }
        }
        UsefulSystem.LogWarning($"指定された調査パートが見つかりませんでした : { type }");
    }

    //全ての調査パートの警戒度をリセット(0)にします
    public void AllResetVigilance() { foreach (var inv in m_invParts)inv.ResetVigilance();}

    /// <summary>全てのInvPartがクリアされているか確認します。この関数はInvPartが一つクリアされる度呼ばれます。</summary>
    public void CheckClear() { 
        foreach(var part in m_invParts) {
            if(!part.ClearFlag) {
                //まだクリアしていないPartがあるので現在のPartを閉じるだけ
                Close();
                return;
            }
        }
        //ここに到達した場合にはクリアしている
        ClearInv();
    }

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
    
    private static InvManager m_instance;        //自身のインスタンス
    private InvManager() { }
    private bool m_vigilanceFlag;                //警戒度上昇フラグ
    private InvType m_currentInvType;            //現在の調査パート状態
    private InvPart[] m_invParts;                //探索パート配列
    private InvPart m_currentPart;               //現在のパート

    private void Awake() {
        m_instance = GetComponent<InvManager>();
        VigilanceFlag = false;
        m_gauge.CloseGauge();
        m_gauge.SetRate(0);
        m_currentInvType = InvType.None;
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
    private void SetCursor(bool? target) {
        Texture2D tex = target == null ? null : (bool)target ? m_cursorTaget : m_cursor;
        Vector2 vec = tex  == null ? Vector2.zero : new Vector2(tex.width/2,tex.height/2);
        Cursor.SetCursor(tex, vec, CursorMode.Auto);
    }   

}
