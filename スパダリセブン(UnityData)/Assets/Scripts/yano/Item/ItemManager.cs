using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;




/// <summary>
/// �A�C�e���̏����Ȃǂ��Ǘ�����N���X�ł�
/// </summary>
public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    //�A�C�e�����b�Z�[�W�̑I�����Ǘ�����񋓌^
    public enum ItemMessageType {
        Investigation = ItemDataCsvIndex.Investigation,             //�T���p�[�g���b�Z�[�W
        Requisitions = ItemDataCsvIndex.Requisitions,               //�����p�[�g���b�Z�[�W
        RequisitionsHint = ItemDataCsvIndex.RequisitionsHint,       //�����p�[�g�̃q���g���b�Z�[�W
    }

    // �֐��ꗗ //

    /// <summary>�A�C�e�����������܂��B</summary>
    /// <param name="id">��������A�C�e����ID</param>
    public void AddItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("���̃A�C�e���͊��Ɏ擾���Ă��܂��B"); } });  
        m_itemFlag.TInstance.SetFlag(id,true); GetItemWindow(id).SetActive(true);
    }

    /// <summary>�A�C�e���̏����t���O�������܂��B</summary>
    /// <param name="id">�����A�C�e����ID</param>
    public void RemoveItem(ItemID id) {
        UsefulSystem.DebugAction(() => { if (!m_itemFlag.TInstance.GetFlag(id)) { Debug.LogWarning("�w�肳�ꂽ�A�C�e���̏����t���O�͊���false�ł�"); } });
        m_itemFlag.TInstance.SetFlag(id, false); GetItemWindow(id).SetActive(false);
    }

    /// <summary>�w�肳�ꂽ�A�C�e���̏����t���O���擾���܂��B</summary>
    /// <param name="id">�����t���O�𒲂ׂ����A�C�e����ID</param>
    /// <returns>�w�肳�ꂽ�A�C�e���̏����t���O</returns>
    public bool GetFlag(ItemID id) { return m_itemFlag.TInstance.GetFlag(id); }


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

    /// <summary>�X�e�[�W�N���A�ɕK�v�ȃA�C�e���̏����P�擾���܂�</summary>
    /// <param name="stageNumber">�X�e�[�W�ԍ�</param>
    /// <param name="itemNumber">���Ԗڂ̃A�C�e����</param>
    /// <returns></returns>
    public bool GetNeedItem(int stageNumber,int itemNumber,out ItemID data) {
        int index = (int)MapSetting.StageCsvIndex.itemStart + itemNumber - 1;
        bool check = m_itemData.GetData(index,stageNumber, out int dataInt);
        data = check ? (ItemID)dataInt : ItemID.Dummy;
        return check; 
    }

    /// <summary>�X�e�[�W�N���A�ɕK�v�ł͂Ȃ����̑��̃A�C�e�����擾���܂�</summary>
    /// <param name="stageNumber">�X�e�[�W�̔ԍ�</param>
    /// <param name="dataID">���̑��A�C�e���̃f�[�^�z��</param>
    /// <returns>���̑��̃A�C�e�������������ꍇ��false��Ԃ��܂�</returns>
    public bool GetOtherItem(int stageNumber,out ItemID[] dataID) {
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
        int length = m_stageData.GetLength(0);  
        for(int i = (int)MapSetting.StageCsvIndex.itemStart; i < length; i++) {
            if (m_itemData.CheckData(i, stageNumber))total++;
            else  break;      
        }
        length = m_stageData.GetLength(0);  //�E�[���擾
        m_stageData.GetData(length, stageNumber,out string str);
        total += str.Split(',').Length;
        return total;
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

    /// <summary>�A�C�e���E�B���h�E�I�u�W�F�N�g���擾���܂�</summary>
    public GameObject GetItemWindow(ItemID id) { return ItemWindowContent.transform.GetChild((int)id - 1).gameObject; }

    // �A�^�b�`�p�֐� //
    [EnumAction(typeof(ItemID))]
    public void Btn_ItemClick(int id) {
        if(SolveManager.CheckScene) {
            SolveManager.Instance.choice((ItemID)id);
        }
    }




    // private //

    //Item��CSV�t�@�C���̃C���f�b�N�X���Ǘ�����Enum�ł�
    private enum ItemDataCsvIndex {
        ID = 0,                     //ID
        Name = 1,                   //���O
        Investigation = 2,          //�T���p�[�g���b�Z�[�W
        Requisitions = 3,           //�����p�[�g���b�Z�[�W
        RequisitionsHint = 4,       //�����p�[�g�̃q���g���b�Z�[�W
    }

    private static JsonSettings<SettingsGetItemFlags> m_itemFlag;  //�A�C�e���������
    private CSVSetting m_itemData;                                 //���A�C�e���f�[�^ 
    private CSVSetting m_stageData;                                //�X�e�[�W�f�[�^
    [SerializeField,Header("�A�C�e���E�B���h�E���Ǘ����Ă���e�I�u�W�F�N�g")]
    private GameObject ItemWindowContent;                          //�A�C�e���E�B���h�E
   

    //��������Json����̃f�[�^�̓ǂݍ���
    protected override void Awake() {
        base.Awake();
        if(m_itemFlag == null) {
            //�A�C�e������ǂݍ���
            m_itemFlag = new JsonSettings<SettingsGetItemFlags>("Data1","JsonSaveFile", "ItemGetFlags");    //�A�C�e�������f�[�^
            m_itemData = new CSVSetting("�A�C�e�����");   //�A�C�e�����(���b�Z�[�W��)       
            m_stageData = new CSVSetting("�X�e�[�W���");

            //�����A�C�e�����ƃI�u�W�F�N�g�̃A�N�e�B�u������v������@TODO:��ŕς���
            int length = ItemWindowContent.transform.childCount;
            for (int i = 0; i < length; i++) {
                ItemWindowContent.transform.GetChild(i).gameObject.SetActive(m_itemFlag.TInstance.GetFlag((ItemID)i + 1));
            }
        }
    }
}
