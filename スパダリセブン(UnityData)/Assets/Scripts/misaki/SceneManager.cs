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

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
           // UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneUnLoaded;
        }
        void SceneUnLoaded(Scene scene)
        {
            displayManager.FadeIn(FadeType.Entire);
        }
        /// <summary>
        /// �V�[���J�ڂ��s���֐�
        /// </summary>
        /// <param name="LoadScene">�V�[���ԍ�</param>
        public void SceneChange(int LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire,()=> UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene)); // �t�F�[�h�A�E�g����
           // await Task.Delay((int)displayManager.FadeTime * 1000); // �Ó]����܂ő҂�(int�^�Ń~���b�P��)
           // UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene); // �w��̃V�[���ɑJ�ڂ���
        }
    }
}
