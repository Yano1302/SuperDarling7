using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : DebugSetting
{
    private bool openWindow = false; // ウィンドウを開いているかどうか
    private Supadari.SceneManager sceneManager; // スパダリのシーンマネージャー用変数
    private AudioManager audioManager; // オーディオマネージャー変数
    public GameObject settingWindow; // 設定のウィンドウ変数
    
    protected override void Awake()
    {
        base.Awake();
        // SceneManagerとAudioManagerを探す
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    /// <summary>
    /// NewGameをクリックしたときの関数
    /// </summary>
    public void NewGame()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
        sceneManager.uiManager.OpenUI(UIType.SaveSlot);
        //sceneManager.SceneChange(1); // 依頼画面へシーン遷移する
        Debug.Log("ニューゲームを開始");
    }
    /// <summary>
    /// Continueをクリックしたときの関数
    /// </summary>
    public void Continue()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
        sceneManager.uiManager.OpenUI(UIType.LoadSlot); // ロードスロットを開く
        Debug.Log("コンティニューを開始");
        
    }
    /// <summary>
    /// SettingGameボタンをクリックしたときの関数
    /// </summary>
    public void EnterSetting()
    {
        // ウィンドウを開いているなら再度開かないようにリターン
        if (openWindow)
        {
            Debug.Log("openWindowが" + openWindow + "です");
            return;
        }
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
        settingWindow.SetActive(true); // ウィンドウを可視化する
        openWindow = true; // trueにする
    }
    /// <summary>
    /// ウィンドウを閉じるボタンを押したときの関数
    /// </summary>
    public void QuitSetting()
    {
        // ウィンドウを閉じているなら再度閉じないようにリターン
        if (!openWindow)
        {
            Debug.Log("openWindowが" + openWindow + "です");
            return;
        }
        sceneManager.enviromentalData.Save(); // 変更した設定を保存する
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
        settingWindow.SetActive(false); // ウィンドウを不可視にする
        openWindow = false; // falseにする
    }
    /// <summary>
    /// ゲームを終了する関数
    /// </summary>
    public void EndGame()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity上でのプレイを終了
#else
        Application.Quit(); // ゲームアプリケーションの終了
#endif
    }
}
