using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// CSVを読み込むクラスです。
/// CSVファイルをResourcesフォルダに格納して使用してください
/// また一行目(Count = 0)にデータの項目一覧が記載されている形を想定しています。
/// </summary>
public class CSVSetting {

    //コンストラクタ
    public CSVSetting(string fileName) {
        string path = UsefulSystem.FindFilePath(fileName+".csv");      //全体パスの取得
        int index = path.IndexOf("Resources/") + 10;                   //相対パスの始めの位置を取得
        m_csvFile = Resources.Load(path.Substring(index, path.Length - (index + 4))) as TextAsset; // CSVファイルのファイル名だけを取得してResourcesにあるCSVファイルを格納
        StringReader reader = new StringReader(m_csvFile.text);           // TextAssetをStringReaderに変換
        m_csvData = new List<string[]>();                                 //メモリ確保
       // ConvertEncoding(m_csvFile, Encoding.UTF8);                        //文字化け防止のためUTF-8に変換
        while (reader.Peek() != -1) {
            string line = reader.ReadLine();// 1行ずつ読み込む
            m_csvData.Add(line.Split(',')); // csvDataリストに追加する
        }
        reader.Close();
    }

    //関数

    /// <summary>項目欄を含めた行数を取得します。</summary>
    public int Row { get { return m_csvData.Count; } }
    /// <summary>項目数を取得します</summary>
    public int Column { get { return m_csvData[0].Length; } }

    /// <summary>項目番号と行数から指定された文字データを取得します</summary>
    ///<param name="row">項目の番号</param>
    ///<param name="column">行数</param>
    public string GetData(int row,int column) { return m_csvData[column][row]; }

    /// <summary>項目名と行数から指定された文字データを取得します</summary>
    /// <param name="name">項目名</param>
    /// <param name="column">行数</param>
    /// <returns></returns>
    public string GetData(string name,int column) {
       int row =   GetRowIndex(name);
        return m_csvData[column][row];
    }

    /// <summary>指定された項目名が何列目かを調べます</summary>
    /// <param name="name">項目名</param>
    /// <returns>列数を返します。見つからなかった場合は-1を返します</returns>
    public int GetRowIndex(string name) {
        for (int i = 0; i < m_csvData[0].Length; i++){
            if (m_csvData[0][i] == name) {
                return i;
            }
        }
        Debug.Assert(false,"指定された項目が見つかりませんでした");
        return -1;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() {
        Debug.Log(m_csvFile);
    }


    //private 
    private CSVSetting() { }
    private TextAsset m_csvFile; // CSVファイル
    private List<string[]> m_csvData; // CSVファイルの中身を入れるリスト
}