using UnityEngine;

public partial class GameClearController : BaseTextController
{

    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

    public override void OnTalkButtonClicked(string storynum = "")
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
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
            BackTitle(); //�^�C�g���֖߂�
        }
    }

    /// <summary>
    /// �^�C�g���֖߂�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE); // SE��炷
        sceneManager.SceneChange(0); // �^�C�g���V�[���֑J�ڂ���
    }

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///

    protected override void StorySetUp(string storynum)
    {
        Debug.Log(storynum + "��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B
        textasset = Resources.Load("�v�����i�[�č��G���A/GameClear/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        /// �����܂� ///
        Debug.Log(storynum + "��ǂݍ��݂܂���");
    }

    protected override void NextDialogue()
    {
        base.NextDialogue();
        if (TalkState == TALKSTATE.LASTTALK) animator.SetTrigger("Stamp");
    }

    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///

    private void Start()
    {
        charaName.text = storyTalks[talkNum].name; // �b���Ă���L�����N�^�[����\��
    }

    private void Update()
    {
        if (animEnd) ButtonActive();
    }

    /// <summary>
    /// �{�^���̕\�����s���֐�
    /// </summary>
    private void ButtonActive()
    {
        if (buttonImage.activeSelf == false) buttonImage.SetActive(true);
    }

    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///
}
public partial class GameClearController
{
    /// --------�ϐ��ꗗ-------- ///

    #region public�ϐ�
    /// -------public�ϐ�------- ///



    /// -------public�ϐ�------- ///
    #endregion

    #region protected�ϐ�
    /// -----protected�ϐ�------ ///



    /// -----protected�ϐ�------ ///
    #endregion

    #region private�ϐ�
    /// ------private�ϐ�------- ///

    private bool animEnd = false; // �A�j���[�V�����I���ʒm

    [SerializeField] private Animator animator; // �X�^���v�̃A�j���[�^�[

    [SerializeField] private GameObject buttonImage; // �{�^���̃C���[�W

    /// ------private�ϐ�------- ///
    #endregion

    #region �v���p�e�B
    /// -------�v���p�e�B------- ///

    /// <summary>
    /// �A�j���[�V�����I���ʒm�Z�b�^�[�֐�
    /// </summary>
    public bool AnimEnd { set { animEnd = value; } }


    /// -------�v���p�e�B------- ///
    #endregion

    /// --------�ϐ��ꗗ-------- ///
}