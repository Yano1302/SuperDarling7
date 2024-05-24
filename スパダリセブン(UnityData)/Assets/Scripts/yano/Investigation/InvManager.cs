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
    /// <summary>�T���p�[�g�`�����p�[�g�Ԃ̂ݎ擾�ł���C���X�^���X</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "�V�[�����Ⴄ�̂ŃC���X�^���X���擾�ł��܂���B"); return m_instance; } }
    /// <summary>���̒����p�[�g�ɔz�u���ꂽ�A�C�e���̒��Ŏ擾���Ă���A�C�e����</summary>
    public int GetItemNum { get { return m_getItemNum; } set { m_getItemNum = value; if (m_getItemNum >= MapManager.Instance.TotalItem){ ClearInv();} } }

    /// <summary>�x���x���オ��t���O��ݒ肵�܂��B</summary>
    public bool VigilanceFlag { get { return m_vigilance.VigilanceFlag; } set { m_vigilance.VigilanceFlag = value; } }

    /// <summary>�����p�[�g���J���܂�</summary>
    public void Open(InvType type) {
        Debug.Assert(!m_isOpen,"�T���p�[�g�����ɊJ����Ă��܂�");
        //�V�[�����J��
        m_currentInvType = type;  m_invObj[(int)m_currentInvType].SetActive(true);
        //�{�^�����A�N�e�B�u�ɂ���
        m_backBtn.SetActive(true);
        //���W������������
        m_vigilance.mouseVec = Input.mousePosition;
        //�t���O�ݒ�
        m_gauge.enabled = true;
        m_gaugefill.enabled = true;
        m_isOpen = true; 
        m_vigilance.VigilanceFlag = true;
        Player.Instance.MoveFlag = false;
        //�J�[�\����ύX
        SetCursor(false);
       
    }

    /// <summary>�����p�[�g����A�T���p�[�g�ɂ��ǂ�܂�(�{�^���ɃA�^�b�`���Ă܂�)</summary>
    public void Close() {
        if (m_vigilance.VigilanceFlag) {
            //�V�[�������
            m_invObj[(int)m_currentInvType].SetActive(false);
            //�{�^�����\����
            m_backBtn.SetActive(false);
            //�t���O�ݒ�
            m_gaugefill.enabled = false;
            m_isOpen = false;
            Player.Instance.MoveFlag = true;
            m_currentInvType = 0;
            //�J�[�\�������ɖ߂�
            SetCursor(null);
        }
    }

    /// <summary>�}�E�X�A�C�R����ݒ肵�܂�<br />
    /// target��null�̏ꍇ;�f�t�H���g�̃}�E�X�J�[�\���ɂȂ�܂�<br />
    /// target��true�̏ꍇ:�J�[�\�����^�[�Q�b�g�J�[�\���ɂȂ�܂�<br />
    /// target��false�̏ꍇ:�J�[�\������^�[�Q�b�g�J�[�\���ɂȂ�܂�</summary>
    public void SetMouseIcon(bool? target) {
       SetCursor(target);
    }

    public void ResetVigilance() { SetVigilance(0); }

    //  �A�^�b�`�p   //------------------------------------------------------------------------------
    #region Attach
    [SerializeField, Header("�߂�{�^��")]
    private GameObject m_backBtn;
    [SerializeField, Header("�x���x�Q�[�W")]
    private Image m_gauge;
    [SerializeField]
    private Image m_gaugefill;
    [SerializeField]
    private Image m_waves;
    [SerializeField]
    private float m_waveDirectionTime;
    [SerializeField]
    private float m_waveInterval;
    [SerializeField, Header("�댯���b�Z�[�W�{�b�N�X")]
    private GameObject m_warningMessage;
    [SerializeField, Header("�}�E�X�J�[�\���摜1")]
    Texture2D m_cursor; 
    [SerializeField, Header("�}�E�X�J�[�\���摜1")]
    Texture2D m_cursorTaget;

    [SerializeField,Header("�T���p�[�g�w�i�pImageObjct"),EnumIndex(typeof(InvType))]
    private GameObject[] m_invObj;
    #endregion
    //-----------------------------------------------------------------------------------------------
    //���g�̃C���X�^���X
    private static InvManager m_instance;
    private InvManager() { }
    
    private AudioManager m_audioManager;

    private InvType  m_currentInvType;           //���݂̒����p�[�g���
    private bool m_isOpen;                       //�J���Ă��邩�̃t���O
    private Vigilance m_vigilance;               //�x���x�p�\����
    private int m_getItemNum;                    //���ݎ擾���Ă���A�C�e����

    private struct Vigilance {
        public float MaxVigilance;               //�ő�x���x
        public float Level { get { return m_VigilanceLevel; } set { if (VigilanceFlag) { m_VigilanceLevel = value;  }  } }
        public float Rate { get { return m_VigilanceLevel / MaxVigilance; } }
        public bool IsOver { get { return m_VigilanceLevel >= MaxVigilance; } }
        public Vector2 mouseVec;               //�}�E�X���W
        public bool VigilanceFlag;             //���̃t���O��On�̎��̂݌x���x��ݒ�ł��܂�
        public float WaveInterval;             //�ۓ��̊Ԋu
        private float m_VigilanceLevel;        //�x���x

    }
    
    private void Awake() {
        //�ő�l��ݒ肷��
        m_vigilance.MaxVigilance = 20;
        //�x���x������������
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
    /// <summary>�}�E�X�����������ǂ����𒲂ׂ܂�</summary>
    /// <returns>�Q�[���I�[�o�[�̏ꍇ�ɂ�false��Ԃ��܂�</returns>
    private void CheckMouseMove() {
        Vector2 pos = Input.mousePosition;
        //�}�E�X����������Ă���ꍇ
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

    /// <summary>�x���x��ǉ����܂�</summary>
    /// <returns>�x���x���ő�l�ȏ�̏ꍇ��true��Ԃ��܂�</returns>
    private bool AddVigilance(float addValue) {
        m_vigilance.Level += m_vigilance.IsOver? 0 : addValue;
        m_gaugefill.fillAmount = m_vigilance.Rate;
        //�x���x���X�V���ꂽ�ꍇ�̏���
        if (m_vigilance.Rate > m_vigilance.WaveInterval) {
            StartCoroutine("Waves");
            m_vigilance.WaveInterval += m_waveInterval;
        }
        m_warningMessage.SetActive(m_vigilance.IsOver);
        return m_vigilance.IsOver;
    }

    /// <summary>�x���x��ݒ肵�܂�</summary>
    /// <returns>�x���x���ő�l�ȏ�̏ꍇ��true��Ԃ��܂�</returns>
    private bool SetVigilance(float value) {
        m_vigilance.Level = value;
        m_gaugefill.fillAmount = m_vigilance.Rate;
        m_warningMessage.SetActive(m_vigilance.IsOver);
        return m_vigilance.IsOver;
    }


    /// <summary>�S�ẴA�C�e�����擾�����ۂ̏������L�q���܂�</summary>
    private void ClearInv() {
        m_vigilance.VigilanceFlag = false;
        StopCoroutine("Waves");
        SceneManager.Instance.SceneChange(SCENENAME.SolveScene, () => { UIManager.Instance.CloseUI(UIType.Timer); SetCursor(null); });
    }

    /// <summary>�J�[�\����ݒ肵�܂�</summary>
    /// <param name="target"></param>
    private void SetCursor(bool? target) {
        Texture2D tex = target == null ? null : (bool)target ? m_cursorTaget : m_cursor;
        Vector2 vec = tex  == null ? Vector2.zero : new Vector2(tex.width/2,tex.height/2);
        Cursor.SetCursor(tex, vec, CursorMode.Auto);
    }

    /// <summary>
    /// �x���x���}�b�N�X�̍ۂɃ}�E�X�𓮂����Ă��܂����ꍇ�̏���
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
