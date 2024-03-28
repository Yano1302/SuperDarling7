using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour <SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // ディスプレイマネージャー用変数
        [SerializeField] UIManager uiManager; // UIマネージャー用変数
        [SerializeField] EnumList.SCENENAME currentSceneName; // 現在のシーン名
        [SerializeField] Scene currentScene; // 現在のシーン
        public EnumList.SCENENAME CheckSceneName { get { return currentSceneName; } }
        public Scene CheckScene { get { return currentScene; } }

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
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
            currentSceneName = (EnumList.SCENENAME)currentScene.buildIndex;
            // 現在のシーンが探索シーンであれば
            if (currentSceneName == EnumList.SCENENAME.Dungeon || currentSceneName == EnumList.SCENENAME.InvestigationScene)
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
        public void SceneChange(EnumList.SCENENAME LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene((int)LoadScene)); // フェードアウトする
        }

    }
}
