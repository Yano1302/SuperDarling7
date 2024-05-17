using Supadari;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow : SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("�A�C�e���E�B���h�E�I�u�W�F�N�g�ꗗ"), EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;
    [SerializeField]
    private GameObject m_judge; // ���ǋL�@�W���b�W�ϐ�

    /// <summary>�E�B���h�E���A�N�e�B�u�����܂�</summary>
    public void ActiveWindows() { machWindow(); m_managerUI ??= UIManager.Instance; m_managerUI.OpenUI(UIType.ItemWindow); }
    /// <summary>�E�B���h�E���A�N�e�B�u�����܂�</summary>
    public void InactiveWindows() { m_managerUI ??= UIManager.Instance; m_managerUI.CloseUI(UIType.ItemWindow); }
    /// <summary>�W���b�W���A�N�e�B�u�����܂�</summary>�@���ǋL
    public void ActiveJudge() { m_judge.SetActive(true); }
    /// <summary>�W���b�W���A�N�e�B�u�����܂�</summary>�@���ǋL
    public void InactiveJudge() { m_judge.SetActive(false); }
    public bool CheckJudge(){ return m_judge.activeSelf; }
    public GameObject GetWinObj(ItemID itemID) { return m_windows[(int)itemID]; } // �Q�b�^�[�֐��@���ǋL

    /// <summary>
    /// �A�C�e���E�B���h�E���X���C�h������֐� ���ǋL
    /// </summary>
    public void WinSlide()
    {
        // ItemOpen�ŃX���C�h��������������߂�
        if (CheckOpen == false)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            CheckOpen = true;
        }
        else if (CheckOpen == true)
        {
            transform.localPosition = new Vector3(1170, 0, 0);
            CheckOpen = false;
        }
    }


    /// <summary>�w�肵���A�C�e���E�B���h�E���J���܂�</summary>
    /// <param name="id">�J���E�B���h�E��ID</param>
    public void SetWindow(ItemID id,bool isOpen) { m_windows[(int)id].SetActive(isOpen);}

    public bool CheckOpen { get { return m_isOpen; } set { m_isOpen = value; } }


    private bool m_isOpen = false; // ���ǋL�@�E�B���h�E���J���Ă��邩�ǂ���
    private ItemManager m_managerIT;
    private UIManager m_managerUI;

    private void machWindow() {
        m_managerIT ??= ItemManager.Instance;
        for (int i = 1; i < m_windows.Length; i++) {
            //�����A�C�e�����ƃI�u�W�F�N�g�̃A�N�e�B�u������v������
            m_windows[i].SetActive(m_managerIT.GetFlag((ItemID)i));
        }
    }
}
