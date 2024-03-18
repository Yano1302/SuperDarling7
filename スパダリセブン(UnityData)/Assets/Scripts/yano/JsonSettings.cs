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
    //�p�u���b�N�ϐ�//
    public T jsonData;

    //�R���X�g���N�^
    /// <summary>Json���Ǘ�����N���X���쐬���܂�</summary>
    /// <param name="jsonFileName">Json�t�@�C����(�g���q����)</param>
    public JsonSettings(string jsonFileName) {
        m_jsonFileName = jsonFileName+".json";
        m_jsonPath = UsefulSystem.FindFilePath(m_jsonFileName);
        Load();
    }
    // �p�u���b�N�ϐ��E�֐�   //
    public void Save() {
        //�������݌��f�[�^���擾����B�����ł�settings.json�Ƃ���
        string jsonData = Resources.Load<TextAsset>(m_jsonFileName).ToString();
        //string�ɕϊ�����
        string jsonstr = JsonUtility.ToJson(this.jsonData);
        //�t�@�C���������ݗp�̃��C�^�[���J��
        StreamWriter writer = new StreamWriter(m_jsonPath,false);
        //��������
        writer.Write(jsonstr);
        //���C�^�[����鏈��
        writer.Flush();
        writer.Close();
    }

    public void Load() {
        //JSON�t�@�C����ǂݍ���
        var json = File.ReadAllText(m_jsonPath);
        //�I�u�W�F�N�g������
        jsonData = JsonUtility.FromJson<T>(json);
    }


    //�@�v���C�x�[�g�ϐ��E�֐��@// 
    private string m_jsonFileName;
    private string m_jsonPath;

    private JsonSettings() { }
}