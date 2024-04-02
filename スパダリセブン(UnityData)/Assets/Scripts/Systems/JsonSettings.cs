using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jsonファイルを読み書きする為のクラスです
/// </summary>
/// <typeparam name="T">[System.Serializable]属性を付けたJsonファイルと同じ形式のクラス</typeparam>
public class JsonSettings<T> where T : class {

    // 変数 //
    /// <summary>T型のクラスのインスタンスを取得します。</summary>
    public T TInstance{ get { return m_tInstance; } }

    //コンストラクタ
    /// <summary>Jsonを管理するクラスを作成します</summary>
    /// <param name="jsonFileName">Jsonファイル名(拡張子抜き)</param>
    public JsonSettings(string jsonFileName,string dataFileName) {
        m_jsonFileName = jsonFileName + ".json";
        m_jsonDefaultPath = UsefulSystem.FindFilePath(m_jsonFileName);
        m_jsonPath = m_jsonDefaultPath.Substring(0, m_jsonDefaultPath.Length - 4) + dataFileName + ".json";
        if (!File.Exists(m_jsonPath)) {
            SettingData();
        }
        Load();
    }
   
    
    //  関数  //
    /// <summary> Jsonデータにインスタンスの情報を書き込みます。</summary>
    public void Save() {
        //stringに変換する
        string jsonstr = JsonUtility.ToJson(TInstance);
        //ファイル書き込み用のライターを開く
        StreamWriter writer = new StreamWriter(m_jsonPath,false);
        //書き込み
        writer.Write(jsonstr);
        //ライターを閉じる処理
        writer.Flush();
        writer.Close();
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
        return JsonUtility.ToJson(ins, true);}




    //　プライベート変数・関数　// 
    private string m_jsonFileName;
    private string m_jsonPath;
    private string m_jsonDefaultPath;
    private T m_tInstance;

    private JsonSettings() { }

    private void SettingData() { 
        //デフォルトのJSONファイルを読み込む
        var json = File.ReadAllText(m_jsonDefaultPath);
        //オブジェクト化する
        m_tInstance = JsonUtility.FromJson<T>(json);
        //初期値をセーブする
        Save();
    }
}