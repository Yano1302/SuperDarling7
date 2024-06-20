using UnityEngine;

public partial class GameClearController : BaseTextController
{

    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///

    public override void OnTalkButtonClicked(string storynum = "")
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // 会話ステータスが話していないなら
        {
            // ストーリー番号があれば
            if (storynum != "") StorySetUp(storynum); // 対応する会話文をセット
            TalkState = TALKSTATE.TALKING; // 会話ステータスを会話中に変更
        }
        else if (TalkState == TALKSTATE.TALKING) // 会話ステータスが話し中なら
        {
            talkSkip = true; // トークスキップフラグを立てる
            TalkState = TALKSTATE.NEXTTALK; // 会話ステータスを次のセリフに変更
            return;
        }
        if (TalkState != TALKSTATE.LASTTALK) // 会話ステータスが話し中,なら
        {
            InitializeTalkField(); // 表示されているテキスト等を初期化
            StartDialogueCoroutine(); // 文章を表示するコルーチンを開始
        }
        else if (TalkState == TALKSTATE.LASTTALK) // 会話ステータスが最後のセリフなら
        {
            BackTitle(); //タイトルへ戻る
        }
    }

    /// <summary>
    /// タイトルへ戻るボタンをクリックしたときの関数
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす
        sceneManager.SceneChange(0); // タイトルシーンへ遷移する
    }

    /// -------public関数------- ///
    #endregion

    #region protected関数
    /// -----protected関数------ ///

    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。
        textasset = Resources.Load("プランナー監獄エリア/GameClear/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }

    protected override void NextDialogue()
    {
        base.NextDialogue();
        if (TalkState == TALKSTATE.LASTTALK) animator.SetTrigger("Stamp");
    }

    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///

    private void Start()
    {
        charaName.text = storyTalks[talkNum].name; // 話しているキャラクター名を表示
    }

    private void Update()
    {
        if (animEnd) ButtonActive();
    }

    /// <summary>
    /// ボタンの表示を行う関数
    /// </summary>
    private void ButtonActive()
    {
        if (buttonImage.activeSelf == false) buttonImage.SetActive(true);
    }

    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///
}
public partial class GameClearController
{
    /// --------変数一覧-------- ///

    #region public変数
    /// -------public変数------- ///



    /// -------public変数------- ///
    #endregion

    #region protected変数
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    #endregion

    #region private変数
    /// ------private変数------- ///

    private bool animEnd = false; // アニメーション終了通知

    [SerializeField] private Animator animator; // スタンプのアニメーター

    [SerializeField] private GameObject buttonImage; // ボタンのイメージ

    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///

    /// <summary>
    /// アニメーション終了通知セッター関数
    /// </summary>
    public bool AnimEnd { set { animEnd = value; } }


    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}