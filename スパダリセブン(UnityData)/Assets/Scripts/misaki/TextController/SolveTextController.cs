using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SolveTextController : BaseTextController
{
    [SerializeField] bool firstWrite = true; // �Q�[���X�^�[�g����CSV1�s�ڂ�\�����邩
    [SerializeField] bool addOnClick = false; // �{�^���`�F�b�N
    private Button[] itemButtons = new Button[10]; // �{�^���z��
    private void Start()
    {
        if (firstWrite) OnTalkButtonClicked(0); // �Q�[���X�^�[�g���ɕ\������
        if (addOnClick) itemButtons[0].onClick.AddListener(() => OnTalkButtonClicked(0)); // �����_����onclick�֐���ݒ�
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
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
        InitializeTalkField(); // �\������Ă���e�L�X�g����������
        StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
    }
}
