using UnityEngine;

public partial class QuestController : BaseTextController
{

    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

    /// <summary>
    /// �˗����e��\������֐�
    /// </summary>
    /// <param name="storynum">�\��������CSV�t�@�C����</param>
    public void ViewQuest(string storynum)
    {
        // �󒍉�ʂ�\�����A�˗��I����ʂ��\���ɂ���
        questUI.SetActive(true);
        selectUI.SetActive(false);
        StorySetUp(storynum); // �w�肳�ꂽCSV�t�@�C�����Z�b�g�A�b�v
        charaName.text = storyTalks[talkNum].name; // �˗�����\��
        textLabel.text = storyTalks[talkNum].talks; // �˗����e��\��
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
        textasset = Resources.Load("�v�����i�[�č��G���A/Requisition/" + storynum, typeof(TextAsset)) as TextAsset;
        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����

        Debug.Log(storynum + "��ǂݍ��݂܂���");
    }

    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///



    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///

}
public partial class QuestController
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

    [SerializeField] private GameObject questUI; // �󒍉��UI
    [SerializeField] private GameObject selectUI; // �˗���I������UI

    /// ------private�ϐ�------- ///
    #endregion

    #region �v���p�e�B
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    #endregion

    /// --------�ϐ��ꗗ-------- ///
}