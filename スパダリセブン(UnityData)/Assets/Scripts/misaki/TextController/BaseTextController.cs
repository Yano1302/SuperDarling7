using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Supadari;

public class BaseTextController : DebugSetting
{
    protected int talkNum = 0; // ダイヤログ番号
    public int displayCharaAnchors = 3; // キャラクター画像の表示箇所数
    [SerializeField]private float textBaseSpeed = 0.05f; // テキスト送りのベーススピード
    public float playerTextSpeed = 0.5f; // プレイヤーが指定したテキスト送りのスピード度合
    public float textDelay = 1.5f; // テキストとテキストの間の時間(オートモードのみ使用)
    protected bool talkSkip = false; // ボタンがクリックされたかどうかを示すフラグ
    protected bool talkAuto = false; // 会話がオート状態なのかを示すフラグ
    [Header("1-1のように入力")]
    public string storynum; //ストーリー番号
    protected string words; // 文章
    private int currentCharIndex = 0; // 文章の表示位置を示す変数
    public TextMeshProUGUI charaName; // キャラクター名のテキスト変数
    public TextMeshProUGUI textLabel; // 文章を格納するテキスト変数
    public TextMeshProUGUI buttonText; // ボタンのテキスト変数

    protected GameObject[] backImages; // csvファイルに記載された背景の格納配列
    protected GameObject backImage = null; // 使用する背景画像
    [Header("背景表示箇所")]
    [SerializeField] protected GameObject backImageAnchor; // 背景表示箇所

    protected bool[,] charaHighlight; // csvファイルに記載されたキャラクターを光らせるかを格納する2次元配列
    protected GameObject[,] charaImages; // csvファイルに記載されたキャラクター画像名を格納する2次元配列
    private GameObject leftCharaImage = null; // 使用するキャラクター画像(左側)
    private GameObject rightCharaImage = null; // 使用するキャラクター画像(右側)
    protected GameObject centerCharaImage = null; // 使用するキャラクター画像(中央側)
    [Header("キャラクター表示箇所 [0]...左側 [1]...右側 [2]...中央")]
    [SerializeField] protected GameObject[] charaAnchors = new GameObject[3]; // キャラクター表示箇所

    string[] nameBGM; // 鳴らすBGM格納配列

    public GameObject talkButton; // 会話を進めるボタン
    protected StoryTalkData[] storyTalks; //csvファイルにある文章を格納する配列

    public bool runtimeCoroutine = false; // コルーチンが実行中かどうか
    private Coroutine dialogueCoroutine; // コルーチンを格納する変数

    [SerializeField]protected SceneManager sceneManager; // シーンマネージャー変数
    [SerializeField]bool testText = false; // ボタンのテキストを表示するかどうか
    [SerializeField] GameObject autoImage; // オートモードの画像

