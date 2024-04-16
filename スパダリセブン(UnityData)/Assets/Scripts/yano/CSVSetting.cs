using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// CSV��ǂݍ��ރN���X�ł��B
/// CSV�t�@�C����Resources�t�H���_�Ɋi�[���Ďg�p���Ă�������
/// �܂���s��(Count = 0)�Ƀf�[�^�̍��ڈꗗ���L�ڂ���Ă���`��z�肵�Ă��܂��B
/// </summary>
public class CSVSetting {

    //�R���X�g���N�^
    public CSVSetting(string fileName) {
        string path = UsefulSystem.FindFilePath(fileName+".csv");      //�S�̃p�X�̎擾
        int index = path.IndexOf("Resources/") + 10;                   //���΃p�X�̎n�߂̈ʒu���擾
        m_csvFile = Resources.Load(path.Substring(index, path.Length - (index + 4))) as TextAsset; // CSV�t�@�C���̃t�@�C�����������擾����Resources�ɂ���CSV�t�@�C�����i�[
        StringReader reader = new StringReader(m_csvFile.text);           // TextAsset��StringReader�ɕϊ�
        m_csvData = new List<string[]>();                                 //�������m��
       // ConvertEncoding(m_csvFile, Encoding.UTF8);                        //���������h�~�̂���UTF-8�ɕϊ�
        while (reader.Peek() != -1) {
            string line = reader.ReadLine();// 1�s���ǂݍ���
            m_csvData.Add(line.Split(',')); // csvData���X�g�ɒǉ�����
        }
        reader.Close();
    }

    //�֐�

    /// <summary>���ڗ����܂߂��s�����擾���܂��B</summary>
    public int Row { get { return m_csvData.Count; } }
    /// <summary>���ڐ����擾���܂�</summary>
    public int Column { get { return m_csvData[0].Length; } }

    /// <summary>���ڔԍ��ƍs������w�肳�ꂽ�����f�[�^���擾���܂�</summary>
    ///<param name="row">���ڂ̔ԍ�</param>
    ///<param name="column">�s��</param>
    public string GetData(int row,int column) { return m_csvData[column][row]; }

    /// <summary>���ږ��ƍs������w�肳�ꂽ�����f�[�^���擾���܂�</summary>
    /// <param name="name">���ږ�</param>
    /// <param name="column">�s��</param>
    /// <returns></returns>
    public string GetData(string name,int column) {
       int row =   GetRowIndex(name);
        return m_csvData[column][row];
    }

    /// <summary>�w�肳�ꂽ���ږ�������ڂ��𒲂ׂ܂�</summary>
    /// <param name="name">���ږ�</param>
    /// <returns>�񐔂�Ԃ��܂��B������Ȃ������ꍇ��-1��Ԃ��܂�</returns>
    public int GetRowIndex(string name) {
        for (int i = 0; i < m_csvData[0].Length; i++){
            if (m_csvData[0][i] == name) {
                return i;
            }
        }
        Debug.Assert(false,"�w�肳�ꂽ���ڂ�������܂���ł���");
        return -1;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Log() {
        Debug.Log(m_csvFile);
    }


    //private 
    private CSVSetting() { }
    private TextAsset m_csvFile; // CSV�t�@�C��
    private List<string[]> m_csvData; // CSV�t�@�C���̒��g�����郊�X�g
}