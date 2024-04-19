using System.Collections;
using UnityEngine;

public class SolveTextController : BaseTextController
{
    private void Start()
    {
        OnTalkButtonClicked(0);
    }
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B
        textasset = Resources.Load("�v�����i�[�č��G���A/Solve/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        /// �����܂� ///
        Debug.Log(storynum + "��ǂݍ��݂܂���");
    }
    protected override IEnumerator Dialogue()
    {
        Debug.Log(storynum + "��" + (talkNum + 1) + "��ڂ��Đ�");
        words = storyTalks[talkNum].talks; // ���͂��擾
        // ������ textLabel �ɒǉ����܂�
        textLabel.text = words;
        yield return true;
    }
    public override void OnTalkButtonClicked(int num = 9999)
    {
        // num��storyTalks.length�ȏ�܂��͌�talkNum�Ɠ�������num==0�ł͂Ȃ��Ȃ烊�^�[��
        if (num >= storyTalks.Length || num == talkNum && num != 0) return;
        talkNum = num; // �w�肳�ꂽ�l����
        sceneManager.audioManager.SE_Play("SE_click");
        InitializeTalkField(); // �\������Ă���e�L�X�g����������
        StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
    }
}
