using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;
using TMPro;
using Unity.VisualScripting;

public class TextController : DebugSetting
{
    private int talkNum = 0; // ダイヤログ番号
    [SerializeField]private float textBaseSpeed = 0.05f; // テキスト送りのベーススピード
    public float playerTextSpeed = 5f; // プレイヤーが指定したテキスト送りのスピード度合
    private bool talkSkip = false; // ボタンがクリックされたかどうかを示すフラグ
    [Header("1-1のように入力")]
    public string storynum; //ストーリー番号
    private string words; // 文章
    public TextMeshProUGUI charaName; // キャラクター名のテキスト変数
    public TextMeshProUGUI textLabel; // 文章を格納するテキスト変数
    public TextMeshProUGUI buttonText; // ボタンのテキスト変数
    private GameObject[] charaImages; // csvファイルに記載されたキャラクター画像名を格納する配列
    private GameObject charaimage = null; // 使用するキャラクター画像
    public GameObject charaImageBack; // キャラクター背景
    public GameObject talkButton; // 会話を進めるボタン
    StoryTalkData[] storytalks; //csvファイルにある文章を格納する配列
    public enum TALKSTATE // 会話関係のステータス
    {
        NOTALK, // 話していない
        TALKING, // 会話中
        NEXTTALK, // 次のセリフ
        LASTTALK // 最後のセリフ
    }
    private TALKSTATE talkState; // 会話ステータス変数
    public TALKSTATE TalkState
    {
        get { return talkState; }
        set
        {
            talkState = value;
            switch (talkState)
            {
                case TALKSTATE.NOTALK:
                    buttonText.text = "会話開始"; // ボタンテキストを"会話開始"に変更
                    break;
                case TALKSTATE.TALKING:
                    buttonText.text = "Skip"; // ボタンテキストを"Skip"に変更
                    break;
                case TALKSTATE.NEXTTALK:
                    buttonText.text = "次へ"; // ボタンテキストを"次へ"に変更
                    break;
                case TALKSTATE.LASTTALK:
                    buttonText.text = "会話終了"; // ボタンテキストを"会話終了"に変更
                    break;
            }
        }
    }
    protected override void Awake()
    {
        base .Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのGameSettingsを参照
        StorySetUp(storynum); // 対応する会話文をセットする
        TalkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
    }
    /// <summary>
    /// 対応する会話文をセットする関数
    /// </summary>
    /// <param name="storynum">読み込むCSVファイルの名前 例(1-1)</param>
    private void StorySetUp(string storynum)
    {
        Debug.Log("Story"+storynum+"を読み込みます");
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
        Debug.Log("Story" + storynum + "を読み込みました");
    }
    /// <summary>
    /// 会話に関するボタン関数
    /// </summary>
    /// <param name="storynum"></param>
    public void OnTalkButtonClicked(string storynum = "")
    {
        if (talkState == TALKSTATE.NOTALK) // 会話ステータスが話していないなら
        {
            // ストーリー番号があれば
            if (storynum != "") StorySetUp(storynum); // 対応する会話文をセット
            TalkState = TALKSTATE.TALKING; // 会話ステータスを会話中に変更
        }
        else if (talkState == TALKSTATE.TALKING) // 会話ステータスが話し中なら
        {
            talkSkip = true; // トークスキップフラグを立てる
            TalkState = TALKSTATE.NEXTTALK; // 会話ステータスを次のセリフに変更
            return;
        }
        textLabel.text = ""; // 会話フィールドをリセットする。
        if (charaimage != null) // キャラクター画像が表示されていれば
        {
            Destroy(charaimage); // 画像を破壊する
        }
        if (talkState != TALKSTATE.LASTTALK) // 会話ステータスが話し中なら
        {
            //キャラクター画像を生成
            charaimage = Instantiate(charaImages[talkNum], charaImageBack.transform);
            StartCoroutine(Dialogue()); // 文章を表示するコルーチンを開始
        }
        else if (talkState == TALKSTATE.LASTTALK) // 会話ステータスが最後のセリフなら
        {
            Debug.Log("会話を終了");
            talkNum = default; // リセットする
            TalkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
        }
    }
    /// <summary>
    /// 文章を表示するコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator Dialogue()
    {
        Debug.Log("Story" + storynum + "の" + (talkNum + 1) + "列目を再生");
        charaName.text = storytalks[talkNum].name; // 話しているキャラクター名を表示
        words = storytalks[talkNum].talks; // 文章を取得
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
        // トークスキップフラグが立ったら
        if (talkSkip == true) textLabel.text = storytalks[talkNum].talks; // 全文を表示
        talkNum++; // 次のダイアログに移動
        talkState = TALKSTATE.NEXTTALK; // 会話ステータスを次のセリフに変更
        talkSkip = false; // トークスキップフラグをfalseにする
        // 次のダイアログで最後なら会話ステータスを最後のセリフに変更
        if (talkNum >= storytalks.Length) TalkState = TALKSTATE.LASTTALK;
    }
    /// <summary>
    /// テキストスピードを計算する関数
    /// </summary>
    /// <returns></returns>
    private float CalculataTextSpeed()
    {
        // 基礎スピード*10段階のうちのどれか(5が基準)
        return textBaseSpeed / (playerTextSpeed / 5);
    }
}
[System.Serializable] // サブプロパティを埋め込む
public class StoryTalkData // StoryTalkDataの中にtalkingCharaとnameとtalksを配置する
{
    public string talkingChara; // キャラクター画像名
    public string name; // キャラクター名
    public string talks; // 文章
}