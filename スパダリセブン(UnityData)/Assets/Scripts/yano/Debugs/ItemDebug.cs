using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ItemDebug : MonoBehaviour
{
    ItemManager ins;
    void Start() {
        ins = ItemManager.Instance;
    }
    void Update() {
        DA(KeyCode.Alpha1, () => ins.AddItem(ItemID.ID0));
        DA(KeyCode.Alpha2, () => ins.AddItem(ItemID.ID1));
        DA(KeyCode.Alpha3, () => ins.AddItem(ItemID.ID3));
        DA(KeyCode.Alpha4, () => ins.AddItem(ItemID.ID4));
        DA(KeyCode.Alpha5, () => ins.AddItem(ItemID.ID5));
        DA(KeyCode.Alpha6, () => ins.AddItem(ItemID.ID6));
        DA(KeyCode.Alpha7, () => ins.AddItem(ItemID.ID7));
        DA(KeyCode.Alpha8, () => ins.AddItem(ItemID.ID8));
        DA(KeyCode.Alpha9, () => ins.AddItem(ItemID.ID9));
        DA(KeyCode.Alpha0, () => ins.AddItem(ItemID.ID10));


        DA(KeyCode.Z, () => ins.Log());
        DA(KeyCode.X, () => ins.Save());
        DA(KeyCode.C, () => ins.Load());

        DA(KeyCode.V, () =>
        {
            ItemID id = (ItemID)Random.Range(0, UsefulSystem.GetEnumLength<ItemID>());
            Debug.Log("�����t���O�𒲂ׂ� ID : " + id);
            Debug.Log("�����t���O : " + ins.GetFlag(id));
        });

        DA(KeyCode.B, () =>
        {
            ItemID id = (ItemID)Random.Range(0, UsefulSystem.GetEnumLength<ItemID>());
            Debug.Log("�j������t���O : " + id);
            Debug.Log("�j���O�̃t���O : " + ins.GetFlag(id));
            ins.RemoveItem(id);
            Debug.Log("�j����̃t���O : " + ins.GetFlag(id));
        });

        DA(KeyCode.N, () =>
        {
            ItemID id = (ItemID)Random.Range(0, UsefulSystem.GetEnumLength<ItemID>());
            Debug.Log("�擾����A�C�e�� ID : " + id);
            ins.AddItem(id);
            Debug.Log("�擾��̃t���O : " + ins.GetFlag(id));
        });

    }


    void DA(KeyCode code, UnityAction action) {
        if (Input.GetKeyDown(code)) {
            action();
        }
    }
}
