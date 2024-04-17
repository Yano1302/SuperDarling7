using System.Collections;
using UnityEngine;

public class TestTextController : BaseTextController
{
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。
        textasset = Resources.Load("プランナー監獄エリア/Json/TestText/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }
    protected override IEnumerator Dialogue()
    {
        talkNum = default; // デフォルトに戻す
        Debug.Log(storynum + "の" + (talkNum + 1) + "列目を再生");
        TalkState = TALKSTATE.TALKING; // 会話ステータスを話し中にする
        words = storyTalks[talkNum].talks; // 文章を取得
        // 各文字に対して繰り返し処理を行います C#のIEnumerable機能により一文字ずつ取り出せる
        foreach (char c in words)
        {
            // 文字を textLabel に追加します
            textLabel.text += c;
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
        sceneManager.audioManager.SE_Play("SE_click");
        InitializeTalkField(); // 表示されているテキスト等を初期化
        StartDialogueCoroutine(); // 文章を表示するコルーチンを開始
    }
}
