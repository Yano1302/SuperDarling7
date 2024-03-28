using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;�@�@�@//�ǉ�

public class CSVSetting<T> where T : class {
    private TextAsset csvFile; // CSV�t�@�C��
    private List<string[]> csvData = new List<string[]>(); // CSV�t�@�C���̒��g�����郊�X�g

    void Start() {
        csvFile = Resources.Load("Sample") as TextAsset; // Resources�ɂ���CSV�t�@�C�����i�[
        StringReader reader = new StringReader(csvFile.text); // TextAsset��StringReader�ɕϊ�

        while (reader.Peek() != -1) {
            string line = reader.ReadLine(); // 1�s���ǂݍ���
            csvData.Add(line.Split(',')); // csvData���X�g�ɒǉ�����
        }

        for (int i = 0; i < csvData.Count; i++) // csvData���X�g�̏����𖞂����l�̐��i�S�āj
        {
            // �f�[�^�̕\��
            UnityEngine.Debug.Log("���{��F" + csvData[i][0] + ", English�F" + csvData[i][1]);
        }
    }
}