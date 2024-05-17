using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public ItemID ID { get { return m_itemID; } }

    [SerializeField,Header("‚±‚ÌƒAƒCƒeƒ€‚ÌID")]
    private ItemID m_itemID;

    private InvManager m_invManager;
    private bool m_OnMouse = false;

    public void GetItem() {
        ItemManager.Instance.AddItem(ID);
        m_invManager.GetItemNum += 1;
        m_invManager.SetMouseIcon(null);
        Destroy(gameObject);
    }

    private void Update() {
        if (m_OnMouse && Input.GetKeyDown(KeyCode.Mouse0)) { 
            GetItem();
        }
    }


    private void OnEnable(){
        m_invManager =  InvManager.Instance;
        if (ItemManager.Instance.GetFlag(m_itemID)) {
            GetItem();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        m_invManager.SetMouseIcon(true);
        m_OnMouse = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        m_invManager.SetMouseIcon(false);
        m_OnMouse = false;
    }

}
