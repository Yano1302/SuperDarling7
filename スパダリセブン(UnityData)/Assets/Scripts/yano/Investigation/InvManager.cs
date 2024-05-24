using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Supadari;
using SceneManager = Supadari.SceneManager;


public enum InvType {
   A,
}

public class InvManager : MonoBehaviour
{
    /// <summary>探索パート～調査パート間のみ取得できるインスタンス</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "シーンが違うのでインスタンスを取得できません。"); return m_instance; } }
    /// <summary>この調査パートに配置されたアイテムの中で取得しているアイテム数</summary>
    public int GetItemNum { get { return m_getItemNum; } set { m_getItemNum = value; if (m_getItemNum >= MapManager.Instance.TotalItem){ ClearInv();} } }

    /// <summary>警戒度が上がるフラグを設定します。</summary>
    public bool VigilanceFlag { get { return m_vigilance.VigilanceFlag; } set { m_vigilance.VigilanceFlag = value; } }

    /// <summary>調査パートを開きます</summary>
    public void Open(InvType type) {
        Debug.Assert(!m_isOpen,"探索パートが既に開かれています");
        //シーンを開く
        m_currentInvType = type;  m_invObj[(int)m_currentInvType].SetActive(true);
        //ボタンをアクティブにする
        m_backBtn.SetActive(true);
        //座標を初期化する
        m_vigilance.mouseVec = Input.mousePosition;
        //フラグ設定
        m_gauge.enabled = true;
        m_gaugefill.enabled = true;
        m_isOpen = true; 
        m_vigilance.VigilanceFlag = true;
        Player.Instance.MoveFlag = false;
        //カーソルを変更
        SetCursor(false);
       
    }

