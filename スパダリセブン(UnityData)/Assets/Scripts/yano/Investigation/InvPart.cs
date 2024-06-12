using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SceneManager = Supadari.SceneManager;

//�e�����p�[�g���Ǘ�����N���X�I�u�W�F�N�g
public class InvPart : MonoBehaviour {
    /// <summary>MapManager����i�[����܂�</summary>-------------------------------------------------------------
    public InvType MyIntType { get { return m_InvType; } }
    //------------------------------------------------------------------------------------------------------------

    /// <summary>���g�̒����p�[�g�̎�ނ�InvManager����ݒ肵�܂�</summary>---------------------------------------
    public void SetUpInvPart(InvType type, GameObject itemCol, InvGauge invGage, GameObject warning) {
        gameObject.name = type.ToString();
        m_img = GetComponent<Image>();
        m_img.sprite = InvTextureholder.Instance.GetSprite(type);
        m_img.enabled = false;
        m_InvType = type;
        m_InvGauge = invGage;
        m_warningMessage = warning;
        InitialiseInvPart(itemCol);
    }

   // public bool ClearInv { get { return  } }

    /// <summary>���̒����p�[�g���J���������s���܂�</summary>
    public void Open() {
        //�}�E�X���W������������
        m_vig.MouseVec = Input.mousePosition;
        //�t���O�ݒ�
        m_InvGauge.OpenGauge(m_vig.Rate);
        m_img.enabled = true;
        foreach (var i in m_itemObj) {
            if (i != null) {
                i.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>���̒����p�[�g����鏈�����s���܂�</summary>
    public void Close() {
        foreach (var i in m_itemObj) {
            if (i != null) {
                i.gameObject.SetActive(false);
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------

    /// <summary>�c��̃A�C�e���̌����m�F���A�S�Ď擾���Ă���ꍇ��InvPart����܂��B</summary>
    private void CheckItemNum() {
        if (m_itemObj.Count <= m_getItemCount) {
            //�����ɓ��B������A�C�e����S�Ď擾���Ă��邱�ƂɂȂ�
            Debug.LogError("�A�C�e����S�Ď擾���܂����B");
        }
    }







    //���݂̌x���x���O�ɂ��܂�
    public void ResetVigilance() {
        m_vig.CurrentVigilance = 0;
        m_InvGauge.ResetGauge();
    }


   

    /// <summary>�\���̂̏������ƃA�C�e���z�u���s���܂�</summary>
    private void InitialiseInvPart(GameObject itemCol) {
        //�ȉ�������
        CSVSetting data = new CSVSetting(m_InvType.ToString());
        m_itemObj = new List<ItemObject>();
        m_getItemCount = 0;
        //�ő�l��ݒ肷��
        data.GetData((int)InvDataCSVIndex.MaxVigilance, 1, out float max);
        m_vig.MaxVigilance = max;
        //�x���x������������
        data.GetData((int)InvDataCSVIndex.InitialiseVigilance, 1, out float initial);
        m_vig.CurrentVigilance = initial;
        //���Ԍo�߂ɂ��㏸�ʂ�ݒ肷��
        data.GetData((int)InvDataCSVIndex.AddTimeValue, 1, out float time);
        m_vig.AddTimeVigilance = time;
        //�}�E�X����ɂ��㏸�ʂ�ݒ肷��
        data.GetData((int)InvDataCSVIndex.AddMoseValue, 1, out float mouse);
        m_vig.AddMouseVigilance = mouse;
        //�P�\���Ԃ�ݒ肷��
        data.GetData((int)InvDataCSVIndex.GraceTime, 1, out float grace);
        m_vig.GraceTime = grace;
        //�A�C�e���I�u�W�F�N�g�𐶐�����
        int length = data.GetLength(1);
        var itemManager = ItemManager.Instance;
        for (int i = (int)InvDataCSVIndex.ItemStart; i < length; i++) {
            if (!data.GetData(i, 1, out string name)) {
                break;
            }
            //�A�C�e���𐶐�����
            var obj = Instantiate(itemCol);
            //�A�C�e����InvPart�̎q�ɂ���
            obj.transform.SetParent(transform, false);
            //���������A�C�e���I�u�W�F�N�g���擾����
            var itemObj = obj.AddComponent<ItemObject>();
            //ID��������U��
            var id = itemManager.GetItemID(name);
            itemObj.ID = id;
            itemObj.Part = this;
            itemObj.name = name;
            m_itemObj.Add(itemObj);
            //���W��ݒ肷��
            var rect = obj.GetComponent<RectTransform>();
            data.GetData(i, 2, out float x);
            data.GetData(i, 3, out float y);
            Vector2 vec = new Vector2(x - 960, y - 540);
            rect.anchoredPosition = vec;
            Debug.LogError($"{ name } : x[{ x }] , y[{y}]");
        }
    }

    /// <summary>�x���x���Ǘ�����\����</summary>
    private struct Vigilance {
        public InvManager invManager;            //InvManager�C���X�^���X
        public float MaxVigilance;               // �ő�x���x
        public float CurrentVigilance;           // ���݂̌x���x  
        public float AddTimeVigilance;           //���Ԍo�߂ɂ��㏸��
        public float AddMouseVigilance;          //�}�E�X�ɂ��㏸��
        public Vector2 MouseVec;                 //�}�E�X���W��ۑ����܂�  
        public float GraceTime;                 // �x���x�ő厞�ɓ����Ă��ǂ��P�\���� ���ǋL
        public float ReprieveGameOver;          // ��L�𔻒肷��ׂ̕ϐ��@���ǋL

        /// <summary>���݂̌x���x�̊������擾���܂�</summary>
        public float Rate { get { return CurrentVigilance / MaxVigilance; } }
        /// <summary>���݂̌x���x���ő�x���x�ȏ�̏ꍇ��true��Ԃ��܂�</summary>
        public bool IsOver { get { return CurrentVigilance >= MaxVigilance; } }
        //�x���x�㏸�t���O
        public bool VigilanceFlag { get { invManager ??= InvManager.Instance; return invManager; } }

    }

    //���̃p�[�g�ɂ���A�C�e���I�u�W�F�N�g
    private List<ItemObject> m_itemObj;
    //���g��Img���
    private Image m_img;
    //�x���x�p�\����
    private Vigilance m_vig;
    //�����p�[�g�̎��
    private InvType m_InvType;
    //�x���x�Q�[�W
    private InvGauge m_InvGauge;
    //�댯���b�Z�[�W
    private GameObject m_warningMessage;
    //�擾���Ă���A�C�e���̐�
    private int m_getItemCount;




    //Update�֐�
    private void Update() {
        if (m_vig.VigilanceFlag) {
            AddVigilance(m_vig.AddTimeVigilance * Time.deltaTime);
            CheckMouseMove();
        }
    }


    /// <summary>�}�E�X�����������ǂ����𒲂ׂ܂�</summary>
    /// <returns>�Q�[���I�[�o�[�̏ꍇ�ɂ�false��Ԃ��܂�</returns>
    private void CheckMouseMove() {
        Vector2 pos = Input.mousePosition;
        //�}�E�X����������Ă���ꍇ
        if (m_vig.MouseVec != pos) {
            //�ő�x���x�𒴂��Ă���ꍇ�̏���
            if (m_vig.IsOver) {
                m_vig.ReprieveGameOver += Time.deltaTime;
                if (m_vig.ReprieveGameOver > m_vig.GraceTime) OverVigilance();
            }
            else {
                m_vig.MouseVec = pos;
                AddVigilance(m_vig.AddMouseVigilance * Time.deltaTime);
            }
        }
    }

    /// <summary>�x���x��ǉ����܂�</summary>
    /// <returns>�x���x���ő�l�ȏ�̏ꍇ��true��Ԃ��܂�</returns>
    private bool AddVigilance(float addValue) {
        m_vig.CurrentVigilance += m_vig.IsOver ? 0 : addValue;
        m_InvGauge.SetRate(m_vig.Rate);
        m_warningMessage.SetActive(m_vig.IsOver);
        return m_vig.IsOver;
    }

    /// <summary>�x���x��ݒ肵�܂�</summary>
    /// <returns>�x���x���ő�l�ȏ�̏ꍇ��true��Ԃ��܂�</returns>
    private bool SetVigilance(float value) {
        m_vig.CurrentVigilance = value;
        m_InvGauge.SetRate(m_vig.Rate);
        m_warningMessage.SetActive(m_vig.IsOver);
        return m_vig.IsOver;
    }

    /// <summary>�x���x���}�b�N�X�̍ۂɃ}�E�X�𓮂����Ă��܂����ꍇ�̏���</summary>
    private void OverVigilance() {
        if (m_vig.VigilanceFlag) {
            m_warningMessage.SetActive(false);
            m_InvGauge.StopWave();
            SceneManager.Instance.SceneChange(SCENENAME.GameOverScene, () =>
            {
                UIManager.Instance.CloseUI(UIType.Timer);
                m_vig.invManager.SetMouseIcon(null);
            });
        }
    }

    //CSV�̃C���f�b�N�X���Ǘ����܂�
    private enum InvDataCSVIndex {
        MaxVigilance = 0,
        InitialiseVigilance = 1,
        AddTimeValue = 2,
        AddMoseValue = 3,
        GraceTime = 4,
        ItemStart = 5,
    }

}
