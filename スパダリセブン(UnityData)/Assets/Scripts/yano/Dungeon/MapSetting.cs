using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//TODO JSONに対応させる
public enum MapType {
    player = 0,
    road   = 1,
    wall   = 2,
    catchtrap = 3,
    pitfall = 4,
    Goal1 = 5,
    Goal2 = 6,
    Goal3 = 7,
    Goal4 = 8,
}

public class MapSetting : SingletonMonoBehaviour<MapSetting> {
    
    /// <summary>マップを生成します</summary>
    /// <param name="stageNumber">マップ番号</param>
    public void CreateMap(int stageNumber) {
        stageNumber -= 1;
        _Create(stageNumber);
    }

    /// <summary>ステージの制限時間を取得します</summary>
    public float Time { get { return m_stageData.time; } }
    /// <summary>マップの総数を取得します</summary>
    public int TotalMapNumber { get { return m_mapData.Length; } }

    /// <summary>
    /// </summary>

    // アタッチ変数  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("マップオブジェクト"), EnumIndex(typeof(MapType))]
    private GameObject[] MapObject;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


    //  プライベート変数  //------------------------------------------------------------------------------------------------------------------------------

    public enum StageCsvIndex {
        number = 0,
        name   = 1,
        time   = 2,
        size   = 3,
        itemStart = 4,
    }

    //マップデータを読み込むクラス配列
    private static CSVSetting[] m_mapData = null;
    //マップデータ
    StageData m_stageData;
    //UIマネージャーインスタンス
    private UIManager m_uiManager;
    //アイテムマネージャーインスタンス
    private ItemManager m_itemManager;

    //ステージのデータを管理する構造体
    private struct StageData {
        public CSVSetting Data;     //マップデータを全て格納したデータCSV
        public int number;          //マップ番号(これを元にデータを返します)
        public string name { get { Data.GetData((int)StageCsvIndex.name,number, out string data); return data; } } //ステージ名
        public float time  { get { Data.GetData((int)StageCsvIndex.time, number, out int data); return data; } }    //制限時間
        public float size  { get { Data.GetData((int)StageCsvIndex.size,number, out int data); return data; } }    //一マスのサイズ
    }

    //  プライベート関数  //------------------------------------------------------------------------------------------------------------------------------

    //初期化
    protected override void Awake() {
        base.Awake();
        //マップ情報だけ先に読み込んでおく
        if (m_mapData == null) {
            //ステージ作成に必要なデータが入ったCSVを読み込む
            m_stageData.Data = new CSVSetting("ステージ情報");
            //読み込んだデータから必要なメモリを確保
            m_mapData = new CSVSetting[m_stageData.Data.TotalLine];
            //ステージ名を格納しているインデックスを確保する
            m_stageData.Data.GetColumnIndex(0,"ステージ名",out int nameIndex);
            //ステージ名からステージ情報を格納しているCSV全て読み込んでいく
            for (int i = 1; i < m_mapData.Length; i++) {
                m_stageData.Data.GetData(i, nameIndex, out string data);
                m_mapData[i] = new CSVSetting(data);
            }
            //デバッグ用
            KeyDebug.AddKeyAction("マップの作成", () => {_Create(1); });
        }
    }

    /// <summary>マップを作成します Note:岬さんのシーンマネージャーから呼び出します</summary>
    /// <param name="mapNumber">作成するステージ番号 Note:ステージ番号はステージ情報一覧.csvに記載</param>
    private void _Create(int mapNumber) {
        //インスタンスを取得
        m_uiManager??= UIManager.Instance;
        //現在のステージ番号を格納する
        m_stageData.number = mapNumber;
        //マップの高さと１マスのサイズを取得
        int maxY = m_mapData[mapNumber].TotalLine;//高さ
         Vector2 scale = new Vector2(m_stageData.size, m_stageData.size); //サイズ
        //縦のループ
        for (int y = 0; y < maxY; y++) {
            //マップの横幅を取得
            int MaxX = m_mapData[mapNumber].GetLength(y);
            //横のループ
            for (int x = 0; x < MaxX; x++) {
                //読み込んだものを数字に変換する。変換できた場合にマスの作成を行う
                if(m_mapData[mapNumber].GetData(x, y, out int typeNum)) {
                    //数字から配置するマスを決定し配置する
                    if (typeNum >= 0) {
                        //マスの配置場所を計算する
                        Vector2 vec = new Vector2(m_stageData.size * x, m_stageData.size - (m_stageData.size * y));
                        //マスを作成
                        var obj = Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                        obj.transform.localScale = scale;
                        if (typeNum != 1 && typeNum != 2) {
                            //他の背景用に道オブジェクトを配置する　TODO:仮置き
                            Instantiate(MapObject[1], vec, Quaternion.identity);
                        }
                    }
                }     
            }        
        }
        //マップ生成後にタイマーを開く
        m_uiManager.OpenUI(UIType.Timer);
        // アイテムウィンドウを表示
        m_uiManager.OpenUI(UIType.ItemWindow);
    }
}
