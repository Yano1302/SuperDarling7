using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndRollScript : DebugSetting
{
    //　テキストのスクロールスピード
    [SerializeField]
    private float textScrollSpeed = 10;
    //　テキストの制限位置
    [Header("スクロールを止めたいPos.Yを/100した値を入力")]
    [SerializeField]
    private float limitPosition = 15f;
    //　エンドロールが終了したかどうか
    [SerializeField]
    private bool isStopEndRoll=false;
    //　シーン移動用コルーチン
    private Coroutine endRollCoroutine;
    CreditData[] creditDates; //csvファイルにある文章を格納する配列
    protected override void Awake()
    {
        base.Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのGameSettingsを参照
        CreditSetUp(); // クレジットをセット
        this.GetComponent<TextMeshProUGUI>().text = creditDates[0].creditText; // テキストをセット
    }
    /// <summary>
    /// クレジットをセットする関数
    /// </summary>
    private void CreditSetUp()
    {
        Debug.Log("Creditを読み込みます");
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。
        //　Resources.Load 内はcsvファイルの名前。今回は Story1-1 や Story2-5 のようにステージ番号によって読み込むファイルが変えられるようにしている。
        textasset = Resources.Load("Credit/Credit", typeof(TextAsset)) as TextAsset;

        /// CSVSerializerを用いてcsvファイルを配列に流し込む。///
        creditDates = CSVSerializer.Deserialize<CreditData>(textasset.text); // CSVのテキストデータを配列に格納する
        /// ここまで ///
        Debug.Log("Creditを読み込みました");
    }
    // Update is called once per frame
    void Update()
    {
        //　エンドロールが終了した時
        if (isStopEndRoll)
        {
            endRollCoroutine = StartCoroutine(GoToNextScene());
        }
        else
        {
            //　エンドロール用テキストがリミットを越えるまで動かす
            if (transform.position.y <= limitPosition)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + textScrollSpeed * Time.deltaTime, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, limitPosition, transform.position.z);
                isStopEndRoll = true;
            }
        }
    }

    IEnumerator GoToNextScene()
    {
        //　5秒間待つ
        yield return new WaitForSeconds(5f);

        if (Input.GetKeyDown("space"))
        {
            StopCoroutine(endRollCoroutine);
            SceneManager.LoadScene("EndRollStartScene");
        }

        yield return null;
    }
}
[System.Serializable] // サブプロパティを埋め込む
public class CreditData // CreditDataの中にtalkingCharaとtalksを配置する
{
    public string creditText; // クレジットテキスト
}