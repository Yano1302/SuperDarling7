using Supadari;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemWindow : SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("アイテムウィンドウオブジェクト一覧"), EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;
    [SerializeField]
    private GameObject m_judge; // 岬追記　ジャッジ変数

    public Supadari.SceneManager sceneManager; // シーンマネージャー変数


    /// <summary>ウィンドウをアクティブ化します</summary>
    public void ActiveWindows() { machWindow();m_uiManager.OpenUI(UIType.ItemWindow); }
    /// <summary>ウィンドウを非アクティブ化します</summary>
    public void InactiveWindows() { m_uiManager.CloseUI(UIType.ItemWindow); }
    /// <summary>ジャッジ変数をアクティブ化します</summary>　岬追記
    public void ActiveJudge() { m_judge.SetActive(true); }
    /// <summary>ジャッジ変数を非アクティブ化します</summary>　岬追記
    public void InactiveJudge() { m_judge.SetActive(false); }
    public bool CheckJudge(){ return m_judge.activeSelf; }

    // ゲッター関数　岬追記
    public GameObject GetWinObj(ItemID itemID) { return m_windows[(int)itemID]; }

    /// <summary>
    /// アイテムウィンドウのスライドを行う関数　岬追記
    /// </summary>
    public void WinSlide()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす

        // ItemOpenを参照してスライド方向を決める
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

    public bool CheckOpen { get { return m_isOpen; } set { m_isOpen = value; } } // m_isOpenのゲッターセッター関数

    /// <summary>アイテムウィンドウのアクティブ状態を取得します</summary>
    public bool IsActiveItemWindow { get { return m_uiManager.ChekIsOpen(UIType.ItemWindow);  }  }

    private bool m_isOpen = false; // アイテムウィンドウが開いているかどうか

    private ItemManager IM; //インスタンス本体
    private UIManager UIM; //インスタンス本体
    private ItemManager m_itemManager { get { IM ??= ItemManager.Instance; return IM; } } //インスタンス取得
    private UIManager m_uiManager { get { UIM ??= UIManager.Instance; return UIM; } }　//インスタンス取得


    private void machWindow() {
        for (int i = 1; i < m_windows.Length; i++) {
            //所持アイテム情報とオブジェクトのアクティブ情報を一致させる
            m_windows[i].SetActive(m_itemManager.GetFlag((ItemID)i));
        }
    }
}
