using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;
using TMPro;
using Unity.VisualScripting;

// �V���O���g������
public class TextController : MonoBehaviour
{
    private int talkNum = 0; // ���͂́Z������
    //�X�g�[���[�ԍ�
    [Header("1-1�̂悤�ɓ���")]
    public string storynum;
    private string words; // ����
    public TextMeshProUGUI textLabel; // ���͂��i�[����e�L�X�g�ϐ�
    public TextMeshProUGUI buttonText; // �{�^���̃e�L�X�g�ϐ�
    private GameObject[] charaImages; // csv�t�@�C���ɋL�ڂ��ꂽ�L�����N�^�[�摜�����i�[����z��
    private GameObject charaimage = null; // �g�p����L�����N�^�[�摜
    public GameObject charaImageBack; // �L�����N�^�[�w�i
    public GameObject talkButton; // ��b��i�߂�{�^��
    StoryTalkData[] storytalks; //csv�t�@�C���ɂ��镶�͂��i�[����z��
    enum TALKSTATE // ��b�֌W�̃X�e�[�^�X
    {
        NOTALK, // �b���Ă��Ȃ�
        TALKING, // ��b��
        LASTTALK // �Ō�̃Z���t
    }
    [SerializeField]TALKSTATE talkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X�ϐ�

    void Awake()
    {
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
    }
    public void Start()
    {
        buttonText.text = "��b�J�n"; // �{�^���e�L�X�g��"��b�J�n"�ɕύX
    }
    // �{�^���������Ɖ�b�X�^�[�g
    public void OnTalkButtonClicked()
    {
        // ��b�X�e�[�^�X���b���Ă��Ȃ��Ȃ�
        if (talkState == TALKSTATE.NOTALK)
        {
            buttonText.text = "����"; // �{�^���e�L�X�g��"����"�ɕύX
            talkState = TALKSTATE.TALKING; // ��b�X�e�[�^�X����b���ɕύX
        }
        // ��b�t�B�[���h�����Z�b�g����B
        textLabel.text = "";
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
            talkButton.SetActive(false);
        }
        else if (talkState == TALKSTATE.LASTTALK)
        {
            talkNum = default;
            talkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
            buttonText.text = "��b�J�n"; // �{�^���e�L�X�g��"��b�J�n"�ɕύX
        }
    }
    /// <summary>
    /// ���͂�\������R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator Dialogue()
    {
        // ��������擾
        words = storytalks[talkNum].talks;
        // �e�����ɑ΂��ČJ��Ԃ��������s���܂� C#��IEnumerable�@�\�ɂ��ꕶ�������o����
        foreach (char c in words)
        {
            // ������ textLabel �ɒǉ����܂�
            textLabel.text += c;
            // ���̕�����\������O�ɏ����҂��܂�
            yield return new WaitForSeconds(0.05f); // �K�v�ɉ����Ă��̑҂����Ԃ𒲐����Ă�������
        }
        // ���̃_�C�A���O�Ɉړ�
        talkNum++;
        // ���ׂẴ_�C�A���O��\��������A�ǉ��̃_�C�A���O�����邩�ǂ������`�F�b�N
        if (talkNum >= storytalks.Length)
        {
            buttonText.text = "��b�I��"; // �{�^���e�L�X�g��"��b�I��"�ɕύX
            talkState = TALKSTATE.LASTTALK; // ��b�X�e�[�^�X���Ō�̃Z���t�ɕύX
        }
        talkButton.SetActive(true); // talkButton ��\�����܂�

    }
}

[System.Serializable] // �T�u�v���p�e�B�𖄂ߍ���
public class StoryTalkData // StoryTalkData�̒���talkingChara��talks��z�u����
{
    public string talkingChara; // �b���Ă���L�����N�^�[��
    public string talks; // ����
}
