using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Supadari;

public class BaseTextController : DebugSetting
{
    protected int talkNum = 0; // �_�C�����O�ԍ�
    public int displayCharaAnchors = 3; // �L�����N�^�[�摜�̕\���ӏ���
    [SerializeField]private float textBaseSpeed = 0.05f; // �e�L�X�g����̃x�[�X�X�s�[�h
    public float playerTextSpeed = 0.5f; // �v���C���[���w�肵���e�L�X�g����̃X�s�[�h�x��
    public float textDelay = 1.5f; // �e�L�X�g�ƃe�L�X�g�̊Ԃ̎���(�I�[�g���[�h�̂ݎg�p)
    protected bool talkSkip = false; // �{�^�����N���b�N���ꂽ���ǂ����������t���O
    protected bool talkAuto = false; // ��b���I�[�g��ԂȂ̂��������t���O
    [Header("1-1�̂悤�ɓ���")]
    public string storynum; //�X�g�[���[�ԍ�
    protected string words; // ����
    private int currentCharIndex = 0; // ���͂̕\���ʒu�������ϐ�
    public TextMeshProUGUI charaName; // �L�����N�^�[���̃e�L�X�g�ϐ�
    public TextMeshProUGUI textLabel; // ���͂��i�[����e�L�X�g�ϐ�
    public TextMeshProUGUI buttonText; // �{�^���̃e�L�X�g�ϐ�

    protected GameObject[] backImages; // csv�t�@�C���ɋL�ڂ��ꂽ�w�i�̊i�[�z��
    protected GameObject backImage = null; // �g�p����w�i�摜
    [Header("�w�i�\���ӏ�")]
    [SerializeField] protected GameObject backImageAnchor; // �w�i�\���ӏ�

    protected bool[,] charaHighlight; // csv�t�@�C���ɋL�ڂ��ꂽ�L�����N�^�[�����点�邩���i�[����2�����z��
    protected GameObject[,] charaImages; // csv�t�@�C���ɋL�ڂ��ꂽ�L�����N�^�[�摜�����i�[����2�����z��
    private GameObject leftCharaImage = null; // �g�p����L�����N�^�[�摜(����)
    private GameObject rightCharaImage = null; // �g�p����L�����N�^�[�摜(�E��)
    protected GameObject centerCharaImage = null; // �g�p����L�����N�^�[�摜(������)
    [Header("�L�����N�^�[�\���ӏ� [0]...���� [1]...�E�� [2]...����")]
    [SerializeField] protected GameObject[] charaAnchors = new GameObject[3]; // �L�����N�^�[�\���ӏ�

    string[] nameBGM; // �炷BGM�i�[�z��

    public GameObject talkButton; // ��b��i�߂�{�^��
    protected StoryTalkData[] storyTalks; //csv�t�@�C���ɂ��镶�͂��i�[����z��

    public bool runtimeCoroutine = false; // �R���[�`�������s�����ǂ���
    private Coroutine dialogueCoroutine; // �R���[�`�����i�[����ϐ�

    [SerializeField]protected SceneManager sceneManager; // �V�[���}�l�[�W���[�ϐ�
    [SerializeField]bool testText = false; // �{�^���̃e�L�X�g��\�����邩�ǂ���
    [SerializeField] GameObject autoImage; // �I�[�g���[�h�̉摜

