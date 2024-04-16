using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// CSVを読み込むクラスです。
/// CSVファイルをResourcesフォルダに格納して使用してください
/// また一行目にデータの項目一覧が記載されている形を想定しています。
/// </summary>
public class CSVSetting {

    //コンストラクタ
    public CSVSetting(string fileName) {
        string path = UsefulSystem.FindFilePath(fileName+".csv");      //全体パスの取得
        int index = path.IndexOf("Resources/") + 10;                   //相対パスの始めの位置を取得
        csvFile = Resources.Load(path.Substring(index, path.Length - (index + 4))) as TextAsset; // CSVファイルのファイル名だけを取得してResourcesにあるCSVファイルを格納
        StringReader reader = new StringReader(csvFile.text);           // TextAssetをStringReaderに変換
        csvData = new List<string[]>();                                 //メモリ確保
        ConvertEncoding(csvFile, Encoding.UTF8);                        //文字化け防止のためUTF-8に変換
        while (reader.Peek() != -1) {
            string line = reader.ReadLine();// 1行ずつ読み込む
            csvData.Add(line.Split(',')); // csvDataリストに追加する
        }
        reader.Close();
    }

    //関数

    /// <summary>項目番号と行数から指定された文字データを取得します</summary>
    ///<param name="row">項目の番号</param>
    ///<param name="column">行数</param>
    public string GetData(int row,int column) { return csvData[column][row]; }

    /// <summary>項目名と行数から指定された文字データを取得します</summary>
    /// <param name="name">項目名</param>
    /// <param name="column">行数</param>
    /// <returns></returns>
    public string GetData(string name,int column) {
       int row =   GetRowIndex(name);
        return csvData[column][row];
    }

    /// <summary>指定された項目名が何列目かを調べます</summary>
    /// <param name="name">項目名</param>
    /// <returns>列数を返します。見つからなかった場合は-1を返します</returns>
    public int GetRowIndex(string name) {
        for (int i = 0; i < csvData[0].Length; i++){
            if (csvData[0][i] == name) {
                return i;
            }
        }
        Debug.Assert(false,"指定された項目が見つかりませんでした");
        return -1;
    }




//private 
    private CSVSetting() { }
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvData; // CSVファイルの中身を入れるリスト


    /// <summary>TextAssetの文字コードを変換します</summary>
    /// <param name="textAsset">変換するTextAsset</param>
    /// <param name="dstEncoding">返還後の文字コード</param>
    /// <returns></returns>
    private string ConvertEncoding(TextAsset textAsset,Encoding dstEncoding) {
        var srcEnc = DetectEncodingFromBOM(textAsset.bytes);
        byte[] srcbyte = srcEnc.GetBytes(textAsset.text);
        byte[] dstbyte = Encoding.Convert(srcEnc, dstEncoding, srcbyte);
        string ret = dstEncoding.GetString(dstbyte);
        return ret;
    }

    /// <summary>文字コードを調べます</summary>
    /// <param name="bytes">文字コードを調べるデータ。</param>
    /// <returns>BOMが見つかった時は、対応するEncodingオブジェクト。
    /// 見つからなかった時は、null。</returns>
    private Encoding DetectEncodingFromBOM(byte[] bytes) {

        if (bytes.Length < 2) {
            return null;
        }
        if ((bytes[0] == 0xfe) && (bytes[1] == 0xff)) {
            //UTF-16 BE
            return new System.Text.UnicodeEncoding(true, true);
        }
        if ((bytes[0] == 0xff) && (bytes[1] == 0xfe)) {
            if ((4 <= bytes.Length) &&
                (bytes[2] == 0x00) && (bytes[3] == 0x00)) {
                //UTF-32 LE
                return new System.Text.UTF32Encoding(false, true);
            }
            //UTF-16 LE
            return new System.Text.UnicodeEncoding(false, true);
        }
        if (bytes.Length < 3) {
            return null;
        }
        if ((bytes[0] == 0xef) && (bytes[1] == 0xbb) && (bytes[2] == 0xbf)) {
            //UTF-8
            return new System.Text.UTF8Encoding(true, true);
        }
        if (bytes.Length < 4) {
            return null;
        }
        if ((bytes[0] == 0x00) && (bytes[1] == 0x00) &&
            (bytes[2] == 0xfe) && (bytes[3] == 0xff)) {
            //UTF-32 BE
            return new System.Text.UTF32Encoding(true, true);
        }

        return null;
    }


}