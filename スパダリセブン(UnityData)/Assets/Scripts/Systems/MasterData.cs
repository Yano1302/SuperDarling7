using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ���쐬�̃Z�[�u�E���[�h����N���X�̃X�N���v�g�ł�
// �Z�[�u�E���[�h�������ϐ��͂����ɋL�ڂ��Ă�������
[Serializable]
public class MasterData // json�f�[�^�Ƃ��ĕۑ�����class
{
    // �؋��H
    //public GameObject[] evidences = new GameObject[10];
    // �V�[����
    public SCENENAME scenename = SCENENAME.TitleScene;
    // �Z�[�u�������Ƃ����邩
    public bool haveSaved = false;
}

