using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;
using TMPro;
using Unity.VisualScripting;

// シングルトンかも
public class TextController : MonoBehaviour
{
    private int talkNum = 0; // 文章の〇文字目
    //ストーリー番号
    [Header("1-1のように入力")]
    public string storynum;
    private string words; // 文章
    public TextMeshProUGUI textLabel; // 文章を格納するテキスト変数
    public TextMeshProUGUI buttonText; // ボタンのテキスト変数
    private GameObject[] charaImages; // csvファイルに記載されたキャラクター画像名を格納する配列
    private GameObject charaimage = null; // 使用するキャラクター画像
    public GameObject charaImageBack; // キャラクター背景
    public GameObject talkButton; // 会話を進めるボタン
    StoryTalkData[] storytalks; //csvファイルにある文章を格納する配列
    enum TALKSTATE // 会話関係のステータス
    {
        NOTALK, // 話していない
        TALKING, // 会話中
        LASTTALK // 最後のセリフ
    }
    [SerializeField]TALKSTATE talkState = TALKSTATE.NOTALK; // 会話ステータス変数

    void Awake()
    {
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。今回は Story1-1 や Story2-5 のようにステージ番号によって読み込むファイルが変えられるようにしている。
        textasset = Resources.Load("Story/Story" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storytalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        charaImages = new GameObject[storytalks.Length]; // キャラクター画像格納配列のサイズを文章の数と同じにする
        // プロジェクト内のTalkCharaImageフォルダにある画像を対応させたい文章ごとに格納する
        for (int i = 0; i < storytalks.Length; i++)
        {
            charaImages[i] = (GameObject)Resources.Load("TalkCharaImage/" + storytalks[i].talkingChara + "Talk");
        }
        /// ここまで ///
    }
    public void Start()
    {
        buttonText.text = "会話開始"; // ボタンテキストを"会話開始"に変更
    }
    // ボタンを押すと会話スタート
    public void OnTalkButtonClicked()
    {
        // 会話ステータスが話していないなら
        if (talkState == TALKSTATE.NOTALK)
        {
            buttonText.text = "次へ"; // ボタンテキストを"次へ"に変更
            talkState = TALKSTATE.TALKING; // 会話ステータスを会話中に変更
        }
        // 会話フィールドをリセットする。
        textLabel.text = "";
        if (charaimage != null) // キャラクター画像が表示されていれば
        {
            Destroy(charaimage); // 画像を破壊する
        }
        // トークボタンを非表示にする。
        if (talkState != TALKSTATE.LASTTALK)
        {
            //キャラクター画像を生成
            charaimage = Instantiate(charaImages[talkNum], charaImageBack.transform);
            StartCoroutine(Dialogue()); // 文章を表示するコルーチンを開始
            talkButton.SetActive(false);
        }
        else if (talkState == TALKSTATE.LASTTALK)
        {
            talkNum = default;
            talkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
            buttonText.text = "会話開始"; // ボタンテキストを"会話開始"に変更
        }
    }
    /// <summary>
    /// 文章を表示するコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator Dialogue()
    {
        // 文字列を取得
        words = storytalks[talkNum].talks;
        // 各文字に対して繰り返し処理を行います C#のIEnumerable機能により一文字ずつ取り出せる
        foreach (char c in words)
        {
            // 文字を textLabel に追加します
            textLabel.text += c;
            // 次の文字を表示する前に少し待ちます
            yield return new WaitForSeconds(0.05f); // 必要に応じてこの待ち時間を調整してください
        }
        // 次のダイアログに移動
        talkNum++;
        // すべてのダイアログを表示した後、追加のダイアログがあるかどうかをチェック
        if (talkNum >= storytalks.Length)
        {
            buttonText.text = "会話終了"; // ボタンテキストを"会話終了"に変更
            talkState = TALKSTATE.LASTTALK; // 会話ステータスを最後のセリフに変更
        }
        talkButton.SetActive(true); // talkButton を表示します

    }
}

[System.Serializable] // サブプロパティを埋め込む
public class StoryTalkData // StoryTalkDataの中にtalkingCharaとtalksを配置する
{
    public string talkingChara; // 話しているキャラクター名
    public string talks; // 文章
}
