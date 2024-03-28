using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : SingletonMonoBehaviour <MenuScript>
{
    private bool openWindow = false; // ウィンドウを開いているかどうか
    [SerializeField] GameObject menuButton; // メニューボタン用変数
    [SerializeField] GameObject menuWindow; // メニューウィンドウ用変数
    [SerializeField] Supadari.SceneManager sceneManager; // スパダリのシーンマネージャー用変数
    private StoryController storyController = null; // ストーリーコントローラー用変数
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneCheck(); // シーンをチェックし表示か非表示かを決める
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneCheck(); // シーンをチェックし表示か非表示かを決める
    }
    /// <summary>
    /// シーンをチェック後、表示か非表示にする関数
    /// </summary>
    private void SceneCheck()
    {
        Scene scene = SceneManager.GetActiveScene(); // 現在のシーンを代入
        storyController = null; // nullを代入しリセット
        if (openWindow) Resume(); // メニューウィンドウを開いていれば閉じる
        if (scene.buildIndex == 0) menuButton.SetActive(false); // タイトルシーン時のみ非表示
        else menuButton.SetActive(true); // それ以外の場合表示
        // 依頼シーンの場合は代入する
        if (scene.buildIndex == 1) { }
        // ストーリーシーンの場合は代入する
        else if (scene.buildIndex == 2) storyController = GameObject.FindWithTag("Coroutine").GetComponent<StoryController>();
    }
    /// <summary>
    /// Menuボタンをクリックしたときの関数
    /// </summary>
    public void EnterMenu()
    {
        // ウィンドウを開いているなら再度開かないようにリターン
        if (openWindow)
        {
            UnityEngine.Debug.Log("openWindowが" + openWindow + "です");
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
            UnityEngine.Debug.Log("openWindowが" + openWindow + "です");
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
    public void Save()
    {
        UnityEngine.Debug.Log("セーブ機能はあとで作ります");
    }
    /// <summary>
    /// ロードボタンをクリックしたときの関数
    /// </summary>
    public void Load()
    {
        UnityEngine.Debug.Log("ロード機能はあとで作ります");
    }
    /// <summary>
    /// タイトルへ戻るボタンをクリックしたときの関数
    /// </summary>
    public void BackTitle()
    {
        sceneManager.SceneChange(0); // タイトルシーンへ遷移する
    }
}
