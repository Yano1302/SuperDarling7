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
    //パブリック変数//
    public T jsonData;

    //コンストラクタ
    /// <summary>Jsonを管理するクラスを作成します</summary>
    /// <param name="jsonFileName">Jsonファイル名(拡張子抜き)</param>
    public JsonSettings(string jsonFileName) {
        m_jsonFileName = jsonFileName+".json";
        m_jsonPath = UsefulSystem.FindFilePath(m_jsonFileName);
        Load();
    }
    // パブリック変数・関数   //
    public void Save() {
        //書き込み元データを取得する。ここではsettings.jsonという
        string jsonData = Resources.Load<TextAsset>(m_jsonFileName).ToString();
        //stringに変換する
        string jsonstr = JsonUtility.ToJson(this.jsonData);
        //ファイル書き込み用のライターを開く
        StreamWriter writer = new StreamWriter(m_jsonPath,false);
        //書き込み
        writer.Write(jsonstr);
        //ライターを閉じる処理
        writer.Flush();
        writer.Close();
    }

    public void Load() {
        //JSONファイルを読み込む
        var json = File.ReadAllText(m_jsonPath);
        //オブジェクト化する
        jsonData = JsonUtility.FromJson<T>(json);
    }


    //　プライベート変数・関数　// 
    private string m_jsonFileName;
    private string m_jsonPath;

    private JsonSettings() { }
}