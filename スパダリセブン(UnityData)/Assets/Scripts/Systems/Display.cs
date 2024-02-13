using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//フェードイン・アウト時のフェードタイプ
public enum FadeType
{
    /// <summary>画面全体が徐々に暗く(明るく)なります</summary>
    Entire,
    /// <summary>右から左に向かってフェードします</summary>
    RightToLeft,
    /// <summary>左から右に向かってフェードします</summary>
    LeftToRight,
    /// <summary>時計回りに渦巻状にフェードします</summary>
    CW,
    /// <summary>反時計回りに渦巻状にフェードします</summary>
    CCW,
}

public class Display : SingletonMonoBehaviour<Display> {
    // アタッチ用変数　//-----------------------------------------------------------------------------------------------------------------
    [SerializeField, Header("フェード用UIオブジェクト")]
    private Image m_FadeObject;
    [SerializeField, Header("フェード用シェーダー"), EnumIndex(typeof(FadeType))]
    private Shader[] m_FadeShaders;
    // パブリック変数 //------------------------------------------------------------------------------------------------------------------

    /// <summary>フェード完了までの時間</summary>
    public float FadeTime { get { return m_fadeTime; } set { m_fadeTime = value > 0 ? value : 0; } }
    /// <summary>フェードイン(明転)後の画面の明るさ(0～1)</summary>
    public float MaxAlpha {
        get {
            return 1 - m_minAlpha;
        }
        set {
            Debug.Assert(value > m_minAlpha, "MaxAlphaにMinAlphaと同じか小さい値が代入されています\n※フェードインが正常に機能しない可能性があります");
            m_minAlpha = 1 - value;
        }
    }
    /// <summary>フェードアウト(暗転)後の画面の明るさ(0～1)</summary>
    public float MinAlpha {
        get {
            return 1 - m_maxAlpha;
        }
        set {
            Debug.Assert(value < m_maxAlpha, "MinAlphaにMaxAlphaと同じか大きい値が代入されています\n※フェードアウトが正常に機能しない可能性があります");
            m_maxAlpha =  1 - value;
        }
    }
    /// <summary>現在の画面の明るさを取得・変更します</summary>
    public float CurrentAlpha {
        get {
            int index = (int)CurrentFadeType;
            if (m_CurrentFadeType == FadeType.Entire) {
                
                return 1 - Mathf.SmoothStep(
                    m_FadeObject.material.GetFloat(m_pID[index].m_ID_minAlpha),
                    m_FadeObject.material.GetFloat(m_pID[index].m_ID_maxAlpha),
                    m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade));
            }
            else {
                if (m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade) == 1) {
                    return 1- m_FadeObject.material.GetFloat(m_pID[index].m_ID_maxAlpha);
                } else {
                    return 1- m_FadeObject.material.GetFloat(m_pID[index].m_ID_minAlpha);
                }
            }
         
        }
        set {
            int index = (int)CurrentFadeType;
            float v = value < 0 ? 1 : value > 1 ? 0 : 1 - value;
            if (m_CurrentFadeType == FadeType.Entire) {
               
                if (v < m_minAlpha) {
                    GameSystem.Log("MaxAlphaが変更されます : " + MaxAlpha);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha, v);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 0);
                }
                else if (v > m_maxAlpha) {
                    GameSystem.Log("MinAlphaが変更されます : " + MinAlpha);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha, v);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 1);
                }
                else {
                    float start = m_FadeObject.material.GetFloat(m_pID[index].m_ID_minAlpha);
                    float end = m_FadeObject.material.GetFloat(m_pID[index].m_ID_maxAlpha);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, (v - start) / (end - start));
                    Debug.Log("t : " + (1 - (v - start) / (end - start)));
                }
            }
            else {
                if(m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade) < 1) {
                   // m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade,0);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha,v);
                }else {
                  //  m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 1);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha, v);
                }

            }
        }
           
    }
    /// <summary>現在フェード関数が実行中であるかどうか(true : 実行中　false : 実行していない)</summary>
    public bool IsFading { get { return m_IsFading; } private set { m_IsFading = value; } }
    /// <summary>現在のフェードタイプ</summary>
    public FadeType CurrentFadeType { get {return m_CurrentFadeType; } private set { m_CurrentFadeType = value; } }


    // パブリック関数　//-----------------------------------------------------------------------------------------------------------------

    /// <summary>画面を徐々に明るくします。呼び出し時のタイムスケールは無視されます</summary>
    /// <param name="type">フェードのタイプを指定します</param>
    /// <param name="action">フェード後に実行したい関数があれば記載してください</param>
    public void FadeIn(FadeType type, UnityAction action = null) {
        if (PrepareForFade(type, false)) {
            StartCoroutine(_FadeInSetting(type, action));
        }
    }

    /// <summary>画面を徐々に暗くします。呼び出し時のタイムスケールは無視されます</summary>
    /// <param name="type">フェードのタイプを指定します</param>
    /// <param name="action">フェード後に実行したい関数があれば記載してください</param>
    public void FadeOut(FadeType type, UnityAction action = null) {
        if (PrepareForFade(type, true)) {
            StartCoroutine(_FadeOutSetting(type, action));
        }   
    }


    //-------------------------------------------------------------------------------------------------------------------------------------



    #region プライベート変数・関数
    //　プライベート変数・関数 //----------------------------------------------------------------------------------------------------------

    //  変数一覧  //
    private FadeType m_CurrentFadeType = FadeType.Entire;     //現在のフェードタイプ
    private bool m_IsFading = false;                             //現在フェードを行っているかどうか

    private  float m_fadeTime = 1.0f;                             //FadeTime用
    private  float m_minAlpha = 0.0f;                             //フェードオブジェクトの最小α値
    private  float m_maxAlpha = 1.0f;                             //フェードオブジェクトの最大α値  
    private  float m_timeScale =1.0f;                            //保存用タイムスケール

    private static PropertiesID[] m_pID;                         //プロパティのID

    //シェーダーID
    private struct PropertiesID {
        public int m_ID_Fade;                                       //_Fade用ID
        public int m_ID_maxAlpha;                                   //_MaxAlpha用ID
        public int m_ID_minAlpha;                                   //_MinAlpha用ID
    }


    //  関数一覧   //----------------------------------------------------------------------------------------------------------------------

    //  初期化関数  //
    protected override void Awake() {
        base.Awake();
        if (m_pID == null) {          
            Debug.Assert(m_FadeObject != null, "フェードオブジェクトがアタッチされていません");
            Debug.Assert(m_FadeShaders.Length > 0 && m_FadeShaders.Length == GameSystem.GetEnumLength<FadeType>(), "シェーダーがアタッチされていません");
            m_pID = new PropertiesID[m_FadeShaders.Length];
            //各IDを取得
            for (int i = 0; i < m_FadeShaders.Length; i++) {             
                m_pID[i].m_ID_Fade = Shader.PropertyToID("_Fade");
                m_pID[i].m_ID_maxAlpha = Shader.PropertyToID("_MaxAlpha");
                m_pID[i].m_ID_minAlpha = Shader.PropertyToID("_MinAlpha");
            }
            //フェード可能フラグを立てる
            m_FadeObject.enabled = true;
            //フェードの初期化
            int index = (int)FadeType.Entire;
            m_FadeObject.material.shader = m_FadeShaders[index];
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade,1);
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha,m_maxAlpha);
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha,m_minAlpha);
        }
    }

    //  実行関数　//

    /// <summary>対応するシェーダーに設定します。</summary>
    private void SetFadeShader(ref FadeType type,bool reverse = false) {
        //フェードの度合を保存しておく
        float t = m_FadeObject.material.GetFloat(m_pID[(int)CurrentFadeType].m_ID_Fade); 
        //フェードタイプを更新
        CurrentFadeType = type;
        //インデックスを取得
        int index = (int)type;
        //必要な場合はフェードを反転させる
        if (reverse) {
            switch (type) {
                case FadeType.RightToLeft: type = FadeType.LeftToRight; break;
                case FadeType.LeftToRight: type = FadeType.RightToLeft; break;
                case FadeType.CW: type = FadeType.CCW; break;
                case FadeType.CCW: type = FadeType.CW; break;
                    default: GameSystem.Log("フェードタイプ : " + type); break;
            }
            index = (int)type;
        }
        //シェーダーを設定     
        m_FadeObject.material.shader = m_FadeShaders[(index)];
    }

    /// <summary>フェードの準備を行います。フェード可能であればtrueを返します</summary>
    private bool PrepareForFade(FadeType type,bool fadeOut) {
        if (m_IsFading) { GameSystem.LogError("フェードの呼び出しが重複しています。重複しているフェードの処理は行われません"); return false; }
        //フェード中に変更
        IsFading = true;
        //タイムスケールを保存する
        m_timeScale = Time.timeScale;
        //UIを前面に移動
        m_FadeObject.transform.SetAsLastSibling();

        //マテリアル等の設定
        SetFadeShader(ref type,fadeOut);
        //tの値が中途半端ではまずいフェードタイプの場合は_Fadeを１か０にする
        if(type != FadeType.Entire) {
            float t = fadeOut ? 0 : 1;
            m_FadeObject.material.SetFloat(m_pID[(int)type].m_ID_Fade,t);
        }
        return true;
    }

    /// <summary>画面のフェードインの実際の処理を行います</summary>
    private IEnumerator _FadeInSetting(FadeType type, UnityAction action) {
        //インデックスを取得
        int index = (int)type;
        //現在の補間情報を取得する
        float t = m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t < 1 ? 1 - t : 1;
        //初めと終わりの明るさを設定する
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1-CurrentAlpha);
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha, m_minAlpha);

        //毎フレームの補間の仕方を設定する
        UnityAction action1 = m_timeScale == 0 ?
            () => { t -= Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t -= Time.deltaTime / m_timeScale / FadeTime * ct; };
        
        //補間する
        float startTime = Time.time;
        while (t > 0) {
            action1();
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade,t);
            yield return null;
        }

        //処理を反転させている場合などはここに入る
        if(m_CurrentFadeType != type) {
            SetFadeShader(ref m_CurrentFadeType);
            index = (int)m_CurrentFadeType;
        }
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 0);
        IsFading = false;
        action?.Invoke();
    }

    /// <summary>画面のフェードアウトの実際の処理を行います</summary>
    private IEnumerator _FadeOutSetting(FadeType type, UnityAction action) {

        //インデックスを取得
        int index = (int)type;
        //現在の補間情報を取得する
        float t = m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t > 0 ? 1 - t : 1;
        //初めと終わりの明るさを設定する
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha, 1-CurrentAlpha);
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha, m_maxAlpha);

        //毎フレームの補間の仕方を設定する
        UnityAction action1 = m_timeScale == 0 ?
            () => { t += Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t += Time.deltaTime / m_timeScale / FadeTime * ct; };

        //補間する
        float startTime = Time.time;
        while (t < 1) {
            action1();
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, t);
            yield return null;
        }

        //処理を反転させている場合などはここに入る
        if (m_CurrentFadeType != type) {
            SetFadeShader(ref m_CurrentFadeType);
            index = (int)m_CurrentFadeType;
        }
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 1);
        IsFading = false;
        action?.Invoke();
    }

    //-------------------------------------------------------------------------------------------------------------------------------------
    #endregion
}
