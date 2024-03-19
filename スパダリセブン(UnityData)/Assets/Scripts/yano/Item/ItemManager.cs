using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムの所持などを管理するクラスです
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    //  public //

    /// <summary>アイテムを取得します。</summary>
    /// <param name="id">取得するアイテムのID</param>
    public void AddItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (m_json.Instance.GetFlag(id)){ Debug.LogWarning("そのアイテムは既に取得しています。"); } });  
        m_json.Instance.SetFlag(id,true);ItemWindow[(int)id].SetActive(true);
    }

    /// <summary>アイテムの所持フラグを消します。</summary>
    /// <param name="id">消すアイテムのID</param>
    public void RemoveItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (!m_json.Instance.GetFlag(id)) { Debug.LogWarning("指定されたアイテムの所持フラグは既にfalseです"); } });
        m_json.Instance.SetFlag(id, false); ItemWindow[(int)id].SetActive(false);
    }

    /// <summary>指定されたアイテムの所持フラグを取得します。</summary>
    /// <param name="id">所持フラグを調べたいアイテムのID</param>
    /// <returns>指定されたアイテムの所持フラグ</returns>
    public bool GetFlag(ItemID id) { return m_json.Instance.GetFlag(id); }

    /// <summary> Jsonデータに現在の所持情報を書き込みます。</summary>
    public void Save() { m_json.Save(); }

    /// <summary>Jsonデータを読み込みます</summary>
    public void Load() { m_json.Load(); }

    /// <summary>フラグの情報をログに表示します(デバッグ用)</summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() { Debug.Log(m_json.JsonToString()); }





    // private //   
    private JsonSettings<SettingsGetItemFlags> m_json;      //Jsonファイルを読み込むクラス
    private GameObject[] ItemWindow;                            //アイテムウィンドウ

    //初期化とJsonからのデータの読み込み
    protected override void Awake() {
        base.Awake();
        if(m_json == null) {
            //アイテム情報を読み込む
            m_json = new JsonSettings<SettingsGetItemFlags>("ItemGetFlags");
            //アイテムウィンドウを設定する
            int count = transform.childCount;
            Debug.Assert(count == UsefulSystem.GetEnumLength<ItemID>(),"設置されているアイテムウィンドウの個数とアイテムのIDの数が一致しません");
            ItemWindow = new GameObject[count];
            for (int i = 0; i < count; i++) {
                ItemWindow[i] = transform.GetChild(i).gameObject;
                ItemWindow[i].SetActive(m_json.Instance.GetFlag((ItemID)i));
            }
        }
    }
}
