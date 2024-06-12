using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Supadari;
using SceneManager = Supadari.SceneManager;


public class InvManager : MonoBehaviour
{

    /// <summary>�T���p�[�g�`�����p�[�g�Ԃ̂ݎ擾�ł���C���X�^���X</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "�V�[�����Ⴄ�̂ŃC���X�^���X���擾�ł��܂���B"); return m_instance; } }

   
    
    /// <summary>���̒����p�[�g�ɔz�u���ꂽ�A�C�e���̒��Ŏ擾���Ă���A�C�e����</summary>
    public int GetItemNum { get { return m_getItemNum; } set { m_getItemNum = value; if (m_getItemNum >= MapManager.Instance.TotalItem) { ClearInv(); } } }

    /// <summary>�x���x�㏸�t���O��ݒ肵�܂��B</summary>
    public bool VigilanceFlag { get { return m_vigilanceFlag; } set { m_vigilanceFlag = value; } }

    /// <summary>�����p�[�g���J���܂�</summary>
    public void Open(InvType type) {
        //�v���C���[���s���s�ɂ���
        Player.Instance.MoveFlag = false;
        //�V�[�����J��
        m_currentInvType = type;
        int index = (int)m_currentInvType;
        m_invParts[index].gameObject.SetActive(true);
        m_invParts[index].Open();
        //�{�^�����A�N�e�B�u�ɂ���
        m_backBtn.SetActive(true);       
        //�J�[�\����ύX
        SetCursor(false);
    }

    /// <summary>�����p�[�g����A�T���p�[�g�ɂ��ǂ�܂�(�{�^���ɃA�^�b�`���Ă܂�)</summary>
    public void Close() {
        if (VigilanceFlag) {
            //�V�[�������
            int index = (int)m_currentInvType;
            m_invParts[index].Close();
            m_invParts[index].gameObject.SetActive(false);
            //�{�^�����\����
            m_backBtn.SetActive(false);
            //�t���O�ݒ�
            VigilanceFlag = false; // ���ǋL�@�x���x�̓��Z�b�g���Ȃ��悤�ɂ��Ă��܂�
            var ins= Player.Instance;
            ins.MoveFlag = true;
            ins.VisibilityImage = true;
            m_currentInvType = InvType.None;
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

    /// <summary>���݂̒����p�[�g�̌x���x�����Z�b�g(0)���܂�</summary>
    public void ResetVigilance() { m_invParts[(int)m_currentInvType].ResetVigilance(); }

    /// <summary>�w�肵�������p�[�g�̌x���x�����Z�b�g(0)���܂�</summary>
    public void ResetVigilance(InvType type) { 
        foreach (var inv in m_invParts) {
            if (inv.MyIntType == type) {
                inv.ResetVigilance();
                return;
            }
        }
        UsefulSystem.LogWarning($"�w�肳�ꂽ�����p�[�g��������܂���ł��� : { type }");
    }

    //�S�Ă̒����p�[�g�̌x���x�����Z�b�g(0)�ɂ��܂�
    public void AllResetVigilance() { foreach (var inv in m_invParts) { inv.ResetVigilance(); } }

    //  �A�^�b�`�p   //------------------------------------------------------------------------------
    #region Attach
    [SerializeField, Header("�߂�{�^��")]
    private GameObject m_backBtn;
    [SerializeField, Header("�x���x�Q�[�W")]
    private InvGauge m_gauge;
    [SerializeField, Header("�댯���b�Z�[�W�{�b�N�X")]
    private GameObject m_warningMessage;
    [SerializeField, Header("�}�E�X�J�[�\���摜1")]
    Texture2D m_cursor;
    [SerializeField, Header("�}�E�X�J�[�\���摜1")]
    Texture2D m_cursorTaget;
    [SerializeField, Header("�T���p�[�g�p�L�����o�X")]
    private GameObject m_invCanvas;
    [SerializeField, Header("�T���p�[�g�w�i�pImageObjct")]
    private GameObject m_baseInvObject;
    [SerializeField, Header("�T���p�[�g�̃A�C�e���̓����蔻��p�I�u�W�F�N�g")]
    private GameObject m_baseInvItem;
    #endregion



    //-----------------------------------------------------------------------------------------------
    //���g�̃C���X�^���X
    private static InvManager m_instance;
    private InvManager() { }
    private bool m_vigilanceFlag;                //�x���x�㏸�t���O
    private InvType m_currentInvType;            //���݂̒����p�[�g���
    private int m_getItemNum;                    //���ݎ擾���Ă���A�C�e����
    private InvPart[] m_invParts;                //�T���p�[�g�z��

    private void Awake() {
        m_instance = GetComponent<InvManager>();
        VigilanceFlag = false;
        m_gauge.CloseGauge();
        m_gauge.SetRate(0);
        m_currentInvType = InvType.None;
        m_getItemNum = 0;
    }

    /// <summary>MapManager���璲���p�[�g�̃Z�b�g�A�b�v���s���܂�</summary>
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

   


    /// <summary>�S�ẴA�C�e�����擾�����ۂ̏������L�q���܂�</summary>
    private void ClearInv() {
        TimerManager timerManager=TimerManager.Instance;
        SceneManager sceneManager = SceneManager.Instance;
        JsonSettings<SettingsGetItemFlags> saveItemData = new JsonSettings<SettingsGetItemFlags>(string.Format("Data{0}", sceneManager.saveSlot), "JsonSaveFile", "ItemGetFlags");
        // �x���Q�[�W�Ɛ������Ԃ��~�߂�@���ǋL
        VigilanceFlag = false;
        timerManager.TimerFlag = false;
        m_gauge.StopWave();

        // �A�C�e�������t���O��ۑ����A�V�[���J��
        saveItemData = ItemManager.Instance.UsingItemFlag;
        saveItemData.Save();
        SceneManager.Instance.SceneChange(SCENENAME.SolveScene, () => { UIManager.Instance.CloseUI(UIType.Timer); SetCursor(null); });
    }

    /// <summary>�J�[�\����ݒ肵�܂�</summary>
    /// <param name="target"></param>
    private void SetCursor(bool? target) {
        Texture2D tex = target == null ? null : (bool)target ? m_cursorTaget : m_cursor;
        Vector2 vec = tex  == null ? Vector2.zero : new Vector2(tex.width/2,tex.height/2);
        Cursor.SetCursor(tex, vec, CursorMode.Auto);
    }   

}
