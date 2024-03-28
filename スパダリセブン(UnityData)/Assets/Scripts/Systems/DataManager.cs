using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : DebugSetting // MasterData��json�`���ɕς��ĕۑ��E�ǂݍ��݂���X�N���v�g
{
    [SerializeField] public MasterData data; // json�ϊ�����f�[�^�̃N���X 
    string filepath; // json�t�@�C���̃p�X
    string fileName = "MasterData.json"; // json�t�@�C����

    protected  override void Awake()
    {
        base.Awake(); // �f�o�b�O���O��\�����邩�ۂ��X�N���v�^�u���I�u�W�F�N�g��DebugSettings���Q��
        CheckSaveData(); // �J�n���Ƀt�@�C���`�F�b�N�A�ǂݍ���
    }
    private void CheckSaveData() // �J�n���Ƀt�@�C���`�F�b�N�A�ǂݍ��݂���֐�
    {
        UnityEngine.Debug.Log("�N�����[�h�J�n");
        data = new MasterData(); // data��MasterData�^����
        filepath = Application.dataPath + "/Resources/Json/" + fileName; // �p�X���擾
        if (!File.Exists(filepath)) // �t�@�C�����Ȃ��Ƃ�
        {
            UnityEngine.Debug.Log("save�f�[�^����낤�Ƃ��Ă��܂�");
            Save(data); // �t�@�C���쐬
        }
        data = Load(filepath); // �t�@�C����ǂݍ����data�Ɋi�[
    }
    public void Save(MasterData data) // json�Ƃ��ăf�[�^��ۑ�����֐�
    {
        string json = JsonUtility.ToJson(data); // json�Ƃ��ĕϊ�
        StreamWriter writer = new StreamWriter(filepath, false); // �t�@�C���������ݎw��
        writer.WriteLine(json); // json�ϊ�����������������
        writer.Close(); // �t�@�C�������
        UnityEngine.Debug.Log("�Z�[�u���Ă��܂�" + json);
    }
    MasterData Load(string path) // json�f�[�^��ǂݍ��ފ֐�
    {
        if (File.Exists(path)) // json�f�[�^�������
        {
            StreamReader reader = new StreamReader(path); // �t�@�C���ǂݍ��ݎw��
            string json = reader.ReadToEnd(); // �t�@�C�����e�S�ēǂݍ���
            reader.Close(); // �t�@�C�������
            UnityEngine.Debug.Log("���[�h���Ă��܂�" + json);
            return JsonUtility.FromJson<MasterData>(json); // json�t�@�C�����^�ɖ߂��ĕԂ�
        }
        else
        {
            UnityEngine.Debug.LogError("�t�@�C����������܂���" + path);
            return null; // null��Ԃ�
        }
    }
    public void ResetMasterData() // �f�[�^������������֐�
    {
        UnityEngine.Debug.Log("�}�X�^�[�f�[�^�̏��������s���܂�");
        data = new MasterData(); // data��MasterData�^����
        Save(data); // �Z�[�u����
    }
    //void OnDestroy() // �Q�[���I�����ɕۑ�
    //{
    //    Save(data);
    //}
    public void DeletaSave()
    {
        File.Delete(filepath);
    }
}

