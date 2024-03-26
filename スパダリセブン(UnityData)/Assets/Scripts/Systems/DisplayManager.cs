using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;


//�t�F�[�h�C���E�A�E�g���̃t�F�[�h�^�C�v
public enum FadeType
{
    /// <summary>��ʑS�̂����X�ɈÂ�(���邭)�Ȃ�܂�</summary>
    Entire,
    /// <summary>�E���獶�Ɍ������ăt�F�[�h���܂�</summary>
    RightToLeft,
    /// <summary>������E�Ɍ������ăt�F�[�h���܂�</summary>
    LeftToRight,
    /// <summary>���v���ɉQ����Ƀt�F�[�h���܂�</summary>
    CW,
    /// <summary>�����v���ɉQ����Ƀt�F�[�h���܂�</summary>
    CCW,
}

/// <summary>
/// ��ʂ̈Ó]�A���]�Ȃǂ��Ǘ����܂�<br />
/// ��FadeImage�ɂ͐�p��Material���쐬���A�^�b�`���Ă����Ă�������<br />
/// (�f�t�H���g�̂܂܂��ƑS�Ă�Default UI Material���o�O��܂�)
/// </summary>
public class DisplayManager : SingletonMonoBehaviour<DisplayManager> {
  
    // �p�u���b�N�ϐ� //------------------------------------------------------------------------------------------------------------------

