using UnityEngine;

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
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE);
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
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/プランナー監獄エリア/Json", "MasterData");
        // 現在のシーンを保存
        saveData.TInstance.scenename = sceneManager.CheckSceneName;
        // セーブしたことがあるをtrueにする
        if (saveData.TInstance.scenename != SCENENAME.TitleScene) saveData.TInstance.haveSaved = true;
        saveData.Save(); // セーブする
        Debug.Log("セーブします");
        // タイトルシーンの場合
        if (saveData.TInstance.scenename == SCENENAME.TitleScene)
        {
            sceneManager.saveSlot = saveSlotIndex; // 現在使用しているスロットを代入
            sceneManager.SceneChange(SCENENAME.StoryScene); // シーン遷移
            sceneManager.uiManager.CloseUI(UIType.SaveSlot); // UIを閉じる
        }
    }
    /// <summary>
    /// ロードボタンをクリックしたときの関数
    /// </summary>
    /// <param name="saveSlotIndex">セーブスロットの番号</param>
    public void Load(int saveSlotIndex)
    {
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/プランナー監獄エリア/Json", "MasterData");
        if (saveData.TInstance.haveSaved == false)
        {
            sceneManager.audioManager.SE_Play("SE_dungeon05");
            Debug.Log("セーブされていません");
            return; // 一度もセーブされたことがないのならリターン
        }
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        // ロードしてシーン遷移
        sceneManager.SceneChange(saveData.TInstance.scenename);
        sceneManager.uiManager.CloseUI(UIType.LoadSlot);
    }
    /// <summary>
    /// タイトルへ戻るボタンをクリックしたときの関数
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE);
        sceneManager.SceneChange(0); // タイトルシーンへ遷移する
        Resume();
    }
    public void ClickSE()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
    }
}
