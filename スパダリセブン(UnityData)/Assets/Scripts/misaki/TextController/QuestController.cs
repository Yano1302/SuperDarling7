using UnityEngine;

public class QuestController : BaseTextController
{
    [SerializeField] GameObject questUI; // �󒍉��UI
    [SerializeField] GameObject selectUI; // �˗���I������UI
    /// <summary>
    /// �˗����e��\������֐�
    /// </summary>
    /// <param name="storynum">�\��������CSV�t�@�C����</param>
    public void ViewQuest(string storynum)
    {
        selectUI.SetActive(false); // �˗��I����ʂ��\��
        questUI.SetActive(true); // �󒍉�ʂ�\��
        StorySetUp(storynum); // �w�肳�ꂽCSV�t�@�C�����Z�b�g�A�b�v
        charaName.text = storyTalks[talkNum].name; // �˗�����\��
        textLabel.text = storyTalks[talkNum].talks; // �˗����e��\��
    }
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
        /// �����܂� ///
        Debug.Log(storynum + "��ǂݍ��݂܂���");
    }
}
