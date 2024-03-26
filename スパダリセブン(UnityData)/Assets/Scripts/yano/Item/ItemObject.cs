using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField,Header("���̃A�C�e����ID")]
    private ItemID m_itemID;

    private void OnCollisionEnter2D(Collision2D collision) {
        ItemManager.Instance.AddItem(m_itemID);
        Destroy(gameObject);
    }
}
