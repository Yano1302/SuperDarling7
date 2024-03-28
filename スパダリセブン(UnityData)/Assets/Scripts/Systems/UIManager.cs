using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>UI�^�C�v</summary>
public enum UIType {
    Close,          //�����͕K�{
    EscapeButton,   //�g���b�v���甲���o���{�^����\������
    Timer,          //�^�C�}�[�\���pUI
    ItemWindow,     //�A�C�e���E�B���h�E
    Clear,          //�����������Ă����ꍇ��UI���(���u��)
    miss,           //�������Ԉ���Ă����ꍇ��UI���(���u��)
}

/// <summary>
/// UI�̕\�����Ǘ�����N���X�ł�<br />
/// �񋓎q���L�ڂ�����ɑΉ�����I�u�W�F�N�g��UIObjects�ɃA�^�b�`���Ďg�p���Ă�������<br />
/// </summary>
/// 

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    //  �p�u���b�N�ϐ�   //--------------------------------------------------------------------------------------------------------

    /// <summary>false�̊Ԃ͏������s���܂���</summary>
    public bool m_IsAvailable { get; set; } = true;

    /// <summary>false�̊Ԃ�UI�J�̍ۂ̃f�B���C���������܂���</summary>
    public bool m_EnableDelay { get; set; } = false;

    /// <summary>���݂�UI�^�C�v</summary>
    public UIType m_CurrentType { get { return m_UIList[m_UIList.Count - 1]; } }


    //  �R���t�B�O�p�ϐ�    //-----------------------------------------------------------------------------------------------------

    [SerializeField,Header("UI�\���E��\����������ĂъJ�����ɓ���܂ł̑ҋ@����")]
    private float OCDeley = 0.2f;
    [SerializeField, Header("�ŏ��ɕ\�����Ă���UI�^�C�v��ݒ肵�܂�")]
    private UIType[] FirstOpenUIs;


    //  �A�^�b�`�p�ϐ�   //--------------------------------------------------------------------------------------------------------

    [SerializeField, EnumIndex(typeof(UIType)), Header("UI�I�u�W�F�N�g�ꗗ")]
    private GameObject[] UIObjects;
    //  �A�^�b�`�p�֐�  //--------------------------------------------------------------------------------------------------------
    /// <summary>�w�肳�ꂽUI����܂�(�{�^���A�^�b�`�p)</summary>
    [EnumAction(typeof(UIType))]
    public void CloseUI(int type) { CloseUI((UIType)type); }

    /// <summary>���݊J����Ă�������UI�܂Ŗ߂�܂�(�{�^���p)</summary>
    [EnumAction(typeof(UIType))]
    public void ReturnUI(int type) { ReturnUI((UIType)type); }

    //  �p�u���b�N�֐�    //------------------------------------------------------------------------------------------------------

    /// <summary>�w�肳�ꂽUI���J���܂�</summary>
    public void OpenUI(UIType type) {
        if (CheckCall()) {
            OpenSettings(type);
        }
    }

    /// <summary>�w�肳�ꂽUI���J���܂�(�{�^���p)</summary>
    [EnumAction(typeof(UIType))]
    public void OpenUI(int type) { OpenUI((UIType)type); }


    /// <summary>������UI�𓯎��ɊJ���܂�</summary>
    /// <param name="UITypes">�J��UI�^�C�v�̔z��<br />
    /// ���z��̏��Ԓʂ��UI��\�����Ă����܂��B<br />
    /// �������A���ɊJ����Ă���UI�^�C�v��z��ɓ��ꂽ�ꍇ�A����UI���ĕ\�����鏈���͍s��Ȃ��̂�<br />
    /// ��ʂ̃��C�A�E�g�AClose�̏��ԓ������������Ȃ�\��������܂��B
    /// </param>
    public void OpenUIs(UIType[] UITypes) {
#if UNITY_EDITOR
        if (UITypes.Length == 0) {
            UnityEngine.Debug.LogError("�����̔z��̒��g������܂���");
            return;
        }
#endif
        if (CheckCall()) {
            for (int i = 0; i < UITypes.Length; i++) {
                OpenSettings(UITypes[i]);
            }
        }
    }

    /// <summary>���݂�UI����܂�</summary>
    public void CloseUI() {
        if (CheckCall()) {
            CloseSetting();
        }
    }

    /// <summary>�w�肳�ꂽUI����܂�</summary>
    public void CloseUI(UIType type) {
        if (CheckCall()) {
            if (m_UIList.Remove(type)) {
                //�폜�������v�f���Ō���ɂ��炷
                m_UIList.Add(type);
                CloseSetting();
            }
#if UNITY_EDITOR
            else {
                UnityEngine.Debug.LogError("�w�肳�ꂽUI���J����Ă��܂���");
                return;
            }
#endif
        }
    }

    /// <summary>���݊J����Ă�������UI�܂Ŗ߂�܂�<br />
    /// ������UI����h������UI�͑S�ĕ����܂�</summary>
    public void ReturnUI(UIType returnUI) {
#if UNITY_EDITOR
        if (!m_UIList.Contains(returnUI)) {
            UnityEngine.Debug.LogError("�w�肳�ꂽUI���J����Ă��܂���");
            return;
        }
        if (m_CurrentType == returnUI) {
            UnityEngine.Debug.LogWarning("���Ɏw�肳�ꂽUI�̏�Ԃł�");
            return;
        }
        if (returnUI == UIType.Close) {
            UnityEngine.Debug.LogWarning("�S�Ă�UI�������܂�");
        }
#endif
        if (CheckCall()) {
            while (m_CurrentType != returnUI) {
                CloseSetting();
            }
        }
    }

    /// <summary>�S�Ă�UI����܂��@</summary>
    public void CloseALLUI() {
        if (CheckCall()) {
            while (m_CurrentType != UIType.Close) {
                CloseSetting();
            }
        }
    }




    //  �v���C�x�[�g�ϐ�  //-------------------------------------------------------------------------------------------------------

    private List<UIType> m_UIList = null;         //�J����Ă���UI�ꗗ
   
    private GameObject m_TargetUI { get { return GetUIObject(m_CurrentType); } }         //�J�Ώۂ�UI

    //  �v���C�x�[�g�֐�  //-------------------------------------------------------------------------------------------------------
   �@protected override void Awake() {
        base.Awake();
        if(m_UIList == null) {
            m_UIList = new List<UIType>() { UIType.Close};
            UnityEngine.Debug.Assert(UIObjects.Length > 1,"UI�I�u�W�F�N�g���A�^�b�`����Ă��܂���");
            for (int i = 1; i < UIObjects.Length; i++) {
                UnityEngine.Debug.Assert(UIObjects[i] != null, (UIType)i + "�ɑΉ�����UI�I�u�W�F�N�g���A�^�b�`����Ă��܂���B");
                UIObjects[i].SetActive(false);
            }
            foreach(var ui in FirstOpenUIs) {
                OpenUI(ui);
            }
        }
    }

    /// <summary>UI�I�u�W�F�N�g���擾���܂�</summary>
    private GameObject GetUIObject(UIType type)
    {
        int index = (int)type;
#if UNITY_EDITOR
        if (type == UIType.Close)
        {
            UnityEngine.Debug.LogWarning("CloseUI�͎擾�ł��܂���");
            return null;
        }
        if(UIObjects.Length < index - 1 || UIObjects[index] == null)
        {
            UnityEngine.Debug.LogError("UI���A�^�b�`����Ă��܂��� : "+ type.ToString());
            return null;
        }
#endif
        return UIObjects[index];
    }

    /// <summary>�Ăяo���\���ǂ������ׂ܂�</summary>
    /// /// <returns>�\�ł����true��Ԃ��܂�</returns>
    private bool CheckCall()
    {
        //�������s��
        if (m_IsAvailable)
        {
            //�������̃t���O�𗧂Ă�
            if (m_EnableDelay)
            {
                m_IsAvailable = false;
                UsefulSystem.Instance.WaitCallBack(OCDeley, () => m_IsAvailable = true);
            }
            return true;
        }
#if UNITY_EDITOR
        if (!m_EnableDelay)
        {
            UnityEngine.Debug.LogWarning("UI�̊J�����̍ۂ̃f�B���C������������Ă��܂�");
        }
        if (!m_IsAvailable)
        {
            UnityEngine.Debug.LogWarning("m_IsAvailable��false�Ȃ̂ŏ������s���܂���");
        }
#endif
        return false;
    }

    /// <summary>��ʂ��J���ۂ̏������s���܂�</summary>
    private void OpenSettings(UIType type)
    {
#if UNITY_EDITOR
        if (type == UIType.Close)
        {
            UnityEngine.Debug.LogError("CloseUI�͊J�������o���܂���B\n�Ăяo�����̈���(�{�^���A�X�N���v�g��)���m�F���Ă�������");
            return;
        }
#endif
        //UI���J����Ă��Ȃ����`�F�b�N����
        if (!m_UIList.Contains(type))
        {
            //���݂�UI����ǉ��A�X�V����
            m_UIList.Add(type);
            //��ԑO��UI�I�u�W�F�N�g�������Ă���
            GetUIObject(type).transform.SetAsLastSibling();
            //UI��\������
            m_TargetUI.SetActive(true);
        }
#if UNITY_EDITOR
        else
        {
            UnityEngine.Debug.LogWarning("���ɂ���UI�͊J����Ă��܂� : " + type.ToString());
            return;
        }
#endif
    }

    /// <summary>��ʂ����ۂ̏������s���܂�</summary>
    private void CloseSetting()
    {
        if (m_CurrentType != UIType.Close)
        {
            m_TargetUI.SetActive(false);               //���݂�UI�t���O��ύX����
            m_UIList.RemoveAt(m_UIList.Count - 1); //����ꂽUI��z�񂩂��菜��             
        }
#if UNITY_EDITOR
        else
        {
            UnityEngine.Debug.LogWarning("���ɑS�Ă�UI�������Ă��܂�");
        }
#endif
    }

}