using Supadari;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow : SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("アイテムウィンドウオブジェクト一覧"), EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;
    [SerializeField]
    private GameObject m_judge; // 岬追記　ジャッジ変数

    /// <summary>ウィンドウをアクティブ化します</summary>
    public void ActiveWindows() { machWindow(); m_managerUI ??= UIManager.Instance; m_managerUI.OpenUI(UIType.ItemWindow); }
    /// <summary>ウィンドウを非アクティブ化します</summary>
    public void InactiveWindows() { m_managerUI ??= UIManager.Instance; m_managerUI.CloseUI(UIType.ItemWindow); }
    /// <summary>ジャッジをアクティブ化します</summary>　岬追記
    public void ActiveJudge() { m_judge.SetActive(true); }
    /// <summary>ジャッジを非アクティブ化します</summary>　岬追記
    public void InactiveJudge() { m_judge.SetActive(false); }
    public bool CheckJudge(){ return m_judge.activeSelf; }
    public GameObject GetWinObj(ItemID itemID) { return m_windows[(int)itemID]; } // ゲッター関数　岬追記

    /// <summary>
    /// アイテムウィンドウをスライドさせる関数 岬追記
    /// </summary>
    public void WinSlide()
    {
        // ItemOpenでスライドさせる方向を決める
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


    /// <summary>指定したアイテムウィンドウを開きます</summary>
    /// <param name="id">開くウィンドウのID</param>
    public void SetWindow(ItemID id,bool isOpen) { m_windows[(int)id].SetActive(isOpen);}

    public bool CheckOpen { get { return m_isOpen; } set { m_isOpen = value; } }


    private bool m_isOpen = false; // 岬追記　ウィンドウを開いているかどうか
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
