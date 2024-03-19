using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Json�t�@�C����ǂݏ�������ׂ̃N���X�ł�
/// </summary>
/// <typeparam name="T">[System.Serializable]������t����Json�t�@�C���Ɠ����`���̃N���X</typeparam>
public class JsonSettings<T> where T : class {

    // �ϐ� //
    /// <summary>T�^�̃N���X�̃C���X�^���X���擾���܂��B</summary>
    public T Instance{ get { return m_instance; } }

    //�R���X�g���N�^
    /// <summary>Json���Ǘ�����N���X���쐬���܂�</summary>
    /// <param name="jsonFileName">Json�t�@�C����(�g���q����)</param>
    public JsonSettings(string jsonFileName) {
        m_jsonFileName = jsonFileName + ".json";
        m_jsonPath = UsefulSystem.FindFilePath(m_jsonFileName);
        Load();
    }
   
    
    //  �֐�  //
    /// <summary> Json�f�[�^�ɃC���X�^���X�̏����������݂܂��B</summary>
    public void Save() {
        //string�ɕϊ�����
        string jsonstr = JsonUtility.ToJson(Instance);
        //�t�@�C���������ݗp�̃��C�^�[���J��
        StreamWriter writer = new StreamWriter(m_jsonPath,false);
        //��������
        writer.Write(jsonstr);
        //���C�^�[����鏈��
        writer.Flush();
        writer.Close();
    }

    /// <summary>Json�f�[�^���C���X�^���X�ɓǂݍ��݂܂�</summary>
    public void Load() {
        //JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonPath);
        //�I�u�W�F�N�g������
        m_instance = JsonUtility.FromJson<T>(json);
    }


    public void Reset() {

    }

    /// <summary>Json�t�@�C�����̏��𕶎���ŕԂ��܂��B</summary>
    public string JsonToString() { return JsonUtility.ToJson(m_instance, true);}




    //�@�v���C�x�[�g�ϐ��E�֐��@// 
    private string m_jsonFileName;
    private string m_jsonPath;
    private T m_instance;

    private JsonSettings() { }
}