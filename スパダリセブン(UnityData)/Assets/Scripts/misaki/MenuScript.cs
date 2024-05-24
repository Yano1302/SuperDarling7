using UnityEngine;

public class MenuScript : SingletonMonoBehaviour <MenuScript>
{
    private bool openWindow = false; // ウィンドウを開いているかどうか
    [SerializeField] GameObject menuWindow; // メニューウィンドウ用変数
    [SerializeField] GameObject saveMessage; // セーブをした際に表示するテキスト変数
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
        sceneManager.audioManager.SE_Play("SE_item01", sceneManager.enviromentalData.TInstance.volumeSE);
        ItemManager itemManager = ItemManager.Instance;
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/プランナー監獄エリア/Json", "MasterData");
        JsonSettings<SettingsGetItemFlags> saveItemData = new JsonSettings<SettingsGetItemFlags>(string.Format("Data{0}", saveSlotIndex), "JsonSaveFile", "ItemGetFlags");
        // 現在のシーンを保存
        saveData.TInstance.scenename = sceneManager.CheckSceneName;
        // セーブしたことがあるをtrueにする
        if (saveData.TInstance.scenename != SCENENAME.TitleScene) saveData.TInstance.haveSaved = true;
        // 現在のアイテム取得情報を上書きする
        saveItemData = itemManager.UsingItemFlag;

        // セーブする
        saveData.Save();
        saveItemData.Save();
        Debug.Log("セーブします");
        // タイトルシーンの場合
        if (saveData.TInstance.scenename == SCENENAME.TitleScene)
        {
            sceneManager.saveSlot = saveSlotIndex; // 現在使用しているスロットを代入
            sceneManager.SceneChange(SCENENAME.StoryScene); // シーン遷移
            sceneManager.uiManager.CloseUI(UIType.SaveSlot); // UIを閉じる
        }
        // それ以外の場合はセーブテキストを表示
        else ActiveSaveMessage();
    }
    /// <summary>
    /// ロードボタンをクリックしたときの関数
    /// </summary>
    /// <param name="saveSlotIndex">セーブスロットの番号</param>
    public void Load(int saveSlotIndex)
    {
        ItemManager itemManager = ItemManager.Instance;
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/プランナー監獄エリア/Json", "MasterData");
        JsonSettings<SettingsGetItemFlags> saveItemData = new JsonSettings<SettingsGetItemFlags>(string.Format("Data{0}", saveSlotIndex), "JsonSaveFile", "ItemGetFlags");
        if (saveData.TInstance.haveSaved == false)
        {
            sceneManager.audioManager.SE_Play("SE_dungeon05");
            Debug.Log("セーブされていません");
            return; // 一度もセーブされたことがないのならリターン
        }
        sceneManager.audioManager.SE_Play("SE_item01", sceneManager.enviromentalData.TInstance.volumeSE);
        itemManager.UsingItemFlag = saveItemData; // ItemManagerで使用するアイテム取得フラグを上書きする
        // ロードしてシーン遷移
        sceneManager.saveSlot = saveSlotIndex;
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
    /// <summary>
    /// クリックした際にSEを鳴らす関数
    /// </summary>
    public void ClickSE()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
    }
    /// <summary>
    /// セーブメッセージを表示する関数
    /// </summary>
    private void ActiveSaveMessage()
    {
        saveMessage.SetActive(true);
    }
    /// <summary>
    /// セーブメッセージを非表示にする関数
    /// </summary>
    public void InactiveSaveMessage()
    {
        saveMessage.SetActive(false);
    }
}
