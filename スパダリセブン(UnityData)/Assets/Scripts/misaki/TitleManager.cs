using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : DebugSetting
{
    private bool openWindow = false; // �E�B���h�E���J���Ă��邩�ǂ���
    private Supadari.SceneManager sceneManager; // �X�p�_���̃V�[���}�l�[�W���[�p�ϐ�
    public GameObject settingWindow; // �ݒ�̃E�B���h�E�ϐ�
    
    protected override void Awake()
    {
        base.Awake();
        // sceneManager��T��
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    /// <summary>
    /// NewGame���N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void NewGame()
    {
        sceneManager.SceneChange(1); // �˗���ʂփV�[���J�ڂ���
        Debug.Log("�j���[�Q�[�����J�n");
    }
    /// <summary>
    /// Continue���N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void Continue()
    {
        sceneManager.SceneChange(2); // �Z�[�u�����V�[���֑J�ڂ���@���������Ƃŕς���
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
        settingWindow.SetActive(false); // �E�B���h�E��s���ɂ���
        openWindow = false; // false�ɂ���
    }
    /// <summary>
    /// �Q�[�����I������֐�
    /// </summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity��ł̃v���C���I��
#else
        Application.Quit(); // �Q�[���A�v���P�[�V�����̏I��
#endif
    }
}
