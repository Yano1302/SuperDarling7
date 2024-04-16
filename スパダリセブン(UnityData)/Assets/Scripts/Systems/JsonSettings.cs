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

    //コンストラクタ
    /// <summary>Jsonを管理するクラスを作成します</summary>
    /// <param name="dataFileName">この名前でファイルを生成しデータを保存します</param>
    /// <param name="saveFolderPath">データを保存するフォルダのパス(Asset/以下)</param>
    /// <param name="defaultJsonFileName">Jsonファイル名(拡張子抜き)</param>
    public JsonSettings(string dataFileName,string saveFolderPath, string defaultJsonFileName) {
        
        m_jsonFileName = dataFileName + ".json";
        m_jsonDefaultPath = UsefulSystem.FindFilePath(defaultJsonFileName + ".json");
        m_jsonPath = Application.dataPath + "\\" + saveFolderPath + "\\" + m_jsonFileName;
        if (!File.Exists(m_jsonPath)) {
            SettingData();
        }
        Load(); 
    }

    /// <summary>マスターデータも自動で作成します</summary>
    /// <param name="dataFileName">この名前でファイルを生成しデータを保存します</param>
    /// <param name="saveFolderPath">データを保存するフォルダのパス(Asset/以下)</param>
    public JsonSettings(string dataFileName,string saveFolderPath) {
        m_jsonFileName = dataFileName + ".json";
        m_jsonPath = Application.dataPath +"\\"+ saveFolderPath + "\\" + m_jsonFileName;
        m_tInstance = new T();
        m_create = true;
        Save();
    }

    public JsonSettings(string dataFileName, string saveFolderPath, string defaultJsonFileName,GameObject obj) {
    
    }

   //  関数  //
    /// <summary> Jsonデータにインスタンスの情報を書き込みます。</summary>
    public void Save() {
        //stringに変換する
        string jsonStr = JsonUtility.ToJson(TInstance);
        //ファイル書き込み用のライターを開く 上書きにしないとjsonデータが崩れるのでfalseにしています
        StreamWriter writer = new StreamWriter(m_jsonPath,m_create);
        //書き込み
        writer.Write(jsonStr);
        //ライターを閉じる処理
        writer.Flush();
        writer.Close();
        m_create = false;
    }

    /// <summary>Jsonデータをインスタンスに読み込みます</summary>
    public void Load() {
        //JSONファイルを読み込む
        var json = File.ReadAllText(m_jsonPath);
        //オブジェクト化する
        m_tInstance = JsonUtility.FromJson<T>(json);
    }

    /// <summary>データを初期値に戻します。</summary>
    public void Reset() {
        SettingData();
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




    //　プライベート変数・関数　// 
    private string m_jsonFileName;
    private string m_jsonPath;
    private string m_jsonDefaultPath = null;
    private bool m_create = false;
    public T m_tInstance; // 他スクリプトから変更するためにpublicにしています
    private JsonSettings() { }
    
    //データを設定します
    private void SettingData() {
        //デフォルトのJSONファイルを読み込む
        var json = File.ReadAllText(m_jsonDefaultPath);
        //オブジェクト化する
        m_tInstance = JsonUtility.FromJson<T>(json);
        //初期値をセーブする
        m_create = true;
        Save();
    }

    //MonoBehaviourが継承されているか調べます
    private bool CheckMono() {return typeof(T).GetType().IsSubclassOf(typeof(MonoBehaviour)); }
}