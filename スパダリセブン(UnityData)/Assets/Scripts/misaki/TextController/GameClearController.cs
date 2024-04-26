using System.Collections;
using UnityEngine;

public class GameClearController : BaseTextController
{
    [SerializeField] Animator animator; // スタンプのアニメーター
    // Start is called before the first frame update
    void Start()
    {
        charaName.text = storyTalks[talkNum].name; // 話しているキャラクター名を表示
    }
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。
        textasset = Resources.Load("プランナー監獄エリア/Json/GameClear/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }
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
    protected override void NextDialogue()
    {
        base.NextDialogue();
        if (TalkState == TALKSTATE.LASTTALK) animator.SetTrigger("Stamp");
    }
    /// <summary>
    /// タイトルへ戻るボタンをクリックしたときの関数
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす
        sceneManager.SceneChange(0); // タイトルシーンへ遷移する
    }
}
