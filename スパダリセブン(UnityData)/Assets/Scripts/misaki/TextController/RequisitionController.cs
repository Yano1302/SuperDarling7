using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class RequisitionController : BaseTextController
{
    [SerializeField] GameObject questUI; // 受注画面UI
    [SerializeField] GameObject selectUI; // 依頼を選択するUI
    [SerializeField] Button[] questButtons; // 受注画面のボタン配列
    private void Start()
    {
        OnTalkButtonClicked();
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
        // 会話中背景格納配列のサイズを[文章の数]にする
        backImages = new GameObject[storyTalks.Length];
        // キャラクター画像格納用2次元配列のサイズを[会話中の最大表示人数,文章の数]にする
        charaImages = new GameObject[displayCharaAnchors, storyTalks.Length];
        // キャラクターハイライト格納用2次元配列のサイズを[会話中の最大表示人数,文章の数]にする
        charaHighlight = new bool[displayCharaAnchors, storyTalks.Length];
        // プロジェクト内のTalkCharaImageフォルダにある画像を対応させたい文章ごとに格納する
        for (int i = 0; i < storyTalks.Length; i++)
        {
            backImages[i] = (GameObject)Resources.Load("TalkBackImage/" + storyTalks[i].backImage);
        }
        // プロジェクト内のTalkCharaImageフォルダにある画像を対応させたい文章ごとに格納する
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            for (int j = 0; j < storyTalks.Length; j++)
            {
                // charaImages[0,j]には中央に表示するキャラクター画像を格納する
                if (i == 0) charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].centerTalkingChara);
            }
        }
        // charaImages[]に格納された画像が光るかどうかの真偽を対応させたい文章ごとに格納する
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            for (int j = 0; j < storyTalks.Length; j++)
            {
                // charaHighlight[0,j]には中央に表示するキャラクター画像が光るかどうかを格納する
                if (i == 0 && storyTalks[j].centerHighlight == "1") charaHighlight[i, j] = true;
            }
        }
        /// ここまで ///
        Debug.Log(storynum + "を読み込みました");
    }
    protected override void InstantiateActors()
    {
        // 背景を生成
        if (backImages[talkNum]) backImage = Instantiate(backImages[talkNum], backImageAnchor.transform);
        // キャラクター画像を生成
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (!charaImages[i, talkNum]) continue; // nullならコンティニューする
            // 中央にキャラクター画像を生成
            if (i == 0) centerCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
        }
        // 発言者以外のキャラクター画像を灰色にする
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (charaHighlight[i, talkNum]) continue; // nullならコンティニューする
            // 中央のキャラクター画像を灰色にする
            if (i == 0 && centerCharaImage) centerCharaImage.GetComponent<Image>().color = Color.gray;
        }
    }
    public override void TalkEnd()
    {
        Debug.Log("会話を終了");
        talkNum = default; // リセットする
        if (TalkState == TALKSTATE.LASTTALK)
        {
            sceneManager.stageNum = int.Parse(storyTalks[talkNum].stage); // ステージ番号をCSVから取得
            sceneManager.SceneChange(SCENENAME.InvestigationScene); // 探索シーンに遷移
            return;
        }
        TalkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
    }
    public override void OnTalkButtonClicked(string storynum = "")
    {
        base.OnTalkButtonClicked(storynum);
        if (questUI.activeSelf == true)
        {
            questUI.SetActive(false); // 受注画面を非表示
            selectUI.SetActive(true); // 依頼選択画面を表維持
        }
    }
    protected override async void NextDialogue()
    {
        base.NextDialogue();
        if (TalkState == TALKSTATE.LASTTALK) // 会話ステータスが最後なら
        {
            // 受注画面のボタンを無効にする
            for (int i = 0; i < questButtons.Length; i++)
            {
                questButtons[i].interactable = false;
            }
            await Task.Delay(1000); // 一秒止める
            TalkEnd(); // 次のシーンへ遷移する
        }
    }
}
