using UnityEngine;

public partial class QuestController : BaseTextController
{

    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///

    /// <summary>
    /// 依頼内容を表示する関数
    /// </summary>
    /// <param name="storynum">表示したいCSVファイル名</param>
    public void ViewQuest(string storynum)
    {
        // 受注画面を表示し、依頼選択画面を非表示にする
        questUI.SetActive(true);
        selectUI.SetActive(false);
        StorySetUp(storynum); // 指定されたCSVファイルをセットアップ
        charaName.text = storyTalks[talkNum].name; // 依頼名を表示
        textLabel.text = storyTalks[talkNum].talks; // 依頼内容を表示
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
        textasset = Resources.Load("プランナー監獄エリア/Requisition/" + storynum, typeof(TextAsset)) as TextAsset;
        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する

        Debug.Log(storynum + "を読み込みました");
    }

    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///



    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///

}
public partial class QuestController
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

    [SerializeField] private GameObject questUI; // 受注画面UI
    [SerializeField] private GameObject selectUI; // 依頼を選択するUI

    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}