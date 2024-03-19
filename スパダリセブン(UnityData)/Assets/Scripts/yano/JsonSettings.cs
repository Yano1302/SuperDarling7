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
    public T Instance{ get { return m_instance; } }

    //コンストラクタ
    /// <summary>Jsonを管理するクラスを作成します</summary>
    /// <param name="jsonFileName">Jsonファイル名(拡張子抜き)</param>
    public JsonSettings(string jsonFileName) {
        m_jsonFileName = jsonFileName + ".json";
        m_jsonPath = UsefulSystem.FindFilePath(m_jsonFileName);
        Load();
    }
   
    
    //  関数  //
    /// <summary> Jsonデータにインスタンスの情報を書き込みます。</summary>
    public void Save() {
        //stringに変換する
        string jsonstr = JsonUtility.ToJson(Instance);
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
        m_instance = JsonUtility.FromJson<T>(json);
    }


    public void Reset() {

    }

    /// <summary>Jsonファイル内の情報を文字列で返します。</summary>
    public string JsonToString() { return JsonUtility.ToJson(m_instance, true);}




    //　プライベート変数・関数　// 
    private string m_jsonFileName;
    private string m_jsonPath;
    private T m_instance;

    private JsonSettings() { }
}