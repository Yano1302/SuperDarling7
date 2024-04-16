using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// CSV��ǂݍ��ރN���X�ł��B
/// CSV�t�@�C����Resources�t�H���_�Ɋi�[���Ďg�p���Ă�������
/// �܂���s�ڂɃf�[�^�̍��ڈꗗ���L�ڂ���Ă���`��z�肵�Ă��܂��B
/// </summary>
public class CSVSetting {

    //�R���X�g���N�^
    public CSVSetting(string fileName) {
        string path = UsefulSystem.FindFilePath(fileName+".csv");      //�S�̃p�X�̎擾
        int index = path.IndexOf("Resources/") + 10;                   //���΃p�X�̎n�߂̈ʒu���擾
        csvFile = Resources.Load(path.Substring(index, path.Length - (index + 4))) as TextAsset; // CSV�t�@�C���̃t�@�C�����������擾����Resources�ɂ���CSV�t�@�C�����i�[
        StringReader reader = new StringReader(csvFile.text);           // TextAsset��StringReader�ɕϊ�
        csvData = new List<string[]>();                                 //�������m��
        ConvertEncoding(csvFile, Encoding.UTF8);                        //���������h�~�̂���UTF-8�ɕϊ�
        while (reader.Peek() != -1) {
            string line = reader.ReadLine();// 1�s���ǂݍ���
            csvData.Add(line.Split(',')); // csvData���X�g�ɒǉ�����
        }
        reader.Close();
    }

    //�֐�

    /// <summary>���ڔԍ��ƍs������w�肳�ꂽ�����f�[�^���擾���܂�</summary>
    ///<param name="row">���ڂ̔ԍ�</param>
    ///<param name="column">�s��</param>
    public string GetData(int row,int column) { return csvData[column][row]; }

    /// <summary>���ږ��ƍs������w�肳�ꂽ�����f�[�^���擾���܂�</summary>
    /// <param name="name">���ږ�</param>
    /// <param name="column">�s��</param>
    /// <returns></returns>
    public string GetData(string name,int column) {
       int row =   GetRowIndex(name);
        return csvData[column][row];
    }

    /// <summary>�w�肳�ꂽ���ږ�������ڂ��𒲂ׂ܂�</summary>
    /// <param name="name">���ږ�</param>
    /// <returns>�񐔂�Ԃ��܂��B������Ȃ������ꍇ��-1��Ԃ��܂�</returns>
    public int GetRowIndex(string name) {
        for (int i = 0; i < csvData[0].Length; i++){
            if (csvData[0][i] == name) {
                return i;
            }
        }
        Debug.Assert(false,"�w�肳�ꂽ���ڂ�������܂���ł���");
        return -1;
    }




//private 
    private CSVSetting() { }
    private TextAsset csvFile; // CSV�t�@�C��
    private List<string[]> csvData; // CSV�t�@�C���̒��g�����郊�X�g


    /// <summary>TextAsset�̕����R�[�h��ϊ����܂�</summary>
    /// <param name="textAsset">�ϊ�����TextAsset</param>
    /// <param name="dstEncoding">�ԊҌ�̕����R�[�h</param>
    /// <returns></returns>
    private string ConvertEncoding(TextAsset textAsset,Encoding dstEncoding) {
        var srcEnc = DetectEncodingFromBOM(textAsset.bytes);
        byte[] srcbyte = srcEnc.GetBytes(textAsset.text);
        byte[] dstbyte = Encoding.Convert(srcEnc, dstEncoding, srcbyte);
        string ret = dstEncoding.GetString(dstbyte);
        return ret;
    }

    /// <summary>�����R�[�h�𒲂ׂ܂�</summary>
    /// <param name="bytes">�����R�[�h�𒲂ׂ�f�[�^�B</param>
    /// <returns>BOM�������������́A�Ή�����Encoding�I�u�W�F�N�g�B
    /// ������Ȃ��������́Anull�B</returns>
    private Encoding DetectEncodingFromBOM(byte[] bytes) {

        if (bytes.Length < 2) {
            return null;
        }
        if ((bytes[0] == 0xfe) && (bytes[1] == 0xff)) {
            //UTF-16 BE
            return new System.Text.UnicodeEncoding(true, true);
        }
        if ((bytes[0] == 0xff) && (bytes[1] == 0xfe)) {
            if ((4 <= bytes.Length) &&
                (bytes[2] == 0x00) && (bytes[3] == 0x00)) {
                //UTF-32 LE
                return new System.Text.UTF32Encoding(false, true);
            }
            //UTF-16 LE
            return new System.Text.UnicodeEncoding(false, true);
        }
        if (bytes.Length < 3) {
            return null;
        }
        if ((bytes[0] == 0xef) && (bytes[1] == 0xbb) && (bytes[2] == 0xbf)) {
            //UTF-8
            return new System.Text.UTF8Encoding(true, true);
        }
        if (bytes.Length < 4) {
            return null;
        }
        if ((bytes[0] == 0x00) && (bytes[1] == 0x00) &&
            (bytes[2] == 0xfe) && (bytes[3] == 0xff)) {
            //UTF-32 BE
            return new System.Text.UTF32Encoding(true, true);
        }

        return null;
    }


}