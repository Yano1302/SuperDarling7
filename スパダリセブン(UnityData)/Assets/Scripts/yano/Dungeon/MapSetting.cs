using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapType {
    player = 0,
    road   = 1,
    wall   = 2,
    catchtrap = 3,
    pitfall = 4,
    Goal = 5,
}

public class MapSetting : SingletonMonoBehaviour<MapSetting>
{
    // Config�ϐ�  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField, Header("���}�b�v��")]
    private int AllMapNum = 1;
    [SerializeField, Header("��}�X�̏c�̃T�C�Y")]
    private int Height = 1;
    [SerializeField,Header("��}�X�̉��̃T�C�Y")]
    private int Width = 1;  
    [SerializeField, Header("�}�b�v�I�u�W�F�N�g"), EnumIndex(typeof(MapType))]
    private GameObject[] MapObject;

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void CreateMap(int mapNumber) {
        mapNumber -= 1;
        _Create(mapNumber);
    }

   
    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------
    private List<string>[] m_mapData = null;
    //  �v���C�x�[�g�֐�  //------------------------------------------------------------------------------------------------------------------------------
    
    //������
    protected override void Awake() {
        base.Awake();
        //�}�b�v��񂾂���ɓǂݍ���ł���
        if(m_mapData == null) {
            m_mapData = new List<string>[AllMapNum];
            for (int i = 0; i < AllMapNum; i++) {
                string path = UsefulSystem.FindFilePath("�X�e�[�W" + (i + 1)+".txt");
                m_mapData[i] =  UsefulSystem.Reader_TextFile(path);
            }
        }
    }

    //TODO�@�ǂ������炱�̊֐����Ăяo��
    private void _Create(int mapNumber) {
        //�E�ォ��ǂݍ���
        int heightCount = m_mapData[mapNumber].Count - 1;
        for (int i = heightCount; i >= 0; i--){
            //����񕪓ǂݍ���
            string line = m_mapData[mapNumber][i];
            for (int j = 0; j < line.Length; j++) {
                //�ǂݍ���ID(Char�^)��int�^�ɕϊ�
                int typeNum = line[j] - '0';
                if(typeNum >= 0) {
                    Vector2 vec = new Vector2(Width * j, Height * (heightCount - i));
                    Instantiate(MapObject[typeNum], vec, Quaternion.identity);
                    if (typeNum != 1 && typeNum != 2) {
                        Instantiate(MapObject[1], vec, Quaternion.identity);
                    }
                       
                } 
            }
        }
        UIManager.Instance.OpenUI(UIType.Timer);
    }
}
