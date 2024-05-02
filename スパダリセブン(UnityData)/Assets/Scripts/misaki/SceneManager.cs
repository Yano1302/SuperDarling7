using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // ディスプレイマネージャー用変数
        [SerializeField] SCENENAME currentSceneName; // 現在のシーン名
        [SerializeField] Scene currentScene; // 現在のシーン
        [SerializeField] MenuScript menuScript; // MenuScript変数
        [SerializeField] Button autoButton; // オートボタン変数
        StoryController controller; // ストーリーコントローラー変数
        public int saveSlot; // 現在使用しているセーブスロット
        public int stageNum; // ステージナンバー
        public UIManager uiManager; // UIマネージャー用変数
        public AudioManager audioManager; // オーディオマネージャー変数
        public SCENENAME CheckSceneName { get { return currentSceneName; } } // 現在のシーン名を取得
        public Scene CheckScene { get { return currentScene; } } // 現在のシーンを取得
        // セーブデータを読み込み
        public JsonSettings<EnvironmentalData> enviromentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData(0)", "/Resources/プランナー監獄エリア/Json", "EnvironmentalData");
        //public JsonSettings<EnvironmentalData> environmentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData", "/Resources/プランナー監獄エリア/Json");
        public JsonSettings<MasterData> saveData = new JsonSettings<MasterData>("MasterData", "/Resources/プランナー監獄エリア/Json");

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }
        // Start is called before the first frame update
        void Start()
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>(); // audioManagerを検索して代入
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            audioManager.BGM_Play("BGM_title", enviromentalData.m_tInstance.volumeBGM); // BGMを流す
            //KeyDebug.AddKeyDebug("GameOver画面へ遷移", GameOver);
            //KeyDebug.AddKeyDebug("GameClear画面へ遷移", GameClear);
            //KeyDebug.AddKeyDebug("調査画面へ遷移", Investigation);
        }
        /// <summary>
        /// イベントハンドラー　シーン遷移時の関数
        /// </summary>
        /// <param name="nextScene"></param>
        /// <param name="mode"></param>
        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            // 現在のシーンを代入する
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene(); 
            currentSceneName = (SCENENAME)currentScene.buildIndex;
            // ストーリーシーンであればストーリーメニューを出す
            if (currentSceneName == SCENENAME.StoryScene) uiManager.OpenUI(UIType.StoryMenu);
            else uiManager.CloseUI(UIType.StoryMenu);
            // 現在のシーンが探索シーンであれば
            if (currentSceneName == SCENENAME.Dungeon || currentSceneName == SCENENAME.InvestigationScene)
            {
                MapSetting setting = GameObject.FindGameObjectWithTag("MapSetting").GetComponent<MapSetting>(); // MapSettingを検索
                setting.CreateMap(stageNum); // マップを生成
            }
            else if (currentSceneName != SCENENAME.SolveScene) uiManager.CloseUI(UIType.ItemWindow); // アイテムウィンドウを閉じる
            // 各シーンでのBGMを流す ストーリーシーンはCSVデータを参照して流すのでここでは流さない
            // 依頼シーンに遷移したときのみ自動セーブ
            switch(currentSceneName)
            {
                case SCENENAME.TitleScene:
                    audioManager.BGM_Play("BGM_title", enviromentalData.m_tInstance.volumeBGM);
                    break;
                case SCENENAME.RequisitionsScene:
                    audioManager.BGM_Play("BGM_quest", enviromentalData.m_tInstance.volumeBGM);
                    menuScript.Save(saveSlot); // 現在使用しているスロットにセーブ
                    break;
                case SCENENAME.StoryScene:
                    audioManager.BGM_Stop();
                    controller = GameObject.FindGameObjectWithTag("Coroutine").GetComponent<StoryController>(); // ストーリーコントローラーを代入
                    autoButton.onClick.AddListener(controller.OnAutoModeCllicked); // OnClick関数にアタッチ
                    break;
                case SCENENAME.InvestigationScene:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.m_tInstance.volumeBGM);
                    break;
                case SCENENAME.SolveScene:
                    audioManager.BGM_Play("BGM_solve", enviromentalData.m_tInstance.volumeBGM);
                    break;
                case SCENENAME.Dungeon:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.m_tInstance.volumeBGM);
                    break;
                case SCENENAME.GameOverScene:
                    audioManager.BGM_Stop();
                    audioManager.SE_Play("BGM_gameover", enviromentalData.m_tInstance.volumeSE);
                    break;
                case SCENENAME.GameClearScene:
                    audioManager.BGM_Stop();
                    audioManager.SE_Play("BGM_clear", enviromentalData.m_tInstance.volumeSE);
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
        void GameOver()
        {
            SceneChange(SCENENAME.GameOverScene);
        }
        void GameClear()
        {
            SceneChange(SCENENAME.GameClearScene);
        }
        void Investigation()
        {
            SceneChange(SCENENAME.Dungeon);
        }
    }
}
