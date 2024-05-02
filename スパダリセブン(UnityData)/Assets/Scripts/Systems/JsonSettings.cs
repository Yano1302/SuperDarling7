using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jsonファイルを読み書きする為のクラスです
/// </summary>
/// <typeparam name="T">Jsonファイルと同じ形式のクラス</typeparam>
public class JsonSettings<T> where T : class, new() {

    // 変数 //
    /// <summary>T型のクラスのインスタンスを取得します。</summary>
    public T TInstance { get { return m_tInstance; } }

    //　コンストラクタ　//
    /// <summary>Jsonを管理するクラスを作成します</summary>
    /// <param name="createFileName">この名前でファイルを生成しデータを保存します</param>
    /// <param name="saveFolderPath">データを保存するフォルダのパス(Asset/以下)</param>
    /// <param name="dataFileName">Jsonファイル名(拡張子抜き)</param>
    public JsonSettings(in string createFileName, in string saveFolderPath, in string dataFileName) {
        JsonSetUp(createFileName, saveFolderPath, dataFileName);
    }

    /// <summary>マスターデータも自動で作成します。マスターデータは"Origin_createFileName.json"として作成されます</summary>
    /// <param name="createFileName">この名前でファイルを生成しデータを保存します</param>
    /// <param name="saveFolderPath">データを保存するフォルダのパス(Asset/以下)</param>
    public JsonSettings(in string createFileName, in string saveFolderPath) {
        var path = Application.dataPath + "\\" + saveFolderPath + "\\" + createFileName + ".json";
        m_jsonDefaultPath = Application.dataPath + "\\" + saveFolderPath + "\\" + "Origin_" + createFileName + ".json";
        if (!File.Exists(path)) {
            //マスターデータ用のパスでマスターデータを作成する
            m_jsonPath = m_jsonDefaultPath;
            m_tInstance = new T();
            _Save(true);
            //正しいパスをJsonパスに格納し、改めてデータを作成する
            m_jsonPath = path;
            SettingData();
        }
        else { m_jsonPath = path; }
        Load();
    }


    //  関数  //
    /// <summary> Jsonデータにインスタンスの情報を書き込みます。</summary>
    public void Save() { _Save(false); }

    /// <summary>Jsonデータをインスタンスに読み込みます</summary>
    public void Load() {
        //JSONファイルを読み込む
        var json = File.ReadAllText(m_jsonPath);
        //オブジェクト化する
        m_tInstance = JsonUtility.FromJson<T>(json);
    }

    /// <summary>データを初期値に戻します。</summary>
    public void Reset() {
        var json = File.ReadAllText(m_jsonDefaultPath);
        m_tInstance = JsonUtility.FromJson<T>(json);
        _Save(false);
    }

    /// <summary>データを削除します。インスタンスも破棄されます。</summary>
    public void Delete(ref JsonSettings<T> data) {
        File.Delete(data.GetJsonPath());
        data = null;
    }
    /// <summary>データを保存しているJsonファイルのパスを取得します。</summary>
    public string GetJsonPath() { return m_jsonPath; }

    /// <summary>Jsonファイル内の情報を文字列で返します。</summary>
    public string JsonToString() {
        //JSONファイルを読み込む
        var json = File.ReadAllText(m_jsonPath);
        //オブジェクト化する
        var ins = JsonUtility.FromJson<T>(json);
        return JsonUtility.ToJson(ins, true);
    }

    /// <summary>Jsonファイル内のデータをログに表示します </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() { Debug.Log(JsonToString()); }




    //　プライベート変数・関数　// 
    private string m_jsonPath;
    private string m_jsonDefaultPath = null;
    private T m_tInstance;
    private JsonSettings() { }

    //Jsonファイルの作成、読み込みを行います
    private void JsonSetUp(in string createFileName, in string saveFolderPath, in string dataFileName) {

        m_jsonDefaultPath = UsefulSystem.FindFilePath(dataFileName + ".json");
        m_jsonPath = Application.dataPath + "\\" + saveFolderPath + "\\" + createFileName + ".json";
        if (!File.Exists(m_jsonPath)) {
            SettingData();
        }
        Load();
    }

    //データを設定します
    private void SettingData() {
        //デフォルトのJSONファイルを読み込む
        var json = File.ReadAllText(m_jsonDefaultPath);
        //オブジェクト化する
        m_tInstance = JsonUtility.FromJson<T>(json);
        //初期値をセーブする
        _Save(true);
    }

    //セーブの実際の処理
    private void _Save(bool initialization) {
        //stringに変換する
        string jsonStr = JsonUtility.ToJson(TInstance);
        //ファイル書き込み用のライターを開く
        StreamWriter writer = new StreamWriter(m_jsonPath, initialization);
        //書き込み
        writer.Write(jsonStr);
        //ライターを閉じる処理
        writer.Flush();
        writer.Close();
    }

    //MonoBehaviourが継承されているか調べます
    private bool CheckMono() { return typeof(T).GetType().IsSubclassOf(typeof(MonoBehaviour)); }
}