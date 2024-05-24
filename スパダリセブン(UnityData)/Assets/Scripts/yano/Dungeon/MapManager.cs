using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

//TODO JSON�ɑΉ�������
public enum MapType {
    Dummy = 0,
    Player = 1,
    road   = 2,
    wall   = 3,
    catchtrap = 4,
    pitfall = 5,
    Goal1 = 6,
    
}

public class MapManager : SingletonMonoBehaviour<MapManager> {
    
    /// <summary>�}�b�v�𐶐����܂�</summary>
    /// <param name="stageNumber">�}�b�v�ԍ�</param>
    public void CreateMap(int stageNumber) {
        _Create(stageNumber);
    }

    /// <summary>���݂̃X�e�[�W�ԍ����擾���܂�</summary>
    public int StageNumber { get { Debug.Assert(m_stageData.number > -1,"�X�e�[�W����������Ă��܂���B");  return m_stageData.number; } }

    /// <summary>���݂̃X�e�[�W�̑S�A�C�e�������擾���܂�</summary>
    public int TotalItem { get { Debug.Assert(m_stageData.number > -1, "�X�e�[�W����������Ă��܂���B"); return m_stageData.totalitem; } }

    /// <summary>�X�e�[�W�̐������Ԃ��擾���܂�</summary>
    public float Time { get { return m_stageData.time; } }
    /// <summary>�}�b�v�̑������擾���܂�</summary>
    public int TotalMapNumber { get { return StageData.Data.TotalLine - 1; } }

    // �A�^�b�`�ϐ�  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("�}�b�v�I�u�W�F�N�g"), EnumIndex(typeof(MapType))]
    private GameObject[] MapObject;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
   
    /// <summary>�X�e�[�W��CSV��ǂݍ��ނ��߂̃C���f�b�N�X���Ǘ����܂�</summary>
    public enum StageCsvIndex {
        number = 0,
        name = 1,
        time = 2,
        size = 3,
        itemStart = 4,
        otherItem = 10,
    }

    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------

    //�}�b�v�f�[�^
    StageData m_stageData;

    //�X�e�[�W�̃f�[�^���Ǘ�����\����
    private struct StageData {
        public static CSVSetting Data;     //�}�b�v�f�[�^��S�Ċi�[�����f�[�^CSV
        public int number;                 //�}�b�v�ԍ�(��������Ƀf�[�^��Ԃ��܂�)
        public string name { get { Data.GetData((int)StageCsvIndex.name,number, out string data); return data; } } //�X�e�[�W��
        public float time  { get { Data.GetData((int)StageCsvIndex.time, number, out int data); return data; } }    //��������
        public float size  { get { Data.GetData((int)StageCsvIndex.size,number, out int data); return data; } }    //��}�X�̃T�C�Y
        public int totalitem { get; set; }
    }

    //  �v���C�x�[�g�֐�  //------------------------------------------------------------------------------------------------------------------------------

    //������
    protected override void Awake() {
        base.Awake();
        //�}�b�v��񂾂���ɓǂݍ���ł���
        if (StageData.Data == null) {
            //�X�e�[�W�쐬�ɕK�v�ȃf�[�^��������CSV��ǂݍ���
            StageData.Data = new CSVSetting("�X�e�[�W���");
            m_stageData.number = -1;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }
    }
    private void Start() {
        //�f�o�b�O�p
        KeyDebug.AddKeyAction("�}�b�v�̍쐬", () => { _Create(1); });
        KeyDebug.AddKeyAction("�}�b�v�������O�ɕ\������", () => { Debug.Log(m_stageData.name); });
        KeyDebug.AddKeyAction("�������Ԃ����O�ɕ\������", () => { Debug.Log(m_stageData.time); });
        KeyDebug.AddKeyAction("�}�b�v�̃T�C�Y�����O�ɕ\������", () => { Debug.Log(m_stageData.size); });
    }

    /// <summary>�}�b�v���쐬���܂� Note:������̃V�[���}�l�[�W���[����Ăяo���܂�</summary>
    /// <param name="mapNumber">�쐬����X�e�[�W�ԍ� Note:�X�e�[�W�ԍ��̓X�e�[�W���ꗗ.csv�ɋL��</param>
    private void _Create(int mapNumber) {
        //�X�e�[�W�����i�[���Ă���C���f�b�N�X���m�ۂ���
        StageData.Data.GetColumnIndex(0, "�X�e�[�W��", out int nameIndex);
        //�X�e�[�W�����擾
        StageData.Data.GetData(nameIndex, mapNumber, out string data);
        //�X�e�[�W���ƈ�v����CSV�t�@�C��������͂��Ȃ̂œǂݍ���
        var mapData = new CSVSetting(data);
        //���݂̃X�e�[�W�ԍ����i�[����
        m_stageData.number = mapNumber;
        //���݂̃X�e�[�W�̃A�C�e�������擾����
        var im = ItemManager.Instance;
        m_stageData.totalitem = im.GetTotalItemNum(mapNumber);
        //�}�b�v�̍����ƂP�}�X�̃T�C�Y���擾
        int maxY = mapData.TotalLine;//����
         Vector2 scale = new Vector2(m_stageData.size, m_stageData.size); //�T�C�Y
        //�c�̃��[�v
        for (int y = 0; y < maxY; y++) {
            //�}�b�v�̉������擾
            int MaxX = mapData.GetLength(y);
            //���̃��[�v
            for (int x = 0; x < MaxX; x++) {
                //�ǂݍ��񂾂��̂𐔎��ɕϊ�����B�ϊ��ł����ꍇ�Ƀ}�X�̍쐬���s��
                if(mapData.GetData(x, y, out int typeNum)) {
                    //��������z�u����}�X�����肵�z�u����
                    if (typeNum == 0) {  continue;  }//�O�̏ꍇ�͔z�u���Ȃ�
                    Vector2 vec = new Vector2(m_stageData.size * x, m_stageData.size - (m_stageData.size * y));//�}�X�̔z�u�ꏊ���v�Z����
                    //�}�X���쐬
                    var obj = Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                    obj.transform.localScale = scale;
                    if (typeNum != (int)MapType.road && typeNum != (int)MapType.wall) {
                        //���̔w�i�p�ɓ��I�u�W�F�N�g��z�u����@TODO:���u��
                        Instantiate(MapObject[(int)MapType.road], vec, Quaternion.identity);
                    }
                }     
            }        
        }
    
        //�}�b�v������Ƀ^�C�}�[���J��
        TimerManager.Instance.SetTimer(m_stageData.time);
        // �A�C�e���E�B���h�E��\��
        ItemWindow.Instance.ActiveWindows();
    }

    private void SceneUnloaded(Scene thisScene) {
        m_stageData.number = -1;
    }
}
