using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �E�B���h�E�̕\�����Ǘ�����N���X�ł�<br />
/// �񋓎q���L�ڂ�����ɑΉ�����I�u�W�F�N�g��WindowObjects�ɃA�^�b�`���Ďg�p���Ă�������<br />
/// </summary>
public class WindowManager : SingletonMonoBehaviour<WindowManager>
{

    /// <summary>�E�B���h�E�^�C�v</summary>
    public enum WindowType
    {
        Close,          //�����͕K�{
        MainWindow,
        Window1, 
        Window2,         
    }


    //  �p�u���b�N�ϐ�   //--------------------------------------------------------------------------------------------------------

    /// <summary>false�̊Ԃ͏������s���܂���</summary>
    public bool m_IsAvailable { get; set; } = true;

    /// <summary>false�̊Ԃ̓E�B���h�E�J�̍ۂ̃f�B���C���������܂���</summary>
    public bool m_EnableDelay { get; set; } = false;

    /// <summary>���݂̃E�B���h�E�^�C�v</summary>
    public WindowType m_CurrentType { get { return m_WindowList[m_WindowList.Count - 1]; } }


    //  �R���t�B�O�p�ϐ�    //-----------------------------------------------------------------------------------------------------

    [SerializeField,Header("�E�B���h�E�J��������ĂъJ�����ɓ���܂ł̑ҋ@����")]
    private float c_OCDeley = 0.2f;                     


    //  �A�^�b�`�p�ϐ�   //--------------------------------------------------------------------------------------------------------

    [SerializeField, Header("�|�[�Y��ʗp�̔w�i�I�u�W�F�N�g\n(�K�v�Ȃ��ꍇ�̓A�^�b�`���Ȃ��Ă����܂��܂���)")]
    private GameObject BackGround;

    [SerializeField, EnumIndex(typeof(WindowType)), Header("�E�B���h�E�I�u�W�F�N�g�ꗗ")]
    private GameObject[] WindowObjects;
    

    //  �v���C�x�[�g�ϐ�  //-------------------------------------------------------------------------------------------------------

    private List<WindowType> m_WindowList = new List<WindowType>() { WindowType.Close };         //�J����Ă���E�B���h�E�ꗗ
   
    private GameObject m_TargetWindow { get { return GetWindowObject(m_CurrentType); } }         //�J�Ώۂ̃E�B���h�E


    //  �L�q�֐�    //-------------------------------------------------------------------------------------------------------------

    //�E�B���h�E���J�����ۂ̏������L�q���܂�(���o�ʂȂ�)�@��OpenWindows������͕�����(�J���E�B���h�E����)�Ă΂�܂�
    private void Open()
    {
       m_TargetWindow.SetActive(true);
      
    }

    //�E�B���h�E�������ۂ̏������L�q���܂�(���o�ʂȂ�) ��AllCloseWindow������͕�����(����E�B���h�E����)�Ă΂�܂�
    private void Close()
    {
        m_TargetWindow.SetActive(false);
    }


    //  �A�^�b�`���ŊO������Ăяo���֐�    //-------------------------------------------------------------------------------------

    /// <summary>�w�肳�ꂽ�E�B���h�E���J���܂�</summary>
    public void OpenWindow(WindowType type)
    {
        if (CheckCall())
        {
            OpenSettings(type);
        }
    }

    /// <summary>�w�肳�ꂽ�E�B���h�E���J���܂�(�{�^���p)</summary>
    [EnumAction(typeof(WindowType))]
    public void OpenWindow(int type){ OpenWindow((WindowType)type); }


    /// <summary>�����̃E�B���h�E�𓯎��ɊJ���܂�</summary>
    /// <param name="windowTypes">�J���E�B���h�E�^�C�v�̔z��<br />
    /// ���z��̏��Ԓʂ�ɃE�B���h�E��\�����Ă����܂��B<br />
    /// �������A���ɊJ����Ă���E�B���h�E�^�C�v��z��ɓ��ꂽ�ꍇ�A���̃E�B���h�E���ĕ\�����鏈���͍s��Ȃ��̂�<br />
    /// ��ʂ̃��C�A�E�g�AClose�̏��ԓ������������Ȃ�\��������܂��B
    /// </param>
    public void OpenWindows(WindowType[] windowTypes)
    {
#if UNITY_EDITOR
        if(windowTypes.Length == 0)
        {
            Debug.LogError("�����̔z��̒��g������܂���");
            return;
        }
#endif
        if (CheckCall())
        {
            for(int i = 0; i < windowTypes.Length; i++)
            {
                OpenSettings(windowTypes[i]);
            }
        }
    }


    /// <summary>���݂̃E�B���h�E����܂�</summary>
    public void CloseWindow()
    {
        if(CheckCall())
        {
            CloseSetting();
        }
    }


    /// <summary>�w�肳�ꂽ�E�B���h�E����܂�</summary>
    public void CloseWindow(WindowType type)
    {
        if (CheckCall())
        {
            if (m_WindowList.Remove(type))
            {
                //�폜�������v�f���Ō���ɂ��炷
                m_WindowList.Add(type);
                CloseSetting();
            }
#if UNITY_EDITOR
            else
            {
                    Debug.LogError("�w�肳�ꂽ�E�B���h�E���J����Ă��܂���");
                    return;
            }
#endif
        }
    }

