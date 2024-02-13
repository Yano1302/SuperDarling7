using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


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

public class Display : SingletonMonoBehaviour<Display> {
    // �A�^�b�`�p�ϐ��@//-----------------------------------------------------------------------------------------------------------------
    [SerializeField, Header("�t�F�[�h�pUI�I�u�W�F�N�g")]
    private Image m_FadeObject;
    [SerializeField, Header("�t�F�[�h�p�V�F�[�_�["), EnumIndex(typeof(FadeType))]
    private Shader[] m_FadeShaders;
    // �p�u���b�N�ϐ� //------------------------------------------------------------------------------------------------------------------

    /// <summary>�t�F�[�h�����܂ł̎���</summary>
    public float FadeTime { get { return m_fadeTime; } set { m_fadeTime = value > 0 ? value : 0; } }
    /// <summary>�t�F�[�h�C��(���])��̉�ʂ̖��邳(0�`1)</summary>
    public float MaxAlpha {
        get {
            return 1 - m_minAlpha;
        }
        set {
            Debug.Assert(value > m_minAlpha, "MaxAlpha��MinAlpha�Ɠ������������l���������Ă��܂�\n���t�F�[�h�C��������ɋ@�\���Ȃ��\��������܂�");
            m_minAlpha = 1 - value;
        }
    }
    /// <summary>�t�F�[�h�A�E�g(�Ó])��̉�ʂ̖��邳(0�`1)</summary>
    public float MinAlpha {
        get {
            return 1 - m_maxAlpha;
        }
        set {
            Debug.Assert(value < m_maxAlpha, "MinAlpha��MaxAlpha�Ɠ������傫���l���������Ă��܂�\n���t�F�[�h�A�E�g������ɋ@�\���Ȃ��\��������܂�");
            m_maxAlpha =  1 - value;
        }
    }
    /// <summary>���݂̉�ʂ̖��邳���擾�E�ύX���܂�</summary>
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
                    GameSystem.Log("MaxAlpha���ύX����܂� : " + MaxAlpha);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha, v);
                    m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 0);
                }
                else if (v > m_maxAlpha) {
                    GameSystem.Log("MinAlpha���ύX����܂� : " + MinAlpha);
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
    /// <summary>���݃t�F�[�h�֐������s���ł��邩�ǂ���(true : ���s���@false : ���s���Ă��Ȃ�)</summary>
    public bool IsFading { get { return m_IsFading; } private set { m_IsFading = value; } }
    /// <summary>���݂̃t�F�[�h�^�C�v</summary>
    public FadeType CurrentFadeType { get {return m_CurrentFadeType; } private set { m_CurrentFadeType = value; } }


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
    private FadeType m_CurrentFadeType = FadeType.Entire;     //���݂̃t�F�[�h�^�C�v
    private bool m_IsFading = false;                             //���݃t�F�[�h���s���Ă��邩�ǂ���

    private  float m_fadeTime = 1.0f;                             //FadeTime�p
    private  float m_minAlpha = 0.0f;                             //�t�F�[�h�I�u�W�F�N�g�̍ŏ����l
    private  float m_maxAlpha = 1.0f;                             //�t�F�[�h�I�u�W�F�N�g�̍ő僿�l  
    private  float m_timeScale =1.0f;                            //�ۑ��p�^�C���X�P�[��

    private static PropertiesID[] m_pID;                         //�v���p�e�B��ID

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
            Debug.Assert(m_FadeObject != null, "�t�F�[�h�I�u�W�F�N�g���A�^�b�`����Ă��܂���");
            Debug.Assert(m_FadeShaders.Length > 0 && m_FadeShaders.Length == GameSystem.GetEnumLength<FadeType>(), "�V�F�[�_�[���A�^�b�`����Ă��܂���");
            m_pID = new PropertiesID[m_FadeShaders.Length];
            //�eID���擾
            for (int i = 0; i < m_FadeShaders.Length; i++) {             
                m_pID[i].m_ID_Fade = Shader.PropertyToID("_Fade");
                m_pID[i].m_ID_maxAlpha = Shader.PropertyToID("_MaxAlpha");
                m_pID[i].m_ID_minAlpha = Shader.PropertyToID("_MinAlpha");
            }
            //�t�F�[�h�\�t���O�𗧂Ă�
            m_FadeObject.enabled = true;
            //�t�F�[�h�̏�����
            int index = (int)FadeType.Entire;
            m_FadeObject.material.shader = m_FadeShaders[index];
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade,1);
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha,m_maxAlpha);
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha,m_minAlpha);
        }
    }

    //  ���s�֐��@//

    /// <summary>�Ή�����V�F�[�_�[�ɐݒ肵�܂��B</summary>
    private void SetFadeShader(ref FadeType type,bool reverse = false) {
        //�t�F�[�h�̓x����ۑ����Ă���
        float t = m_FadeObject.material.GetFloat(m_pID[(int)CurrentFadeType].m_ID_Fade); 
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
                    default: GameSystem.Log("�t�F�[�h�^�C�v : " + type); break;
            }
            index = (int)type;
        }
        //�V�F�[�_�[��ݒ�     
        m_FadeObject.material.shader = m_FadeShaders[(index)];
    }

    /// <summary>�t�F�[�h�̏������s���܂��B�t�F�[�h�\�ł����true��Ԃ��܂�</summary>
    private bool PrepareForFade(FadeType type,bool fadeOut) {
        if (m_IsFading) { GameSystem.LogError("�t�F�[�h�̌Ăяo�����d�����Ă��܂��B�d�����Ă���t�F�[�h�̏����͍s���܂���"); return false; }
        //�t�F�[�h���ɕύX
        IsFading = true;
        //�^�C���X�P�[����ۑ�����
        m_timeScale = Time.timeScale;
        //UI��O�ʂɈړ�
        m_FadeObject.transform.SetAsLastSibling();

        //�}�e���A�����̐ݒ�
        SetFadeShader(ref type,fadeOut);
        //t�̒l�����r���[�ł͂܂����t�F�[�h�^�C�v�̏ꍇ��_Fade���P���O�ɂ���
        if(type != FadeType.Entire) {
            float t = fadeOut ? 0 : 1;
            m_FadeObject.material.SetFloat(m_pID[(int)type].m_ID_Fade,t);
        }
        return true;
    }

    /// <summary>��ʂ̃t�F�[�h�C���̎��ۂ̏������s���܂�</summary>
    private IEnumerator _FadeInSetting(FadeType type, UnityAction action) {
        //�C���f�b�N�X���擾
        int index = (int)type;
        //���݂̕�ԏ����擾����
        float t = m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t < 1 ? 1 - t : 1;
        //���߂ƏI���̖��邳��ݒ肷��
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha, 1-CurrentAlpha);
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha, m_minAlpha);

        //���t���[���̕�Ԃ̎d����ݒ肷��
        UnityAction action1 = m_timeScale == 0 ?
            () => { t -= Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t -= Time.deltaTime / m_timeScale / FadeTime * ct; };
        
        //��Ԃ���
        float startTime = Time.time;
        while (t > 0) {
            action1();
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade,t);
            yield return null;
        }

        //�����𔽓]�����Ă���ꍇ�Ȃǂ͂����ɓ���
        if(m_CurrentFadeType != type) {
            SetFadeShader(ref m_CurrentFadeType);
            index = (int)m_CurrentFadeType;
        }
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, 0);
        IsFading = false;
        action?.Invoke();
    }

    /// <summary>��ʂ̃t�F�[�h�A�E�g�̎��ۂ̏������s���܂�</summary>
    private IEnumerator _FadeOutSetting(FadeType type, UnityAction action) {

        //�C���f�b�N�X���擾
        int index = (int)type;
        //���݂̕�ԏ����擾����
        float t = m_FadeObject.material.GetFloat(m_pID[index].m_ID_Fade);
        float ct = t > 0 ? 1 - t : 1;
        //���߂ƏI���̖��邳��ݒ肷��
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_minAlpha, 1-CurrentAlpha);
        m_FadeObject.material.SetFloat(m_pID[index].m_ID_maxAlpha, m_maxAlpha);

        //���t���[���̕�Ԃ̎d����ݒ肷��
        UnityAction action1 = m_timeScale == 0 ?
            () => { t += Time.unscaledDeltaTime / FadeTime * ct; }
          : () => { t += Time.deltaTime / m_timeScale / FadeTime * ct; };

        //��Ԃ���
        float startTime = Time.time;
        while (t < 1) {
            action1();
            m_FadeObject.material.SetFloat(m_pID[index].m_ID_Fade, t);
            yield return null;
        }

        //�����𔽓]�����Ă���ꍇ�Ȃǂ͂����ɓ���
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
