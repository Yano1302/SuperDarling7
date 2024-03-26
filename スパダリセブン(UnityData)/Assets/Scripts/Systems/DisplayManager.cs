using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;


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

/// <summary>
/// 画面の暗転、明転などを管理します<br />
/// ※FadeImageには専用のMaterialを作成しアタッチしておいてください<br />
/// (デフォルトのままだと全てのDefault UI Materialがバグります)
/// </summary>
public class DisplayManager : SingletonMonoBehaviour<DisplayManager> {
  
    // パブリック変数 //------------------------------------------------------------------------------------------------------------------

    /// <summary>フェード完了までの時間</summary>
    public float FadeTime { get { return m_fadeTime; } set { m_fadeTime = value > 0 ? value : 0; } }
    /// <summary>フェードイン(明転)後の画面の明るさ(0～1)</summary>
    public float MaxAlpha { get {return m_maxAlpha;}set {m_maxAlpha = Mathf.Clamp(value, 0.0f, 1.0f);Debug.Assert(m_maxAlpha > m_minAlpha, "MaxAlphaにMinAlphaと同じか小さい値が代入されています\n※フェードインが正常に機能しない可能性があります"); }}
    /// <summary>フェードアウト(暗転)後の画面の明るさ(0～1)</summary>
    public float MinAlpha {get {return m_minAlpha;}set {m_minAlpha = Mathf.Clamp(value, 0.0f, 1.0f);Debug.Assert(m_minAlpha < m_maxAlpha, "MinAlphaにMaxAlphaと同じか大きい値が代入されています\n※フェードアウトが正常に機能しない可能性があります");}}
    /// <summary>現在の画面の明るさを取得・変更します</summary>
    public float CurrentAlpha {
        get {
            int index = (int)CurrentFadeType;
            if (m_currentFadeType == FadeType.Entire) {
                //shaderの補間はSmoothstepなので厳密にはちょっと違う
                return 1 - Mathf.Lerp(
                    m_fadeImage.material.GetFloat(m_pID[index].m_ID_minAlpha),
                    m_fadeImage.material.GetFloat(m_pID[index].m_ID_maxAlpha),
                    m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade));
            }
            else {
                if (m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade) == 1) {
                    return 1- m_fadeImage.material.GetFloat(m_pID[index].m_ID_maxAlpha);
                } else {
                    return 1- m_fadeImage.material.GetFloat(m_pID[index].m_ID_minAlpha);
                }
            }
         
        }
        set {
            int index = (int)CurrentFadeType;
            float v = Mathf.Clamp(value, 0, 1);
            if (m_currentFadeType == FadeType.Entire) 
            {
                if (v > m_maxAlpha || v < m_minAlpha) {
                    Log("画面の明るさが変更されます : " + v,false);
                    float t = m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade);
                    if(t <= 1) {
                        m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - v);
                    }
                    else {
                        m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - v);
                        m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, 0);
                    }                  
                }
                else {
                    float start = m_fadeImage.material.GetFloat(m_pID[index].m_ID_minAlpha);
                    float end = m_fadeImage.material.GetFloat(m_pID[index].m_ID_maxAlpha);
                    m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, 1 - (v - start) / (end - start));
                    Log("画面の明るさが変更されます : " + ((v - start) / (end - start)),false);
                }
            }
            else {
                Log("画面の明るさが変更されます : " + v,false);
                if(m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade) < 1) {
                    m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - v);
                }else {
                    m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - v);
                }

            }
        }
           
    }
    /// <summary>現在フェード関数が実行中であるかどうか(true : 実行中　false : 実行していない)</summary>
    public bool IsFading { get { return m_IsFading; } private set { m_IsFading = value; } }
    /// <summary>現在のフェードタイプ</summary>
    public FadeType CurrentFadeType { get { return m_currentFadeType; } private set { m_currentFadeType = value; if (value != m_currentFadeType) Log("フェードタイプが " + m_currentFadeType + " から " + value + " に変更されます", false); } }
    /// <summary>シーンを切り替えた際に自動でフェードインするかどうか</summary>
    public bool AutoFading { get { return m_autoFading; } set { m_autoFading = value; } }


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
    [Header("------コンフィグ ----------------------------------------------------------------------------------------------------------------")]
    [SerializeField,Header("フェードタイプ")]
    private FadeType m_currentFadeType = FadeType.Entire;       //現在のフェードタイプ
    [SerializeField,Header("シーン切り替え時に自動でフェードインするかどうか")]
    private bool m_autoFading = true;
    private bool m_IsFading = false;                             //現在フェードを行っているかどうか  
    [SerializeField,Header("フェード時間")]
    private  float m_fadeTime = 1.0f;                             //FadeTime用
    [SerializeField,Header("フェードイン後の画面の明るさ(0 ~ 1)")]
    private  float m_maxAlpha = 1.0f;                           //画面の明るさの最大値  
    [SerializeField, Header("フェードアウト後の画面の明るさ(0 ~ 1)")]
    private float m_minAlpha = 0.0f;                           　//画面の明るさの最小値
    private  float m_timeScale =1.0f;                            //保存用タイムスケール
    [SerializeField,Header("ログの表示を有効にするかどうか(エラーメッセージは除く)")]
    private bool m_Log = true;

    [Header("------シェーダーとフェードオブジェクト-------------------------------------------------------------------------------------------")]
    [SerializeField, Header("フェード用UIオブジェクト")]
    private Image m_fadeImage;
    [SerializeField, Header("フェード用シェーダー"), EnumIndex(typeof(FadeType))]
    private Shader[] m_FadeShaders;

    private static PropertiesID[] m_pID;                            //プロパティのID

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
            Debug.Assert(m_fadeImage != null, "フェードオブジェクトがアタッチされていません");
            Debug.Assert(m_FadeShaders.Length > 0 && m_FadeShaders.Length == UsefulSystem.GetEnumLength<FadeType>(), "シェーダーがアタッチされていません");
            Debug.Assert(m_fadeImage.material.name != "Default UI Material","Displayオブジェクトに専用のマテリアルを設定してください");
            m_pID = new PropertiesID[m_FadeShaders.Length];
            //シーン切り替えの際にフラグが有効であればフェードインを行う
            SceneManager.sceneLoaded += AutoFadeIn;
            //各IDを取得
            for (int i = 0; i < m_FadeShaders.Length; i++) {             
                m_pID[i].m_ID_Fade = Shader.PropertyToID("_Fade");
                m_pID[i].m_ID_maxAlpha = Shader.PropertyToID("_MaxAlpha");
                m_pID[i].m_ID_minAlpha = Shader.PropertyToID("_MinAlpha");
            }
            //正しい値になっているか確認する
            m_maxAlpha = Mathf.Clamp(m_maxAlpha, 0.0f, 1.0f);
            m_minAlpha = Mathf.Clamp(m_minAlpha, 0.0f, 1.0f);
            Debug.Assert(m_maxAlpha > m_minAlpha,"MinAlphaがMaxAlphaを上回っています。設定を確認してください。");
            //フェード可能フラグを立てる
            m_fadeImage.enabled = true;
            //フェードの初期化
            int index = (int)m_currentFadeType;
            m_fadeImage.material.shader = m_FadeShaders[index];
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade,1);
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha,1 - m_minAlpha);
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - m_maxAlpha);
        }
    }

    //  実行関数　//

    /// <summary>対応するシェーダーに設定します。</summary>
    private void SetFadeShader(ref FadeType type,bool reverse = false) {
        //フェードの度合を保存しておく
        float t = m_fadeImage.material.GetFloat(m_pID[(int)CurrentFadeType].m_ID_Fade); 
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
                    default: UsefulSystem.Log("フェードタイプ : " + type); break;
            }
            index = (int)type;
        }
        //シェーダーを設定     
        m_fadeImage.material.shader = m_FadeShaders[(index)];
    }

    /// <summary>フェードの準備を行います。フェード可能であればtrueを返します</summary>
    private bool PrepareForFade(FadeType type,bool fadeOut) {
        if (m_IsFading) { Log("フェードの呼び出しが重複しています。重複しているフェードの処理は行われません",true); return false; }
        //フェード中に変更
        IsFading = true;
        //タイムスケールを保存する
        m_timeScale = Time.timeScale;
        //フェードUIを前面に移動
        m_fadeImage.transform.SetAsLastSibling();
        //フェードUIのレイキャスト設定
        m_fadeImage.raycastTarget = fadeOut;
        //マテリアル等の設定
        SetFadeShader(ref type,fadeOut);
        //tの値が中途半端ではまずいフェードタイプの場合は_Fadeを１か０にする
        if(type != FadeType.Entire) {
            float t = fadeOut ? 0 : 1;
            m_fadeImage.material.SetFloat(m_pID[(int)type].m_ID_Fade,t);
        }
        return true;
    }

    /// <summary>画面のフェードインの実際の処理を行います</summary>
    private IEnumerator _FadeInSetting(FadeType type, UnityAction action) {
        Log("フェードインを開始します", false);
        //インデックスを取得
        int index = (int)type;
        //現在の補間情報を取得する
        float t = m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t < 1 ? 1 - t : 1;
        //初めと終わりの明るさを設定する
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - CurrentAlpha);
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - m_maxAlpha);

        //毎フレームの補間の仕方を設定する
        UnityAction action1 = m_timeScale == 0 ?
            () => { t -= Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t -= Time.deltaTime / m_timeScale / FadeTime * ct; };
        
        //補間する
        while (t > 0) {
            action1();
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade,t);
            yield return null;
        }

        //処理を反転させている場合などはここに入る
        if(m_currentFadeType != type) {
            SetFadeShader(ref m_currentFadeType);
            index = (int)m_currentFadeType;
        }
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, 0);
        IsFading = false;
        Log("フェードが完了しました。", false);
        action?.Invoke();
    }

    /// <summary>画面のフェードアウトの実際の処理を行います</summary>
    private IEnumerator _FadeOutSetting(FadeType type, UnityAction action) {
        Log("フェードアウトを開始します", false);
        //インデックスを取得
        int index = (int)type;
        //現在の補間情報を取得する
        float t = m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t > 0 ? 1 - t : 1;
        //初めと終わりの明るさを設定する
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - CurrentAlpha);
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - m_minAlpha);

        //毎フレームの補間の仕方を設定する
        UnityAction action1 = m_timeScale == 0 ?
            () => { t += Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t += Time.deltaTime / m_timeScale / FadeTime * ct; };

        //補間する
        while (t < 1) {
            action1();
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, t);
            yield return null;
        }

        //処理を反転させている場合などはここに入る
        if (m_currentFadeType != type) {
            SetFadeShader(ref m_currentFadeType);
            index = (int)m_currentFadeType;
        }
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, 1);
        IsFading = false;
        Log("フェードが完了しました。", false);
        action?.Invoke();
    }

    /// <summary>自動フェードインの処理を行います</summary>
    private void AutoFadeIn(Scene scene, LoadSceneMode mode) {
        if (m_autoFading) {
            Log("AutoFadingがtrueの為、自動でフェードインします。",false);
            FadeIn(CurrentFadeType);
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void Log(string message,bool warning) { if (!m_Log) return;  if (warning) Debug.LogWarning(message); else Debug.Log(message); }

    //-------------------------------------------------------------------------------------------------------------------------------------
    #endregion
}
