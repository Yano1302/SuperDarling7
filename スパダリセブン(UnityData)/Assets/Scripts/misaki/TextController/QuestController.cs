using UnityEngine;

public class QuestController : BaseTextController
{
    [SerializeField] GameObject questUI; // 受注画面UI
    [SerializeField] GameObject selectUI; // 依頼を選択するUI
    /// <summary>
    /// 依頼内容を表示する関数
    /// </summary>
    /// <param name="storynum">表示したいCSVファイル名</param>
    public void ViewQuest(string storynum)
    {
        selectUI.SetActive(false); // 依頼選択画面を非表示
        questUI.SetActive(true); // 受注画面を表示
        StorySetUp(storynum); // 指定されたCSVファイルをセットアップ
        charaName.text = storyTalks[talkNum].name; // 依頼名を表示
        textLabel.text = storyTalks[talkNum].talks; // 依頼内容を表示
    }
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。
        textasset = Resources.Load("プランナー監獄エリア/Requisition/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }
}
