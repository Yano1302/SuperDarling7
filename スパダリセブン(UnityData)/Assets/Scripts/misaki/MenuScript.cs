using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : SingletonMonoBehaviour <MenuScript>
{
    private bool openWindow = false; // ウィンドウを開いているかどうか
    [SerializeField] GameObject menuWindow; // メニューウィンドウ用変数
    [SerializeField] Supadari.SceneManager sceneManager; // スパダリのシーンマネージャー用変数
    private StoryController storyController = null; // ストーリーコントローラー用変数
    protected override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// Menuボタンをクリックしたときの関数
    /// </summary>
    public void EnterMenu()
    {
        // ウィンドウを開いているなら再度開かないようにリターン
        if (openWindow)
        {
            Debug.Log("openWindowが" + openWindow + "です");
            return;
        }
        menuWindow.SetActive(true); // ウィンドウを可視化する
        openWindow = true; // trueにする
        Time.timeScale = 0; // タイムスケールを0にしてFixedUpdateを止める
        if (storyController) storyController.PauseDialogueCoroutine(); // storyControllerのコルーチンを一時停止
    }
    /// <summary>
    /// ゲーム再開ボタンを押したときの関数
    /// </summary>
    public void Resume()
    {
        // ウィンドウを閉じているなら再度閉じないようにリターン
        if (!openWindow)
        {
            Debug.Log("openWindowが" + openWindow + "です");
            return;
        }
        menuWindow.SetActive(false); // ウィンドウを不可視にする
        openWindow = false; // falseにする
        Time.timeScale = 1; // タイムスケールを0にしてFixedUpdateを止める
        if (storyController) storyController.ResumeDialogueCoroutine(); // storyControllerのコルーチンを再開
    }
    /// <summary>
    /// セーブボタンをクリックしたときの関数
    /// </summary>
    /// <param name="saveSlotIndex">セーブスロットの番号</param>
    public void Save(int saveSlotIndex)
    {
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}",saveSlotIndex), "/Resources/プランナー監獄エリア/Json", "MasterData");
        // 現在のシーンを保存
        saveData.m_tInstance.scenename = sceneManager.CheckSceneName;
        // セーブしたことがあるをtrueにする
        saveData.m_tInstance.haveSaved = true;
        saveData.Save();
    }
    /// <summary>
    /// ロードボタンをクリックしたときの関数
    /// </summary>
    /// <param name="saveSlotIndex">セーブスロットの番号</param>
    public void Load(int saveSlotIndex)
    {
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/プランナー監獄エリア/Json", "MasterData");
        if (saveData.m_tInstance.haveSaved == false)
        {
            Debug.Log("セーブされていません");
            return; // 一度もセーブされたことがないのならリターン
        }
        // ロードしてシーン遷移
        sceneManager.SceneChange(saveData.m_tInstance.scenename);
    }
    /// <summary>
    /// タイトルへ戻るボタンをクリックしたときの関数
    /// </summary>
    public void BackTitle()
    {
        sceneManager.SceneChange(0); // タイトルシーンへ遷移する
        Resume();
    }
}