    /// <summary>�w�肳�ꂽ�E�B���h�E����܂�(�{�^���p)</summary>
    [EnumAction(typeof(WindowType))]
    public void CloseWindow(int type){ CloseWindow((WindowType)type); }


    /// <summary>���݊J����Ă������̃E�B���h�E�܂Ŗ߂�܂�<br />
    /// �����̃E�B���h�E����h�������E�B���h�E�͑S�ĕ����܂�</summary>
    public void ReturnWindow(WindowType returnWindow)
    {
#if UNITY_EDITOR
        if (!m_WindowList.Contains(returnWindow))
        {
            Debug.LogError("�w�肳�ꂽ�E�B���h�E���J����Ă��܂���");
            return;
        }
        if(m_CurrentType == returnWindow)
        {
            Debug.LogWarning("���Ɏw�肳�ꂽ�E�B���h�E�̏�Ԃł�");
            return;
        }
        if (returnWindow == WindowType.Close)
        {
            Debug.LogWarning("�S�ẴE�B���h�E�������܂�");
        }
#endif
            if (CheckCall())
            {
                while (m_CurrentType != returnWindow)
                {
                    CloseSetting();
                }
            }
    }

    /// <summary>���݊J����Ă������̃E�B���h�E�܂Ŗ߂�܂�(�{�^���p)</summary>
    [EnumAction(typeof(WindowType))]
    public void ReturnWindow(int type) { ReturnWindow((WindowType)type); }


    /// <summary>�S�ẴE�B���h�E����܂��@</summary>
    public void CloseALLWindow()
    {
        if (CheckCall())
        {
            while (m_CurrentType != WindowType.Close)
            {
                CloseSetting();
            }
        }
    }


    //  ���̑��֐�   //------------------------------------------------------------------------------------------------------------

    /// <summary>�E�B���h�E�I�u�W�F�N�g���擾���܂�</summary>
    private GameObject GetWindowObject(WindowType type)
    {
        int index = (int)type;
#if UNITY_EDITOR
        if (type == WindowType.Close)
        {
            Debug.LogWarning("Close�E�B���h�E�͎擾�ł��܂���");
            return null;
        }
        if(WindowObjects.Length < index - 1 || WindowObjects[index] == null)
        {
            Debug.LogError("�E�B���h�E���A�^�b�`����Ă��܂��� : "+type.ToString());
            return null;
        }
#endif
        return WindowObjects[index];
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
                UsefulSystem.Instance.WaitCallBack(c_OCDeley, () => m_IsAvailable = true);
            }
            return true;
        }
#if UNITY_EDITOR
        if (!m_EnableDelay)
        {
            Debug.LogWarning("�E�B���h�E�̊J�����̍ۂ̃f�B���C������������Ă��܂�");
        }
        if (!m_IsAvailable)
        {
            Debug.LogWarning("m_IsAvailable��false�Ȃ̂ŏ������s���܂���");
        }
#endif
        return false;
    }

    /// <summary>�o�b�N�O���E���h��\���A��\���ɂ��܂�</summary>
    private void SetBackGround(bool setActive)
    {
        if (BackGround != null)
        {
            BackGround.SetActive(setActive);
            //�o�b�N�O���E���h����Ԍ��Ɏ����Ă���
            BackGround.transform.SetAsFirstSibling();
        }
    }

    /// <summary>��ʂ��J���ۂ̏������s���܂�</summary>
    private void OpenSettings(WindowType type)
    {
#if UNITY_EDITOR
        if (type == WindowType.Close)
        {
            Debug.LogError("Close�E�B���h�E�͊J�������o���܂���B\n�Ăяo�����̈���(�{�^���A�X�N���v�g��)���m�F���Ă�������");
            return;
        }
#endif
        //�E�B���h�E���J����Ă��Ȃ����`�F�b�N����
        if (!m_WindowList.Contains(type))
        {
            //�E�B���h�E�������Ă���Ȃ�o�b�N�O���E���h���o���������s��
            if (m_CurrentType == WindowType.Close)
            {
                SetBackGround(true);
            }
            //���݂̃E�B���h�E����ǉ��A�X�V����
            m_WindowList.Add(type);
            //��ԑO�ɃE�B���h�E�I�u�W�F�N�g�������Ă���
            GetWindowObject(type).transform.SetAsLastSibling();
            //�E�B���h�E���J��
            Open();
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning("���ɂ��̃E�B���h�E�͊J����Ă��܂� : " + type.ToString());
            return;
        }
#endif
    }

    /// <summary>��ʂ����ۂ̏������s���܂�</summary>
    private void CloseSetting()
    {
        if (m_CurrentType != WindowType.Close)
        {
                Close();                                       //���݂̃E�B���h�E�t���O��ύX����
                m_WindowList.RemoveAt(m_WindowList.Count - 1); //����ꂽ�E�B���h�E��z�񂩂��菜��             
                if (m_CurrentType == WindowType.Close)
                {
                    //�S�ẴE�B���h�E�������Ă���̂Ńo�b�N�O���E���h�����
                    SetBackGround(false);
                }
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning("���ɑS�ẴE�B���h�E�������Ă��܂�");
        }
#endif
    }

}