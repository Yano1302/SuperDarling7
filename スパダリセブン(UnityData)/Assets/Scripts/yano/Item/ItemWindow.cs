using Supadari;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemWindow : SingletonMonoBehaviour<ItemWindow>
{
    [SerializeField, Header("アイテムウィンドウオブジェクト一覧"), EnumIndex(typeof(ItemID))]
    private GameObject[] m_windows;
    [SerializeField]
    private GameObject m_window; // 岬追記　アイテムウィンドウ変数
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
    // ゲッター関数　岬追記
    public int GetWinLength() { return m_windows.Length; }

    /// <summary>
    /// アイテムウィンドウのスライドを行う関数　岬追記
    /// </summary>
    public void WinSlide()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす

        // ItemOpenを参照してスライド方向を決める
        if (CheckOpen == false)
        {
            m_window.transform.localPosition = new Vector3(0, 0, 0);
            CheckOpen = true;
            // 探索シーンの場合のみ
            if (sceneManager.CheckSceneName == SCENENAME.InvestigationScene)
            {
                TimerManager timerManager = TimerManager.Instance; // TimerManagerを取得
                InvManager invManager = InvManager.Instance; // InvManagerを取得
                timerManager.TimerFlag = false; // 制限時間を止める
                invManager.VigilanceFlag = false; // 警戒度上昇フラグを消す
            }
        }
        else if (CheckOpen == true)
        {
            m_window.transform.localPosition = new Vector3(1170, 0, 0);
            CheckOpen = false;
            // 探索シーンの場合のみ
            if (sceneManager.CheckSceneName == SCENENAME.InvestigationScene)
            {
                TimerManager timerManager = TimerManager.Instance; // TimerManagerを取得
                InvManager invManager = InvManager.Instance; // InvManagerを取得
                timerManager.TimerFlag = true; // 制限時間を動かす
                invManager.VigilanceFlag = true; // 警戒度上昇フラグを消す
            }
        }
    }

    public void machWindow()
    {
        for (int i = 1; i < m_windows.Length; i++)
        {
            //所持アイテム情報とオブジェクトのアクティブ情報を一致させる
            if (m_itemManager.GetFlag(((ItemID)i))) m_itemManager.AddItem((ItemID)i);
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



}
