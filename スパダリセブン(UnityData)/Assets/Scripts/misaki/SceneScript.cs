using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
// ������̃V�[���`�F���W+�t�F�[�h�C���A�E�g���s���X�N���v�g
// maincamera�ȂǏ�ɂ�����̂ɃA�^�b�`���邱�Ɛ���
// �X�N���v�g�uFadeManager�v�Ƌ��Ɏg�p����

/*public class SceneScript : SingletonMonoBehaviour<SceneScript>
{
    [SerializeField] GameObject fadeCanvas; //prefab��FadeCanvas������
    public AudioSource SEAudioSource; // SE�p�I�[�f�B�I�\�[�X

    protected override void Awake()
    {
        base.Awake(); // �f�o�b�O���O��\�����邩�ۂ��X�N���v�^�u���I�u�W�F�N�g��GameSettings���Q��
        if (!FadeManager.isFadeInstance)  // �t�F�[�h�pCanvas�������ł��Ă��Ȃ����
        {
            Instantiate(fadeCanvas);     // Canvas����
        }
        Invoke("findFadeObject", 0.02f); // �N�����p��Canvas�̏�����������Ƒ҂�
    }
    void findFadeObject()                                      // ��������Canvas�̃t�F�[�h�C���t���O�𗧂Ă�֐�
    {
        fadeCanvas = GameObject.FindGameObjectWithTag("Fade"); // FadeCanvas��������
        fadeCanvas.GetComponent<FadeManager>().fadeIn();       // �t�F�[�h�C���֐����Ăяo��
    }
    public async void sceneChange(string sceneName)       // �V�[���`�F���W�֐��@�{�^������ȂǂŌĂяo��
    {
        if (SEAudioSource) SEAudioSource.Play(); // �I��SE��炷
        await Task.Delay(0);                           // ������܂ő҂�
        fadeCanvas.GetComponent<FadeManager>().fadeOut(); // �t�F�[�h�A�E�g�֐����Ăяo��
        await Task.Delay(2000);                           // �Ó]����܂ő҂�
        SceneManager.LoadScene(sceneName);                // �V�[���`�F���W
    }
}
*/
