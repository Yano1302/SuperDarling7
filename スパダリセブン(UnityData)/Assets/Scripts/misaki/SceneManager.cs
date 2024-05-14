using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // �f�B�X�v���C�}�l�[�W���[�p�ϐ�
        [SerializeField] SCENENAME currentSceneName; // ���݂̃V�[����
        [SerializeField] Scene currentScene; // ���݂̃V�[��
        [SerializeField] Button autoButton; // �I�[�g�{�^���ϐ�
        public int saveSlot; // �Z�[�u�X���b�g�ϐ�
        public int stageNum = 0; // �X�e�[�W�ԍ�
        StoryController controller; // �X�g�[���[�R���g���[���[�ϐ�
        public UIManager uiManager; // UI�}�l�[�W���[�p�ϐ�
        public AudioManager audioManager; // �I�[�f�B�I�}�l�[�W���[�ϐ�
        public SCENENAME CheckSceneName { get { return currentSceneName; } } // ���݂̃V�[�������擾
        public Scene CheckScene { get { return currentScene; } } // ���݂̃V�[�����擾
        // �Z�[�u�f�[�^��ǂݍ���
        public JsonSettings<EnvironmentalData> enviromentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData(0)", "/Resources/�v�����i�[�č��G���A/Json", "EnvironmentalData");
        //public JsonSettings<EnvironmentalData> environmentalData = new JsonSettings<EnvironmentalData>("EnvironmentalData", "/Resources/�v�����i�[�č��G���A/Json");
        //public JsonSettings<MasterData> saveData = new JsonSettings<MasterData>("MasterData", "/Resources/�v�����i�[�č��G���A/Json");

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }
        // Start is called before the first frame update
        void Start()
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>(); // audioManager���������đ��
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            audioManager.BGM_Play("BGM_title", enviromentalData.TInstance.volumeBGM); // BGM�𗬂�
            //KeyDebug.AddKeyDebug("GameOver��ʂ֑J��", GameOver);
            //KeyDebug.AddKeyDebug("GameClear��ʂ֑J��", GameClear);
            //KeyDebug.AddKeyDebug("������ʂ֑J��", Investigation);
        }
        /// <summary>
        /// �C�x���g�n���h���[�@�V�[���J�ڎ��̊֐�
        /// </summary>
        /// <param name="nextScene"></param>
        /// <param name="mode"></param>
        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            JsonSettings<MasterData> saveData = new JsonSettings<MasterData>(string.Format("SaveData{0}", saveSlot), "/Resources/�v�����i�[�č��G���A/Json", "MasterData");
            // ���݂̃V�[����������
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene(); 
            currentSceneName = (SCENENAME)currentScene.buildIndex;
            // �X�g�[���[�V�[���ł���΃X�g�[���[���j���[���o��
            if (currentSceneName == SCENENAME.StoryScene) uiManager.OpenUI(UIType.StoryMenu);
            else uiManager.CloseUI(UIType.StoryMenu);
            // ���݂̃V�[�����T���V�[���ł����
            if (currentSceneName == SCENENAME.Dungeon || currentSceneName == SCENENAME.InvestigationScene) {
                MapSetting setting = MapSetting.Instance; // MapSetting�̃C���X�^���X�擾�@�����ύX
                setting.CreateMap(stageNum); // �}�b�v�𐶐�
            }
            else if (currentSceneName != SCENENAME.SolveScene) 
            {
                TimerManager.Instance.CloseTimer(true);   // �^�C�}�[�����@�����ǋL
                uiManager.CloseUI(UIType.ItemWindow); // �A�C�e���E�B���h�E�����
            } 
            // �e�V�[���ł�BGM�𗬂� �X�g�[���[�V�[����CSV�f�[�^���Q�Ƃ��ė����̂ł����ł͗����Ȃ�
            switch(currentSceneName)
            {
                case SCENENAME.TitleScene:
                    audioManager.BGM_Play("BGM_title", enviromentalData.TInstance.volumeBGM);
                    break;
                case SCENENAME.RequisitionsScene:
                    audioManager.BGM_Play("BGM_quest", enviromentalData.TInstance.volumeBGM);
                    saveData.TInstance.scenename = currentSceneName; // ���݂̃V�[������
                    saveData.Save(); // �Z�[�u����
                    break;
                case SCENENAME.StoryScene:
                    audioManager.BGM_Stop();
                    controller = GameObject.FindGameObjectWithTag("Coroutine").GetComponent<StoryController>();
                    autoButton.onClick.AddListener(controller.OnAutoModeCllicked);
                    break;
                case SCENENAME.InvestigationScene:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.TInstance.volumeBGM);
                    break;
                case SCENENAME.SolveScene:
                    audioManager.BGM_Play("BGM_solve", enviromentalData.TInstance.volumeBGM);
                    break;
                case SCENENAME.Dungeon:
                    audioManager.BGM_Play("BGM_dungeon", enviromentalData.TInstance.volumeBGM);
                    break;
                case SCENENAME.GameOverScene:
                    audioManager.BGM_Stop();
                    audioManager.SE_Play("BGM_gameover", enviromentalData.TInstance.volumeSE);
                    break;
                case SCENENAME.GameClearScene:
                    audioManager.BGM_Stop();
                    audioManager.SE_Play("BGM_clear", enviromentalData.TInstance.volumeSE);
                    break;
            }
        }
        /// <summary>
        /// �V�[���J�ڂ��s���֐�
        /// </summary>
        /// <param name="LoadScene">�V�[���ԍ�</param>
        public void SceneChange(int LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // �t�F�[�h�A�E�g����
        }
        /// <summary>
        /// �V�[���J�ڂ��s���֐�
        /// </summary>
        /// <param name="LoadScene">�V�[����</param>
        public void SceneChange(string LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // �t�F�[�h�A�E�g����
        }
        /// <summary>
        /// �V�[���J�ڂ��s���֐�
        /// </summary>
        /// <param name="LoadScene">�V�[����</param>
        public void SceneChange(SCENENAME LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene((int)LoadScene)); // �t�F�[�h�A�E�g����
        }
        void GameOver()
        {
            SceneChange(SCENENAME.GameOverScene);
        }
        void GameClear()
        {
            SceneChange(SCENENAME.GameClearScene);
        }
        void Investigation()
        {
            SceneChange(SCENENAME.Dungeon);
        }
    }
}
