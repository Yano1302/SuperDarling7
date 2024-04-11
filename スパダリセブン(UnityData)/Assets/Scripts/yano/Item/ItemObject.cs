using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemID ID { get { return m_itemID; } }


    [SerializeField,Header("‚±‚ÌƒAƒCƒeƒ€‚ÌID")]
    private ItemID m_itemID;

    private void OnEnable(){
        if (ItemManager.Instance.GetFlag(m_itemID)) {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        ItemManager.Instance.AddItem(m_itemID);
        Destroy(gameObject);
    }
}
