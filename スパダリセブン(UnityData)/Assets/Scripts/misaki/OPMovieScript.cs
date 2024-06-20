using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class OPMovieScript : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera = null;            // 動画描画場所
    [SerializeField]
    private TextMeshProUGUI stateText = null;    // 動画ステータス表示
    [SerializeField]
    private Button button = null;                // 動画用ボタン
    [SerializeField]
    private TextMeshProUGUI buttonText = null;   // ボタンテキスト
    [SerializeField]
    private Button tutorialButton = null;        // チュートリアル遷移ボタン
    [SerializeField]
    private Button startButton = null;           // バトル画面遷移ボタン
    [SerializeField]
    private Button CreditButton = null;          // クレジット画面遷移ボタン

    private Art.Sample.VideoPlay opVideo = null; // 動画用namespase
    public AudioSource AS_TitleBGM;              // BGMオーディオソース
    public AudioClip TitleBGM;                   // タイトルBGM

    void Awake()
    {
        StartMovie(); // スタートムービー関数呼び出し
    }

    public void OnClick() // 動画用ボタン関数
    {
        if (buttonText.text == "Play")        //Playなら
        {
            StartMovie();                     // スタートムービー関数呼び出し
        }
        else if (buttonText.text == "Pause")  // Pauseなら
        {
            Pause();                          // ポーズ関数呼び出し
        }
        else if (buttonText.text == "Resume") // Resumeなら
        {
            Resume();                         // レジュメ関数呼び出し
        }
    }

    private void StartMovie() // スタートムービー関数
    {
        stateText.text = "ロード中です。"; // ロード中ですを表示
        button.interactable = false;       // ボタンが作動しないようにする

        // インスタンスの生成
        
        if (opVideo == null)
        {
            opVideo = Art.Sample.VideoPlay.Create(mainCamera);
        }

        // イベント設定
        
        opVideo.OnPrepareCompleted = PrepareCompleted;
        opVideo.OnStarted = Started;
        opVideo.OnEnd = PlayEnd;
        opVideo.OnErrorReceived = ErrorReceived;

        opVideo.SetEnabled(true); // 動画を表示

        opVideo.Preload(Application.streamingAssetsPath + "/openingmovie.mp4"); // 指定の動画を挿入
    }

    private void Pause() // ポーズ関数
    {
        stateText.text = "Paused";
        buttonText.text = "Resume";
        
        opVideo.Pause(); // ポーズ関数呼び出し
    }

    private void Resume() // レジュメ関数
    {
        buttonText.text = "Pause";
        
        opVideo.Resume(); // レジュメ関数呼び出し
    }

    private void Started() // 動画再生後後関数
    {
        
          if (opVideo.State == Art.Sample.VideoPlay.PlayState.Playing) // 動画が再生中なら
          { 
            button.interactable = true;                                // 一時停止ボタンが作動するようにする
            buttonText.text = "Pause";
          }
        stateText.text = "Started";

        Debug.Log("Started");
    }

    private void PlayEnd() // 再生終了関数
    {
        stateText.text = "PlayEnd";
        Debug.Log("PlayEnd");


        if (opVideo.State == Art.Sample.VideoPlay.PlayState.Loaded) // 動画を読み込み中なら
        {
            buttonText.text = "Stop";

            button.interactable = false;                            // ボタンが作動しないようにする
            opVideo.PlayPrepared(false, true);
        }
        else if (opVideo.State == Art.Sample.VideoPlay.PlayState.Stoped) // 動画が終了後なら
        {
            buttonText.text = "Play";
            opVideo.SetEnabled(false);                                   // 動画を非表示にする
            startButton.interactable = true;                             // バトル画面遷移ボタンが作動するようにする
            tutorialButton.interactable = true;                          // チュートリアル遷移ボタンが作動するようにする
            CreditButton.interactable = true;                            // クレジット画面遷移ボタンが作動するようにする
            button.interactable = true;                                  // 再生ボタンが作動するようにする
            AS_TitleBGM.PlayOneShot(TitleBGM);                           // タイトルBGMを流す
        }
    }

    private void ErrorReceived(string message) // エラーデバッグ関数
    {
        stateText.text = "ErrorReceived";
        Debug.LogError("ErrorReceived : " + message);
    }

    private void PrepareCompleted() // 事前ダウンロードデバッグ関数
    {
        stateText.text = "PrepareCompleted";
        Debug.Log("PrepareCompleted");

        
        if (opVideo.State == Art.Sample.VideoPlay.PlayState.Loaded)
        {
            opVideo.PlayPrepared(false, true);
        }
    }
}