    /// <summary>調査パートを閉じ、探索パートにもどります(ボタンにアタッチしてます)</summary>
    public void Close() {
        if (m_vigilance.VigilanceFlag) {
            //シーンを閉じる
            m_invObj[(int)m_currentInvType].SetActive(false);
            //ボタンを非表示に
            m_backBtn.SetActive(false);
            //フラグ設定
            m_gaugefill.enabled = false;
            m_isOpen = false;
            Player.Instance.MoveFlag = true;
            m_currentInvType = 0;
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

    public void ResetVigilance() { SetVigilance(0); }

    //  アタッチ用   //------------------------------------------------------------------------------
    #region Attach
    [SerializeField, Header("戻るボタン")]
    private GameObject m_backBtn;
    [SerializeField, Header("警戒度ゲージ")]
    private Image m_gauge;
    [SerializeField]
    private Image m_gaugefill;
    [SerializeField]
    private Image m_waves;
    [SerializeField]
    private float m_waveDirectionTime;
    [SerializeField]
    private float m_waveInterval;
    [SerializeField, Header("危険メッセージボックス")]
    private GameObject m_warningMessage;
    [SerializeField, Header("マウスカーソル画像1")]
    Texture2D m_cursor; 
    [SerializeField, Header("マウスカーソル画像1")]
    Texture2D m_cursorTaget;

    [SerializeField,Header("探索パート背景用ImageObjct"),EnumIndex(typeof(InvType))]
    private GameObject[] m_invObj;
    #endregion
    //-----------------------------------------------------------------------------------------------
    //自身のインスタンス
    private static InvManager m_instance;
    private InvManager() { }
    
    private AudioManager m_audioManager;

    private InvType  m_currentInvType;           //現在の調査パート状態
    private bool m_isOpen;                       //開いているかのフラグ
    private Vigilance m_vigilance;               //警戒度用構造体
    private int m_getItemNum;                    //現在取得しているアイテム数

    private struct Vigilance {
        public float MaxVigilance;               //最大警戒度
        public float Level { get { return m_VigilanceLevel; } set { if (VigilanceFlag) { m_VigilanceLevel = value;  }  } }
        public float Rate { get { return m_VigilanceLevel / MaxVigilance; } }
        public bool IsOver { get { return m_VigilanceLevel >= MaxVigilance; } }
        public Vector2 mouseVec;               //マウス座標
        public bool VigilanceFlag;             //このフラグがOnの時のみ警戒度を設定できます
        public float WaveInterval;             //鼓動の間隔
        private float m_VigilanceLevel;        //警戒度

    }
    
    private void Awake() {
        //最大値を設定する
        m_vigilance.MaxVigilance = 20;
        //警戒度を初期化する
        SetVigilance(0);
        m_isOpen = false;
        m_getItemNum = 0;
        m_vigilance.WaveInterval = m_waveInterval;
        m_instance = GetComponent<InvManager>();
        m_gauge.enabled = false;
        m_gaugefill.enabled = false;
    }
    private void Start() {
        m_audioManager = AudioManager.Instance;
    }

    private void Update() {
        
        if (m_vigilance.VigilanceFlag) {
            AddVigilance(Time.deltaTime);
            CheckMouseMove();
        }
    }
    /// <summary>マウスが動いたかどうかを調べます</summary>
    /// <returns>ゲームオーバーの場合にはfalseを返します</returns>
    private void CheckMouseMove() {
        Vector2 pos = Input.mousePosition;
        //マウスが動かされている場合
        if (m_vigilance.mouseVec != pos) {
            if (m_vigilance.IsOver) {
                OverVigilance();
            }
            else {
                m_vigilance.mouseVec = pos;
                AddVigilance(Time.deltaTime);
            }
        }
    }

    /// <summary>警戒度を追加します</summary>
    /// <returns>警戒度が最大値以上の場合にtrueを返します</returns>
    private bool AddVigilance(float addValue) {
        m_vigilance.Level += m_vigilance.IsOver? 0 : addValue;
        m_gaugefill.fillAmount = m_vigilance.Rate;
        //警戒度が更新された場合の処理
        if (m_vigilance.Rate > m_vigilance.WaveInterval) {
            StartCoroutine("Waves");
            m_vigilance.WaveInterval += m_waveInterval;
        }
        m_warningMessage.SetActive(m_vigilance.IsOver);
        return m_vigilance.IsOver;
    }

    /// <summary>警戒度を設定します</summary>
    /// <returns>警戒度が最大値以上の場合にtrueを返します</returns>
    private bool SetVigilance(float value) {
        m_vigilance.Level = value;
        m_gaugefill.fillAmount = m_vigilance.Rate;
        m_warningMessage.SetActive(m_vigilance.IsOver);
        return m_vigilance.IsOver;
    }


    /// <summary>全てのアイテムを取得した際の処理を記述します</summary>
    private void ClearInv() {
        m_vigilance.VigilanceFlag = false;
        StopCoroutine("Waves");
        SceneManager.Instance.SceneChange(SCENENAME.SolveScene, () => { UIManager.Instance.CloseUI(UIType.Timer); SetCursor(null); });
    }

    /// <summary>カーソルを設定します</summary>
    /// <param name="target"></param>
    private void SetCursor(bool? target) {
        Texture2D tex = target == null ? null : (bool)target ? m_cursorTaget : m_cursor;
        Vector2 vec = tex  == null ? Vector2.zero : new Vector2(tex.width/2,tex.height/2);
        Cursor.SetCursor(tex, vec, CursorMode.Auto);
    }

    /// <summary>
    /// 警戒度がマックスの際にマウスを動かしてしまった場合の処理
    /// </summary>
    private void OverVigilance() {
        if (m_vigilance.VigilanceFlag) {
            m_warningMessage.SetActive(false);
            StopCoroutine("Waves");
            SceneManager.Instance.SceneChange(SCENENAME.GameOverScene, () => { UIManager.Instance.CloseUI(UIType.Timer); SetCursor(null); });
        }
    }

    private IEnumerator Waves() {
        float time = 0;
        if (m_vigilance.IsOver) { m_audioManager.SE_Play("SE_survey01"); }
        else { m_audioManager.SE_Play("SE_survey02"); }

        WaitForSeconds wait = new WaitForSeconds(Time.deltaTime);
        while (time < 0.5) {
            float t = Time.deltaTime / m_waveDirectionTime;
            Color c = m_waves.color;
            c.a += t * 2;
            m_waves.color = c;
            time += t;
            yield return wait;
        }
        while(time < 1) {
            float t = Time.deltaTime / m_waveDirectionTime;
            Color c = m_waves.color;
            c.a -= t * 2;
            m_waves.color = c;
            time += t;
            yield return wait;
        }
        Color col = m_waves.color;
        col.a = 0;
        m_waves.color = col;
    }
}
