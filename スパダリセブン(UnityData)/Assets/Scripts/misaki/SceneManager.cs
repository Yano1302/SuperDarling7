using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour <SceneManager>
    {
        public DisplayManager displayManager; // ディスプレイマネージャー用変数

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneUnLoaded;
        }

        // Update is called once per frame
        void Update()
        {
        }
        public async void SceneChange(int LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire); // フェードアウトする
            await Task.Delay((int)displayManager.FadeTime * 1000); // 暗転するまで待つ(int型でミリ秒単位)
            UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene); // 指定のシーンに遷移する
        }
        void SceneUnLoaded(Scene scene)
        {
            displayManager.FadeIn(FadeType.Entire);
        }
    }
}
