using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // ディスプレイマネージャー用変数
        [SerializeField] SCENENAME currentSceneName; // 現在のシーン名
        [SerializeField] Scene currentScene; // 現在のシーン
        [SerializeField] Button autoButton; // オートボタン変数
        public int saveSlot; // セーブスロット変数
        public int stageNum = 0; // ステージ番号
        BaseTextController controller; // ストーリーコントローラー変数
        public UIManager uiManager; // UIマネージャー用変数
        public AudioManager audioManager; // オーディオマネージャー変数
        public ItemWindow itemWindow; // アイテムウィンドウ変数
        public SCENENAME CheckSceneName { get { return currentSceneName; } } // 現在のシーン名を取得
        public Scene CheckScene { get { return currentScene; } } // 現在のシーンを取得
        // セーブデータを読み込み
        public JsonSettings<EnvironmentalData> enviromentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData(0)", "/Resources/プランナー監獄エリア/Json", "EnvironmentalData");
        //public JsonSettings<EnvironmentalData> environmentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData", "/Resources/プランナー監獄エリア/Json");
        //public JsonSettings<MasterData> saveData = new JsonSettings<MasterData>("MasterData", "/Resources/プランナー監獄エリア/Json");

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }
        void Start()
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
        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlot), "/Resources/プランナー監獄エリア/Json", "MasterData");
            // 現在のシーンを代入する
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene(); 
            currentSceneName = (SCENENAME)currentScene.buildIndex;
            // ストーリーシーンまたは解決シーンであればストーリーメニューを出す
            if (currentSceneName == SCENENAME.StoryScene || currentSceneName == SCENENAME.SolveScene) uiManager.OpenUI(UIType.StoryMenu);
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
                    saveData.Save(); // セーブする
                    break;
                case SCENENAME.StoryScene:
                    audioManager.BGM_Stop();
                    controller = GameObject.FindGameObjectWithTag("Coroutine").GetComponent<StoryController>();
                    autoButton.onClick.AddListener(controller.OnAutoModeCllicked);
                    break;
                case SCENENAME.InvestigationScene:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.TInstance.volumeBGM);
                    uiManager.OpenUI(UIType.ItemWindow);
                    break;
                case SCENENAME.SolveScene:
                    audioManager.BGM_Play("BGM_solve", enviromentalData.TInstance.volumeBGM);
                    controller = GameObject.FindGameObjectWithTag("Coroutine").GetComponent<SolveTextController>();
                    autoButton.onClick.AddListener(controller.OnAutoModeCllicked);
                    uiManager.OpenUI(UIType.ItemWindow);
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

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン番号</param>
        public void SceneChange(int LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // フェードアウトする
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン名</param>
        public void SceneChange(string LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // フェードアウトする
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        /// <param name="LoadScene">シーン名</param>
        public void SceneChange(SCENENAME LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene((int)LoadScene)); // フェードアウトする
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
    }
}
