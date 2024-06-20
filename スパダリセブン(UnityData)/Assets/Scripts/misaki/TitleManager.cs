using UnityEngine;

public partial class TitleManager : DebugSetting
{
    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

    /// <summary>
    /// NewGame���N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void NewGame()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        sceneManager.uiManager.OpenUI(UIType.SaveSlot); // �Z�[�u�X���b�g���J��
        //sceneManager.SceneChange(SCENENAME.GameClearScene);
        Debug.Log("�j���[�Q�[�����J�n");
    }

    /// <summary>
    /// Continue���N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void Continue()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        sceneManager.uiManager.OpenUI(UIType.LoadSlot); // ���[�h�X���b�g���J��
        //sceneManager.SceneChange(SCENENAME.GameOverScene);
        Debug.Log("�R���e�B�j���[���J�n");
    }

    /// <summary>
    /// SettingGame�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void EnterSetting()
    {
        // �E�B���h�E���J���Ă���Ȃ�ēx�J���Ȃ��悤�Ƀ��^�[��
        if (openWindow)
        {
            Debug.Log("openWindow��" + openWindow + "�ł�");
            return;
        }
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        settingWindow.SetActive(true); // �E�B���h�E����������
        openWindow = true; // true�ɂ���
    }

    /// <summary>
    /// �E�B���h�E�����{�^�����������Ƃ��̊֐�
    /// </summary>
    public void QuitSetting()
    {
        // �E�B���h�E����Ă���Ȃ�ēx���Ȃ��悤�Ƀ��^�[��
        if (!openWindow)
        {
            Debug.Log("openWindow��" + openWindow + "�ł�");
            return;
        }
        sceneManager.enviromentalData.Save(); // �ύX�����ݒ��ۑ�����
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        settingWindow.SetActive(false); // �E�B���h�E��s���ɂ���
        openWindow = false; // false�ɂ���
    }

    /// <summary>
    /// �Q�[�����I������֐�
    /// </summary>
    public void EndGame()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity��ł̃v���C���I��
#else
        Application.Quit(); // �Q�[���A�v���P�[�V�����̏I��
#endif
    }

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///

    protected override void Awake()
    {
        base.Awake();
        // SceneManager��AudioManager��T��
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///



    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///
}
public partial class TitleManager
{
    /// --------�ϐ��ꗗ-------- ///

    #region public�ϐ�
    /// -------public�ϐ�------- ///

    public GameObject settingWindow; // �ݒ�̃E�B���h�E�ϐ�

    /// -------public�ϐ�------- ///
    #endregion

    #region protected�ϐ�
    /// -----protected�ϐ�------ ///



    /// -----protected�ϐ�------ ///
    #endregion

    #region private�ϐ�
    /// ------private�ϐ�------- ///

    private bool openWindow = false; // �E�B���h�E���J���Ă��邩�ǂ���

    private Supadari.SceneManager sceneManager; // �X�p�_���̃V�[���}�l�[�W���[�p�ϐ�

    private AudioManager audioManager; // �I�[�f�B�I�}�l�[�W���[�ϐ�

    /// ------private�ϐ�------- ///
    #endregion

    #region �v���p�e�B
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    #endregion

    /// --------�ϐ��ꗗ-------- ///
}