using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// CSVを読み込むクラスです。<br />
/// CSVファイルをResourcesフォルダに格納して使用してください<br />
/// またCSVファイルはUTF-8で作成してください
/// </summary>
public class CSVSetting {

    /// <summary>コンストラクタからCSVファイルを読み込みます</summary>
    /// <param name="fileName">CSVファイル名(拡張子不要)</param>
    public CSVSetting(string fileName) {
        string path = UsefulSystem.FindFilePath(fileName+".csv");      //全体パスの取得
        int index = path.IndexOf("Resources/") + 10;                   //相対パスの始めの位置を取得
        m_csvFile = Resources.Load(path.Substring(index, path.Length - (index + 4))) as TextAsset; // CSVファイルのファイル名だけを取得してResourcesにあるCSVファイルを格納    
        StringReader reader = new StringReader(m_csvFile.text);           // TextAsset内の文字列をStringReaderに変換
        m_csvData = new List<string[]>();                                 //メモリ確保                 
        while (reader.Peek() != -1) {
            string line = reader.ReadLine();// 1行ずつ読み込む
            Debug.Log(line);
            m_csvData.Add(line.Split(',')); // csvDataリストに追加する
        }
        reader.Close();
    }


    /// <summary>CSVファイルのデータ(文字列)を取得します</summary>
    /// <param name="x">取得する列数</param>
    /// <param name="y">取得する行数</param>
    /// <param name="data">取得データ</param>
    /// <returns>指定されたデータが空白だった場合はfalseを返します</returns>
    public bool GetData(int x, int y, out string data) { data = m_csvData[y][x];  return data != ""; }
    /// <summary>CSVファイルのデータ(数字)を取得します</summary>
    /// <param name="x">取得する列数</param>
    /// <param name="y">取得する行数</param>
    /// <param name="data">取得データ</param>
    /// <returns>指定されたデータに数字が書かれていない場合はfalseを返します</returns>
    public bool GetData(int x, int y, out int data) { return int.TryParse(m_csvData[y][x], out data); }

    /// <summary>指定された箇所に記述があるかどうか確認します</summary>
    /// <param name="x">取得する列数</param>
    /// <param name="y">取得する行数</param>
    /// <returns>記述があった場合はtrueを返します</returns>
    public bool CheckData(int x, int y) { return m_csvData[y][x] != ""; }

    /// <summary>総行数を取得します</summary>
    public int TotalLine { get { return m_csvData.Count; } }

    /// <summary>指定された行数の列数(要素数)を取得します</summary>
    /// <param name="y">指定行</param>
    /// <returns>要素数を返します</returns>
    public int GetLength(int y) { return m_csvData[y].Length; }

    /// <summary>指定された文字列の列インデックスを指定された行数から調べます</summary>
    /// <param name="index_y">指定する行インデックス</param>
    /// <param name="name">検索する文字列</param>
    /// <param name="index_x">インデックスを返します。見つからなかった場合は-1</param>
    /// <returns>見つかったかどうかを返します</returns>
    public bool GetColumnIndex(int index_y,string name,out int index_x) {
        for (int i = 0; i < m_csvData[0].Length; i++){
            if (m_csvData[index_y][i] == name) {
                index_x = i;
                return true;
            }
        }
        LogWarning("指定された項目が見つかりませんでした　 項目名 : " + name);
        index_x = -1;
        return false;
    }

    /// <summary>指定された文字列の行インデックスを指定された列数から調べます</summary>
    /// <param name="index_x">行インデックスを指定します</param>
    /// <param name="name">検索する文字列</param>
    /// <param name="index_y">インデックスを返します。見つからなかった場合は-1</param>
    /// <returns>見つかったかどうかを返します</returns>
    public bool GetLineIndex(int index_x, string name,out int index_y) {
        for (int i = 0; i < m_csvData.Count; i++) {
            if (m_csvData[i][index_x] == name) {
                index_y = i;
                return true;
            }
        }
        LogWarning( "指定された項目が見つかりませんでした　 項目名 : " + name);
        index_y = -1;
        return false;
    }

    /// <summary>CSVの中身をログに表示します(デバッグ時のみ呼び出し)</summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void LogCSV() {
        Debug.Log(m_csvFile);
    }


    //private 
    private CSVSetting() { }            // new抑制
    private List<string[]> m_csvData;   // 実際のテキストデータ
    private TextAsset m_csvFile;        // CSVファイルを読み込むテキストアセット

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void LogWarning(object obj) { Debug.LogWarning(obj); }
}