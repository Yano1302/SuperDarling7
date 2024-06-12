using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


//TODO JSONに対応させる
public enum MapType {
    Dummy = 0,
    Player = 1,
    road   = 2,
    wall   = 3,
    catchtrap = 4,
    pitfall = 5,
    Goal = 6,
    
}

public class MapManager : SingletonMonoBehaviour<MapManager> {
    
    /// <summary>マップを生成します</summary>
    /// <param name="stageNumber">マップ番号</param>
    public void CreateMap(int stageNumber) {
        _Create(stageNumber);
    }

    /// <summary>現在のステージ番号を取得します</summary>
    public int StageNumber { get { Debug.Assert(m_stageData.number > -1,"ステージが生成されていません。");  return m_stageData.number; } }

    /// <summary>現在のステージの全アイテム数を取得します</summary>
    public int TotalItem { get { Debug.Assert(m_stageData.number > -1, "ステージが生成されていません。"); return m_stageData.totalitem; } }

    /// <summary>ステージの制限時間を取得します</summary>
    public float Time { get { return m_stageData.time; } }
    /// <summary>マップの総数を取得します</summary>
    public int TotalMapNumber { get { return StageData.Data.TotalLine - 1; } }

    // アタッチ変数  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("マップオブジェクト"), EnumIndex(typeof(MapType))]
    private GameObject[] MapObject;

    [SerializeField, Header("Invマネージャーオブジェクト")]
    private GameObject InvManagerObject;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
   
    /// <summary>ステージのCSVを読み込むためのインデックスを管理します</summary>
    public enum StageCsvIndex {
        number = 0,
        name = 1,
        time = 2,
        size = 3,
        InvPart= 4,
        itemStart = 5,
        otherItem = 11,
    }

    //  プライベート変数  //------------------------------------------------------------------------------------------------------------------------------

    //マップデータ
    StageData m_stageData;

    //ステージのデータを管理する構造体
    private struct StageData {
        public static CSVSetting Data;     //マップデータを全て格納したデータCSV
        public int number;                 //マップ番号(これを元にデータを返します)
        public string name { get { Data.GetData((int)StageCsvIndex.name,number, out string data); return data; } } //ステージ名
        public float time  { get { Data.GetData((int)StageCsvIndex.time, number, out int data); return data; } }    //制限時間
        public float size  { get { Data.GetData((int)StageCsvIndex.size,number, out int data); return data; } }    //一マスのサイズ
        public int totalitem { get; set; }
    }

    //  プライベート関数  //------------------------------------------------------------------------------------------------------------------------------

    //初期化
    protected override void Awake() {
        base.Awake();
        //ステージ情報だけ先に読み込んでおく
        if (StageData.Data == null) {
            //ステージ作成に必要なデータが入ったCSVを読み込む
            StageData.Data = new CSVSetting("ステージ情報");
            m_stageData.number = -1;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }
    }
    private void Start() {
        //デバッグ用
        KeyDebug.AddKeyAction("マップの作成", () => { _Create(1); });
        KeyDebug.AddKeyAction("マップ名をログに表示する", () => { Debug.Log(m_stageData.name); });
        KeyDebug.AddKeyAction("制限時間をログに表示する", () => { Debug.Log(m_stageData.time); });
        KeyDebug.AddKeyAction("マップのサイズをログに表示する", () => { Debug.Log(m_stageData.size); });
    }

    /// <summary>マップを作成します Note:岬さんのシーンマネージャーから呼び出します</summary>
    /// <param name="mapNumber">作成するステージ番号 Note:ステージ番号はステージ情報一覧.csvに記載</param>
    private void _Create(int mapNumber) {
        //調査パートを管理するオブジェクトを生成する
        Instantiate(InvManagerObject);
        //マップ情報の調査パート一覧を読み込む
        StageData.Data.GetData((int)StageCsvIndex.InvPart,mapNumber,out string invStr);
        //調査パート名は'/'で区切られているはずなので分解してそれぞれ取得する
        var invData = invStr.Split('/');
        //調査パートの配列にこのステージの全ての調査パートを格納する
        InvType[] invTypes = new InvType[invData.Length];                               
        for (int i = 0; i < invData.Length; i++) {
            if(UsefulSystem.GetEnum(out InvType t, invData[i])) {
                invTypes[i] = t;
            }
            else {
                UsefulSystem.LogError($"{ invData[i]  }に一致する調査パート名が見つかりません。InvTypeの記述やCSV内を確認してください。");
            }    
        }
        //調査パート全体の管理オブジェクトに情報を渡す
        InvManager.Instance.SetUpInv(invTypes);                                        
        
        //ステージ名を取得する
        StageData.Data.GetData((int)StageCsvIndex.name, mapNumber, out string data);
        //ステージ名と一致するCSVファイルがあるはずなので読み込む
        var mapData = new CSVSetting(data);
        //現在のステージ番号を格納する
        m_stageData.number = mapNumber;
        

        //現在のステージのアイテム数を取得する
        var im = ItemManager.Instance;
        m_stageData.totalitem = im.GetTotalItemNum(mapNumber);
        //マップの高さと１マスのサイズを取得
        int maxY = mapData.TotalLine;//高さ
         Vector2 scale = new Vector2(m_stageData.size, m_stageData.size); //サイズ
        //縦のループ
        for (int y = 0; y < maxY; y++) {
            //マップの横幅を取得
            int MaxX = mapData.GetLength(y);
            //横のループ
            for (int x = 0; x < MaxX; x++) {
                //読み込んだものを数字に変換する。変換できた場合にマスの作成を行う
                if(mapData.GetData(x, y, out int typeNum)) {
                    //数字から配置するマスを決定し配置する
                    if (typeNum == 0) {  continue;  }//０の場合は配置しない
                    Vector2 vec = new Vector2(m_stageData.size * x, m_stageData.size - (m_stageData.size * y));//マスの配置場所を計算する
                    //マスを作成
                    var obj = Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                    obj.transform.localScale = scale;
                    if (typeNum != (int)MapType.road && typeNum != (int)MapType.wall) {
                        //他の背景用に道オブジェクトを配置する　TODO:仮置き
                        Instantiate(MapObject[(int)MapType.road], vec, Quaternion.identity);
                    }
                }
                else {
                    //変換できない場合の例外処理
                    mapData.GetData(x, y,out string str);
                    int type = CutParentheses(str,out string d);
                    switch ((MapType)type) {
                        case MapType.Goal:
                            Vector2 vec = new Vector2(m_stageData.size * x, m_stageData.size - (m_stageData.size * y));//マスの配置場所を計算する
                            var obj = Instantiate(MapObject[type], vec, Quaternion.identity);
                            UsefulSystem.GetEnum(out InvType t, str);
                            obj.GetComponent<Goal>().InvType = t;
                            break;
                    } 
                }
            }        
        }
    
        //マップ生成後にタイマーを開く
        TimerManager.Instance.SetTimer(m_stageData.time);
        // アイテムウィンドウを表示
        ItemWindow.Instance.ActiveWindows();
    }

    private void SceneUnloaded(Scene thisScene) {
        m_stageData.number = -1;
    }

    private int CutParentheses(in string str,out string Data) {
        int startIndex = str.IndexOf('[');
        Data = str.Substring(startIndex,str.Length - startIndex - 1 );  //-1は閉じかっこ　] の分
        return int.Parse(str.Substring(0,startIndex));
    }

}
