using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : DebugSetting
{
    private bool openWindow = false; // ウィンドウを開いているかどうか
    private Supadari.SceneManager sceneManager; // スパダリのシーンマネージャー用変数
    public GameObject settingWindow; // 設定のウィンドウ変数
    
    protected override void Awake()
    {
        base.Awake();
        // sceneManagerを探す
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    /// <summary>
    /// NewGameをクリックしたときの関数
    /// </summary>
    public void NewGame()
    {
        sceneManager.SceneChange(1); // 依頼画面へシーン遷移する
        Debug.Log("ニューゲームを開始");
    }
    /// <summary>
    /// Continueをクリックしたときの関数
    /// </summary>
    public void Continue()
    {
        sceneManager.SceneChange(2); // セーブしたシーンへ遷移する　ここをあとで変える
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
        settingWindow.SetActive(false); // ウィンドウを不可視にする
        openWindow = false; // falseにする
    }
    /// <summary>
    /// ゲームを終了する関数
    /// </summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity上でのプレイを終了
#else
        Application.Quit(); // ゲームアプリケーションの終了
#endif
    }
}
