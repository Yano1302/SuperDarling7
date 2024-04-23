using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//TODO JSON�ɑΉ�������
public enum MapType {
    player = 0,
    road   = 1,
    wall   = 2,
    catchtrap = 3,
    pitfall = 4,
    Goal1 = 5,
    Goal2 = 6,
    Goal3 = 7,
    Goal4 = 8,
}

public class MapSetting : SingletonMonoBehaviour<MapSetting> {
    
    /// <summary>�}�b�v�𐶐����܂�</summary>
    /// <param name="stageNumber">�}�b�v�ԍ�</param>
    public void CreateMap(int stageNumber) {
        stageNumber -= 1;
        _Create(stageNumber);
    }

    /// <summary>�X�e�[�W�̐������Ԃ��擾���܂�</summary>
    public float Time { get { return m_stageData.time; } }
    /// <summary>�}�b�v�̑������擾���܂�</summary>
    public int TotalMapNumber { get { return m_mapData.Length; } }

    /// <summary>
    /// </summary>

    // �A�^�b�`�ϐ�  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("�}�b�v�I�u�W�F�N�g"), EnumIndex(typeof(MapType))]
    private GameObject[] MapObject;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------

    public enum StageCsvIndex {
        number = 0,
        name   = 1,
        time   = 2,
        size   = 3,
        itemStart = 4,
    }

    //�}�b�v�f�[�^��ǂݍ��ރN���X�z��
    private static CSVSetting[] m_mapData = null;
    //�}�b�v�f�[�^
    StageData m_stageData;
    //UI�}�l�[�W���[�C���X�^���X
    private UIManager m_uiManager;
    //�A�C�e���}�l�[�W���[�C���X�^���X
    private ItemManager m_itemManager;

    //�X�e�[�W�̃f�[�^���Ǘ�����\����
    private struct StageData {
        public CSVSetting Data;     //�}�b�v�f�[�^��S�Ċi�[�����f�[�^CSV
        public int number;          //�}�b�v�ԍ�(��������Ƀf�[�^��Ԃ��܂�)
        public string name { get { Data.GetData((int)StageCsvIndex.name,number, out string data); return data; } } //�X�e�[�W��
        public float time  { get { Data.GetData((int)StageCsvIndex.time, number, out int data); return data; } }    //��������
        public float size  { get { Data.GetData((int)StageCsvIndex.size,number, out int data); return data; } }    //��}�X�̃T�C�Y
    }

    //  �v���C�x�[�g�֐�  //------------------------------------------------------------------------------------------------------------------------------

    //������
    protected override void Awake() {
        base.Awake();
        //�}�b�v��񂾂���ɓǂݍ���ł���
        if (m_mapData == null) {
            //�X�e�[�W�쐬�ɕK�v�ȃf�[�^��������CSV��ǂݍ���
            m_stageData.Data = new CSVSetting("�X�e�[�W���");
            //�ǂݍ��񂾃f�[�^����K�v�ȃ��������m��
            m_mapData = new CSVSetting[m_stageData.Data.TotalLine];
            //�X�e�[�W�����i�[���Ă���C���f�b�N�X���m�ۂ���
            m_stageData.Data.GetColumnIndex(0,"�X�e�[�W��",out int nameIndex);
            //�X�e�[�W������X�e�[�W�����i�[���Ă���CSV�S�ēǂݍ���ł���
            for (int i = 1; i < m_mapData.Length; i++) {
                m_stageData.Data.GetData(i, nameIndex, out string data);
                m_mapData[i] = new CSVSetting(data);
            }
            //�f�o�b�O�p
            KeyDebug.AddKeyAction("�}�b�v�̍쐬", () => {_Create(1); });
        }
    }

    /// <summary>�}�b�v���쐬���܂� Note:������̃V�[���}�l�[�W���[����Ăяo���܂�</summary>
    /// <param name="mapNumber">�쐬����X�e�[�W�ԍ� Note:�X�e�[�W�ԍ��̓X�e�[�W���ꗗ.csv�ɋL��</param>
    private void _Create(int mapNumber) {
        //�C���X�^���X���擾
        m_uiManager??= UIManager.Instance;
        //���݂̃X�e�[�W�ԍ����i�[����
        m_stageData.number = mapNumber;
        //�}�b�v�̍����ƂP�}�X�̃T�C�Y���擾
        int maxY = m_mapData[mapNumber].TotalLine;//����
         Vector2 scale = new Vector2(m_stageData.size, m_stageData.size); //�T�C�Y
        //�c�̃��[�v
        for (int y = 0; y < maxY; y++) {
            //�}�b�v�̉������擾
            int MaxX = m_mapData[mapNumber].GetLength(y);
            //���̃��[�v
            for (int x = 0; x < MaxX; x++) {
                //�ǂݍ��񂾂��̂𐔎��ɕϊ�����B�ϊ��ł����ꍇ�Ƀ}�X�̍쐬���s��
                if(m_mapData[mapNumber].GetData(x, y, out int typeNum)) {
                    //��������z�u����}�X�����肵�z�u����
                    if (typeNum >= 0) {
                        //�}�X�̔z�u�ꏊ���v�Z����
                        Vector2 vec = new Vector2(m_stageData.size * x, m_stageData.size - (m_stageData.size * y));
                        //�}�X���쐬
                        var obj = Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                        obj.transform.localScale = scale;
                        if (typeNum != 1 && typeNum != 2) {
                            //���̔w�i�p�ɓ��I�u�W�F�N�g��z�u����@TODO:���u��
                            Instantiate(MapObject[1], vec, Quaternion.identity);
                        }
                    }
                }     
            }        
        }
        //�}�b�v������Ƀ^�C�}�[���J��
        m_uiManager.OpenUI(UIType.Timer);
        // �A�C�e���E�B���h�E��\��
        m_uiManager.OpenUI(UIType.ItemWindow);
    }
}
