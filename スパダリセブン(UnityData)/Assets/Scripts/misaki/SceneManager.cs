using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // �f�B�X�v���C�}�l�[�W���[�p�ϐ�
        [SerializeField] UIManager uiManager; // UI�}�l�[�W���[�p�ϐ�
        [SerializeField] AudioManager audioManager; // �I�[�f�B�I�}�l�[�W���[�ϐ�
        [SerializeField] SCENENAME currentSceneName; // ���݂̃V�[����
        [SerializeField] Scene currentScene; // ���݂̃V�[��
        public SCENENAME CheckSceneName { get { return currentSceneName; } } // ���݂̃V�[�������擾
        public Scene CheckScene { get { return currentScene; } } // ���݂̃V�[�����擾
        // �Z�[�u�f�[�^��ǂݍ���
        public JsonSettings<MasterData> saveData = new JsonSettings<MasterData>("SaveData", "/Resources/�v�����i�[�č��G���A/Json", "MasterData");


        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            audioManager.BGM_Play("BGM", saveData.m_tInstance.volumeBGM);
        }
        /// <summary>
        /// �C�x���g�n���h���[�@�V�[���J�ڎ��̊֐�
        /// </summary>
        /// <param name="nextScene"></param>
        /// <param name="mode"></param>
        void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            // ���݂̃V�[����������
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene(); 
            currentSceneName = (SCENENAME)currentScene.buildIndex;
            // ���݂̃V�[�����T���V�[���ł����
            if (currentSceneName == SCENENAME.Dungeon || currentSceneName == SCENENAME.InvestigationScene)
            {
                MapSetting setting = GameObject.FindGameObjectWithTag("MapSetting").GetComponent<MapSetting>(); // MapSetting������
                setting.CreateMap(1); // �}�b�v�𐶐�
                uiManager.OpenUI(UIType.Timer); // �^�C�}�[��\��
                uiManager.OpenUI(UIType.ItemWindow); // �A�C�e���E�B���h�E��\��
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

    }
}
