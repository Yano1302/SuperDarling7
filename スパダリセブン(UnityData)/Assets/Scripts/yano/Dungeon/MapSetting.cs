using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapType {
    player = 0,
    road   = 1,
    wall   = 2,
    catchtrap = 3,
    pitfall = 4,
    Goal = 5,
}

public class MapSetting : SingletonMonoBehaviour<MapSetting>
{
    // Config変数  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("総マップ数")]
    private int AllMapNum = 1;
    [SerializeField, Header("一マスの縦のサイズ")]
    private int Height = 1;
    [SerializeField,Header("一マスの横のサイズ")]
    private int Width = 1;  
    [SerializeField, Header("マップオブジェクト"), EnumIndex(typeof(MapType))]
    private GameObject[] MapObject;

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void CreateMap(int mapNumber) {
        mapNumber -= 1;
        _Create(mapNumber);
    }

   
    //  プライベート変数  //------------------------------------------------------------------------------------------------------------------------------
    private List<string>[] m_mapData = null;
    //  プライベート関数  //------------------------------------------------------------------------------------------------------------------------------
    
    //初期化
    protected override void Awake() {
        base.Awake();
        //マップ情報だけ先に読み込んでおく
        if(m_mapData == null) {
            m_mapData = new List<string>[AllMapNum];
            for (int i = 0; i < AllMapNum; i++) {
                string path = UsefulSystem.FindFilePath("ステージ" + (i + 1)+".txt");
                m_mapData[i] =  UsefulSystem.Reader_TextFile(path);
            }
        }
    }

    //TODO　どこかからこの関数を呼び出す
    private void _Create(int mapNumber) {
        //右上から読み込む
        int heightCount = m_mapData[mapNumber].Count - 1;
        for (int i = heightCount; i >= 0; i--){
            //横一列分読み込む
            string line = m_mapData[mapNumber][i];
            for (int j = 0; j < line.Length; j++) {
                //読み込んだID(Char型)をint型に変換
                int typeNum = line[j] - '0';
                if(typeNum >= 0) {
                    Vector2 vec = new Vector2(Width * j, Height * (heightCount - i));
                    Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                    if (typeNum != 1 && typeNum != 2) {
                        Instantiate(MapObject[1], vec, Quaternion.identity);
                    }
                       
                } 
            }
        }
        UIManager.Instance.OpenUI(UIType.Timer);
    }
}
