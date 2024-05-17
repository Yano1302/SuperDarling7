using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow :SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("アイテムウィンドウオブジェクト一覧"),EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;

    /// <summary>ウィンドウをアクティブ化します</summary>
    public void ActiveWindows() { machWindow();m_uiManager.OpenUI(UIType.ItemWindow); }
    /// <summary>ウィンドウを非アクティブ化します</summary>
    public void InactiveWindows() { m_uiManager.CloseUI(UIType.ItemWindow); }

    public GameObject GetWinObj(ItemID itemID) { return m_windows[(int)itemID]; } // ゲッター関数　岬追記


    /// <summary>指定したアイテムウィンドウを開きます</summary>
    /// <param name="id">開くウィンドウのID</param>
    public void SetWindow(ItemID id,bool isOpen) { m_windows[(int)id].SetActive(isOpen);}

    /// <summary>アイテムウィンドウのアクティブ状態を取得します</summary>
    public bool IsActiveItemWindow { get { return m_uiManager.ChekIsOpen(UIType.ItemWindow);  }  }



    private ItemManager m_itemManager { get { IM ??= ItemManager.Instance; return IM; } } //インスタンス取得
    private UIManager m_uiManager { get { UIM ??= UIManager.Instance; return UIM; } }　　//インスタンス取得
    private ItemManager IM; //インスタンス本体
    private UIManager UIM; //インスタンス本体

    private void machWindow() {
        for (int i = 1; i < m_windows.Length; i++) {
            //所持アイテム情報とオブジェクトのアクティブ情報を一致させる
            m_windows[i].SetActive(m_itemManager.GetFlag((ItemID)i));
        }
    }
}
