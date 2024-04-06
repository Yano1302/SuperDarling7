using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // ディスプレイマネージャー用変数
        [SerializeField] UIManager uiManager; // UIマネージャー用変数
        [SerializeField] AudioManager audioManager; // オーディオマネージャー変数
        [SerializeField] SCENENAME currentSceneName; // 現在のシーン名
        [SerializeField] Scene currentScene; // 現在のシーン
        public SCENENAME CheckSceneName { get { return currentSceneName; } } // 現在のシーン名を取得
        public Scene CheckScene { get { return currentScene; } } // 現在のシーンを取得
        // セーブデータを読み込み
        public JsonSettings<MasterData> saveData = new JsonSettings<MasterData>("SaveData", "/Resources/プランナー監獄エリア/Json", "MasterData");


        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            audioManager.BGM_Play("BGM", saveData.m_tInstance.volumeBGM);
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
            // 現在のシーンが探索シーンであれば
            if (currentSceneName == SCENENAME.Dungeon || currentSceneName == SCENENAME.InvestigationScene)
            {
                MapSetting setting = GameObject.FindGameObjectWithTag("MapSetting").GetComponent<MapSetting>(); // MapSettingを検索
                setting.CreateMap(1); // マップを生成
                uiManager.OpenUI(UIType.Timer); // タイマーを表示
                uiManager.OpenUI(UIType.ItemWindow); // アイテムウィンドウを表示
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

    }
}
