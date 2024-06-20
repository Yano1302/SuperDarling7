using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public partial class EndRollScript : DebugSetting
{

    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///



    /// -------public関数------- ///
    #endregion

    #region protected関数
    /// -----protected関数------ ///

    protected override void Awake()
    {
        base.Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのGameSettingsを参照
        CreditSetUp(); // クレジットをセット
        this.GetComponent<TextMeshProUGUI>().text = creditDates[0].creditText; // テキストをセット
    }

    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///

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

    private void Update()
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

    private IEnumerator GoToNextScene()
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


    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///
}
public partial class EndRollScript
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

    //　エンドロールが終了したかどうか
    [SerializeField] private bool isStopEndRoll = false;

    //　テキストのスクロールスピード
    [SerializeField] private float textScrollSpeed = 10;
    //　テキストの制限位置
    [Header("スクロールを止めたいPos.Yを/100した値を入力")]
    [SerializeField] private float limitPosition = 15f;

    //　シーン移動用コルーチン
    private Coroutine endRollCoroutine;

    private CreditData[] creditDates; //csvファイルにある文章を格納する配列

    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}
[System.Serializable] // サブプロパティを埋め込む
public class CreditData // CreditDataの中にtalkingCharaとtalksを配置する
{
    /// --------変数一覧-------- ///

    #region public変数
    /// -------public変数------- ///

    public string creditText; // クレジットテキスト

    /// -------public変数------- ///
    #endregion

    #region protected変数
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    #endregion

    #region private変数
    /// ------private変数------- ///



    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}