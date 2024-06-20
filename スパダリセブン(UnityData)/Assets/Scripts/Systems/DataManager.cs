using UnityEngine;
using System.IO;

public partial class DataManager : DebugSetting // MasterDataをjson形式に変えて保存・読み込みするスクリプト
{
    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///

    public void Save(MasterData data) // jsonとしてデータを保存する関数
    {
        string json = JsonUtility.ToJson(data); // jsonとして変換
        StreamWriter writer = new StreamWriter(filepath, false); // ファイル書き込み指定
        writer.WriteLine(json); // json変換した情報を書き込み
        writer.Close(); // ファイルを閉じる
        Debug.Log("セーブしています" + json);
    }

    public void ResetMasterData() // データを初期化する関数
    {
        Debug.Log("マスターデータの初期化を行います");
        data = new MasterData(); // dataにMasterData型を代入
        Save(data); // セーブする
    }

    public void DeletaSave()
    {
        File.Delete(filepath);
    }

    /// -------public関数------- ///
    #endregion

    #region protected関数
    /// -----protected関数------ ///

    protected override void Awake()
    {
        base.Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのDebugSettingsを参照
        CheckSaveData(); // 開始時にファイルチェック、読み込み
    }

    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///

    private void CheckSaveData() // 開始時にファイルチェック、読み込みする関数
    {
        Debug.Log("起動ロード開始");
        data = new MasterData(); // dataにMasterData型を代入
        filepath = Application.dataPath + "/Resources/Json/" + fileName; // パス名取得
        if (!File.Exists(filepath)) // ファイルがないとき
        {
            Debug.Log("saveデータを作ろうとしています");
            Save(data); // ファイル作成
        }
        data = Load(filepath); // ファイルを読み込んでdataに格納
    }

    private MasterData Load(string path) // jsonデータを読み込む関数
    {
        if (File.Exists(path)) // jsonデータがあれば
        {
            StreamReader reader = new StreamReader(path); // ファイル読み込み指定
            string json = reader.ReadToEnd(); // ファイル内容全て読み込み
            reader.Close(); // ファイルを閉じる
            Debug.Log("ロードしています" + json);
            return JsonUtility.FromJson<MasterData>(json); // jsonファイルを型に戻して返す
        }
        else
        {
            Debug.LogError("ファイルが見つかりません" + path);
            return null; // nullを返す
        }
    }

    //private void OnDestroy() // ゲーム終了時に保存
    //{
    //    Save(data);
    //}

    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///
}
public partial class DataManager
{
    /// --------変数一覧-------- ///

    #region public変数
    /// -------public変数------- ///

    [SerializeField] public MasterData data; // json変換するデータのクラス 

    /// -------public変数------- ///
    #endregion

    #region protected変数
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    #endregion

    #region private変数
    /// ------private変数------- ///

    private string filepath; // jsonファイルのパス
    private string fileName = "MasterData.json"; // jsonファイル名

    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}
