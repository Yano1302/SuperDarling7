using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // �V�[���̖��́@�����̓r���h�ԍ��ɍ��킹�Ă���
    public enum SCENENAME
    {
        TitleScene = 0, // �^�C�g��
        RequisitionsScene = 1, // �˗��V�[��
        StoryScene = 2, // �X�g�[���[�V�[��
        InvestigationScene = 3, // �����V�[��
        SolveScene = 4, // �����V�[��
        Dungeon = 5 // �����e�X�g�V�[��
    }
    // ��b�֌W�̃X�e�[�^�X
    public enum TALKSTATE 
    {
        NOTALK, // �b���Ă��Ȃ�
        TALKING, // ��b��
        NEXTTALK, // ���̃Z���t
        LASTTALK // �Ō�̃Z���t
    }

