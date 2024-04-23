using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;




/// <summary>
/// アイテムの所持などを管理するクラスです
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    //アイテムメッセージの選択を管理する列挙型
    public enum ItemMessageType {
        Investigation = ItemDataCsvIndex.Investigation,             //探索パートメッセージ
        Requisitions = ItemDataCsvIndex.Requisitions,               //推理パートメッセージ
        RequisitionsHint = ItemDataCsvIndex.RequisitionsHint,       //推理パートのヒントメッセージ
    }

    // 関数一覧 //

    /// <summary>アイテムを所持します。</summary>
    /// <param name="id">所持するアイテムのID</param>
    public void AddItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("そのアイテムは既に取得しています。"); } });  
        m_itemFlag.TInstance.SetFlag(id,true); GetItemWindow(id).SetActive(true);
    }

    /// <summary>アイテムの所持フラグを消します。</summary>
    /// <param name="id">消すアイテムのID</param>
    public void RemoveItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (!m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("指定されたアイテムの所持フラグは既にfalseです"); } });
        m_itemFlag.TInstance.SetFlag(id, false); GetItemWindow(id).SetActive(false);
    }

    /// <summary>指定されたアイテムの所持フラグを取得します。</summary>
    /// <param name="id">所持フラグを調べたいアイテムのID</param>
    /// <returns>指定されたアイテムの所持フラグ</returns>
    public bool GetFlag(ItemID id) { return m_itemFlag.TInstance.GetFlag(id); }


    /// <summary>名前からアイテムのIDを取得します</summary>
    /// <param name="name">アイテム名</param>
    /// <returns>IDを返します</returns>
    public ItemID GetItemID(string name) {
        m_itemData.GetLineIndex((int)ItemDataCsvIndex.Name, name, out int index_y);
        m_itemData.GetData((int)ItemDataCsvIndex.ID, index_y, out int data);
        return (ItemID)data;
    }  
    /// <summary>名前からアイテムのIDを取得します</summary>
    /// <param name="name">アイテム名(複数)</param>
    /// <returns>IDを返します</returns>
    public void GetItemID(in string[] names,out ItemID[] ids) {
        ids = new ItemID[names.Length];
        m_itemData.GetLineIndex((int)ItemDataCsvIndex.Name, name, out int index_y);
        for(int i = 0; i < names.Length; i++) {
            m_itemData.GetData((int)ItemDataCsvIndex.ID, index_y, out int data);
            ids[i] = (ItemID)data;
        }
    }

    /// <summary>アイテムのメッセージを取得します</summary>
    /// <param name="id">アイテムのID</param>
    /// <param name="messageType">メッセージタイプ</param>
    /// <param name="message">渡されるメッセージ</param>
    /// <returns>指定されたメッセージが空白だった場合はfalseを返します</returns>
    public bool GetItemMessage(ItemID id,ItemMessageType messageType,out string message) {
        return m_itemData.GetData((int)messageType, (int)id, out message);
    }
    /// <summary>アイテムのメッセージを取得します</summary>
    /// <param name="name">アイテムの名前</param>
    /// <param name="messageType">メッセージタイプ</param>
    /// <param name="message">渡されるメッセージ</param>
    /// <returns>指定されたメッセージが空白だった場合はfalseを返します</returns>
    public bool GetItemMessage(string name,ItemMessageType messageType, out string message) {
        m_itemData.GetLineIndex((int)messageType, name, out int index_y);
        return m_itemData.GetData((int)messageType, index_y, out message);
    }

    /// <summary>ステージクリアに必要なアイテムの情報を１つ取得します</summary>
    /// <param name="stageNumber">ステージ番号</param>
    /// <param name="itemNumber">何番目のアイテムか</param>
    /// <returns></returns>
    public bool GetNeedItem(int stageNumber,int itemNumber,out ItemID data) {
        int index = (int)MapSetting.StageCsvIndex.itemStart + itemNumber - 1;
        bool check = m_itemData.GetData(index,stageNumber, out int dataInt);
        data = check ? (ItemID)dataInt : ItemID.Dummy;
        return check; 
    }

    /// <summary>ステージクリアに必要ではないその他のアイテムを取得します</summary>
    /// <param name="stageNumber">ステージの番号</param>
    /// <param name="dataID">その他アイテムのデータ配列</param>
    /// <returns>その他のアイテムが無かった場合はfalseを返します</returns>
    public bool GetOtherItem(int stageNumber,out ItemID[] dataID) {
        int index =  m_stageData.GetLength(0) - 1;  //右端を取得
        bool check = m_stageData.GetData(index,stageNumber,out string str);  //読み込む
        var data = str.Split(',');                                          　//分離する
        dataID = check? new ItemID[data.Length]: null;
        for (int i = 0; i < data.Length; i++) {
            dataID[i] = (ItemID)int.Parse(data[i]);
        }
        return check;
    }

    /// <summary>ステージに配置するアイテムの総数を取得します</summary>
    /// <param name="stageNumber"></param>
    /// <returns></returns>
    public int GetTotalItemNum(int stageNumber) {
        int total = 0;
        int length = m_stageData.GetLength(0);  
        for(int i = (int)MapSetting.StageCsvIndex.itemStart; i < length; i++) {
            if (m_itemData.CheckData(i, stageNumber))total++;
            else  break;      
        }
        length = m_stageData.GetLength(0);  //右端を取得
        m_stageData.GetData(length, stageNumber,out string str);
        total += str.Split(',').Length;
        return total;
    }


    /// <summary> Jsonデータに現在の所持情報を書き込みます。</summary>
    public void Save() { m_itemFlag.Save(); }

    /// <summary>Jsonデータを読み込みます</summary>
    public void Load() { m_itemFlag.Load(); }

    /// <summary>アイテムの取得情報を初期の状態に戻します。</summary>
    public void _Reset() { m_itemFlag.Reset(); }
   
    /// <summary>フラグの情報をログに表示します(デバッグ用)</summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() { Debug.Log(m_itemFlag.JsonToString()); }

    /// <summary>アイテムウィンドウオブジェクトを取得します</summary>
    public GameObject GetItemWindow(ItemID id) { return ItemWindowContent.transform.GetChild((int)id - 1).gameObject; }

    // アタッチ用関数 //
    [EnumAction(typeof(ItemID))]
    public void Btn_ItemClick(int id) {
        if(SolveManager.CheckScene) {
            SolveManager.Instance.choice((ItemID)id);
        }
    }




    // private //

    //ItemのCSVファイルのインデックスを管理するEnumです
    private enum ItemDataCsvIndex {
        ID = 0,                     //ID
        Name = 1,                   //名前
        Investigation = 2,          //探索パートメッセージ
        Requisitions = 3,           //推理パートメッセージ
        RequisitionsHint = 4,       //推理パートのヒントメッセージ
    }

    private static JsonSettings<SettingsGetItemFlags> m_itemFlag;  //アイテム所持情報
    private CSVSetting m_itemData;                                 //総アイテムデータ 
    private CSVSetting m_stageData;                                //ステージデータ
    [SerializeField,Header("アイテムウィンドウを管理している親オブジェクト")]
    private GameObject ItemWindowContent;                          //アイテムウィンドウ
   

    //初期化とJsonからのデータの読み込み
    protected override void Awake() {
        base.Awake();
        if(m_itemFlag == null) {
            //アイテム情報を読み込む
            m_itemFlag = new JsonSettings<SettingsGetItemFlags>("Data1","JsonSaveFile", "ItemGetFlags");    //アイテム所持データ
            m_itemData = new CSVSetting("アイテム情報");   //アイテム情報(メッセージ等)       
            m_stageData = new CSVSetting("ステージ情報");

            //所持アイテム情報とオブジェクトのアクティブ情報を一致させる　TODO:後で変える
            int length = ItemWindowContent.transform.childCount;
            for (int i = 0; i < length; i++) {
                ItemWindowContent.transform.GetChild(i).gameObject.SetActive(m_itemFlag.TInstance.GetFlag((ItemID)i + 1));
            }
        }
    }
}
