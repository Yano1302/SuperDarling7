using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow :SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("アイテムウィンドウオブジェクト一覧"),EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;

    /// <summary>ウィンドウをアクティブ化します</summary>
    public void ActiveWindows() { machWindow();m_managerUI ??=UIManager.Instance; m_managerUI.OpenUI(UIType.ItemWindow); }
    /// <summary>ウィンドウを非アクティブ化します</summary>
    public void InactiveWindows() { m_managerUI ??= UIManager.Instance; m_managerUI.CloseUI(UIType.ItemWindow); }


    /// <summary>指定したアイテムウィンドウを開きます</summary>
    /// <param name="id">開くウィンドウのID</param>
    public void SetWindow(ItemID id,bool isOpen) { m_windows[(int)id].SetActive(isOpen);}

    



    private ItemManager m_managerIT;
    private UIManager m_managerUI;

    private void machWindow() {
        m_managerIT ??= ItemManager.Instance;
        for (int i = 1; i < m_windows.Length; i++) {
            //所持アイテム情報とオブジェクトのアクティブ情報を一致させる
            m_windows[i].SetActive(m_managerIT.GetFlag((ItemID)i));
        }
    }
}
