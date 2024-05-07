using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// CSV��ǂݍ��ރN���X�ł��B<br />
/// CSV�t�@�C����Resources�t�H���_�Ɋi�[���Ďg�p���Ă�������<br />
/// �܂�CSV�t�@�C����UTF-8�ō쐬���Ă�������
/// </summary>
public class CSVSetting {

    /// <summary>�R���X�g���N�^����CSV�t�@�C����ǂݍ��݂܂�</summary>
    /// <param name="fileName">CSV�t�@�C����(�g���q�s�v)</param>
    public CSVSetting(string fileName) {
        string path = UsefulSystem.FindFilePath(fileName+".csv");      //�S�̃p�X�̎擾
        int index = path.IndexOf("Resources/") + 10;                   //���΃p�X�̎n�߂̈ʒu���擾
        m_csvFile = Resources.Load(path.Substring(index, path.Length - (index + 4))) as TextAsset; // CSV�t�@�C���̃t�@�C�����������擾����Resources�ɂ���CSV�t�@�C�����i�[    
        StringReader reader = new StringReader(m_csvFile.text);           // TextAsset���̕������StringReader�ɕϊ�
        m_csvData = new List<string[]>();                                 //�������m��                 
        while (reader.Peek() != -1) {
            string line = reader.ReadLine();// 1�s���ǂݍ���
            Debug.Log(line);
            m_csvData.Add(line.Split(',')); // csvData���X�g�ɒǉ�����
        }
        reader.Close();
    }


    /// <summary>CSV�t�@�C���̃f�[�^(������)���擾���܂�</summary>
    /// <param name="x">�擾�����</param>
    /// <param name="y">�擾����s��</param>
    /// <param name="data">�擾�f�[�^</param>
    /// <returns>�w�肳�ꂽ�f�[�^���󔒂������ꍇ��false��Ԃ��܂�</returns>
    public bool GetData(int x, int y, out string data) { data = m_csvData[y][x];  return data != ""; }
    /// <summary>CSV�t�@�C���̃f�[�^(����)���擾���܂�</summary>
    /// <param name="x">�擾�����</param>
    /// <param name="y">�擾����s��</param>
    /// <param name="data">�擾�f�[�^</param>
    /// <returns>�w�肳�ꂽ�f�[�^�ɐ�����������Ă��Ȃ��ꍇ��false��Ԃ��܂�</returns>
    public bool GetData(int x, int y, out int data) { return int.TryParse(m_csvData[y][x], out data); }

    /// <summary>�w�肳�ꂽ�ӏ��ɋL�q�����邩�ǂ����m�F���܂�</summary>
    /// <param name="x">�擾�����</param>
    /// <param name="y">�擾����s��</param>
    /// <returns>�L�q���������ꍇ��true��Ԃ��܂�</returns>
    public bool CheckData(int x, int y) { return m_csvData[y][x] != ""; }

    /// <summary>���s�����擾���܂�</summary>
    public int TotalLine { get { return m_csvData.Count; } }

    /// <summary>�w�肳�ꂽ�s���̗�(�v�f��)���擾���܂�</summary>
    /// <param name="y">�w��s</param>
    /// <returns>�v�f����Ԃ��܂�</returns>
    public int GetLength(int y) { return m_csvData[y].Length; }

    /// <summary>�w�肳�ꂽ������̗�C���f�b�N�X���w�肳�ꂽ�s�����璲�ׂ܂�</summary>
    /// <param name="index_y">�w�肷��s�C���f�b�N�X</param>
    /// <param name="name">�������镶����</param>
    /// <param name="index_x">�C���f�b�N�X��Ԃ��܂��B������Ȃ������ꍇ��-1</param>
    /// <returns>�����������ǂ�����Ԃ��܂�</returns>
    public bool GetColumnIndex(int index_y,string name,out int index_x) {
        for (int i = 0; i < m_csvData[0].Length; i++){
            if (m_csvData[index_y][i] == name) {
                index_x = i;
                return true;
            }
        }
        LogWarning("�w�肳�ꂽ���ڂ�������܂���ł����@ ���ږ� : " + name);
        index_x = -1;
        return false;
    }

    /// <summary>�w�肳�ꂽ������̍s�C���f�b�N�X���w�肳�ꂽ�񐔂��璲�ׂ܂�</summary>
    /// <param name="index_x">�s�C���f�b�N�X���w�肵�܂�</param>
    /// <param name="name">�������镶����</param>
    /// <param name="index_y">�C���f�b�N�X��Ԃ��܂��B������Ȃ������ꍇ��-1</param>
    /// <returns>�����������ǂ�����Ԃ��܂�</returns>
    public bool GetLineIndex(int index_x, string name,out int index_y) {
        for (int i = 0; i < m_csvData.Count; i++) {
            if (m_csvData[i][index_x] == name) {
                index_y = i;
                return true;
            }
        }
        LogWarning( "�w�肳�ꂽ���ڂ�������܂���ł����@ ���ږ� : " + name);
        index_y = -1;
        return false;
    }

    /// <summary>CSV�̒��g�����O�ɕ\�����܂�(�f�o�b�O���̂݌Ăяo��)</summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void LogCSV() {
        Debug.Log(m_csvFile);
    }


    //private 
    private CSVSetting() { }            // new�}��
    private List<string[]> m_csvData;   // ���ۂ̃e�L�X�g�f�[�^
    private TextAsset m_csvFile;        // CSV�t�@�C����ǂݍ��ރe�L�X�g�A�Z�b�g

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void LogWarning(object obj) { Debug.LogWarning(obj); }
}