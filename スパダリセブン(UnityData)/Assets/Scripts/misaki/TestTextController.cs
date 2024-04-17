using System.Collections;
using UnityEngine;

public class TestTextController : BaseTextController
{
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B
        textasset = Resources.Load("�v�����i�[�č��G���A/Json/TestText/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        /// �����܂� ///
        Debug.Log(storynum + "��ǂݍ��݂܂���");
    }
    protected override IEnumerator Dialogue()
    {
        talkNum = default; // �f�t�H���g�ɖ߂�
        Debug.Log(storynum + "��" + (talkNum + 1) + "��ڂ��Đ�");
        TalkState = TALKSTATE.TALKING; // ��b�X�e�[�^�X��b�����ɂ���
        words = storyTalks[talkNum].talks; // ���͂��擾
        // �e�����ɑ΂��ČJ��Ԃ��������s���܂� C#��IEnumerable�@�\�ɂ��ꕶ�������o����
        foreach (char c in words)
        {
            // ������ textLabel �ɒǉ����܂�
            textLabel.text += c;
            // ���̕�����\������O�ɏ����҂��܂�
            yield return new WaitForSeconds(CalculataTextSpeed());
        }
        NextDialogue(); // ���̃_�C�A���O�ɕύX����
        if (talkAuto) // �I�[�g���[�h�ł����
        {
            yield return new WaitForSeconds(textDelay); // textDelay�b�҂�
            OnTalkButtonClicked(); // ���̉�b�������ŃX�^�[�g����
        }
    }
    public override void OnTalkButtonClicked(string storynum = "")
    {
        sceneManager.audioManager.SE_Play("SE_click");
        InitializeTalkField(); // �\������Ă���e�L�X�g����������
        StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
    }
}
