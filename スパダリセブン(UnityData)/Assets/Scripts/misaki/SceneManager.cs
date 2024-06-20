using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Supadari
{
    public partial class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン番号</param>
        public void SceneChange(int LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire, () => UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // フェードアウトする
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン名</param>
        public void SceneChange(string LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire, () => UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // フェードアウトする
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン名</param>
        public void SceneChange(SCENENAME LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire, () => UnityEngine.SceneManagement.SceneManager.LoadScene((int)LoadScene)); // フェードアウトする
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン名</param>
        /// <param name="action">この関数はシーン遷移直前に呼ばれます</param>
        public void SceneChange(SCENENAME LoadScene, UnityAction action)
        {
            displayManager.FadeOut(FadeType.Entire, () => { action?.Invoke(); UnityEngine.SceneManagement.SceneManager.LoadScene((int)LoadScene); }); // フェードアウトする
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void Start()
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>(); // audioManagerを検索して代入
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            audioManager.BGM_Play("BGM_title", enviromentalData.TInstance.volumeBGM); // BGMを流す
        }

        /// <summary>
        /// イベントハンドラー　シーン遷移時の関数
        /// </summary>
        /// <param name="nextScene"></param>
        /// <param name="mode"></param>
        private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlot), "JsonSaveFile", "MasterData");

            // 現在のシーンを代入する
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            currentSceneName = (SCENENAME)currentScene.buildIndex;
            // ストーリーシーンまたは解決シーンであればストーリーメニューを出す
            if (currentSceneName == SCENENAME.StoryScene || currentSceneName == SCENENAME.SolveScene)
            {
                uiManager.OpenUI(UIType.StoryMenu);
                slideWindow.InitializeStoryMenu(); // メニューを初期化する
            }
            else uiManager.CloseUI(UIType.StoryMenu);
            // 現在のシーンが探索シーンであれば
            if (currentSceneName == SCENENAME.Dungeon || currentSceneName == SCENENAME.InvestigationScene)
            {
                MapManager setting = MapManager.Instance; // MapManagerのインスタンス取得　※矢野変更
                setting.CreateMap(stageNum); // マップを生成
            }
            // 探索シーン以外の場合
            if (currentSceneName != SCENENAME.InvestigationScene) TimerManager.Instance.CloseTimer(true);   // タイマーを閉じる　※矢野追記
            // 解決シーン以外の場合
            if (currentSceneName != SCENENAME.SolveScene)
            {
                uiManager.CloseUI(UIType.ItemWindow); // アイテムウィンドウを閉じる
                // ジャッジオブジェクトを非表示にする
                if (itemWindow.CheckJudge() == true) itemWindow.InactiveJudge();
            }

            // 各シーンでの個別設定 ストーリーシーンはCSVデータを参照して流すのでここでは流さない
            switch (currentSceneName)
            {
                case SCENENAME.TitleScene:
                    audioManager.BGM_Play("BGM_title", enviromentalData.TInstance.volumeBGM);
                    break;
                case SCENENAME.RequisitionsScene:
                    audioManager.BGM_Play("BGM_quest", enviromentalData.TInstance.volumeBGM);
                    saveData.TInstance.scenename = currentSceneName; // 現在のシーンを代入
                    if (!saveData.TInstance.haveSaved) saveData.TInstance.haveSaved = true; // セーブされたことがなかったらtrueにする
                    saveData.Save(); // セーブする
                    ItemManager.Instance._Reset(); // アイテム取得情報をリセット
                    break;
                case SCENENAME.StoryScene:
                    audioManager.BGM_Stop();
                    controller = GameObject.FindGameObjectWithTag("Coroutine").GetComponent<StoryController>();
                    autoButton.onClick.AddListener(controller.OnAutoModeCllicked);
                    break;
                case SCENENAME.InvestigationScene:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.TInstance.volumeBGM);
                    if (uiManager.ChekIsOpen(UIType.ItemWindow) == true) uiManager.CloseUI(UIType.ItemWindow);
                    uiManager.OpenUI(UIType.ItemWindow);
                    break;
                case SCENENAME.SolveScene:
                    audioManager.BGM_Play("BGM_solve", enviromentalData.TInstance.volumeBGM);
                    saveData.TInstance.scenename = currentSceneName; // 現在のシーンを代入
                    if (!saveData.TInstance.haveSaved) saveData.TInstance.haveSaved = true; // セーブされたことがなかったらtrueにする
                    saveData.Save(); // セーブする
                    controller = GameObject.FindGameObjectWithTag("Coroutine").GetComponent<SolveTextController>();
                    autoButton.onClick.AddListener(controller.OnAutoModeCllicked);
                    if (uiManager.ChekIsOpen(UIType.ItemWindow) == true) uiManager.CloseUI(UIType.ItemWindow);
                    uiManager.OpenUI(UIType.ItemWindow);
                    // アイテムウィンドウとアイテム所持情報を一致させる
                    itemWindow.machWindow();
                    // ジャッジオブジェクトを表示する
                    if (itemWindow.CheckJudge() == false) itemWindow.ActiveJudge();
                    break;
                case SCENENAME.Dungeon:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.TInstance.volumeBGM);
                    break;
                case SCENENAME.GameOverScene:
                    audioManager.BGM_Stop();
                    audioManager.SE_Play("BGM_gameover", enviromentalData.TInstance.volumeSE);
                    break;
                case SCENENAME.GameClearScene:
                    audioManager.BGM_Stop();
                    audioManager.SE_Play("BGM_clear", enviromentalData.TInstance.volumeSE);
                    break;
            }
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class SceneManager
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        public int saveSlot; // セーブスロット変数
        public int stageNum = 0; // ステージ番号

        public UIManager uiManager; // UIマネージャー用変数

        public AudioManager audioManager; // オーディオマネージャー変数

        public ItemWindow itemWindow; // アイテムウィンドウ変数

        public SCENENAME CheckSceneName { get { return currentSceneName; } } // 現在のシーン名を取得

        public Scene CheckScene { get { return currentScene; } } // 現在のシーンを取得

        // セーブデータを読み込み
        public JsonSettings<EnvironmentalData> enviromentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData(0)", "JsonSaveFile", "EnvironmentalData");
        //public JsonSettings<EnvironmentalData> enviromentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData(0)", "/Resources/プランナー監獄エリア/Json", "EnvironmentalData");
        //public JsonSettings<EnvironmentalData> environmentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData", "/Resources/プランナー監獄エリア/Json");
        //public JsonSettings<MasterData> saveData = new JsonSettings<MasterData>("MasterData", "/Resources/プランナー監獄エリア/Json");


        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private BaseTextController controller; // ストーリーコントローラー変数

        [SerializeField] private DisplayManager displayManager; // ディスプレイマネージャー用変数

        [SerializeField] private SCENENAME currentSceneName; // 現在のシーン名

        [SerializeField] private Scene currentScene; // 現在のシーン

        [SerializeField] private Button autoButton; // オートボタン変数

        [SerializeField] private SlideWindow slideWindow; // スライドウィンドウ変数

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}