    public TALKSTATE talkState; // ��b�X�e�[�^�X�ϐ�
    public TALKSTATE TalkState
    {
        get { return talkState; }
        set
        {
            talkState = value;
            switch (talkState)
            {
                case TALKSTATE.NOTALK:
                    if(testText) buttonText.text = "��b�J�n"; // �{�^���e�L�X�g��"��b�J�n"�ɕύX
                    break;
                case TALKSTATE.TALKING:
                    if (testText) buttonText.text = "Skip"; // �{�^���e�L�X�g��"Skip"�ɕύX
                    break;
                case TALKSTATE.NEXTTALK:
                    if (testText) buttonText.text = "����"; // �{�^���e�L�X�g��"����"�ɕύX
                    break;
                case TALKSTATE.LASTTALK:
                    if (testText) buttonText.text = "��b�I��"; // �{�^���e�L�X�g��"��b�I��"�ɕύX
                    break;
            }
        }
    }
    protected override void Awake()
    {
        base .Awake(); // �f�o�b�O���O��\�����邩�ۂ��X�N���v�^�u���I�u�W�F�N�g��GameSettings���Q��
        if(storynum!="") StorySetUp(storynum); // �Ή������b�����Z�b�g����
        TalkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>(); // �I�[�f�B�I�}�l�[�W���[���擾
        playerTextSpeed = sceneManager.enviromentalData.TInstance.textSpeed; // �e�L�X�g�X�s�[�h��ݒ�
    }
    /// <summary>
    /// �Ή������b�����Z�b�g����֐�
    /// </summary>
    /// <param name="storynum">�ǂݍ���CSV�t�@�C���̖��O ��(1-1)</param>
    protected virtual void StorySetUp(string storynum)
    {
        Debug.Log("Story"+storynum+"��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B����� Story1-1 �� Story2-5 �̂悤�ɃX�e�[�W�ԍ��ɂ���ēǂݍ��ރt�@�C�����ς�����悤�ɂ��Ă���B
        textasset = Resources.Load("�v�����i�[�č��G���A/Story/Story" + storynum, typeof(TextAsset)) as TextAsset;
        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        storyTalks = CSVSerializer.Deserialize<StoryTalkData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        // ��b���w�i�i�[�z��̃T�C�Y��[���͂̐�]�ɂ���
        backImages = new GameObject[storyTalks.Length];
        // �L�����N�^�[�摜�i�[�p2�����z��̃T�C�Y��[��b���̍ő�\���l��,���͂̐�]�ɂ���
        charaImages = new GameObject[displayCharaAnchors, storyTalks.Length];
        // �L�����N�^�[�n�C���C�g�i�[�p2�����z��̃T�C�Y��[��b���̍ő�\���l��,���͂̐�]�ɂ���
        charaHighlight = new bool[displayCharaAnchors, storyTalks.Length];
        // BGM���i�[����z��̃T�C�Y��[���͂̐�]�ɂ���
        nameBGM = new string[storyTalks.Length];
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
                // charaImages[0,j]�ɂ͍����ɕ\������L�����N�^�[�摜���i�[����
                if (i == 0) charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].leftTalkingChara);
                // charaImages[1,j]�ɂ͉E���ɕ\������L�����N�^�[�摜���i�[����
                else if (i == 1) charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].rightTalkingChara);
                // charaImages[2,j]�ɂ͒����ɕ\������L�����N�^�[�摜���i�[����
                else charaImages[i, j] = (GameObject)Resources.Load("TalkCharaImage/" + storyTalks[j].centerTalkingChara);
            }
        }
        // charaImages[]�Ɋi�[���ꂽ�摜�����邩�ǂ����̐^�U��Ή������������͂��ƂɊi�[����
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            for (int j = 0; j < storyTalks.Length; j++)
            {
                // charaHighlight[0,j]�ɂ͍����ɕ\������L�����N�^�[�摜�����邩�ǂ������i�[����
                if (i == 0 && storyTalks[j].leftHighlight == "1") charaHighlight[i, j] = true;
                // charaHighlight[1,j]�ɂ͉E���ɕ\������L�����N�^�[�摜�����邩�ǂ������i�[����
                else if (i == 1 && storyTalks[j].rightHighlight == "1") charaHighlight[i, j] = true;
                // charaHighlight[2,j]�ɂ͒����ɕ\������L�����N�^�[�摜�����邩�ǂ������i�[����
                else if (i == 2 && storyTalks[j].centerHighlight == "1") charaHighlight[i, j] = true;
            }
        }
        // BGM����Ή������������͂��ƂɊi�[����
        for (int i = 0; i < storyTalks.Length; i++)
        {
            nameBGM[i] = storyTalks[i].BGM;
        }
        /// �����܂� ///
        Debug.Log("Story" + storynum + "��ǂݍ��݂܂���");
    }
    /// <summary>
    /// ��b�Ɋւ���{�^���֐�(�ύX�s��)
    /// </summary>
    public virtual void OnTalkButtonClicked()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // ��b�X�e�[�^�X���b���Ă��Ȃ��Ȃ�
        {
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
            InstantiateActors(); // �o��l�����𐶐�
            StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
        }
        else if (TalkState == TALKSTATE.LASTTALK) // ��b�X�e�[�^�X���Ō�̃Z���t�Ȃ�
        {
            TalkEnd(); //��b���I������
        }
    }
    /// <summary>
    /// ��b�Ɋւ���{�^���֐�(�ǂݍ���CSV�ύX��)
    /// </summary>
    /// <param name="storynum">�ǂݍ��݂���CSV��</param>
    public virtual void OnTalkButtonClicked(string storynum = "")
    {
        // �X�g�[���[�ԍ��������
        if (storynum != "")
        {
            StorySetUp(storynum); // �Ή������b�����Z�b�g
            talkNum = default; // �����ɖ߂�
        }
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // ��b�X�e�[�^�X���b���Ă��Ȃ��Ȃ�
        {
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
            InstantiateActors(); // �o��l�����𐶐�
            StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
        }
        else if (TalkState == TALKSTATE.LASTTALK) // ��b�X�e�[�^�X���Ō�̃Z���t�Ȃ�
        {
            TalkEnd(); //��b���I������
        }
    }
    /// <summary>
    /// ��b�Ɋւ���{�^���֐�(talkNum�ύX��)
    /// </summary>
    /// <param name="num">�ǂݍ��݂���CSV�̍s</param>
    public virtual void OnTalkButtonClicked(int num = 9999)
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        if (TalkState == TALKSTATE.NOTALK) // ��b�X�e�[�^�X���b���Ă��Ȃ��Ȃ�
        {
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
            if (num != 9999 && num < storyTalks.Length) talkNum = num;
            InitializeTalkField(); // �\������Ă���e�L�X�g����������
            InstantiateActors(); // �o��l�����𐶐�
            StartDialogueCoroutine(); // ���͂�\������R���[�`�����J�n
        }
        else if (TalkState == TALKSTATE.LASTTALK) // ��b�X�e�[�^�X���Ō�̃Z���t�Ȃ�
        {
            TalkEnd(); //��b���I������
        }
    }
    /// <summary>
    /// ��b���I������֐�
    /// </summary>
    public virtual void TalkEnd()
    {
        Debug.Log("��b���I��");
        talkNum = default; // ���Z�b�g����
        TalkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
        if (talkAuto) OnAutoModeCllicked(); // �I�[�g���[�h���I���ł���΃I�t�ɂ���
    }
    /// <summary>
    /// ��b�֌W�̕\��������������֐�
    /// </summary>
    protected void InitializeTalkField()
    {
        textLabel.text = ""; // ��b�t�B�[���h�����Z�b�g����
        if(charaName) charaName.text = ""; // �b���Ă���L�����N�^�[�������Z�b�g����
        // �w�i�摜���\������Ă���Ή摜��j�󂷂�
        if (backImage) Destroy(backImage);
        // �����L�����N�^�[�摜���\������Ă���Ή摜��j�󂷂�
        if (leftCharaImage) Destroy(leftCharaImage);
        // �E���L�����N�^�[�摜���\������Ă���Ή摜��j�󂷂�
        if (rightCharaImage) Destroy(rightCharaImage);
        // �����L�����N�^�[�摜���\������Ă���Ή摜��j�󂷂�
        if (centerCharaImage) Destroy(centerCharaImage);
    }
    /// <summary>
    /// �o��l�����𐶐�����֐�
    /// </summary>
    protected virtual void InstantiateActors()
    {
        // �w�i�𐶐�
        if (backImages[talkNum]) backImage = Instantiate(backImages[talkNum], backImageAnchor.transform);
        // �L�����N�^�[�摜�𐶐�
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (!charaImages[i, talkNum]) continue; // null�Ȃ�R���e�B�j���[����
                                                    // �����ɃL�����N�^�[�摜�𐶐�
            if (i == 0) leftCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
            // �E���ɃL�����N�^�[�摜�𐶐�
            else if (i == 1) rightCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
            // �����ɃL�����N�^�[�摜�𐶐�
            else if (i == 2) centerCharaImage = Instantiate(charaImages[i, talkNum], charaAnchors[i].transform);
        }
        // �����҈ȊO�̃L�����N�^�[�摜���D�F�ɂ���
        for (int i = 0; i < displayCharaAnchors; i++)
        {
            if (charaHighlight[i, talkNum]) continue; // null�Ȃ�R���e�B�j���[����
                                                      // �����̃L�����N�^�[�摜���D�F�ɂ���
            if (i == 0 && leftCharaImage) leftCharaImage.GetComponent<Image>().color = Color.gray;
            // �E���̃L�����N�^�[�摜���D�F�ɂ���
            else if (i == 1 && rightCharaImage) rightCharaImage.GetComponent<Image>().color = Color.gray;
            // �����̃L�����N�^�[�摜���D�F�ɂ���
            else if (i == 2 && centerCharaImage) centerCharaImage.GetComponent<Image>().color = Color.gray;
        }
        // BGM��炷
        if (nameBGM[talkNum] == "Stop") sceneManager.audioManager.BGM_Stop(); // Stop�Ȃ�BGM���~�߂�
        else if (nameBGM[talkNum] != "0") sceneManager.audioManager.BGM_Play(nameBGM[talkNum], sceneManager.enviromentalData.TInstance.volumeBGM); // BGM���������Ă�����؂�ւ��@�󔒂Ȃ瑱�s
    }
    /// <summary>
    /// �R���[�`���J�n�֐�
    /// </summary>
    protected void StartDialogueCoroutine()
    {
        // �R���[�`�������łɎ��s����Ă���ꍇ�͒�~
        if (runtimeCoroutine) StopCoroutine(dialogueCoroutine);
        // �R���[�`���J�n
        dialogueCoroutine = StartCoroutine(Dialogue());
        runtimeCoroutine = true; // �t���O�����s���ɕύX
    }
    /// <summary>
    /// �R���[�`���ꎞ��~�֐�
    /// </summary>
    public void PauseDialogueCoroutine()
    {
        // �����Ă���R���[�`���������
        if (runtimeCoroutine)
        {
            StopCoroutine(dialogueCoroutine); // �R���[�`�����~�߂�
            currentCharIndex = textLabel.text.Length; // ���݂̕����̕\���ʒu��ۑ�
        }
    }
    /// <summary>
    /// �R���[�`���ĊJ�֐�
    /// </summary>
    public void ResumeDialogueCoroutine()
    {
        // �R���[�`�����~�܂��Ă���΁A�ĊJ�p�̃R���[�`���J�n
        if (runtimeCoroutine) dialogueCoroutine = StartCoroutine(ResumeDialogue());
    }
    /// <summary>
    /// ���͂�\������R���[�`��
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Dialogue()
    {
        Debug.Log(storynum + "��" + (talkNum + 1) + "��ڂ��Đ�");
        TalkState = TALKSTATE.TALKING; // ��b�X�e�[�^�X��b�����ɂ���
        charaName.text = storyTalks[talkNum].name; // �b���Ă���L�����N�^�[����\��
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
    /// <summary>
    /// �ꎞ��~�����ӏ�����\������R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator ResumeDialogue()
    {
        // ���͂̎c����ĕ\��
        for (int i = currentCharIndex; i < words.Length; i++)
        {
            // ������ textLabel �ɒǉ����܂�
            textLabel.text += words[i];
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
    /// <summary>
    /// ���̃_�C�A���O�ɕύX����֐�
    /// </summary>
    protected virtual void NextDialogue()
    {
        // �g�[�N�X�L�b�v�t���O����������
        if (talkSkip == true) textLabel.text = storyTalks[talkNum].talks; // �S����\��
        talkNum++; // ���̃_�C�A���O�Ɉړ�
        
        TalkState = TALKSTATE.NEXTTALK; // ��b�X�e�[�^�X�����̃Z���t�ɕύX
        talkSkip = false; // �g�[�N�X�L�b�v�t���O��false�ɂ���
        // ���̃_�C�A���O�ōŌ�Ȃ��b�X�e�[�^�X���Ō�̃Z���t�ɕύX
        if (talkNum >= storyTalks.Length) TalkState = TALKSTATE.LASTTALK;
        runtimeCoroutine = false; // �t���O�𖢎��s�ɕύX
    }
    /// <summary>
    /// �e�L�X�g�X�s�[�h���v�Z����֐�
    /// </summary>
    /// <returns></returns>
    protected float CalculataTextSpeed()
    {
        // ��b�X�s�[�h*10�i�K�̂����̂ǂꂩ(5���)
        return textBaseSpeed / (playerTextSpeed / 0.5f);
    }
    /// <summary>
    /// �I�[�g���[�h��؂�ւ���֐�
    /// </summary>
    public void OnAutoModeCllicked()
    {
        // talkAuto��true�Ȃ�false�ɁAfalse�Ȃ�true�ɕϊ�
        talkAuto = !talkAuto;
        // �I�[�g���[�h�Ȃ�I�[�g���[�h�摜���o���@�I�[�g���[�h�ł͂Ȃ��Ȃ�摜���o���Ȃ�
        if (!autoImage) return;
        if(talkAuto) autoImage.SetActive(true);
        else autoImage.SetActive(false);
    }
}
[System.Serializable] // �T�u�v���p�e�B�𖄂ߍ���
public class StoryTalkData
{
    public string backImage; // �w�i�摜
    public string leftTalkingChara; // �L�����N�^�[�摜��(����)
    public string rightTalkingChara; // �L�����N�^�[�摜��(�E��)
    public string centerTalkingChara; // �L�����N�^�[�摜��(����)
    public string leftHighlight; // �L�����N�^�[�摜�����点�邩(����)
    public string rightHighlight; // �L�����N�^�[�����点�邩(����)
    public string centerHighlight; // �L�����N�^�[�����点�邩(����)
    public string name; // �L�����N�^�[��
    public string talks; // ����
    public string BGM; // BGM��
    public string stage; // �X�e�[�W�ԍ�
}