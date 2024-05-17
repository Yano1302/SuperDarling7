using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow :SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("�A�C�e���E�B���h�E�I�u�W�F�N�g�ꗗ"),EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;

    /// <summary>�E�B���h�E���A�N�e�B�u�����܂�</summary>
    public void ActiveWindows() { machWindow();m_uiManager.OpenUI(UIType.ItemWindow); }
    /// <summary>�E�B���h�E���A�N�e�B�u�����܂�</summary>
    public void InactiveWindows() { m_uiManager.CloseUI(UIType.ItemWindow); }

    public GameObject GetWinObj(ItemID itemID) { return m_windows[(int)itemID]; } // �Q�b�^�[�֐��@���ǋL


    /// <summary>�w�肵���A�C�e���E�B���h�E���J���܂�</summary>
    /// <param name="id">�J���E�B���h�E��ID</param>
    public void SetWindow(ItemID id,bool isOpen) { m_windows[(int)id].SetActive(isOpen);}

    /// <summary>�A�C�e���E�B���h�E�̃A�N�e�B�u��Ԃ��擾���܂�</summary>
    public bool IsActiveItemWindow { get { return m_uiManager.ChekIsOpen(UIType.ItemWindow);  }  }



    private ItemManager m_itemManager { get { IM ??= ItemManager.Instance; return IM; } } //�C���X�^���X�擾
    private UIManager m_uiManager { get { UIM ??= UIManager.Instance; return UIM; } }�@�@//�C���X�^���X�擾
    private ItemManager IM; //�C���X�^���X�{��
    private UIManager UIM; //�C���X�^���X�{��

    private void machWindow() {
        for (int i = 1; i < m_windows.Length; i++) {
            //�����A�C�e�����ƃI�u�W�F�N�g�̃A�N�e�B�u������v������
            m_windows[i].SetActive(m_itemManager.GetFlag((ItemID)i));
        }
    }
}
