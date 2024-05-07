using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemID ID { get { return m_itemID; } }

    [SerializeField,Header("‚±‚ÌƒAƒCƒeƒ€‚ÌID")]
    private ItemID m_itemID;

    public void GetItem() {
        ItemManager.Instance.AddItem(ID);
        InvManager.GetItemNum += 1;
        Destroy(gameObject);
    }

    private void OnEnable(){
        if (ItemManager.Instance.GetFlag(m_itemID)) {
            GetItem();
        }
    }
  
}
