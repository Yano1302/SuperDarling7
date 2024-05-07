using UnityEngine;

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
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE);
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
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/�v�����i�[�č��G���A/Json", "MasterData");
        // ���݂̃V�[����ۑ�
        saveData.TInstance.scenename = sceneManager.CheckSceneName;
        // �Z�[�u�������Ƃ������true�ɂ���
        if (saveData.TInstance.scenename != SCENENAME.TitleScene) saveData.TInstance.haveSaved = true;
        saveData.Save(); // �Z�[�u����
        Debug.Log("�Z�[�u���܂�");
        // �^�C�g���V�[���̏ꍇ
        if (saveData.TInstance.scenename == SCENENAME.TitleScene)
        {
            sceneManager.saveSlot = saveSlotIndex; // ���ݎg�p���Ă���X���b�g����
            sceneManager.SceneChange(SCENENAME.StoryScene); // �V�[���J��
            sceneManager.uiManager.CloseUI(UIType.SaveSlot); // UI�����
        }
    }
    /// <summary>
    /// ���[�h�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    /// <param name="saveSlotIndex">�Z�[�u�X���b�g�̔ԍ�</param>
    public void Load(int saveSlotIndex)
    {
        JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlotIndex), "/Resources/�v�����i�[�č��G���A/Json", "MasterData");
        if (saveData.TInstance.haveSaved == false)
        {
            sceneManager.audioManager.SE_Play("SE_dungeon05");
            Debug.Log("�Z�[�u����Ă��܂���");
            return; // ��x���Z�[�u���ꂽ���Ƃ��Ȃ��̂Ȃ烊�^�[��
        }
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
        // ���[�h���ăV�[���J��
        sceneManager.SceneChange(saveData.TInstance.scenename);
        sceneManager.uiManager.CloseUI(UIType.LoadSlot);
    }
    /// <summary>
    /// �^�C�g���֖߂�{�^�����N���b�N�����Ƃ��̊֐�
    /// </summary>
    public void BackTitle()
    {
        sceneManager.audioManager.SE_Play("SE_dungeon05", sceneManager.enviromentalData.TInstance.volumeSE);
        sceneManager.SceneChange(0); // �^�C�g���V�[���֑J�ڂ���
        Resume();
    }
    public void ClickSE()
    {
        sceneManager.audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE);
    }
}
