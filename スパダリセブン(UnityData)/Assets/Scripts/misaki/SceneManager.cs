using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour <SceneManager>
    {
        [SerializeField] DisplayManager displayManager; // �f�B�X�v���C�}�l�[�W���[�p�ϐ�
        [SerializeField] UIManager uiManager; // UI�}�l�[�W���[�p�ϐ�
        [SerializeField] EnumList.SCENENAME currentSceneName; // ���݂̃V�[����
        [SerializeField] Scene currentScene; // ���݂̃V�[��
        public EnumList.SCENENAME CheckSceneName { get { return currentSceneName; } }
        public Scene CheckScene { get { return currentScene; } }

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
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
            currentSceneName = (EnumList.SCENENAME)currentScene.buildIndex;
            // ���݂̃V�[�����T���V�[���ł����
            if (currentSceneName == EnumList.SCENENAME.Dungeon || currentSceneName == EnumList.SCENENAME.InvestigationScene)
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
        public void SceneChange(EnumList.SCENENAME LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene((int)LoadScene)); // �t�F�[�h�A�E�g����
        }

    }
}
