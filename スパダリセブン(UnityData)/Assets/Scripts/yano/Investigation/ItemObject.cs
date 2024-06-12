using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    //親のinvpartから格納されます
    public ItemID ID { get { return m_itemID; } set { Debug.Assert(m_itemID == ItemID.Dummy, "アイテムのIDは既に割り振られています"); m_itemID = value; } }
    public InvPart Part { set { m_invPart ??= value; } }

    private ItemID m_itemID = ItemID.Dummy;

    private InvPart m_invPart;
    private InvManager m_invManager;
    private bool m_OnMouse = false;

    private void GetItem() {
        ItemManager.Instance.AddItem(ID);
        AudioManager.Instance.SE_Play("SE_item01");
        m_invManager.SetMouseIcon(false);
        Destroy(gameObject);
            
    }

    private void Update() {
        if (m_OnMouse && Input.GetKeyDown(KeyCode.Mouse0)) { 
            GetItem();
        }
    }


    private void OnEnable(){
        m_invManager ??=  InvManager.Instance;
        //既に取得している場合はオブジェクトを破棄する
        if (ItemManager.Instance.GetFlag(m_itemID)) {
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        m_invManager.SetMouseIcon(true);
        m_OnMouse = true;
        Debug.LogError("呼ばれました");
    }

    public void OnPointerExit(PointerEventData eventData) {
        m_invManager.SetMouseIcon(false);
        m_OnMouse = false;
    }

   

}
