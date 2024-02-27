using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapType {
    player,
    white,
    black,

}

public class MapSetting : SingletonMonoBehaviour<MapSetting>
{
    // Config変数  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("作成するマップの数")]
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

    private void _Create(int mapNumber) {
        //右上から読み込む
        for (int i = 0; i < m_mapData[mapNumber].Count; i++){
            //横一列分読み込む
            string line = m_mapData[mapNumber][i];
            for (int j = 0; j < line.Length; j++) {
                //読み込んだID(Char型)をint型に変換
                int typeNum = line[j] - '0';
                if(typeNum >= 0) {
                    Vector2 vec = new Vector2(Width * j, Height * i);
                    Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                } 
            }
        }

    }
}