    /// <summary>�t�F�[�h�����܂ł̎���</summary>
    public float FadeTime { get { return m_fadeTime; } set { m_fadeTime = value > 0 ? value : 0; } }
    /// <summary>�t�F�[�h�C��(���])��̉�ʂ̖��邳(0�`1)</summary>
    public float MaxAlpha { get {return m_maxAlpha;}set {m_maxAlpha = Mathf.Clamp(value, 0.0f, 1.0f);Debug.Assert(m_maxAlpha > m_minAlpha, "MaxAlpha��MinAlpha�Ɠ������������l���������Ă��܂�\n���t�F�[�h�C��������ɋ@�\���Ȃ��\��������܂�"); }}
    /// <summary>�t�F�[�h�A�E�g(�Ó])��̉�ʂ̖��邳(0�`1)</summary>
    public float MinAlpha {get {return m_minAlpha;}set {m_minAlpha = Mathf.Clamp(value, 0.0f, 1.0f);Debug.Assert(m_minAlpha < m_maxAlpha, "MinAlpha��MaxAlpha�Ɠ������傫���l���������Ă��܂�\n���t�F�[�h�A�E�g������ɋ@�\���Ȃ��\��������܂�");}}
    /// <summary>���݂̉�ʂ̖��邳���擾�E�ύX���܂�</summary>
    public float CurrentAlpha {
        get {
            int index = (int)CurrentFadeType;
            if (m_currentFadeType == FadeType.Entire) {
                //shader�̕�Ԃ�Smoothstep�Ȃ̂Ō����ɂ͂�����ƈႤ
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
                    Log("��ʂ̖��邳���ύX����܂� : " + v,false);
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
                    Log("��ʂ̖��邳���ύX����܂� : " + ((v - start) / (end - start)),false);
                }
            }
            else {
                Log("��ʂ̖��邳���ύX����܂� : " + v,false);
                if(m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade) < 1) {
                    m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - v);
                }else {
                    m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - v);
                }

            }
        }
           
    }
    /// <summary>���݃t�F�[�h�֐������s���ł��邩�ǂ���(true : ���s���@false : ���s���Ă��Ȃ�)</summary>
    public bool IsFading { get { return m_IsFading; } private set { m_IsFading = value; } }
    /// <summary>���݂̃t�F�[�h�^�C�v</summary>
    public FadeType CurrentFadeType { get { return m_currentFadeType; } private set { m_currentFadeType = value; if (value != m_currentFadeType) Log("�t�F�[�h�^�C�v�� " + m_currentFadeType + " ���� " + value + " �ɕύX����܂�", false); } }
    /// <summary>�V�[����؂�ւ����ۂɎ����Ńt�F�[�h�C�����邩�ǂ���</summary>
    public bool AutoFading { get { return m_autoFading; } set { m_autoFading = value; } }


    // �p�u���b�N�֐��@//-----------------------------------------------------------------------------------------------------------------

    /// <summary>��ʂ����X�ɖ��邭���܂��B�Ăяo�����̃^�C���X�P�[���͖�������܂�</summary>
    /// <param name="type">�t�F�[�h�̃^�C�v���w�肵�܂�</param>
    /// <param name="action">�t�F�[�h��Ɏ��s�������֐�������΋L�ڂ��Ă�������</param>
    public void FadeIn(FadeType type, UnityAction action = null) {
        if (PrepareForFade(type, false)) {
            StartCoroutine(_FadeInSetting(type, action));
        }
    }

    /// <summary>��ʂ����X�ɈÂ����܂��B�Ăяo�����̃^�C���X�P�[���͖�������܂�</summary>
    /// <param name="type">�t�F�[�h�̃^�C�v���w�肵�܂�</param>
    /// <param name="action">�t�F�[�h��Ɏ��s�������֐�������΋L�ڂ��Ă�������</param>
    public void FadeOut(FadeType type, UnityAction action = null) {
        if (PrepareForFade(type, true)) {
            StartCoroutine(_FadeOutSetting(type, action));
        }   
    }


    //-------------------------------------------------------------------------------------------------------------------------------------



    #region �v���C�x�[�g�ϐ��E�֐�
    //�@�v���C�x�[�g�ϐ��E�֐� //----------------------------------------------------------------------------------------------------------

    //  �ϐ��ꗗ  //
    [Header("------�R���t�B�O ----------------------------------------------------------------------------------------------------------------")]
    [SerializeField,Header("�t�F�[�h�^�C�v")]
    private FadeType m_currentFadeType = FadeType.Entire;       //���݂̃t�F�[�h�^�C�v
    [SerializeField,Header("�V�[���؂�ւ����Ɏ����Ńt�F�[�h�C�����邩�ǂ���")]
    private bool m_autoFading = true;
    private bool m_IsFading = false;                             //���݃t�F�[�h���s���Ă��邩�ǂ���  
    [SerializeField,Header("�t�F�[�h����")]
    private  float m_fadeTime = 1.0f;                             //FadeTime�p
    [SerializeField,Header("�t�F�[�h�C����̉�ʂ̖��邳(0 ~ 1)")]
    private  float m_maxAlpha = 1.0f;                           //��ʂ̖��邳�̍ő�l  
    [SerializeField, Header("�t�F�[�h�A�E�g��̉�ʂ̖��邳(0 ~ 1)")]
    private float m_minAlpha = 0.0f;                           �@//��ʂ̖��邳�̍ŏ��l
    private  float m_timeScale =1.0f;                            //�ۑ��p�^�C���X�P�[��
    [SerializeField,Header("���O�̕\����L���ɂ��邩�ǂ���(�G���[���b�Z�[�W�͏���)")]
    private bool m_Log = true;

    [Header("------�V�F�[�_�[�ƃt�F�[�h�I�u�W�F�N�g-------------------------------------------------------------------------------------------")]
    [SerializeField, Header("�t�F�[�h�pUI�I�u�W�F�N�g")]
    private Image m_fadeImage;
    [SerializeField, Header("�t�F�[�h�p�V�F�[�_�["), EnumIndex(typeof(FadeType))]
    private Shader[] m_FadeShaders;

    private static PropertiesID[] m_pID;                            //�v���p�e�B��ID

    //�V�F�[�_�[ID
    private struct PropertiesID {
        public int m_ID_Fade;                                       //_Fade�pID
        public int m_ID_maxAlpha;                                   //_MaxAlpha�pID
        public int m_ID_minAlpha;                                   //_MinAlpha�pID
    }


    //  �֐��ꗗ   //----------------------------------------------------------------------------------------------------------------------

    //  �������֐�  //
    protected override void Awake() {
        base.Awake();
        if (m_pID == null) {
            Debug.Assert(m_fadeImage != null, "�t�F�[�h�I�u�W�F�N�g���A�^�b�`����Ă��܂���");
            Debug.Assert(m_FadeShaders.Length > 0 && m_FadeShaders.Length == UsefulSystem.GetEnumLength<FadeType>(), "�V�F�[�_�[���A�^�b�`����Ă��܂���");
            Debug.Assert(m_fadeImage.material.name != "Default UI Material","Display�I�u�W�F�N�g�ɐ�p�̃}�e���A����ݒ肵�Ă�������");
            m_pID = new PropertiesID[m_FadeShaders.Length];
            //�V�[���؂�ւ��̍ۂɃt���O���L���ł���΃t�F�[�h�C�����s��
            SceneManager.sceneLoaded += AutoFadeIn;
            //�eID���擾
            for (int i = 0; i < m_FadeShaders.Length; i++) {             
                m_pID[i].m_ID_Fade = Shader.PropertyToID("_Fade");
                m_pID[i].m_ID_maxAlpha = Shader.PropertyToID("_MaxAlpha");
                m_pID[i].m_ID_minAlpha = Shader.PropertyToID("_MinAlpha");
            }
            //�������l�ɂȂ��Ă��邩�m�F����
            m_maxAlpha = Mathf.Clamp(m_maxAlpha, 0.0f, 1.0f);
            m_minAlpha = Mathf.Clamp(m_minAlpha, 0.0f, 1.0f);
            Debug.Assert(m_maxAlpha > m_minAlpha,"MinAlpha��MaxAlpha�������Ă��܂��B�ݒ���m�F���Ă��������B");
            //�t�F�[�h�\�t���O�𗧂Ă�
            m_fadeImage.enabled = true;
            //�t�F�[�h�̏�����
            int index = (int)m_currentFadeType;
            m_fadeImage.material.shader = m_FadeShaders[index];
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade,1);
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha,1 - m_minAlpha);
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - m_maxAlpha);
        }
    }

    //  ���s�֐��@//

    /// <summary>�Ή�����V�F�[�_�[�ɐݒ肵�܂��B</summary>
    private void SetFadeShader(ref FadeType type,bool reverse = false) {
        //�t�F�[�h�̓x����ۑ����Ă���
        float t = m_fadeImage.material.GetFloat(m_pID[(int)CurrentFadeType].m_ID_Fade); 
        //�t�F�[�h�^�C�v���X�V
        CurrentFadeType = type;
        //�C���f�b�N�X���擾
        int index = (int)type;
        //�K�v�ȏꍇ�̓t�F�[�h�𔽓]������
        if (reverse) {
            switch (type) {
                case FadeType.RightToLeft: type = FadeType.LeftToRight; break;
                case FadeType.LeftToRight: type = FadeType.RightToLeft; break;
                case FadeType.CW: type = FadeType.CCW; break;
                case FadeType.CCW: type = FadeType.CW; break;
                    default: UsefulSystem.Log("�t�F�[�h�^�C�v : " + type); break;
            }
            index = (int)type;
        }
        //�V�F�[�_�[��ݒ�     
        m_fadeImage.material.shader = m_FadeShaders[(index)];
    }

    /// <summary>�t�F�[�h�̏������s���܂��B�t�F�[�h�\�ł����true��Ԃ��܂�</summary>
    private bool PrepareForFade(FadeType type,bool fadeOut) {
        if (m_IsFading) { Log("�t�F�[�h�̌Ăяo�����d�����Ă��܂��B�d�����Ă���t�F�[�h�̏����͍s���܂���",true); return false; }
        //�t�F�[�h���ɕύX
        IsFading = true;
        //�^�C���X�P�[����ۑ�����
        m_timeScale = Time.timeScale;
        //�t�F�[�hUI��O�ʂɈړ�
        m_fadeImage.transform.SetAsLastSibling();
        //�t�F�[�hUI�̃��C�L���X�g�ݒ�
        m_fadeImage.raycastTarget = fadeOut;
        //�}�e���A�����̐ݒ�
        SetFadeShader(ref type,fadeOut);
        //t�̒l�����r���[�ł͂܂����t�F�[�h�^�C�v�̏ꍇ��_Fade���P���O�ɂ���
        if(type != FadeType.Entire) {
            float t = fadeOut ? 0 : 1;
            m_fadeImage.material.SetFloat(m_pID[(int)type].m_ID_Fade,t);
        }
        return true;
    }

    /// <summary>��ʂ̃t�F�[�h�C���̎��ۂ̏������s���܂�</summary>
    private IEnumerator _FadeInSetting(FadeType type, UnityAction action) {
        Log("�t�F�[�h�C�����J�n���܂�", false);
        //�C���f�b�N�X���擾
        int index = (int)type;
        //���݂̕�ԏ����擾����
        float t = m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t < 1 ? 1 - t : 1;
        //���߂ƏI���̖��邳��ݒ肷��
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - CurrentAlpha);
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - m_maxAlpha);

        //���t���[���̕�Ԃ̎d����ݒ肷��
        UnityAction action1 = m_timeScale == 0 ?
            () => { t -= Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t -= Time.deltaTime / m_timeScale / FadeTime * ct; };
        
        //��Ԃ���
        while (t > 0) {
            action1();
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade,t);
            yield return null;
        }

        //�����𔽓]�����Ă���ꍇ�Ȃǂ͂����ɓ���
        if(m_currentFadeType != type) {
            SetFadeShader(ref m_currentFadeType);
            index = (int)m_currentFadeType;
        }
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, 0);
        IsFading = false;
        Log("�t�F�[�h���������܂����B", false);
        action?.Invoke();
    }

    /// <summary>��ʂ̃t�F�[�h�A�E�g�̎��ۂ̏������s���܂�</summary>
    private IEnumerator _FadeOutSetting(FadeType type, UnityAction action) {
        Log("�t�F�[�h�A�E�g���J�n���܂�", false);
        //�C���f�b�N�X���擾
        int index = (int)type;
        //���݂̕�ԏ����擾����
        float t = m_fadeImage.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t > 0 ? 1 - t : 1;
        //���߂ƏI���̖��邳��ݒ肷��
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_minAlpha, 1 - CurrentAlpha);
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1 - m_minAlpha);

        //���t���[���̕�Ԃ̎d����ݒ肷��
        UnityAction action1 = m_timeScale == 0 ?
            () => { t += Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t += Time.deltaTime / m_timeScale / FadeTime * ct; };

        //��Ԃ���
        while (t < 1) {
            action1();
            m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, t);
            yield return null;
        }

        //�����𔽓]�����Ă���ꍇ�Ȃǂ͂����ɓ���
        if (m_currentFadeType != type) {
            SetFadeShader(ref m_currentFadeType);
            index = (int)m_currentFadeType;
        }
        m_fadeImage.material.SetFloat(m_pID[index].m_ID_Fade, 1);
        IsFading = false;
        Log("�t�F�[�h���������܂����B", false);
        action?.Invoke();
    }

    /// <summary>�����t�F�[�h�C���̏������s���܂�</summary>
    private void AutoFadeIn(Scene scene, LoadSceneMode mode) {
        if (m_autoFading) {
            Log("AutoFading��true�ׁ̈A�����Ńt�F�[�h�C�����܂��B",false);
            FadeIn(CurrentFadeType);
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void Log(string message,bool warning) { if (!m_Log) return;  if (warning) Debug.LogWarning(message); else Debug.Log(message); }

    //-------------------------------------------------------------------------------------------------------------------------------------
    #endregion
}
