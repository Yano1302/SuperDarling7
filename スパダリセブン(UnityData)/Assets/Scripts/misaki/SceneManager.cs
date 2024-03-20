using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Supadari
{
    public class SceneManager : SingletonMonoBehaviour <SceneManager>
    {
        public DisplayManager displayManager; // �f�B�X�v���C�}�l�[�W���[�p�ϐ�

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneUnLoaded;
        }

        // Update is called once per frame
        void Update()
        {
        }
        public async void SceneChange(int LoadScene)
        {
            displayManager.FadeOut(FadeType.Entire); // �t�F�[�h�A�E�g����
            await Task.Delay((int)displayManager.FadeTime * 1000); // �Ó]����܂ő҂�(int�^�Ń~���b�P��)
            UnityEngine.SceneManagement.SceneManager.LoadScene(LoadScene); // �w��̃V�[���ɑJ�ڂ���
        }
        void SceneUnLoaded(Scene scene)
        {
            displayManager.FadeIn(FadeType.Entire);
        }
    }
}
