using System.Collections;
using UnityEngine;

public class GameOverController : BaseTextController
{
    // Start is called before the first frame update
    void Start()
    {
        OnTalkButtonClicked("");
    }
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。
        textasset = Resources.Load("プランナー監獄エリア/GameOver/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }
    protected override IEnumerator Dialogue()
    {
        talkNum = Random.Range(0, storyTalks.Length); // 表示する文章をランダムで設定
        Debug.Log(storynum + "の" + (talkNum + 1) + "列目を再生");
        TalkState = TALKSTATE.TALKING; // 会話ステータスを話し中にする
        // charaName.text = storyTalks[talkNum].name; // 話しているキャラクター名を表示
        words = storyTalks[talkNum].talks; // 文章を取得
        // 各文字に対して繰り返し処理を行います C#のIEnumerable機能により一文字ずつ取り出せる
        foreach (char c in words)
        {
            // 文字を textLabel に追加します
            textLabel.text += c;
            // ボタンがクリックされたらフラグを立ててループを抜ける
            if (talkSkip) break;
            // 次の文字を表示する前に少し待ちます
            yield return new WaitForSeconds(CalculataTextSpeed());
        }
        NextDialogue(); // 次のダイアログに変更する
        if (talkAuto) // オートモードであれば
        {
            yield return new WaitForSeconds(textDelay); // textDelay秒待つ
            OnTalkButtonClicked(); // 次の会話を自動でスタートする
        }
    }
    public override void OnTalkButtonClicked(string storynum = "")
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
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
            TalkEnd(); //会話を終了する
        }
    }
    /// <summary>
    /// タイトルへ戻るボタンをクリックしたときの関数
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.m_tInstance.volumeSE); // SEを鳴らす
        sceneManager.SceneChange(0); // タイトルシーンへ遷移する
    }
    /// <summary>
    /// ロードスロットを開く関数
    /// </summary>
    public void LoadButton()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE); // SEを鳴らす
        sceneManager.uiManager.OpenUI(UIType.LoadSlot); // ロードスロットを表示
    }
}
