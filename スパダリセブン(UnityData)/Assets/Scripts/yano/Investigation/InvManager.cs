using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Supadari;
using SceneManager = Supadari.SceneManager;

/// <summary>
/// MapManager���}�b�v�����̂��łɃX�e�[�W����S�Ď擾����
///                     ��
///  MapManager��InvManager���쐬���A�X�e�[�W���ɂ���T���p�[�g����n��
///                     ��
///  InvManager���n���ꂽ��������InvPart�N���X���쐬���AInvType���w�肷��B
///  ���̍ۂɍ쐬����InvPart����ۊǂ��Ă����AOpen()�֐��ł��̍쐬����InvPart���J����悤�ɂ���
///                     ��
///  InvType�͎w�肳�ꂽInvType�ɑΉ�����e�N�X�`����InvTextureholder����擾����Ɠ����ɁA
///  �Ή�����CSV�t�@�C����ǂݍ��݁A�A�C�e���I�u�W�F�N�g���q�Ƃ��č쐬����
///                     ��
///  ���̍ۂ�ItemObject��InvPart�Q�Ƃ�n���AItemObject����InvPart�ɃA�C�e�����擾���ꂽ����񍐂���
///  
/// 
///   �Ǘ���  /�Ǘ��I�u�W�F�N�g
///     
/// InvManager/�x���x�t���O ->�@InvPart���Ƀt���O��ݒ肷���ł͂Ȃ��̂ŊO��������t���O���ݒ肵�₷��InvManager�ɐݒu
/// InvManager/�}�E�X�J�[�\��-> ������P��ł���̂��ۏ؂���Ă���̂�InvManager�ɐݒu��ItemObject��������Ăяo���₷���悤�ɂ��Ă���
///  
/// InvPart   / �x���x�Q�[�W -> �����p�[�g���Ɍx���x�̐ݒ肪�Ⴄ�̂ŃQ�[�W�N���X�̊Ǘ���InvPart�ɈϏ�(SetUpInv()�ŃQ�[�W�N���X��n���Ă���)
/// InvPart   / �댯���b�Z�[�W-> �ŏ���InvManager�ŕۊǂ�����肾�������A�X�e�[�W�ɂ���Ċ댯���b�Z�[�W���o�Ă���̗P�\��ς���\�������邽��InvPart�ɈϏ�
/// 
/// �A�C�e���ɂ���
/// 
///                     �S�Ď擾���Ă���ꍇ�͐����p�[�g�Ɉڍs����
///                                         ��
///             InvManager���S�Ă�InvPart���m�F���A�A�C�e����S�Ď擾���Ă��邩�m�F����@���@�擾���Ă��Ȃ��ꍇ�͌��݂�InvPart����邾��
///                                         ��
///             InvPart�����g�̃p�[�g�̃A�C�e�����S�Ė����Ȃ���(�擾�ς�)�Ȃ̂𔻒f���A�Ή�����S�[���I�u�W�F�N�g��j�������̂�InvManager�ɕ񍐂���
///                                         ��
///             InvPart���c��̃A�C�e�������m�F����(���g�̎q�̎c�������̂܂܎c���) <----------------------------------------------------------------|
///                                         ��                                                                                                         |
///             ItemObject�����g���擾���ꂽ�ꍇ��ItemManager�փA�C�e����ǉ����AInvPart�ɕ񍐂���                                                 |
///                                         ��                                                                                                         |
///             InvPart���J���ꂽ�^�C�~���O��ItemObject�����g�̃A�C�e�������Ɏ擾���Ă��邩�ǂ���ItemManager�Ɋm�F���ɍs���@���@���Ɏ����Ă����ꍇ�ɂ͏d����h�����ߔj��InvPart�ɕ񍐂���
///                                         ��
/// //�T���p�[�g���J���܂ł̗���            ��
///             �S�[���I�u�W�F�N�g�ɓ��������ۂ�InvManager��Open����S�[���I�u�W�F�N�g�������Ă���InvType�̒����p�[�g���Ăяo���B      
///                                         ��
///              MapManager���}�b�v��������ۂɃS�[���I�u�W�F�N�g�ɑΉ�����InvType��o�^���Ă����B
///   
/// </summary>




public class InvManager : MonoBehaviour
{

    /// <summary>�T���p�[�g�`�����p�[�g�Ԃ̂ݎ擾�ł���C���X�^���X</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "�V�[�����Ⴄ�̂ŃC���X�^���X���擾�ł��܂���B"); return m_instance; } }

    /// <summary>�x���x�㏸�t���O��ݒ肵�܂��B</summary>
    public bool VigilanceFlag { get { return m_vigilanceFlag; } set { m_vigilanceFlag = value; } }

    ///
    public InvPart CurrentPart { get { return m_currentPart; } }

