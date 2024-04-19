using System.Collections;
using UnityEngine;

public class SolveTextController : BaseTextController
{
    private void Start()
    {
        OnTalkButtonClicked(0);
    }
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。
        textasset = Resources.Load("プランナー監獄エリア/Solve/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }
    protected override IEnumerator Dialogue()
    {
        Debug.Log(storynum + "の" + (talkNum + 1) + "列目を再生");
        words = storyTalks[talkNum].talks; // 文章を取得
        // 文字を textLabel に追加します
        textLabel.text = words;
        yield return true;
    }
    public override void OnTalkButtonClicked(int num = 9999)
    {
        // numがstoryTalks.length以上または現talkNumと同じかつnum==0ではないならリターン
        if (num >= storyTalks.Length || num == talkNum && num != 0) return;
        talkNum = num; // 指定された値を代入
        sceneManager.audioManager.SE_Play("SE_click");
        InitializeTalkField(); // 表示されているテキスト等を初期化
        StartDialogueCoroutine(); // 文章を表示するコルーチンを開始
    }
}
