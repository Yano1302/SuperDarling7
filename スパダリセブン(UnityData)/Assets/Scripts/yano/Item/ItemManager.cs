using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムの所持などを管理するクラスです
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    //  public //

    public void AddItem(ItemID id) {m_itemData.jsonData.SetFlag(id,true);ItemWindow[(int)id].SetActive(true);}

    // private //   
    private JsonSettings<SettingsGetItemFlags> m_itemData;      //Jsonファイルを読み込むクラス
    private GameObject[] ItemWindow;                            //アイテムウィンドウ


    protected override void Awake() {
        base.Awake();
        if(m_itemData == null) {
            //アイテム情報を読み込む
            m_itemData = new JsonSettings<SettingsGetItemFlags>("ItemGetFlags");
            //アイテムウィンドウを設定する
            int count = transform.childCount;
            Debug.Assert(count == UsefulSystem.GetEnumLength<ItemID>(),"設置されているアイテムウィンドウの個数とアイテムのIDの数が一致しません");
            ItemWindow = new GameObject[count];
            for (int i = 0; i < count; i++) {
                ItemWindow[i] = transform.GetChild(i).gameObject;
                ItemWindow[i].SetActive(m_itemData.jsonData.GetFlag((ItemID)i));
            }
        }
    }
}
