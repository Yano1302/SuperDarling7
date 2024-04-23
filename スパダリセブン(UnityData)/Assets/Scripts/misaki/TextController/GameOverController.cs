using System.Collections;
using UnityEngine;

public class GameOverController : BaseTextController
{
    // Start is called before the first frame update
    void Start()
    {
        OnTalkButtonClicked("");
    }
    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B
        textasset = Resources.Load("�v�����i�[�č��G���A/GameOver/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        /// �����܂� ///
        Debug.Log(storynum + "��ǂݍ��݂܂���");
    }
    protected override IEnumerator Dialogue()
    {
        talkNum = Random.Range(0, storyTalks.Length); // �\�����镶�͂������_���Őݒ�
        Debug.Log(storynum + "��" + (talkNum + 1) + "��ڂ��Đ�");
        TalkState = TALKSTATE.TALKING; // ��b�X�e�[�^�X��b�����ɂ���
        // charaName.text = storyTalks[talkNum].name; // �b���Ă���L�����N�^�[����\��
        words = storyTalks[talkNum].talks; // ���͂��擾
        // �e�����ɑ΂��ČJ��Ԃ��������s���܂� C#��IEnumerable�@�\�ɂ��ꕶ�������o����
        foreach (char c in words)
        {
            // ������ textLabel �ɒǉ����܂�
            textLabel.text += c;
            // �{�^�����N���b�N���ꂽ��t���O�𗧂Ăă��[�v�𔲂���
            if (talkSkip) break;
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
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // ��b�X�e�[�^�X���b���Ă��Ȃ��Ȃ�
        {
            // �X�g�[���[�ԍ��������
            if (storynum != "") StorySetUp(storynum); // �Ή������b�����Z�b�g
            TalkState = TALKSTATE.TALKING; // ��b�X�e�[�^�X����b���ɕύX
        }
        else if (TalkState == TALKSTATE.TALKING) // ��b�X�e�[�^�X���b�����Ȃ�
        {
            talkSkip = true; // �g�[�N�X�L�b�v�t���O�𗧂Ă�
            TalkState = TALKSTATE.NEXTTALK; // ��b�X�e�[�^�X�����̃Z���t�ɕύX
            return;
        }
        if (TalkState != TALKSTATE.LASTTALK) // ��b�X�e�[�^�X���b����,�Ȃ�
        {
            InitializeTalkField(); // �\������Ă���e�L�X�g����������
            StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
        }
        else if (TalkState == TALKSTATE.LASTTALK) // ��b�X�e�[�^�X���Ō�̃Z���t�Ȃ�
        {
            TalkEnd(); //��b���I������
        }
    }
    /// <summary>
    /// �^�C�g���֖߂�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.m_tInstance.volumeSE); // SE��炷
        sceneManager.SceneChange(0); // �^�C�g���V�[���֑J�ڂ���
    }
    /// <summary>
    /// ���[�h�X���b�g���J���֐�
    /// </summary>
    public void LoadButton()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.m_tInstance.volumeSE); // SE��炷
        sceneManager.uiManager.OpenUI(UIType.LoadSlot); // ���[�h�X���b�g��\��
    }
}
