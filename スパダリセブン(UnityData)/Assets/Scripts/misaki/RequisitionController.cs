using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RequisitionController : BaseTextController
{
    private Supadari.SceneManager sceneManager; // �X�p�_���̃V�[���}�l�[�W���[�p�ϐ�

    protected override void Awake()
    {
        base.Awake();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    private void Start()
    {
        OnTalkButtonClicked();
    }
    protected override void StorySetUp(string storynum)
    {
        UnityEngine.Debug.Log(storynum + "��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B
        textasset = Resources.Load("�v�����i�[�č��G���A/Requisition/" + storynum, typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        // ��b���w�i�i�[�z��̃T�C�Y��[���͂̐�]�ɂ���
        backImages = new GameObject[storyTalks.Length];
        // �L�����N�^�[�摜�i�[�p2�����z��̃T�C�Y��[��b���̍ő�\���l��,���͂̐�]�ɂ���
        charaImages = new GameObject[displayCharaAnchors, storyTalks.Length];
        // �L�����N�^�[�n�C���C�g�i�[�p2�����z��̃T�C�Y��[��b���̍ő�\���l��,���͂̐�]�ɂ���
        charaHighlight = new bool[displayCharaAnchors, storyTalks.Length];
        // �v���W�F�N�g����TalkCharaImage�t�H���_�ɂ���摜��Ή������������͂��ƂɊi�[����
        for (int i = 0; i < storyTalks.Length; i++)
        {
            backImages[i] = (GameObject)Resources.Load("TalkBackImage/" + storyTalks[i].backImage);
        }
        // �v���W�F�N�g����TalkCharaImage�t�H���_�ɂ���摜��Ή������������͂��ƂɊi�[����
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            for (int j = 0; j < storyTalks.Length; j++)
            {
                // charaImages[0,j]�ɂ͒����ɕ\������L�����N�^�[�摜���i�[����
                if (i == 0) charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].centerTalkingChara);
            }
        }
        // charaImages[]�Ɋi�[���ꂽ�摜�����邩�ǂ����̐^�U��Ή������������͂��ƂɊi�[����
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            for (int j = 0; j < storyTalks.Length; j++)
            {
                // charaHighlight[0,j]�ɂ͒����ɕ\������L�����N�^�[�摜�����邩�ǂ������i�[����
                if (i == 0 && storyTalks[j].centerHighlight == "1") charaHighlight[i, j] = true;
            }
        }
        /// �����܂� ///
        UnityEngine.Debug.Log(storynum + "��ǂݍ��݂܂���");
    }
    protected override void InstantiateActors()
    {
        // �w�i�𐶐�
        if (backImages[talkNum]) backImage = Instantiate(backImages[talkNum], backImageAnchor.transform);
        // �L�����N�^�[�摜�𐶐�
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (!charaImages[i, talkNum]) continue; // null�Ȃ�R���e�B�j���[����
            // �����ɃL�����N�^�[�摜�𐶐�
            if (i == 0) centerCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
        }
        // �����҈ȊO�̃L�����N�^�[�摜���D�F�ɂ���
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (charaHighlight[i, talkNum]) continue; // null�Ȃ�R���e�B�j���[����
            // �����̃L�����N�^�[�摜���D�F�ɂ���
            if (i == 0 && centerCharaImage) centerCharaImage.GetComponent<Image>().color = Color.gray;
        }
    }
    public override void TalkEnd()
    {
        UnityEngine.Debug.Log("��b���I��");
        talkNum = default; // ���Z�b�g����
        if (TalkState == TALKSTATE.LASTTALK)
        {
            sceneManager.SceneChange(SCENENAME.StoryScene); // �X�g�[���[�փV�[���J�ڂ���
        }
        TalkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
    }
}
