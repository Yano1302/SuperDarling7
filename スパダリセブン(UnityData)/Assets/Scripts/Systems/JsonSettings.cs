using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Json�t�@�C����ǂݏ�������ׂ̃N���X�ł�
/// </summary>
/// <typeparam name="T">Json�t�@�C���Ɠ����`���̃N���X</typeparam>
public class JsonSettings<T> where T : class, new() {

    // �ϐ� //
    /// <summary>T�^�̃N���X�̃C���X�^���X���擾���܂��B</summary>
    public T TInstance { get { return m_tInstance; } }

    //�R���X�g���N�^
    /// <summary>Json���Ǘ�����N���X���쐬���܂�</summary>
    /// <param name="dataFileName">���̖��O�Ńt�@�C���𐶐����f�[�^��ۑ����܂�</param>
    /// <param name="saveFolderPath">�f�[�^��ۑ�����t�H���_�̃p�X(Asset/�ȉ�)</param>
    /// <param name="defaultJsonFileName">Json�t�@�C����(�g���q����)</param>
    public JsonSettings(string dataFileName,string saveFolderPath, string defaultJsonFileName) {
        
        m_jsonFileName = dataFileName + ".json";
        m_jsonDefaultPath = UsefulSystem.FindFilePath(defaultJsonFileName + ".json");
        m_jsonPath = Application.dataPath + "\\" + saveFolderPath + "\\" + m_jsonFileName;
        if (!File.Exists(m_jsonPath)) {
            SettingData();
        }
        Load(); 
    }

    /// <summary>�}�X�^�[�f�[�^�������ō쐬���܂�</summary>
    /// <param name="dataFileName">���̖��O�Ńt�@�C���𐶐����f�[�^��ۑ����܂�</param>
    /// <param name="saveFolderPath">�f�[�^��ۑ�����t�H���_�̃p�X(Asset/�ȉ�)</param>
    public JsonSettings(string dataFileName,string saveFolderPath) {
        m_jsonFileName = dataFileName + ".json";
        m_jsonPath = Application.dataPath +"\\"+ saveFolderPath + "\\" + m_jsonFileName;
        m_tInstance = new T();
        m_create = true;
        Save();
    }

    public JsonSettings(string dataFileName, string saveFolderPath, string defaultJsonFileName,GameObject obj) {
    
    }

   //  �֐�  //
    /// <summary> Json�f�[�^�ɃC���X�^���X�̏����������݂܂��B</summary>
    public void Save() {
        //string�ɕϊ�����
        string jsonStr = JsonUtility.ToJson(TInstance);
        //�t�@�C���������ݗp�̃��C�^�[���J�� �㏑���ɂ��Ȃ���json�f�[�^�������̂�false�ɂ��Ă��܂�
        StreamWriter writer = new StreamWriter(m_jsonPath,m_create);
        //��������
        writer.Write(jsonStr);
        //���C�^�[����鏈��
        writer.Flush();
        writer.Close();
        m_create = false;
    }

    /// <summary>Json�f�[�^���C���X�^���X�ɓǂݍ��݂܂�</summary>
    public void Load() {
        //JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonPath);
        //�I�u�W�F�N�g������
        m_tInstance = JsonUtility.FromJson<T>(json);
    }

    /// <summary>�f�[�^�������l�ɖ߂��܂��B</summary>
    public void Reset() {
        SettingData();
    }

    /// <summary>�f�[�^���폜���܂��B�C���X�^���X���j������܂��B</summary>
    public void Delete(ref JsonSettings<T> data) {
        File.Delete(data.GetJsonPath());
        data = null;
    }
    /// <summary>�f�[�^��ۑ����Ă���Json�t�@�C���̃p�X���擾���܂��B</summary>
    public string GetJsonPath() { return m_jsonPath; }

    /// <summary>Json�t�@�C�����̏��𕶎���ŕԂ��܂��B</summary>
    public string JsonToString() {
        //JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonPath);
        //�I�u�W�F�N�g������
        var ins = JsonUtility.FromJson<T>(json);
        return JsonUtility.ToJson(ins, true);
    }




    //�@�v���C�x�[�g�ϐ��E�֐��@// 
    private string m_jsonFileName;
    private string m_jsonPath;
    private string m_jsonDefaultPath = null;
    private bool m_create = false;
    public T m_tInstance; // ���X�N���v�g����ύX���邽�߂�public�ɂ��Ă��܂�
    private JsonSettings() { }
    
    //�f�[�^��ݒ肵�܂�
    private void SettingData() {
        //�f�t�H���g��JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonDefaultPath);
        //�I�u�W�F�N�g������
        m_tInstance = JsonUtility.FromJson<T>(json);
        //�����l���Z�[�u����
        m_create = true;
        Save();
    }

    //MonoBehaviour���p������Ă��邩���ׂ܂�
    private bool CheckMono() {return typeof(T).GetType().IsSubclassOf(typeof(MonoBehaviour)); }
}