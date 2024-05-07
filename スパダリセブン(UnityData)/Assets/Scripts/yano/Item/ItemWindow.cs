using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow :SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("�A�C�e���E�B���h�E�I�u�W�F�N�g�ꗗ"),EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;

    /// <summary>�E�B���h�E���A�N�e�B�u�����܂�</summary>
    public void ActiveWindows() { machWindow();m_managerUI ??=UIManager.Instance; m_managerUI.OpenUI(UIType.ItemWindow); }
    /// <summary>�E�B���h�E���A�N�e�B�u�����܂�</summary>
    public void InactiveWindows() { m_managerUI ??= UIManager.Instance; m_managerUI.CloseUI(UIType.ItemWindow); }


    /// <summary>�w�肵���A�C�e���E�B���h�E���J���܂�</summary>
    /// <param name="id">�J���E�B���h�E��ID</param>
    public void SetWindow(ItemID id,bool isOpen) { m_windows[(int)id].SetActive(isOpen);}

    



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
