using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : SingletonMonoBehaviour <MenuScript>
{
    private bool openWindow = false; // �E�B���h�E���J���Ă��邩�ǂ���
    [SerializeField] GameObject menuWindow; // ���j���[�E�B���h�E�p�ϐ�
    [SerializeField] Supadari.SceneManager sceneManager; // �X�p�_���̃V�[���}�l�[�W���[�p�ϐ�
    private StoryController storyController = null; // �X�g�[���[�R���g���[���[�p�ϐ�
    protected override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// Menu�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void EnterMenu()
    {
        // �E�B���h�E���J���Ă���Ȃ�ēx�J���Ȃ��悤�Ƀ��^�[��
        if (openWindow)
        {
            Debug.Log("openWindow��" + openWindow + "�ł�");
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
            Debug.Log("openWindow��" + openWindow + "�ł�");
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
    /// <param name="saveSlotIndex">�Z�[�u�X���b�g�̔ԍ�</param>
    public void Save(int saveSlotIndex)
    {
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}",saveSlotIndex), "/Resources/�v�����i�[�č��G���A/Json", "MasterData");
        // ���݂̃V�[����ۑ�
        saveData.m_tInstance.scenename = sceneManager.CheckSceneName;
        // �Z�[�u�������Ƃ������true�ɂ���
        saveData.m_tInstance.haveSaved = true;
        saveData.Save();
    }
    /// <summary>
    /// ���[�h�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    /// <param name="saveSlotIndex">�Z�[�u�X���b�g�̔ԍ�</param>
    public void Load(int saveSlotIndex)
    {
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/�v�����i�[�č��G���A/Json", "MasterData");
        if (saveData.m_tInstance.haveSaved == false)
        {
            Debug.Log("�Z�[�u����Ă��܂���");
            return; // ��x���Z�[�u���ꂽ���Ƃ��Ȃ��̂Ȃ烊�^�[��
        }
        // ���[�h���ăV�[���J��
        sceneManager.SceneChange(saveData.m_tInstance.scenename);
    }
    /// <summary>
    /// �^�C�g���֖߂�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void BackTitle()
    {
        sceneManager.SceneChange(0); // �^�C�g���V�[���֑J�ڂ���
        Resume();
    }
}
