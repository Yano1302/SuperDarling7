using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
// 岬製作のシーンチェンジ+フェードインアウトを行うスクリプト
// maincameraなど常にいるものにアタッチすること推奨
// スクリプト「FadeManager」と共に使用する

/*public class SceneScript : SingletonMonoBehaviour<SceneScript>
{
    [SerializeField] GameObject fadeCanvas; //prefabのFadeCanvasを入れる
    public AudioSource SEAudioSource; // SE用オーディオソース

    protected override void Awake()
    {
        base.Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのGameSettingsを参照
        if (!FadeManager.isFadeInstance)  // フェード用Canvasが召喚できていなければ
        {
            Instantiate(fadeCanvas);     // Canvas召喚
        }
        Invoke("findFadeObject", 0.02f); // 起動時用にCanvasの召喚をちょっと待つ
    }
    void findFadeObject()                                      // 召喚したCanvasのフェードインフラグを立てる関数
    {
        fadeCanvas = GameObject.FindGameObjectWithTag("Fade"); // FadeCanvasを見つける
        fadeCanvas.GetComponent<FadeManager>().fadeIn();       // フェードイン関数を呼び出し
    }
    public async void sceneChange(string sceneName)       // シーンチェンジ関数　ボタン操作などで呼び出す
    {
        if (SEAudioSource) SEAudioSource.Play(); // 選択SEを鳴らす
        await Task.Delay(0);                           // 音が鳴るまで待つ
        fadeCanvas.GetComponent<FadeManager>().fadeOut(); // フェードアウト関数を呼び出し
        await Task.Delay(2000);                           // 暗転するまで待つ
        SceneManager.LoadScene(sceneName);                // シーンチェンジ
    }
}
*/
