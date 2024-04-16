using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���̏����Ȃǂ��Ǘ�����N���X�ł�
/// TODO : �S�̂̍\����������
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    // �֐��ꗗ //

    /// <summary>�A�C�e�����������܂��B</summary>
    /// <param name="id">��������A�C�e����ID</param>
    public void AddItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (m_json.TInstance.GetFlag(id)) { Debug.LogWarning("���̃A�C�e���͊��Ɏ擾���Ă��܂��B"); } });  
        m_json.TInstance.SetFlag(id,true);ItemWindow[(int)id].SetActive(true);
    }

    /// <summary>�A�C�e���̏����t���O�������܂��B</summary>
    /// <param name="id">�����A�C�e����ID</param>
    public void RemoveItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (!m_json.TInstance.GetFlag(id)) { Debug.LogWarning("�w�肳�ꂽ�A�C�e���̏����t���O�͊���false�ł�"); } });
        m_json.TInstance.SetFlag(id, false); ItemWindow[(int)id].SetActive(false);
    }

    /// <summary>�w�肳�ꂽ�A�C�e���̏����t���O���擾���܂��B</summary>
    /// <param name="id">�����t���O�𒲂ׂ����A�C�e����ID</param>
    /// <returns>�w�肳�ꂽ�A�C�e���̏����t���O</returns>
    public bool GetFlag(ItemID id) { return m_json.TInstance.GetFlag(id); }

    /// <summary> Json�f�[�^�Ɍ��݂̏��������������݂܂��B</summary>
    public void Save() { m_json.Save(); }

    /// <summary>Json�f�[�^��ǂݍ��݂܂�</summary>
    public void Load() { m_json.Load(); }

    /// <summary>�A�C�e���̎擾���������̏�Ԃɖ߂��܂��B</summary>
    public void _Reset() { m_json.Reset(); }
   
    /// <summary>�t���O�̏������O�ɕ\�����܂�(�f�o�b�O�p)</summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() { Debug.Log(m_json.JsonToString()); }

    public GameObject GetItemWindow(ItemID id) { return ItemWindow[(int)id]; }

    // �A�^�b�`�p�֐� //
    [EnumAction(typeof(ItemID))]
    public void Btn_ItemClick(int id) {
        if(SolveManager.CheckScene) {
            SolveManager.Instance.choice((ItemID)id);
        }
    }




    // private //   
    private JsonSettings<SettingsGetItemFlags> m_json;      //Json�t�@�C����ǂݍ��ރN���X
    private GameObject[] ItemWindow;                        //�A�C�e���E�B���h�E

    //��������Json����̃f�[�^�̓ǂݍ���
    protected override void Awake() {
        base.Awake();
        if(m_json == null) {
            //�A�C�e������ǂݍ���
            m_json = new JsonSettings<SettingsGetItemFlags>("Data1","JsonSaveFile", "ItemGetFlags");
            //�A�C�e���E�B���h�E��ݒ肷��
            int count = transform.childCount;
            Debug.Assert(count == UsefulSystem.GetEnumLength<ItemID>(),"�ݒu����Ă���A�C�e���E�B���h�E�̌��ƃA�C�e����ID�̐�����v���܂���");
            ItemWindow = new GameObject[count];
            for (int i = 0; i < count; i++) {
                ItemWindow[i] = transform.GetChild(i).gameObject;
                ItemWindow[i].SetActive(m_json.TInstance.GetFlag((ItemID)i));
            }
        }
    }
}