    /// <summary>MapManager���璲���p�[�g�̃Z�b�g�A�b�v���s���܂�</summary>
    public void SetUpInv(in InvType[] type,List<Goal> goalList) {
        if (m_invParts == null) {
            m_invParts = new InvPart[type.Length];
            for (int i = 0; i < type.Length; i++) {
                //InvPart�p�I�u�W�F�N�g���쐬���A����Manager�N���X�̎q�ɂ���
                var obj = Instantiate(m_baseInvObject);
                obj.transform.SetParent(m_invCanvas.transform, false);
                //InvPart�N���X���A�^�b�`���A�Z�b�g�A�b�v�֐����Ăяo��
                m_invParts[i] = obj.AddComponent<InvPart>();
                m_invParts[i].SetUpInvPart(type[i], m_baseInvItem, m_gauge, m_warningMessage);
                //InvPart�ɑΉ�����S�[�����������i�[����
                foreach(var g in goalList) {
                    if(g.InvType == m_invParts[i].InvPartType) {
                        m_invParts[i].SetGoal(g);
                    }
                }
            }
            //���������A�C�e���Ŗ�����Ȃ��l��UI����ԏ�Ɏ����Ă���
            m_backBtn.gameObject.transform.SetAsLastSibling();
            m_gauge.gameObject.transform.SetAsLastSibling();
            m_warningMessage.gameObject.transform.SetAsLastSibling();

        }
    }

    /// <summary>�����p�[�g���J���܂�</summary>
    public void Open(InvType type) {
        //�v���C���[���s���s�ɂ���
        Player.Instance.MoveFlag = false;
        //�V�[�����J��
        m_currentInvType = type;
        int index = (int)m_currentInvType;
        m_currentPart = m_invParts[index];
        m_invParts[index].gameObject.SetActive(true);
        m_invParts[index].Open();
        //�{�^�����A�N�e�B�u�ɂ���
        m_backBtn.SetActive(true);       
        //�J�[�\����ύX
        SetCursor(false);
        VigilanceFlag = true;
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

    /// <summary>�}�E�X�A�C�R����ݒ肵�܂�</summary>
    ///  <param name="target">
    ///  target��null�̏ꍇ ;�f�t�H���g�̃}�E�X�J�[�\���ɂȂ�܂�          <br />
    ///  target��true�̏ꍇ :�J�[�\�����^�[�Q�b�g�J�[�\���ɂȂ�܂�        <br />
    ///  target��false�̏ꍇ:�J�[�\������^�[�Q�b�g�J�[�\���ɂȂ�܂�      </param>
    public void SetMouseIcon(bool? target) {
        SetCursor(target);
    }

    /// <summary>���݂̒����p�[�g�̌x���x�����Z�b�g(0)���܂�</summary>
    public void ResetVigilance() { m_invParts[(int)m_currentInvType].ResetVigilance(); }

    /// <summary>�w�肵�������p�[�g�̌x���x�����Z�b�g(0)���܂�</summary>
    public void ResetVigilance(InvType type) { 
        foreach (var inv in m_invParts) {
            if (inv.InvPartType == type) {
                inv.ResetVigilance();
                return;
            }
        }
        UsefulSystem.LogWarning($"�w�肳�ꂽ�����p�[�g��������܂���ł��� : { type }");
    }

    //�S�Ă̒����p�[�g�̌x���x�����Z�b�g(0)�ɂ��܂�
    public void AllResetVigilance() { foreach (var inv in m_invParts)inv.ResetVigilance();}

    /// <summary>�S�Ă�InvPart���N���A����Ă��邩�m�F���܂��B���̊֐���InvPart����N���A�����x�Ă΂�܂��B</summary>
    public void CheckClear() { 
        foreach(var part in m_invParts) {
            if(!part.ClearFlag) {
                //�܂��N���A���Ă��Ȃ�Part������̂Ō��݂�Part����邾��
                Close();
                return;
            }
        }
        //�����ɓ��B�����ꍇ�ɂ̓N���A���Ă���
        ClearInv();
    }

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
    
    private static InvManager m_instance;        //���g�̃C���X�^���X
    private InvManager() { }
    private bool m_vigilanceFlag;                //�x���x�㏸�t���O
    private InvType m_currentInvType;            //���݂̒����p�[�g���
    private InvPart[] m_invParts;                //�T���p�[�g�z��
    private InvPart m_currentPart;               //���݂̃p�[�g

    private void Awake() {
        m_instance = GetComponent<InvManager>();
        VigilanceFlag = false;
        m_gauge.CloseGauge();
        m_gauge.SetRate(0);
        m_currentInvType = InvType.None;
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
    private void SetCursor(bool? target) {
        Texture2D tex = target == null ? null : (bool)target ? m_cursorTaget : m_cursor;
        Vector2 vec = tex  == null ? Vector2.zero : new Vector2(tex.width/2,tex.height/2);
        Cursor.SetCursor(tex, vec, CursorMode.Auto);
    }   

}
