using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Supadari;
using SceneManager = Supadari.SceneManager;

/// <summary>
/// アイテムの所持などを管理するクラスです
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
   

    //アイテムメッセージの選択を管理する列挙型
    public enum ItemMessageType {
        Investigation = ItemDataCsvIndex.Investigation,             //探索パートメッセージ
        Solve = ItemDataCsvIndex.Solve,                             //推理パートメッセージ
        SolveHint = ItemDataCsvIndex.SolveHint,                     //推理パートのヒントメッセージ
    }

    // 関数一覧 //

    /// <summary>アイテムを取得します。※現在アイテムは重複して持てないので複数回呼んでも変わりません</summary>
    /// <param name="id">所持するアイテムのID</param>
    public void AddItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("そのアイテムは既に取得しています。"); } });
        string itemName;
        string imageName;

        m_itemFlag.TInstance.SetFlag(id, true); // アイテムの所持を記録する

        m_itemData.GetData(1, (int)id, out itemName); // アイテム情報よりアイテム名を取得 岬追記
        GameObject item = m_itemWindow.GetWinObj(id); // アイテムのオブジェクトを取得 岬追記
        item.name = itemName; // アイテム名をオブジェクトの名前に代入　岬追記
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName; // アイテム名を子オブジェクトに代入 岬追記
        item.GetComponent<Button>().onClick.AddListener(() => ItemDetails(itemName)); // ボタンにItemDetails関数を設定 岬追記
        m_itemWindow.SetWindow(id, true);
        // 以降探索シーンのみ処理する
        if (m_sceneManager.CheckSceneName == SCENENAME.InvestigationScene)
        {
            getMessage.SetActive(true); // アイテム取得メッセージを表示する 岬追記
            getItemName.text = itemName; // アイテム名を代入 岬追記
            m_itemData.GetData(5, (int)id, out imageName); // アイテム画像名を取得
            ActiveItemImage(imageName, getItemImage); // アイテム画像を表示する 岬追記
            TimerManager timerManager = TimerManager.Instance; // TimerManagerを取得 岬追記
            InvManager invManager = InvManager.Instance; // InvManagerを取得 岬追記
            timerManager.TimerFlag = false; // 制限時間を止める 岬追記
            invManager.VigilanceFlag = false; // 警戒度上昇フラグを消す 岬追記
        }
    }

    /// <summary>アイテムの所持フラグを消します。</summary>
    /// <param name="id">消すアイテムのID</param>
    public void RemoveItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (!m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("指定されたアイテムの所持フラグは既にfalseです"); } });
        m_itemWindow.SetWindow(id,false);
    }

    /// <summary>指定されたアイテムの所持フラグを取得します。</summary>
    /// <param name="id">所持フラグを調べたいアイテムのID</param>
    /// <returns>指定されたアイテムの所持フラグ</returns>
    public bool GetFlag(ItemID id) { return m_itemFlag.TInstance.GetFlag(id); }

    /// <summary>IDに対応するアイテムの名前を取得します</summary>
    public string GetItemName(ItemID id) { m_itemData.GetData((int)ItemDataCsvIndex.Name, (int)id, out string data); return data; }

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
    /// <summary>ステージクリアに必要なアイテムの情報を全て取得します</summary>
    /// <param name="stageNumber">ステージ番号(1~)</param>
    /// <param name="itemNumber">何番目のアイテムか</param>
    /// <returns>アイテムが一つでも見つかった場合はtrueを返します</returns>
    public bool GetAllNeedItem(int stageNumber, out List<ItemID> data)
    {
        data = new List<ItemID>();
        int end = (int)MapManager.StageCsvIndex.otherItem;
        for (int i = (int)MapManager.StageCsvIndex.itemStart; i < end; i++)
        {
            if (m_stageData.GetData(i, stageNumber, out int dataInt))
            {
                data.Add((ItemID)dataInt);
            }
        }
        return data.Count > 0;
    }
    /// <summary>ステージクリアに必要なアイテムの情報を１つ取得します</summary>
    /// <param name="stageNumber">ステージ番号(1~)</param>
    /// <param name="itemNumber">何番目のアイテムか</param>
    /// <returns></returns>
    public bool GetNeedItem(int stageNumber,int itemNumber,out ItemID data) {
        int index = (int)MapManager.StageCsvIndex.itemStart + itemNumber - 1;
        bool check = m_itemData.GetData(index,stageNumber, out int dataInt);
        data = check ? (ItemID)dataInt : ItemID.Dummy;
        return check; 
    }

    /// <summary>ステージクリアに必要ではないその他のアイテムを取得します</summary>
    /// <param name="stageNumber">ステージの番号</param>
    /// <param name="dataID">その他アイテムのデータ配列</param>
    /// <returns>その他のアイテムが無かった場合はfalseを返します</returns>
    public bool GetOtherItems(int stageNumber,out ItemID[] dataID) {
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
        int length = m_stageData.GetLength(0) - 1; //右端を取得
        for (int i = (int)MapManager.StageCsvIndex.itemStart; i < length; i++) {
            if (m_stageData.CheckData(i, stageNumber))total++;
            else  break;      
        }
        m_stageData.GetData(length - 1, stageNumber,out string str);
        total += str != "" ? str.Split(',').Length : 0;
        return total;
    }

    /// <summary>アイテムウィンドウを開きます</summary>
    public void SetItemWindow(bool IsSetting) {
        if (IsSetting) m_itemWindow.ActiveWindows();
        else m_itemWindow.InactiveWindows();
    }

    /// <summary>アイテムウィンドウの状態を変更します</summary>
    public void SwichItemWindow() {
        if (m_itemWindow.IsActiveItemWindow) m_itemWindow.InactiveWindows();
        else m_itemWindow.ActiveWindows();
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

    /// <summary>アイテム取得情報のゲッターセッター関数岬追記</summary>
    public JsonSettings<SettingsGetItemFlags> UsingItemFlag {  get { return m_itemFlag; } set { m_itemFlag = value; } }


    /// <summary>
    /// アイテムの詳細を表示する関数 岬追記
    /// </summary>
    /// <param name="itemName">アイテム名</param>
    public void ItemDetails(string itemName)
    {
        m_sceneManager.audioManager.SE_Play("SE_click", m_sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす

        string details;
        string imageName;
        ItemID id = GetItemID(itemName); // アイテムIDを取得
        GetItemMessage(id, ItemMessageType.Investigation, out details); // アイテム詳細文をdetailsに代入
        itemText.text = details; // アイテムテキストにアイテム詳細文を代入
        m_itemData.GetData(5, (int)id, out imageName); // アイテム画像名を取得
        ActiveItemImage(imageName,itemImage); // アイテム画像を表示する
        selectedID = id; // 選択したアイテムのIDを代入
    }

    /// <summary>
    /// アイテム画像を表示する関数
    /// </summary>
    /// <param name="imageName">表示したいアイテム名</param>
    /// <param name="image">アイテム画像の代入先</param>
    private void ActiveItemImage(string imageName,Image image)
    {
        image.gameObject.SetActive(true); // アイテム画像を表示
        image.sprite = Resources.Load<Sprite>("小物イラスト/" + imageName); // アイテム画像を代入
    }


    // private //
    [SerializeField]
    private TextMeshProUGUI itemText; // アイテムの詳細を表示するテキスト　岬追記
    [SerializeField]
    private Image itemImage;         // アイテム画像　岬追記
    [SerializeField]
    private GameObject getMessage; // アイテム取得メッセージ　岬追記
    private TextMeshProUGUI getItemName; // 取得したアイテム名を表示するテキスト　岬追記
    private Image getItemImage; // 取得したアイテムの画像　岬追記

    //ItemのCSVファイルのインデックスを管理するEnumです
    private enum ItemDataCsvIndex {
        ID = 0,                     //ID
        Name = 1,                   //名前
        Investigation = 2,          //探索パートメッセージ
        Solve = 3,                  //推理パートメッセージ
        SolveHint = 4,              //推理パートのヒントメッセージ
    }

    private static JsonSettings<SettingsGetItemFlags> m_itemFlag;  //アイテム所持情報
    private CSVSetting m_itemData;                                 //総アイテムデータ 
    private CSVSetting m_stageData;                                //ステージデータ
    private ItemWindow m_itemWindow { get { IW ??= ItemWindow.Instance; return IW; } }//アイテムウィンドウ取得プロパティ
    private ItemWindow IW;                                          //アイテムウィンドウ管理インスタンス
    private SceneManager m_sceneManager;    // シーンマネージャー変数
    private UIManager m_uiManager; // UIマネージャー変数　岬追記
    static private ItemID selectedID = 0; // 選択したアイテム 岬追記
    static public ItemID GetSelectedID { get { return selectedID; }  } // 選択したアイテムのゲッター関数　岬追記


    //初期化とJsonからのデータの読み込み
    protected override void Awake() {
        base.Awake();
        if(m_itemFlag == null) {
            //アイテム情報を読み込む   TODO : セーブファイル情報を他で管理する
            m_itemFlag = new JsonSettings<SettingsGetItemFlags>("DataDefault", "JsonSaveFile", "ItemGetFlags");    //アイテム所持データ
            m_itemData = new CSVSetting("アイテム情報");   //アイテム情報(メッセージ等)       
            m_stageData = new CSVSetting("ステージ情報");
        }
    }

    protected void Start() {
        m_sceneManager = SceneManager.Instance;
        m_uiManager = UIManager.Instance; // 岬追記
        ItemMessageSetUp(); // アイテム取得メッセージのセットアップを行う　岬追記
    }


    private void Update()
    {
        if (m_sceneManager.CheckSceneName == SCENENAME.InvestigationScene && Input.GetKeyDown(KeyCode.Escape) || m_sceneManager.CheckSceneName == SCENENAME.SolveScene && Input.GetKeyDown(KeyCode.Escape))
        {
            m_itemWindow.WinSlide();
        }
        // アイテムウィンドウを閉じた際にアイテムが選択されていない状態にする
        if (m_itemWindow.CheckOpen == false && itemText.text != "")
        {
            itemText.text = null;
            itemImage.gameObject.SetActive(false);
            selectedID = default;
        }
    }
    /// <summary>
    /// アイテム取得メッセージの準備を行う関数　岬追記
    /// getMessageの子オブジェクトの順番が変わると動かなくなります
    /// </summary>
    private void ItemMessageSetUp()
    {
        getItemName = getMessage.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        getItemImage = getMessage.transform.GetChild(1).GetChild(2).GetComponent<Image>();
    }
    /// <summary>
    /// アイテム取得メッセージの非表示とテキストの初期化を行う関数
    /// </summary>
    public void ItemMessageRelease()
    {
        itemText.text = null;
        getItemImage.gameObject.SetActive(false);
        getMessage.gameObject.SetActive(false);
        TimerManager timerManager = TimerManager.Instance; // TimerManagerを取得
        InvManager invManager = InvManager.Instance; // InvManagerを取得
        timerManager.TimerFlag = true; // 制限時間を動かす
        invManager.VigilanceFlag = true; // 警戒度上昇フラグを立てる
        invManager.GetItemNum += 1; // 取得したアイテム数を加算する
    }
}
