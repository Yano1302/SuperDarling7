using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;

// �V���O���g������
public class TextController : MonoBehaviour
{
    //�X�g�[���[�ԍ�
    [Header("1-1�̂悤�ɓ���")]
    public string storynum;

    private string[] words;
    public Text textLabel;

    private GameObject[] CharaImages;
    public GameObject CharaImageBack;
    private GameObject charaimage = null;

    public GameObject talkButton;
    private int talkNum = 0;

    //csv�t�@�C���p
    StoryTalkData[] storytalks;

    void Awake()
    {
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B����� Story1-1 �� Story2-5 �̂悤�ɃX�e�[�W�ԍ��ɂ���ēǂݍ��ރt�@�C�����ς�����悤�ɂ��Ă���B
        textasset = Resources.Load("Story" + storynum, typeof(TextAsset)) as TextAsset;
        //�@CSVSerializer��p����csv�t�@�C����z��ɗ������ށB
        storytalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text);

        CharaImages = new GameObject[storytalks.Length];

        for (int i = 0; i < storytalks.Length; i++)
        {
            CharaImages[i] = (GameObject)Resources.Load("TalkCharaImage/" + storytalks[i].talkingChara + "Talk");
        }
    }

    public void Start()
    {
        talkButton.SetActive(false);
        OnTalkButtonClicked();
    }

    // �{�^���������Ɖ�b�X�^�[�g
    public void OnTalkButtonClicked()
    {
        // ��b�t�B�[���h�����Z�b�g����B
        textLabel.text = "";

        //�L�����N�^�[�摜�𐶐�
        if (charaimage != null)
        {
            Destroy(charaimage);
        }

        charaimage = Instantiate(CharaImages[talkNum], CharaImageBack.transform);

        StartCoroutine(Dialogue());

        // �g�[�N�{�^�����\���ɂ���B
        talkButton.SetActive(false);
    }

    // �R���[�`�����g���āA�P�������ƕ\������B
    IEnumerator Dialogue()
    {
        // ���p�X�y�[�X�ŕ����𕪊�����B
        words = storytalks[talkNum].talks.Split(' ');

        foreach (var word in words)
        {
            // 0.1�b���݂łP�������\������B
            textLabel.text = textLabel.text + word;
            yield return new WaitForSeconds(0.1f);
        }

        // ���̃Z���t������ꍇ�ɂ́A�g�[�N�{�^����\������B
        if (talkNum + 1 < storytalks.Length)
        {
            talkButton.SetActive(true);
        }

        // ���̃Z���t���Z�b�g����B
        talkNum = talkNum + 1;
    }
}

[System.Serializable]
public class StoryTalkData
{
    public string talkingChara;
    public string talks;
}
