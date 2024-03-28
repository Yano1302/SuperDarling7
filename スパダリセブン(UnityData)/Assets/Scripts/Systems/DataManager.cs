using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : DebugSetting // MasterDataをjson形式に変えて保存・読み込みするスクリプト
{
    [SerializeField] public MasterData data; // json変換するデータのクラス 
    string filepath; // jsonファイルのパス
    string fileName = "MasterData.json"; // jsonファイル名

    protected  override void Awake()
    {
        base.Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのDebugSettingsを参照
        CheckSaveData(); // 開始時にファイルチェック、読み込み
    }
    private void CheckSaveData() // 開始時にファイルチェック、読み込みする関数
    {
        UnityEngine.Debug.Log("起動ロード開始");
        data = new MasterData(); // dataにMasterData型を代入
        filepath = Application.dataPath + "/Resources/Json/" + fileName; // パス名取得
        if (!File.Exists(filepath)) // ファイルがないとき
        {
            UnityEngine.Debug.Log("saveデータを作ろうとしています");
            Save(data); // ファイル作成
        }
        data = Load(filepath); // ファイルを読み込んでdataに格納
    }
    public void Save(MasterData data) // jsonとしてデータを保存する関数
    {
        string json = JsonUtility.ToJson(data); // jsonとして変換
        StreamWriter writer = new StreamWriter(filepath, false); // ファイル書き込み指定
        writer.WriteLine(json); // json変換した情報を書き込み
        writer.Close(); // ファイルを閉じる
        UnityEngine.Debug.Log("セーブしています" + json);
    }
    MasterData Load(string path) // jsonデータを読み込む関数
    {
        if (File.Exists(path)) // jsonデータがあれば
        {
            StreamReader reader = new StreamReader(path); // ファイル読み込み指定
            string json = reader.ReadToEnd(); // ファイル内容全て読み込み
            reader.Close(); // ファイルを閉じる
            UnityEngine.Debug.Log("ロードしています" + json);
            return JsonUtility.FromJson<MasterData>(json); // jsonファイルを型に戻して返す
        }
        else
        {
            UnityEngine.Debug.LogError("ファイルが見つかりません" + path);
            return null; // nullを返す
        }
    }
    public void ResetMasterData() // データを初期化する関数
    {
        UnityEngine.Debug.Log("マスターデータの初期化を行います");
        data = new MasterData(); // dataにMasterData型を代入
        Save(data); // セーブする
    }
    //void OnDestroy() // ゲーム終了時に保存
    //{
    //    Save(data);
    //}
    public void DeletaSave()
    {
        File.Delete(filepath);
    }
}

