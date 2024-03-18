using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���̏����Ȃǂ��Ǘ�����N���X�ł�
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    //  public //

    public void AddItem(ItemID id) {m_itemData.jsonData.SetFlag(id,true);ItemWindow[(int)id].SetActive(true);}

    // private //   
    private JsonSettings<SettingsGetItemFlags> m_itemData;      //Json�t�@�C����ǂݍ��ރN���X
    private GameObject[] ItemWindow;                            //�A�C�e���E�B���h�E


    protected override void Awake() {
        base.Awake();
        if(m_itemData == null) {
            //�A�C�e������ǂݍ���
            m_itemData = new JsonSettings<SettingsGetItemFlags>("ItemGetFlags");
            //�A�C�e���E�B���h�E��ݒ肷��
            int count = transform.childCount;
            Debug.Assert(count == UsefulSystem.GetEnumLength<ItemID>(),"�ݒu����Ă���A�C�e���E�B���h�E�̌��ƃA�C�e����ID�̐�����v���܂���");
            ItemWindow = new GameObject[count];
            for (int i = 0; i < count; i++) {
                ItemWindow[i] = transform.GetChild(i).gameObject;
                ItemWindow[i].SetActive(m_itemData.jsonData.GetFlag((ItemID)i));
            }
        }
    }
}
