using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class OPMovieScript : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera = null;            // ����`��ꏊ
    [SerializeField]
    private TextMeshProUGUI stateText = null;    // ����X�e�[�^�X�\��
    [SerializeField]
    private Button button = null;                // ����p�{�^��
    [SerializeField]
    private TextMeshProUGUI buttonText = null;   // �{�^���e�L�X�g
    [SerializeField]
    private Button tutorialButton = null;        // �`���[�g���A���J�ڃ{�^��
    [SerializeField]
    private Button startButton = null;           // �o�g����ʑJ�ڃ{�^��
    [SerializeField]
    private Button CreditButton = null;          // �N���W�b�g��ʑJ�ڃ{�^��

    private Art.Sample.VideoPlay opVideo = null; // ����pnamespase
    public AudioSource AS_TitleBGM;              // BGM�I�[�f�B�I�\�[�X
    public AudioClip TitleBGM;                   // �^�C�g��BGM

    void Awake()
    {
        StartMovie(); // �X�^�[�g���[�r�[�֐��Ăяo��
    }

    public void OnClick() // ����p�{�^���֐�
    {
        if (buttonText.text == "Play")        //Play�Ȃ�
        {
            StartMovie();                     // �X�^�[�g���[�r�[�֐��Ăяo��
        }
        else if (buttonText.text == "Pause")  // Pause�Ȃ�
        {
            Pause();                          // �|�[�Y�֐��Ăяo��
        }
        else if (buttonText.text == "Resume") // Resume�Ȃ�
        {
            Resume();                         // ���W�����֐��Ăяo��
        }
    }

    private void StartMovie() // �X�^�[�g���[�r�[�֐�
    {
        stateText.text = "���[�h���ł��B"; // ���[�h���ł���\��
        button.interactable = false;       // �{�^�����쓮���Ȃ��悤�ɂ���

        // �C���X�^���X�̐���
        
        if (opVideo == null)
        {
            opVideo = Art.Sample.VideoPlay.Create(mainCamera);
        }

        // �C�x���g�ݒ�
        
        opVideo.OnPrepareCompleted = PrepareCompleted;
        opVideo.OnStarted = Started;
        opVideo.OnEnd = PlayEnd;
        opVideo.OnErrorReceived = ErrorReceived;

        opVideo.SetEnabled(true); // �����\��

        opVideo.Preload(Application.streamingAssetsPath + "/openingmovie.mp4"); // �w��̓����}��
    }

    private void Pause() // �|�[�Y�֐�
    {
        stateText.text = "Paused";
        buttonText.text = "Resume";
        
        opVideo.Pause(); // �|�[�Y�֐��Ăяo��
    }

    private void Resume() // ���W�����֐�
    {
        buttonText.text = "Pause";
        
        opVideo.Resume(); // ���W�����֐��Ăяo��
    }

    private void Started() // ����Đ����֐�
    {
        
          if (opVideo.State == Art.Sample.VideoPlay.PlayState.Playing) // ���悪�Đ����Ȃ�
          { 
            button.interactable = true;                                // �ꎞ��~�{�^�����쓮����悤�ɂ���
            buttonText.text = "Pause";
          }
        stateText.text = "Started";

        Debug.Log("Started");
    }

    private void PlayEnd() // �Đ��I���֐�
    {
        stateText.text = "PlayEnd";
        Debug.Log("PlayEnd");


        if (opVideo.State == Art.Sample.VideoPlay.PlayState.Loaded) // �����ǂݍ��ݒ��Ȃ�
        {
            buttonText.text = "Stop";

            button.interactable = false;                            // �{�^�����쓮���Ȃ��悤�ɂ���
            opVideo.PlayPrepared(false, true);
        }
        else if (opVideo.State == Art.Sample.VideoPlay.PlayState.Stoped) // ���悪�I����Ȃ�
        {
            buttonText.text = "Play";
            opVideo.SetEnabled(false);                                   // ������\���ɂ���
            startButton.interactable = true;                             // �o�g����ʑJ�ڃ{�^�����쓮����悤�ɂ���
            tutorialButton.interactable = true;                          // �`���[�g���A���J�ڃ{�^�����쓮����悤�ɂ���
            CreditButton.interactable = true;                            // �N���W�b�g��ʑJ�ڃ{�^�����쓮����悤�ɂ���
            button.interactable = true;                                  // �Đ��{�^�����쓮����悤�ɂ���
            AS_TitleBGM.PlayOneShot(TitleBGM);                           // �^�C�g��BGM�𗬂�
        }
    }

    private void ErrorReceived(string message) // �G���[�f�o�b�O�֐�
    {
        stateText.text = "ErrorReceived";
        Debug.LogError("ErrorReceived : " + message);
    }

    private void PrepareCompleted() // ���O�_�E�����[�h�f�o�b�O�֐�
    {
        stateText.text = "PrepareCompleted";
        Debug.Log("PrepareCompleted");

        
        if (opVideo.State == Art.Sample.VideoPlay.PlayState.Loaded)
        {
            opVideo.PlayPrepared(false, true);
        }
    }
}