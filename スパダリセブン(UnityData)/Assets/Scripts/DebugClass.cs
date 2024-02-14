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
        }
    }
}