    public TALKSTATE talkState; // 会話ステータス変数
    public TALKSTATE TalkState
    {
        get { return talkState; }
        set
        {
            talkState = value;
            switch (talkState)
            {
                case TALKSTATE.NOTALK:
                    if(testText) buttonText.text = "会話開始"; // ボタンテキストを"会話開始"に変更
                    break;
                case TALKSTATE.TALKING:
                    if (testText) buttonText.text = "Skip"; // ボタンテキストを"Skip"に変更
                    break;
                case TALKSTATE.NEXTTALK:
                    if (testText) buttonText.text = "次へ"; // ボタンテキストを"次へ"に変更
                    break;
                case TALKSTATE.LASTTALK:
                    if (testText) buttonText.text = "会話終了"; // ボタンテキストを"会話終了"に変更
                    break;
            }
        }
    }
    protected override void Awake()
    {
        base .Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのGameSettingsを参照
        if(storynum!="") StorySetUp(storynum); // 対応する会話文をセットする
        TalkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>(); // オーディオマネージャーを取得
        playerTextSpeed = sceneManager.enviromentalData.TInstance.textSpeed; // テキストスピードを設定
    }
    /// <summary>
    /// 対応する会話文をセットする関数
    /// </summary>
    /// <param name="storynum">読み込むCSVファイルの名前 例(1-1)</param>
    protected virtual void StorySetUp(string storynum)
    {
        Debug.Log("Story"+storynum+"を読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。今回は Story1-1 や Story2-5 のようにステージ番号によって読み込むファイルが変えられるようにしている。
        textasset = Resources.Load("プランナー監獄エリア/Story/Story" + storynum, typeof(TextAsset)) as TextAsset;
        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSVのテキストデータを配列に格納する
        // 会話中背景格納配列のサイズを[文章の数]にする
        backImages = new GameObject[storyTalks.Length];
        // キャラクター画像格納用2次元配列のサイズを[会話中の最大表示人数,文章の数]にする
        charaImages = new GameObject[displayCharaAnchors, storyTalks.Length];
        // キャラクターハイライト格納用2次元配列のサイズを[会話中の最大表示人数,文章の数]にする
        charaHighlight = new bool[displayCharaAnchors, storyTalks.Length];
        // BGMを格納する配列のサイズを[文章の数]にする
        nameBGM = new string[storyTalks.Length];
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
                // charaImages[0,j]には左側に表示するキャラクター画像を格納する
                if (i == 0) charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].leftTalkingChara);
                // charaImages[1,j]には右側に表示するキャラクター画像を格納する
                else if (i == 1) charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].rightTalkingChara);
                // charaImages[2,j]には中央に表示するキャラクター画像を格納する
                else charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].centerTalkingChara);
            }
        }
        // charaImages[]に格納された画像が光るかどうかの真偽を対応させたい文章ごとに格納する
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            for (int j = 0; j < storyTalks.Length; j++)
            {
                // charaHighlight[0,j]には左側に表示するキャラクター画像が光るかどうかを格納する
                if (i == 0 && storyTalks[j].leftHighlight == "1") charaHighlight[i, j] = true;
                // charaHighlight[1,j]には右側に表示するキャラクター画像が光るかどうかを格納する
                else if (i == 1 && storyTalks[j].rightHighlight == "1") charaHighlight[i, j] = true;
                // charaHighlight[2,j]には中央に表示するキャラクター画像が光るかどうかを格納する
                else if (i == 2 && storyTalks[j].centerHighlight == "1") charaHighlight[i, j] = true;
            }
        }
        // BGM名を対応させたい文章ごとに格納する
        for (int i = 0; i < storyTalks.Length; i++)
        {
            nameBGM[i] = storyTalks[i].BGM;
        }
        /// ここまで ///
        Debug.Log("Story" + storynum + "を読み込みました");
    }
    /// <summary>
    /// 会話に関するボタン関数(変更不可)
    /// </summary>
    public virtual void OnTalkButtonClicked()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // 会話ステータスが話していないなら
        {
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
            InstantiateActors(); // 登場人物等を生成
            StartDialogueCoroutine(); // 文章を表示するコルーチンを開始
        }
        else if (TalkState == TALKSTATE.LASTTALK) // 会話ステータスが最後のセリフなら
        {
            TalkEnd(); //会話を終了する
        }
    }
    /// <summary>
    /// 会話に関するボタン関数(読み込むCSV変更可)
    /// </summary>
    /// <param name="storynum">読み込みたいCSV名</param>
    public virtual void OnTalkButtonClicked(string storynum = "")
    {
        // ストーリー番号があれば
        if (storynum != "")
        {
            StorySetUp(storynum); // 対応する会話文をセット
            talkNum = default; // 初期に戻す
        }
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // 会話ステータスが話していないなら
        {
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
            InstantiateActors(); // 登場人物等を生成
            StartDialogueCoroutine(); // 文章を表示するコルーチンを開始
        }
        else if (TalkState == TALKSTATE.LASTTALK) // 会話ステータスが最後のセリフなら
        {
            TalkEnd(); //会話を終了する
        }
    }
    /// <summary>
    /// 会話に関するボタン関数(talkNum変更可)
    /// </summary>
    /// <param name="num">読み込みたいCSVの行</param>
    public virtual void OnTalkButtonClicked(int num = 9999)
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // 会話ステータスが話していないなら
        {
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
            if (num != 9999 && num < storyTalks.Length) talkNum = num;
            InitializeTalkField(); // 表示されているテキスト等を初期化
            InstantiateActors(); // 登場人物等を生成
            StartDialogueCoroutine(); // 文章を表示するコルーチンを開始
        }
        else if (TalkState == TALKSTATE.LASTTALK) // 会話ステータスが最後のセリフなら
        {
            TalkEnd(); //会話を終了する
        }
    }
    /// <summary>
    /// 会話を終了する関数
    /// </summary>
    public virtual void TalkEnd()
    {
        Debug.Log("会話を終了");
        talkNum = default; // リセットする
        TalkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
        if (talkAuto) OnAutoModeCllicked(); // オートモードがオンであればオフにする
    }
    /// <summary>
    /// 会話関係の表示を初期化する関数
    /// </summary>
    protected void InitializeTalkField()
    {
        textLabel.text = ""; // 会話フィールドをリセットする
        if(charaName) charaName.text = ""; // 話しているキャラクター名をリセットする
        // 背景画像が表示されていれば画像を破壊する
        if (backImage) Destroy(backImage);
        // 左側キャラクター画像が表示されていれば画像を破壊する
        if (leftCharaImage) Destroy(leftCharaImage);
        // 右側キャラクター画像が表示されていれば画像を破壊する
        if (rightCharaImage) Destroy(rightCharaImage);
        // 中央キャラクター画像が表示されていれば画像を破壊する
        if (centerCharaImage) Destroy(centerCharaImage);
    }
    /// <summary>
    /// 登場人物等を生成する関数
    /// </summary>
    protected virtual void InstantiateActors()
    {
        // 背景を生成
        if (backImages[talkNum]) backImage = Instantiate(backImages[talkNum], backImageAnchor.transform);
        // キャラクター画像を生成
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (!charaImages[i, talkNum]) continue; // nullならコンティニューする
                                                    // 左側にキャラクター画像を生成
            if (i == 0) leftCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
            // 右側にキャラクター画像を生成
            else if (i == 1) rightCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
            // 中央にキャラクター画像を生成
            else if (i == 2) centerCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
        }
        // 発言者以外のキャラクター画像を灰色にする
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (charaHighlight[i, talkNum]) continue; // nullならコンティニューする
                                                      // 左側のキャラクター画像を灰色にする
            if (i == 0 && leftCharaImage) leftCharaImage.GetComponent<Image>().color = Color.gray;
            // 右側のキャラクター画像を灰色にする
            else if (i == 1 && rightCharaImage) rightCharaImage.GetComponent<Image>().color = Color.gray;
            // 中央のキャラクター画像を灰色にする
            else if (i == 2 && centerCharaImage) centerCharaImage.GetComponent<Image>().color = Color.gray;
        }
        // BGMを鳴らす
        if (nameBGM[talkNum] == "Stop") sceneManager.audioManager.BGM_Stop(); // StopならBGMを止める
        else if (nameBGM[talkNum] != "0") sceneManager.audioManager.BGM_Play(nameBGM[talkNum], sceneManager.enviromentalData.TInstance.volumeBGM); // BGM名が入っていたら切り替え　空白なら続行
    }
    /// <summary>
    /// コルーチン開始関数
    /// </summary>
    protected void StartDialogueCoroutine()
    {
        // コルーチンがすでに実行されている場合は停止
        if (runtimeCoroutine) StopCoroutine(dialogueCoroutine);
        // コルーチン開始
        dialogueCoroutine = StartCoroutine(Dialogue());
        runtimeCoroutine = true; // フラグを実行中に変更
    }
    /// <summary>
    /// コルーチン一時停止関数
    /// </summary>
    public void PauseDialogueCoroutine()
    {
        // 動いているコルーチンがあれば
        if (runtimeCoroutine)
        {
            StopCoroutine(dialogueCoroutine); // コルーチンを止める
            currentCharIndex = textLabel.text.Length; // 現在の文字の表示位置を保存
        }
    }
    /// <summary>
    /// コルーチン再開関数
    /// </summary>
    public void ResumeDialogueCoroutine()
    {
        // コルーチンが止まっていれば、再開用のコルーチン開始
        if (runtimeCoroutine) dialogueCoroutine = StartCoroutine(ResumeDialogue());
    }
    /// <summary>
    /// 文章を表示するコルーチン
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Dialogue()
    {
        Debug.Log(storynum + "の" + (talkNum + 1) + "列目を再生");
        TalkState = TALKSTATE.TALKING; // 会話ステータスを話し中にする
        charaName.text = storyTalks[talkNum].name; // 話しているキャラクター名を表示
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
    /// <summary>
    /// 一時停止した箇所から表示するコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator ResumeDialogue()
    {
        // 文章の残りを再表示
        for (int i = currentCharIndex; i < words.Length; i++)
        {
            // 文字を textLabel に追加します
            textLabel.text += words[i];
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
    /// <summary>
    /// 次のダイアログに変更する関数
    /// </summary>
    protected virtual void NextDialogue()
    {
        // トークスキップフラグが立ったら
        if (talkSkip == true) textLabel.text = storyTalks[talkNum].talks; // 全文を表示
        talkNum++; // 次のダイアログに移動
        
        TalkState = TALKSTATE.NEXTTALK; // 会話ステータスを次のセリフに変更
        talkSkip = false; // トークスキップフラグをfalseにする
        // 次のダイアログで最後なら会話ステータスを最後のセリフに変更
        if (talkNum >= storyTalks.Length) TalkState = TALKSTATE.LASTTALK;
        runtimeCoroutine = false; // フラグを未実行に変更
    }
    /// <summary>
    /// テキストスピードを計算する関数
    /// </summary>
    /// <returns></returns>
    protected float CalculataTextSpeed()
    {
        // 基礎スピード*10段階のうちのどれか(5が基準)
        return textBaseSpeed / (playerTextSpeed / 0.5f);
    }
    /// <summary>
    /// オートモードを切り替える関数
    /// </summary>
    public void OnAutoModeCllicked()
    {
        // talkAutoがtrueならfalseに、falseならtrueに変換
        talkAuto = !talkAuto;
        // オートモードならオートモード画像を出す　オートモードではないなら画像を出さない
        if (!autoImage) return;
        if(talkAuto) autoImage.SetActive(true);
        else autoImage.SetActive(false);
    }
}
[System.Serializable] // サブプロパティを埋め込む
public class StoryTalkData
{
    public string backImage; // 背景画像
    public string leftTalkingChara; // キャラクター画像名(左側)
    public string rightTalkingChara; // キャラクター画像名(右側)
    public string centerTalkingChara; // キャラクター画像名(中央)
    public string leftHighlight; // キャラクター画像を光らせるか(左側)
    public string rightHighlight; // キャラクターが光らせるか(左側)
    public string centerHighlight; // キャラクターが光らせるか(中央)
    public string name; // キャラクター名
    public string talks; // 文章
    public string BGM; // BGM名
    public string stage; // ステージ番号
}