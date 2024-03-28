using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : SingletonMonoBehaviour <MenuScript>
{
    private bool openWindow = false; // �E�B���h�E���J���Ă��邩�ǂ���
    [SerializeField] GameObject menuButton; // ���j���[�{�^���p�ϐ�
    [SerializeField] GameObject menuWindow; // ���j���[�E�B���h�E�p�ϐ�
    [SerializeField] Supadari.SceneManager sceneManager; // �X�p�_���̃V�[���}�l�[�W���[�p�ϐ�
    private StoryController storyController = null; // �X�g�[���[�R���g���[���[�p�ϐ�
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneCheck(); // �V�[�����`�F�b�N���\������\���������߂�
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneCheck(); // �V�[�����`�F�b�N���\������\���������߂�
    }
    /// <summary>
    /// �V�[�����`�F�b�N��A�\������\���ɂ���֐�
    /// </summary>
    private void SceneCheck()
    {
        Scene scene = SceneManager.GetActiveScene(); // ���݂̃V�[������
        storyController = null; // null���������Z�b�g
        if (openWindow) Resume(); // ���j���[�E�B���h�E���J���Ă���Ε���
        if (scene.buildIndex == 0) menuButton.SetActive(false); // �^�C�g���V�[�����̂ݔ�\��
        else menuButton.SetActive(true); // ����ȊO�̏ꍇ�\��
        // �˗��V�[���̏ꍇ�͑������
        if (scene.buildIndex == 1) { }
        // �X�g�[���[�V�[���̏ꍇ�͑������
        else if (scene.buildIndex == 2) storyController = GameObject.FindWithTag("Coroutine").GetComponent<StoryController>();
    }
    /// <summary>
    /// Menu�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void EnterMenu()
    {
        // �E�B���h�E���J���Ă���Ȃ�ēx�J���Ȃ��悤�Ƀ��^�[��
        if (openWindow)
        {
            UnityEngine.Debug.Log("openWindow��" + openWindow + "�ł�");
            return;
        }
        menuWindow.SetActive(true); // �E�B���h�E����������
        openWindow = true; // true�ɂ���
        Time.timeScale = 0; // �^�C���X�P�[����0�ɂ���FixedUpdate���~�߂�
        if (storyController) storyController.PauseDialogueCoroutine(); // storyController�̃R���[�`�����ꎞ��~
    }
    /// <summary>
    /// �Q�[���ĊJ�{�^�����������Ƃ��̊֐�
    /// </summary>
    public void Resume()
    {
        // �E�B���h�E����Ă���Ȃ�ēx���Ȃ��悤�Ƀ��^�[��
        if (!openWindow)
        {
            UnityEngine.Debug.Log("openWindow��" + openWindow + "�ł�");
            return;
        }
        menuWindow.SetActive(false); // �E�B���h�E��s���ɂ���
        openWindow = false; // false�ɂ���
        Time.timeScale = 1; // �^�C���X�P�[����0�ɂ���FixedUpdate���~�߂�
        if (storyController) storyController.ResumeDialogueCoroutine(); // storyController�̃R���[�`�����ĊJ
    }
    /// <summary>
    /// �Z�[�u�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void Save()
    {
        UnityEngine.Debug.Log("�Z�[�u�@�\�͂��Ƃō��܂�");
    }
    /// <summary>
    /// ���[�h�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void Load()
    {
        UnityEngine.Debug.Log("���[�h�@�\�͂��Ƃō��܂�");
    }
    /// <summary>
    /// �^�C�g���֖߂�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void BackTitle()
    {
        sceneManager.SceneChange(0); // �^�C�g���V�[���֑J�ڂ���
    }
}
