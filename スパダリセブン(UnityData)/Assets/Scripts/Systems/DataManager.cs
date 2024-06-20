using UnityEngine;
using System.IO;

public partial class DataManager : DebugSetting // MasterData��json�`���ɕς��ĕۑ��E�ǂݍ��݂���X�N���v�g
{
    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

    public void Save(MasterData data) // json�Ƃ��ăf�[�^��ۑ�����֐�
    {
        string json = JsonUtility.ToJson(data); // json�Ƃ��ĕϊ�
        StreamWriter writer = new StreamWriter(filepath, false); // �t�@�C���������ݎw��
        writer.WriteLine(json); // json�ϊ�����������������
        writer.Close(); // �t�@�C�������
        Debug.Log("�Z�[�u���Ă��܂�" + json);
    }

    public void ResetMasterData() // �f�[�^������������֐�
    {
        Debug.Log("�}�X�^�[�f�[�^�̏��������s���܂�");
        data = new MasterData(); // data��MasterData�^����
        Save(data); // �Z�[�u����
    }

    public void DeletaSave()
    {
        File.Delete(filepath);
    }

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///

    protected override void Awake()
    {
        base.Awake(); // �f�o�b�O���O��\�����邩�ۂ��X�N���v�^�u���I�u�W�F�N�g��DebugSettings���Q��
        CheckSaveData(); // �J�n���Ƀt�@�C���`�F�b�N�A�ǂݍ���
    }

    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///

    private void CheckSaveData() // �J�n���Ƀt�@�C���`�F�b�N�A�ǂݍ��݂���֐�
    {
        Debug.Log("�N�����[�h�J�n");
        data = new MasterData(); // data��MasterData�^����
        filepath = Application.dataPath + "/Resources/Json/" + fileName; // �p�X���擾
        if (!File.Exists(filepath)) // �t�@�C�����Ȃ��Ƃ�
        {
            Debug.Log("save�f�[�^����낤�Ƃ��Ă��܂�");
            Save(data); // �t�@�C���쐬
        }
        data = Load(filepath); // �t�@�C����ǂݍ����data�Ɋi�[
    }

    private MasterData Load(string path) // json�f�[�^��ǂݍ��ފ֐�
    {
        if (File.Exists(path)) // json�f�[�^�������
        {
            StreamReader reader = new StreamReader(path); // �t�@�C���ǂݍ��ݎw��
            string json = reader.ReadToEnd(); // �t�@�C�����e�S�ēǂݍ���
            reader.Close(); // �t�@�C�������
            Debug.Log("���[�h���Ă��܂�" + json);
            return JsonUtility.FromJson<MasterData>(json); // json�t�@�C�����^�ɖ߂��ĕԂ�
        }
        else
        {
            Debug.LogError("�t�@�C����������܂���" + path);
            return null; // null��Ԃ�
        }
    }

    //private void OnDestroy() // �Q�[���I�����ɕۑ�
    //{
    //    Save(data);
    //}

    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///
}
public partial class DataManager
{
    /// --------�ϐ��ꗗ-------- ///

    #region public�ϐ�
    /// -------public�ϐ�------- ///

    [SerializeField] public MasterData data; // json�ϊ�����f�[�^�̃N���X 

    /// -------public�ϐ�------- ///
    #endregion

    #region protected�ϐ�
    /// -----protected�ϐ�------ ///



    /// -----protected�ϐ�------ ///
    #endregion

    #region private�ϐ�
    /// ------private�ϐ�------- ///

    private string filepath; // json�t�@�C���̃p�X
    private string fileName = "MasterData.json"; // json�t�@�C����

    /// ------private�ϐ�------- ///
    #endregion

    #region �v���p�e�B
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    #endregion

    /// --------�ϐ��ꗗ-------- ///
}
