using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// �A�C�e���̏����Ȃǂ��Ǘ�����N���X�ł�
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    public TextMeshProUGUI itemText; // �A�C�e���̏ڍׂ�\������e�L�X�g�@���ǋL
    public Image itemImage; // �A�C�e���摜�@���ǋL

    //�A�C�e�����b�Z�[�W�̑I�����Ǘ�����񋓌^
    public enum ItemMessageType {
        Investigation = ItemDataCsvIndex.Investigation,             //�T���p�[�g���b�Z�[�W
        Solve = ItemDataCsvIndex.Solve,                             //�����p�[�g���b�Z�[�W
        SolveHint = ItemDataCsvIndex.SolveHint,                     //�����p�[�g�̃q���g���b�Z�[�W
    }

    // �֐��ꗗ //

    /// <summary>�A�C�e�����擾���܂��B�����݃A�C�e���͏d�����Ď��ĂȂ��̂ŕ�����Ă�ł��ς��܂���</summary>
    /// <param name="id">��������A�C�e����ID</param>
    public void AddItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("���̃A�C�e���͊��Ɏ擾���Ă��܂��B"); } });
        string itemName;
        m_itemData.GetData(1, (int)id, out itemName); // �A�C�e�������A�C�e�������擾 ���ǋL
        GameObject item = m_itemWindow.GetWinObj(id); // �A�C�e���̃I�u�W�F�N�g���擾
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName; // �A�C�e�������q�I�u�W�F�N�g�ɑ�� ���ǋL
        item.GetComponent<Button>().onClick.AddListener(() => ItemDetails(itemName)); // �{�^����ItemDetails�֐���ݒ�
        m_itemWindow.SetWindow(id,true);
    }

    /// <summary>�A�C�e���̏����t���O�������܂��B</summary>
    /// <param name="id">�����A�C�e����ID</param>
    public void RemoveItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (!m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("�w�肳�ꂽ�A�C�e���̏����t���O�͊���false�ł�"); } });
        m_itemWindow.SetWindow(id,false);
    }

    /// <summary>�w�肳�ꂽ�A�C�e���̏����t���O���擾���܂��B</summary>
    /// <param name="id">�����t���O�𒲂ׂ����A�C�e����ID</param>
    /// <returns>�w�肳�ꂽ�A�C�e���̏����t���O</returns>
    public bool GetFlag(ItemID id) { return m_itemFlag.TInstance.GetFlag(id); }

    /// <summary>ID�ɑΉ�����A�C�e���̖��O���擾���܂�</summary>
    public string GetItemName(ItemID id) { m_itemData.GetData((int)ItemDataCsvIndex.Name, (int)id, out string data); return data; }

    /// <summary>���O����A�C�e����ID���擾���܂�</summary>
    /// <param name="name">�A�C�e����</param>
    /// <returns>ID��Ԃ��܂�</returns>
    public ItemID GetItemID(string name) {
        m_itemData.GetLineIndex((int)ItemDataCsvIndex.Name, name, out int index_y);
        m_itemData.GetData((int)ItemDataCsvIndex.ID, index_y, out int data);
        return (ItemID)data;
    }  
    /// <summary>���O����A�C�e����ID���擾���܂�</summary>
    /// <param name="name">�A�C�e����(����)</param>
    /// <returns>ID��Ԃ��܂�</returns>
    public void GetItemID(in string[] names,out ItemID[] ids) {
        ids = new ItemID[names.Length];
        m_itemData.GetLineIndex((int)ItemDataCsvIndex.Name, name, out int index_y);
        for(int i = 0; i < names.Length; i++) {
            m_itemData.GetData((int)ItemDataCsvIndex.ID, index_y, out int data);
            ids[i] = (ItemID)data;
        }
    }

    /// <summary>�A�C�e���̃��b�Z�[�W���擾���܂�</summary>
    /// <param name="id">�A�C�e����ID</param>
    /// <param name="messageType">���b�Z�[�W�^�C�v</param>
    /// <param name="message">�n����郁�b�Z�[�W</param>
    /// <returns>�w�肳�ꂽ���b�Z�[�W���󔒂������ꍇ��false��Ԃ��܂�</returns>
    public bool GetItemMessage(ItemID id,ItemMessageType messageType,out string message) {
        return m_itemData.GetData((int)messageType, (int)id, out message);
    }
    /// <summary>�A�C�e���̃��b�Z�[�W���擾���܂�</summary>
    /// <param name="name">�A�C�e���̖��O</param>
    /// <param name="messageType">���b�Z�[�W�^�C�v</param>
    /// <param name="message">�n����郁�b�Z�[�W</param>
    /// <returns>�w�肳�ꂽ���b�Z�[�W���󔒂������ꍇ��false��Ԃ��܂�</returns>
    public bool GetItemMessage(string name,ItemMessageType messageType, out string message) {
        m_itemData.GetLineIndex((int)messageType, name, out int index_y);
        return m_itemData.GetData((int)messageType, index_y, out message);
    }
    /// <summary>�X�e�[�W�N���A�ɕK�v�ȃA�C�e���̏���S�Ď擾���܂�</summary>
    /// <param name="stageNumber">�X�e�[�W�ԍ�(1~)</param>
    /// <param name="itemNumber">���Ԗڂ̃A�C�e����</param>
    /// <returns>�A�C�e������ł����������ꍇ��true��Ԃ��܂�</returns>
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
    /// <summary>�X�e�[�W�N���A�ɕK�v�ȃA�C�e���̏����P�擾���܂�</summary>
    /// <param name="stageNumber">�X�e�[�W�ԍ�(1~)</param>
    /// <param name="itemNumber">���Ԗڂ̃A�C�e����</param>
    /// <returns></returns>
    public bool GetNeedItem(int stageNumber,int itemNumber,out ItemID data) {
        int index = (int)MapManager.StageCsvIndex.itemStart + itemNumber - 1;
        bool check = m_itemData.GetData(index,stageNumber, out int dataInt);
        data = check ? (ItemID)dataInt : ItemID.Dummy;
        return check; 
    }

    /// <summary>�X�e�[�W�N���A�ɕK�v�ł͂Ȃ����̑��̃A�C�e�����擾���܂�</summary>
    /// <param name="stageNumber">�X�e�[�W�̔ԍ�</param>
    /// <param name="dataID">���̑��A�C�e���̃f�[�^�z��</param>
    /// <returns>���̑��̃A�C�e�������������ꍇ��false��Ԃ��܂�</returns>
    public bool GetOtherItems(int stageNumber,out ItemID[] dataID) {
        int index =  m_stageData.GetLength(0) - 1;  //�E�[���擾
        bool check = m_stageData.GetData(index,stageNumber,out string str);  //�ǂݍ���
        var data = str.Split(',');                                          �@//��������
        dataID = check? new ItemID[data.Length]: null;
        for (int i = 0; i < data.Length; i++) {
            dataID[i] = (ItemID)int.Parse(data[i]);
        }
        return check;
    }

    /// <summary>�X�e�[�W�ɔz�u����A�C�e���̑������擾���܂�</summary>
    /// <param name="stageNumber"></param>
    /// <returns></returns>
    public int GetTotalItemNum(int stageNumber) {
        int total = 0;
        int length = m_stageData.GetLength(0) - 1; //�E�[���擾
        for (int i = (int)MapManager.StageCsvIndex.itemStart; i < length; i++) {
            if (m_stageData.CheckData(i, stageNumber))total++;
            else  break;      
        }
        m_stageData.GetData(length - 1, stageNumber,out string str);
        total += str != "" ? str.Split(',').Length : 0;
        return total;
    }

    /// <summary>�A�C�e���E�B���h�E���J���܂�</summary>
    public void SetItemWindow(bool IsSetting) {
        if (IsSetting) m_itemWindow.ActiveWindows();
        else m_itemWindow.InactiveWindows();
    }

    /// <summary>�A�C�e���E�B���h�E�̏�Ԃ�ύX���܂�</summary>
    public void SwichItemWindow() {
        if (m_itemWindow.IsActiveItemWindow) m_itemWindow.InactiveWindows();
        else m_itemWindow.ActiveWindows();
    }

    /// <summary> Json�f�[�^�Ɍ��݂̏��������������݂܂��B</summary>
    public void Save() { m_itemFlag.Save(); }

    /// <summary>Json�f�[�^��ǂݍ��݂܂�</summary>
    public void Load() { m_itemFlag.Load(); }

    /// <summary>�A�C�e���̎擾���������̏�Ԃɖ߂��܂��B</summary>
    public void _Reset() { m_itemFlag.Reset(); }
   
    /// <summary>�t���O�̏������O�ɕ\�����܂�(�f�o�b�O�p)</summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() { Debug.Log(m_itemFlag.JsonToString()); }

  

    // private //

    //Item��CSV�t�@�C���̃C���f�b�N�X���Ǘ�����Enum�ł�
    private enum ItemDataCsvIndex {
        ID = 0,                     //ID
        Name = 1,                   //���O
        Investigation = 2,          //�T���p�[�g���b�Z�[�W
        Solve = 3,                  //�����p�[�g���b�Z�[�W
        SolveHint = 4,              //�����p�[�g�̃q���g���b�Z�[�W
    }

    private static JsonSettings<SettingsGetItemFlags> m_itemFlag;  //�A�C�e���������
    private CSVSetting m_itemData;                                 //���A�C�e���f�[�^ 
    private CSVSetting m_stageData;                                //�X�e�[�W�f�[�^
    private ItemWindow m_itemWindow { get { IW ??= ItemWindow.Instance; return IW; } }//�A�C�e���E�B���h�E�擾�v���p�e�B
    private ItemWindow IW;                                          //�A�C�e���E�B���h�E�Ǘ��C���X�^���X
   

    //��������Json����̃f�[�^�̓ǂݍ���
    protected override void Awake() {
        base.Awake();
        if(m_itemFlag == null) {
            //�A�C�e������ǂݍ���   TODO : �Z�[�u�t�@�C�����𑼂ŊǗ�����
            m_itemFlag = new JsonSettings<SettingsGetItemFlags>("Data1","JsonSaveFile", "ItemGetFlags");    //�A�C�e�������f�[�^
            m_itemData = new CSVSetting("�A�C�e�����");   //�A�C�e�����(���b�Z�[�W��)       
            m_stageData = new CSVSetting("�X�e�[�W���");
        }
    }
    ///�e�X�g�p
    /*private void Start()
    {
        //�����A�C�e�����ƃI�u�W�F�N�g�̃A�N�e�B�u������v������@TODO:��ŕς���
        int length = 6;
        for (int i = 1; i < length; i++)
        {
            string itemName;
            m_itemData.GetData(1, i, out itemName); // �A�C�e�������A�C�e�������擾 ���ǋL
            GameObject g = m_itemWindow.GetWinObj((ItemID)i); // �A�C�e�������q�I�u�W�F�N�g�ɑ�� ���ǋL
            g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName; // �{�^����ItemDetails�֐���ݒ�
            g.GetComponent<Button>().onClick.AddListener(() => ItemDetails(itemName)); // �{�^����ItemDetails�֐���ݒ�
            g.SetActive(m_itemFlag.TInstance.GetFlag((ItemID)i + 1)); // �\��
        }
    }*/

    /// <summary>
    /// �A�C�e���̏ڍׂ�\������֐�
    /// </summary>
    /// <param name="itemName">�A�C�e����</param>
    public void ItemDetails(string itemName)
    {
        string details;
        string imageName;
        ItemID id = GetItemID(itemName); // �A�C�e��ID���擾
        GetItemMessage(id,ItemMessageType.Investigation, out details); // �A�C�e���ڍו���details�ɑ��
        itemText.text = details; // �A�C�e���e�L�X�g�ɃA�C�e���ڍו�����
        m_itemData.GetData(5, (int)id, out imageName); // �A�C�e���摜�����擾
        itemImage.gameObject.SetActive(true); // �A�C�e���摜��\��
        itemImage.sprite = Resources.Load<Sprite>("�����C���X�g/" + imageName); // �A�C�e���摜����
    }
}
