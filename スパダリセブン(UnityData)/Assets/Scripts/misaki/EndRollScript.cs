using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndRollScript : DebugSetting
{
    //�@�e�L�X�g�̃X�N���[���X�s�[�h
    [SerializeField]
    private float textScrollSpeed = 10;
    //�@�e�L�X�g�̐����ʒu
    [Header("�X�N���[�����~�߂���Pos.Y��/100�����l�����")]
    [SerializeField]
    private float limitPosition = 15f;
    //�@�G���h���[�����I���������ǂ���
    [SerializeField]
    private bool isStopEndRoll=false;
    //�@�V�[���ړ��p�R���[�`��
    private Coroutine endRollCoroutine;
    CreditData[] creditDates; //csv�t�@�C���ɂ��镶�͂��i�[����z��
    protected override void Awake()
    {
        base.Awake(); // �f�o�b�O���O��\�����邩�ۂ��X�N���v�^�u���I�u�W�F�N�g��GameSettings���Q��
        CreditSetUp(); // �N���W�b�g���Z�b�g
        this.GetComponent<TextMeshProUGUI>().text = creditDates[0].creditText; // �e�L�X�g���Z�b�g
    }
    /// <summary>
    /// �N���W�b�g���Z�b�g����֐�
    /// </summary>
    private void CreditSetUp()
    {
        Debug.Log("Credit��ǂݍ��݂܂�");
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB
        //�@Resources.Load ����csv�t�@�C���̖��O�B����� Story1-1 �� Story2-5 �̂悤�ɃX�e�[�W�ԍ��ɂ���ēǂݍ��ރt�@�C�����ς�����悤�ɂ��Ă���B
        textasset = Resources.Load("Credit/Credit", typeof(TextAsset)) as TextAsset;

        /// CSVSerializer��p����csv�t�@�C����z��ɗ������ށB///
        creditDates = CSVSerializer.Deserialize<CreditData>(textasset.text); // CSV�̃e�L�X�g�f�[�^��z��Ɋi�[����
        /// �����܂� ///
        Debug.Log("Credit��ǂݍ��݂܂���");
    }
    // Update is called once per frame
    void Update()
    {
        //�@�G���h���[�����I��������
        if (isStopEndRoll)
        {
            endRollCoroutine = StartCoroutine(GoToNextScene());
        }
        else
        {
            //�@�G���h���[���p�e�L�X�g�����~�b�g���z����܂œ�����
            if (transform.position.y <= limitPosition)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + textScrollSpeed * Time.deltaTime, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, limitPosition, transform.position.z);
                isStopEndRoll = true;
            }
        }
    }

    IEnumerator GoToNextScene()
    {
        //�@5�b�ԑ҂�
        yield return new WaitForSeconds(5f);

        if (Input.GetKeyDown("space"))
        {
            StopCoroutine(endRollCoroutine);
            SceneManager.LoadScene("EndRollStartScene");
        }

        yield return null;
    }
}
[System.Serializable] // �T�u�v���p�e�B�𖄂ߍ���
public class CreditData // CreditData�̒���talkingChara��talks��z�u����
{
    public string creditText; // �N���W�b�g�e�L�X�g
}