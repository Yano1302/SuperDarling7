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
    Dungeon = 5, // �����e�X�g�V�[��
    GameOverScene = 6, // �Q�[���I�[�o�[�V�[��
    GameClearScene = 7 // �Q�[���N���A�V�[��
}
// ��b�֌W�̃X�e�[�^�X
public enum TALKSTATE 
{
    NOTALK, // �b���Ă��Ȃ�
    TALKING, // ��b��
    NEXTTALK, // ���̃Z���t
    LASTTALK // �Ō�̃Z���t
}
// ���ݒ�̃X�e�[�^�X
public enum SETTINGSTATE
{
    BGM = 0, // BGM�̐ݒ�
    SE = 1, // SE�̐ݒ�
    TEXTSPEED = 2 // �e�L�X�g�X�s�[�h�̐ݒ�
}
// �X�g�[���[���j���[��ʂ̃X���C�h�̃X�e�[�^�X
public enum SLIDESTATE
{
    DEFAULT = 0, // �X���C�h���Ă��Ȃ�
    SLIDE = 1 // �X���C�h��
}