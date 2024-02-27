using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;

// シングルトンかも
public class TextController : MonoBehaviour
{
    //ストーリー番号
    [Header("1-1のように入力")]
    public string storynum;

    private string[] words;
    public Text textLabel;

    private GameObject[] CharaImages;
    public GameObject CharaImageBack;
    private GameObject charaimage = null;

    public GameObject talkButton;
    private int talkNum = 0;

    //csvファイル用
    StoryTalkData[] storytalks;

    void Awake()
    {
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。今回は Story1-1 や Story2-5 のようにステージ番号によって読み込むファイルが変えられるようにしている。
        textasset = Resources.Load("Story" + storynum, typeof(TextAsset)) as TextAsset;
        //　CSVSerializerを用いてcsvファイルを配列に流し込む。
        storytalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text);

        CharaImages = new GameObject[storytalks.Length];

        for (int i = 0; i < storytalks.Length; i++)
        {
            CharaImages[i] = (GameObject)Resources.Load("TalkCharaImage/" + storytalks[i].talkingChara + "Talk");
        }
    }

    public void Start()
    {
        talkButton.SetActive(false);
        OnTalkButtonClicked();
    }

    // ボタンを押すと会話スタート
    public void OnTalkButtonClicked()
    {
        // 会話フィールドをリセットする。
        textLabel.text = "";

        //キャラクター画像を生成
        if (charaimage != null)
        {
            Destroy(charaimage);
        }

        charaimage = Instantiate(CharaImages[talkNum], CharaImageBack.transform);

        StartCoroutine(Dialogue());

        // トークボタンを非表示にする。
        talkButton.SetActive(false);
    }

    // コルーチンを使って、１文字ごと表示する。
    IEnumerator Dialogue()
    {
        // 半角スペースで文字を分割する。
        words = storytalks[talkNum].talks.Split(' ');

        foreach (var word in words)
        {
            // 0.1秒刻みで１文字ずつ表示する。
            textLabel.text = textLabel.text + word;
            yield return new WaitForSeconds(0.1f);
        }

        // 次のセリフがある場合には、トークボタンを表示する。
        if (talkNum + 1 < storytalks.Length)
        {
            talkButton.SetActive(true);
        }

        // 次のセリフをセットする。
        talkNum = talkNum + 1;
    }
}

[System.Serializable]
public class StoryTalkData
{
    public string talkingChara;
    public string talks;
}
