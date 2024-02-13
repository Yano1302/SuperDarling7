using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugClass : SingletonMonoBehaviour<DebugClass>
{
    private Display Ins;
    private FadeType type;
    private int sceneNumber = 0;

    private void Start() {
        Ins = Display.Instance;
        type = 0;
    }

    //�f�o�b�O�p
    private void Update() {
        //�t�F�[�h�C��(Esc)
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Ins.FadeIn(type, () => GameSystem.Log("���� : " + Ins.CurrentFadeType));
        }
        //�t�F�[�h�A�E�g(Enter)
        else if (Input.GetKeyDown(KeyCode.Return)) {
            Ins.FadeOut(type, () => GameSystem.Log("���� : " + Ins.CurrentFadeType));
        }
        else {
            //�t�F�[�h�^�C�v�ύX(��)
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                type = type == FadeType.Entire ? (FadeType)(GameSystem.GetEnumLength<FadeType>() - 1) : type - 1;
                GameSystem.Log("<color=white>�t�F�[�h�^�C�v</color>��" + "<color=cyan>" + type + "</color>" + "�ɕύX���܂���");
            }
            //�t�F�[�h�^�C�v�ύX(��)
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                int index = (int)type;
                type = index == GameSystem.GetEnumLength<FadeType>() - 1 ? FadeType.Entire : type + 1;

                GameSystem.Log("<color=white>�t�F�[�h�^�C�v</color>��" + "<color=cyan>" + type + "</color>" + "�ɕύX���܂���");
            }
            //���]��̃��l�ύX(Z)
            else if (Input.GetKeyDown(KeyCode.Z)) {
                Ins.MaxAlpha = Random.Range(0.5f, 1.0f);
                Debug.Log("MaxAlpha(���]���̉�ʂ̖��邳)���ύX����܂��� : " + Ins.MaxAlpha);
            }
            //�Ó]��̃��l�ύX(X)
            else if (Input.GetKeyDown(KeyCode.X)) {
                Ins.MinAlpha = Random.Range(0.0f, 0.4f);
                Debug.Log("MinAlpha(�Ó]���̉�ʂ̖��邳)���ύX����܂��� : " + Ins.MinAlpha);
            }
            //���邳�𒼐ڕύX(C)
            else if (Input.GetKeyDown(KeyCode.C)) {
                float ram = Random.Range(0.0f, 1.0f);
                Ins.CurrentAlpha = ram;
                Debug.Log("��ʂ̖��邳��ύX���܂��@: " + ram);
            }
            //�^�C���X�P�[���ύX(V)
            else if (Input.GetKeyDown(KeyCode.V)) {
                float t = Random.Range(0.0f, 3.0f);
                Time.timeScale = t;
                GameSystem.Log("�^�C���X�P�[����" + t + "�ɕύX����܂�");
            }
            //�I�[�f�B�I��炷�@B
            else if (Input.GetKeyDown(KeyCode.B)) {
                Debug.Log("SE��炵�܂�");
                AudioManager.Instance.PlaySound("SE",0.5f,false,action: () => { Debug.Log("�f�o�b�O���O���b�Z�[�W�ł�"); });
            }
            //BGM��炷�@N
            else if (Input.GetKeyDown(KeyCode.N)) {
                AudioManager.Instance.BGM_Play("BGM");
            }
            //�I�[�f�B�I�ɊԈ��������n�� M
            else if (Input.GetKeyDown(KeyCode.M)) {
                AudioManager.Instance.PlaySound("��������������������������������");
            }
            //�I�[�f�B�I��PlayBGM��SE�𗬂� A
            else if (Input.GetKeyDown(KeyCode.A)) {
                AudioManager.Instance.BGM_Play("SE");
            }
            //�I�[�f�B�I��PlaySound��BGM�𗬂� S
            else if (Input.GetKeyDown(KeyCode.S)) {
                AudioManager.Instance.PlaySound("BGM");
            }
            //BGM���t�F�[�h�C���Đ����� D
            else if (Input.GetKeyDown(KeyCode.D)) {
                float s = Random.Range(1.0f, 10.0f);
                Debug.Log(s + "�b�Ԃ����t�F�[�h�Đ����܂�");
                AudioManager.Instance.BGM_Play_Fadein("BGM", s, action: () => { Debug.Log("�f�o�b�O���O���b�Z�[�W�ł�"); });
            }
            //BGM���~����@F
            else if (Input.GetKeyDown(KeyCode.F)) {
                AudioManager.Instance.BGM_Stop();
            }
            //BGM���t�F�[�h��~����@G
            else if (Input.GetKeyDown(KeyCode.G)) {
                float s = Random.Range(1.0f, 5.0f);
                AudioManager.Instance.BGM_Stop_FadeOut(action: () => { Debug.Log("�f�o�b�O���O���b�Z�[�W�ł�"); });
            }
            //�a�f�l�̉��ʂ�ύX���� H
            else if (Input.GetKeyDown(KeyCode.H)) {
                float volume = Random.Range(0.1f, 1.0f);
                Debug.Log("BGM�̉��ʂ�" + volume + "�ɕύX����܂�");
                AudioManager.Instance.BGM_Volume = volume;
            }
            //�a�f�l�̉��ʂ����X�ɕύX���� J
            else if (Input.GetKeyDown(KeyCode.J)) {
                float volume = Random.Range(0.1f, 1.0f);
                Debug.Log("BGM�̉��ʂ�" + volume + "�ɕύX����܂�");
                AudioManager.Instance.BGM_SetVolume_Fade(volume);
            }
            //���Ă��鉹��S�Ē�~������ K
            else if (Input.GetKeyDown(KeyCode.K)) {
                AudioManager.Instance.StopAllSound(true);
            }
            //BGM���|�[�Y L
            else if (Input.GetKeyDown(KeyCode.L)) {
                AudioManager.Instance.BGM_Pause();
            }
            //�|�[�Y����Ă���BGM���ĊJ�@Q
            else if (Input.GetKeyDown(KeyCode.Q)) {
                AudioManager.Instance.BGM_Restert();
            }
            //�S�Ẳ������|�[�Y�@W
            else if (Input.GetKeyDown(KeyCode.W)) {
                AudioManager.Instance.PauseAllSound(true);
            }
            //�S�Ẳ������|�[�Y����Đ��@E
            else if (Input.GetKeyDown(KeyCode.E)) {
                AudioManager.Instance.PauseAllSound(true);
            }
        }
    }
}
