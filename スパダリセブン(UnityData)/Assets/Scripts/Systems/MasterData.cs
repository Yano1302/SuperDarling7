using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ���쐬�̃Z�[�u�E���[�h����N���X�̃X�N���v�g�ł�
// �Z�[�u�E���[�h�������ϐ��͂����ɋL�ڂ��Ă�������
[Serializable]
public class MasterData // json�f�[�^�Ƃ��ĕۑ�����class
{
    // ���L�͗�ł�
    /*
    public float[] rhythmRanking = new float[10]; // ���Y�������L���O
    public string rhythmHighScoreRank = "D"; // ���Y���n�C�X�R�A�����N
    */
    // �؋��H
    //public GameObject[] evidences = new GameObject[10];
    // �V�[���ԍ��H
    public int sceneNum = 0;
    // BGM����
    public float volumeBGM = 0.5f;
    // SE����
    public float volumeSE = 0.5f;
    // �e�L�X�g�X�s�[�h
    public float textSpeed = 0.5f;

}

