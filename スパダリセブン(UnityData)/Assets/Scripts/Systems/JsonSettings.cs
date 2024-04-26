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

    //�@�R���X�g���N�^�@//
    /// <summary>Json���Ǘ�����N���X���쐬���܂�</summary>
    /// <param name="createFileName">���̖��O�Ńt�@�C���𐶐����f�[�^��ۑ����܂�</param>
    /// <param name="saveFolderPath">�f�[�^��ۑ�����t�H���_�̃p�X(Asset/�ȉ�)</param>
    /// <param name="dataFileName">Json�t�@�C����(�g���q����)</param>
    public JsonSettings(in string createFileName, in string saveFolderPath, in string dataFileName) {
        JsonSetUp(createFileName, saveFolderPath, dataFileName);
    }

    /// <summary>�}�X�^�[�f�[�^�������ō쐬���܂��B�}�X�^�[�f�[�^��"Origin_createFileName.json"�Ƃ��č쐬����܂�</summary>
    /// <param name="createFileName">���̖��O�Ńt�@�C���𐶐����f�[�^��ۑ����܂�</param>
    /// <param name="saveFolderPath">�f�[�^��ۑ�����t�H���_�̃p�X(Asset/�ȉ�)</param>
    public JsonSettings(in string createFileName, in string saveFolderPath) {
        var path = Application.dataPath + "\\" + saveFolderPath + "\\" + createFileName + ".json";
        m_jsonDefaultPath = Application.dataPath + "\\" + saveFolderPath + "\\" + "Origin_" + createFileName + ".json";
        if (!File.Exists(path)) {
            //�}�X�^�[�f�[�^�p�̃p�X�Ń}�X�^�[�f�[�^���쐬����
            m_jsonPath = m_jsonDefaultPath;
            m_tInstance = new T();
            _Save(true);
            //�������p�X��Json�p�X�Ɋi�[���A���߂ăf�[�^���쐬����
            m_jsonPath = path;
            SettingData();
        }
        else { m_jsonPath = path; }
        Load();
    }


    //  �֐�  //
    /// <summary> Json�f�[�^�ɃC���X�^���X�̏����������݂܂��B</summary>
    public void Save() { _Save(false); }

    /// <summary>Json�f�[�^���C���X�^���X�ɓǂݍ��݂܂�</summary>
    public void Load() {
        //JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonPath);
        //�I�u�W�F�N�g������
        m_tInstance = JsonUtility.FromJson<T>(json);
    }

    /// <summary>�f�[�^�������l�ɖ߂��܂��B</summary>
    public void Reset() {
        var json = File.ReadAllText(m_jsonDefaultPath);
        m_tInstance = JsonUtility.FromJson<T>(json);
        _Save(false);
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

    /// <summary>Json�t�@�C�����̃f�[�^�����O�ɕ\�����܂� </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() { Debug.Log(JsonToString()); }




    //�@�v���C�x�[�g�ϐ��E�֐��@// 
    private string m_jsonPath;
    private string m_jsonDefaultPath = null;
    private T m_tInstance;
    private JsonSettings() { }

    //Json�t�@�C���̍쐬�A�ǂݍ��݂��s���܂�
    private void JsonSetUp(in string createFileName, in string saveFolderPath, in string dataFileName) {

        m_jsonDefaultPath = UsefulSystem.FindFilePath(dataFileName + ".json");
        m_jsonPath = Application.dataPath + "\\" + saveFolderPath + "\\" + createFileName + ".json";
        if (!File.Exists(m_jsonPath)) {
            SettingData();
        }
        Load();
    }

    //�f�[�^��ݒ肵�܂�
    private void SettingData() {
        //�f�t�H���g��JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonDefaultPath);
        //�I�u�W�F�N�g������
        m_tInstance = JsonUtility.FromJson<T>(json);
        //�����l���Z�[�u����
        _Save(true);
    }

    //�Z�[�u�̎��ۂ̏���
    private void _Save(bool initialization) {
        //string�ɕϊ�����
        string jsonStr = JsonUtility.ToJson(TInstance);
        //�t�@�C���������ݗp�̃��C�^�[���J��
        StreamWriter writer = new StreamWriter(m_jsonPath, initialization);
        //��������
        writer.Write(jsonStr);
        //���C�^�[����鏈��
        writer.Flush();
        writer.Close();
    }

    //MonoBehaviour���p������Ă��邩���ׂ܂�
    private bool CheckMono() { return typeof(T).GetType().IsSubclassOf(typeof(MonoBehaviour)); }
}