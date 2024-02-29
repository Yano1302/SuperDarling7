using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;
using TMPro;
using Unity.VisualScripting;

public class TextController : DebugSetting
{
    private int talkNum = 0; // �_�C�����O�ԍ�
    private bool talkSkip = false; // �{�^�����N���b�N���ꂽ���ǂ����������t���O
    [Header("1-1�̂悤�ɓ���")]
    public string storynum; //�X�g�[���[�ԍ�
    private string words; // ����
    public TextMeshProUGUI charaName; // �L�����N�^�[���̃e�L�X�g�ϐ�
    public TextMeshProUGUI textLabel; // ���͂��i�[����e�L�X�g�ϐ�
    public TextMeshProUGUI buttonText; // �{�^���̃e�L�X�g�ϐ�
    private GameObject[] charaImages; // csv�t�@�C���ɋL�ڂ��ꂽ�L�����N�^�[�摜�����i�[����z��
    private GameObject charaimage = null; // �g�p����L�����N�^�[�摜
    public GameObject charaImageBack; // �L�����N�^�[�w�i
    public GameObject talkButton; // ��b��i�߂�{�^��
    StoryTalkData[] storytalks; //csv�t�@�C���ɂ��镶�͂��i�[����z��
    public enum TALKSTATE // ��b�֌W�̃X�e�[�^�X
    {
        NOTALK, // �b���Ă��Ȃ�
        TALKING, // ��b��
        NEXTTALK, // ���̃Z���t
        LASTTALK // �Ō�̃Z���t
    }
    private TALKSTATE talkState; // ��b�X�e�[�^�X�ϐ�
    public TALKSTATE TalkState
    {
        get { return talkState; }
        set
        {
            talkState = value;
            switch (talkState)
            {
                case TALKSTATE.NOTALK:
                    buttonText.text = "��b�J�n"; // �{�^���e�L�X�g��"��b�J�n"�ɕύX
                    break;
                case TALKSTATE.TALKING:
                    buttonText.text = "Skip"; // �{�^���e�L�X�g��"Skip"�ɕύX
                    break;
                case TALKSTATE.NEXTTALK:
                    buttonText.text = "����"; // �{�^���e�L�X�g��"����"�ɕύX
                    break;
                case TALKSTATE.LASTTALK:
                    buttonText.text = "��b�I��"; // �{�^���e�L�X�g��"��b�I��"�ɕύX
                    break;
            }
        }
    }
    protected override void Awake()
    {
        base .Awake(); // �f�o�b�O���O��\�����邩�ۂ��X�N���v�^�u���I�u�W�F�N�g��GameSettings���Q��
        StorySetUp(storynum); // �Ή������b�����Z�b�g����
        TalkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
    }
    /// <summary>
    /// �Ή������b�����Z�b�g����֐�
    /// </summary>
    /// <param name="storynum">�ǂݍ���CSV�t�@�C���̖��O ��(1-1)</param>
    private void StorySetUp(string storynum)
    {
        Debug.Log("Story"+storynum+"��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B����� Story1-1 �� Story2-5 �̂悤�ɃX�e�[�W�ԍ��ɂ���ēǂݍ��ރt�@�C�����ς�����悤�ɂ��Ă���B
        textasset = Resources.Load("Story/Story" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storytalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        charaImages = new GameObject[storytalks.Length]; // �L�����N�^�[�摜�i�[�z��̃T�C�Y�𕶏͂̐��Ɠ����ɂ���
        // �v���W�F�N�g����TalkCharaImage�t�H���_�ɂ���摜��Ή������������͂��ƂɊi�[����
        for (int i = 0; i < storytalks.Length; i++)
        {
            charaImages[i] = (GameObject)Resources.Load("TalkCharaImage/" + storytalks[i].talkingChara + "Talk");
        }
        /// �����܂� ///
        Debug.Log("Story" + storynum + "��ǂݍ��݂܂���");
    }

    // �{�^���������Ɖ�b�X�^�[�g
    public void OnTalkButtonClicked(string storynum = "")
    {
        // ��b�X�e�[�^�X���b���Ă��Ȃ��Ȃ�
        if (talkState == TALKSTATE.NOTALK)
        {
            if(storynum != "")
            {
                StorySetUp(storynum);
            }
            TalkState = TALKSTATE.TALKING; // ��b�X�e�[�^�X����b���ɕύX
        }
        else if (talkState == TALKSTATE.TALKING)
        {
            talkSkip = true; // �g�[�N�X�L�b�v�t���O�𗧂Ă�
            TalkState = TALKSTATE.NEXTTALK; // ��b�X�e�[�^�X�����̃Z���t�ɕύX
            return;
        }
        // ��b�t�B�[���h�����Z�b�g����B
        textLabel.text = "";
        //textLabel.text = storytalks[talkNum].talks; // �ꊇ�őS����\��
        if (charaimage != null) // �L�����N�^�[�摜���\������Ă����
        {
            Destroy(charaimage); // �摜��j�󂷂�
        }
        // �g�[�N�{�^�����\���ɂ���B
        if (talkState != TALKSTATE.LASTTALK)
        {
            //�L�����N�^�[�摜�𐶐�
            charaimage = Instantiate(charaImages[talkNum], charaImageBack.transform);
            StartCoroutine(Dialogue()); // ���͂�\������R���[�`�����J�n
        }
        else if (talkState == TALKSTATE.LASTTALK)
        {
            Debug.Log("��b���I��");
            talkNum = default; // ���Z�b�g����
            TalkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
        }
    }
    /// <summary>
    /// ���͂�\������R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator Dialogue()
    {
        Debug.Log("Story" + storynum + "��" + (talkNum + 1) + "��ڂ��Đ�");
        charaName.text = storytalks[talkNum].name; // �b���Ă���L�����N�^�[����\��
        words = storytalks[talkNum].talks; // ���͂��擾
        // �e�����ɑ΂��ČJ��Ԃ��������s���܂� C#��IEnumerable�@�\�ɂ��ꕶ�������o����
        foreach (char c in words)
        {
            // ������ textLabel �ɒǉ����܂�
            textLabel.text += c;
            // �{�^�����N���b�N���ꂽ��t���O�𗧂Ăă��[�v�𔲂���
            if (talkSkip) break;
            // ���̕�����\������O�ɏ����҂��܂�
            yield return new WaitForSeconds(0.05f); // �K�v�ɉ����Ă��̑҂����Ԃ𒲐����Ă�������
        }
        if (talkSkip == true) // �g�[�N�X�L�b�v�t���O����������
        {
            // �S����\��
            textLabel.text = storytalks[talkNum].talks;
        }
        talkNum++; // ���̃_�C�A���O�Ɉړ�
        talkState = TALKSTATE.NEXTTALK; // ��b�X�e�[�^�X�����̃Z���t�ɕύX
        talkSkip = false; // �g�[�N�X�L�b�v�t���O��false�ɂ���
        // ���ׂẴ_�C�A���O��\��������A�ǉ��̃_�C�A���O�����邩�ǂ������`�F�b�N
        if (talkNum >= storytalks.Length)
        {
            TalkState = TALKSTATE.LASTTALK; // ��b�X�e�[�^�X���Ō�̃Z���t�ɕύX
        }
        talkButton.SetActive(true); // talkButton ��\�����܂�
    }
}
[System.Serializable] // �T�u�v���p�e�B�𖄂ߍ���
public class StoryTalkData // StoryTalkData�̒���talkingChara��talks��z�u����
{
    public string talkingChara; // �L�����N�^�[�摜��
    public string name; // �L�����N�^�[��
    public string talks; // ����
}