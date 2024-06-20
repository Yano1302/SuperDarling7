using UnityEngine;

public partial class TitleManager : DebugSetting
{
    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///

    /// <summary>
    /// NewGameをクリックしたときの関数
    /// </summary>
    public void NewGame()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        sceneManager.uiManager.OpenUI(UIType.SaveSlot); // セーブスロットを開く
        //sceneManager.SceneChange(SCENENAME.GameClearScene);
        Debug.Log("ニューゲームを開始");
    }

    /// <summary>
    /// Continueをクリックしたときの関数
    /// </summary>
    public void Continue()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        sceneManager.uiManager.OpenUI(UIType.LoadSlot); // ロードスロットを開く
        //sceneManager.SceneChange(SCENENAME.GameOverScene);
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
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
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
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        settingWindow.SetActive(false); // ウィンドウを不可視にする
        openWindow = false; // falseにする
    }

    /// <summary>
    /// ゲームを終了する関数
    /// </summary>
    public void EndGame()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity上でのプレイを終了
#else
        Application.Quit(); // ゲームアプリケーションの終了
#endif
    }

    /// -------public関数------- ///
    #endregion

    #region protected関数
    /// -----protected関数------ ///

    protected override void Awake()
    {
        base.Awake();
        // SceneManagerとAudioManagerを探す
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///



    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///
}
public partial class TitleManager
{
    /// --------変数一覧-------- ///

    #region public変数
    /// -------public変数------- ///

    public GameObject settingWindow; // 設定のウィンドウ変数

    /// -------public変数------- ///
    #endregion

    #region protected変数
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    #endregion

    #region private変数
    /// ------private変数------- ///

    private bool openWindow = false; // ウィンドウを開いているかどうか

    private Supadari.SceneManager sceneManager; // スパダリのシーンマネージャー用変数

    private AudioManager audioManager; // オーディオマネージャー変数

    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}