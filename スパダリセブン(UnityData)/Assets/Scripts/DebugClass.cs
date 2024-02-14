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

    //デバッグ用
    private void Update() {
        //フェードイン(Esc)
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Ins.FadeIn(type, () => GameSystem.Log("完了 : " + Ins.CurrentFadeType));
        }
        //フェードアウト(Enter)
        else if (Input.GetKeyDown(KeyCode.Return)) {
            Ins.FadeOut(type, () => GameSystem.Log("完了 : " + Ins.CurrentFadeType));
        }
        else {
            //フェードタイプ変更(↑)
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                type = type == FadeType.Entire ? (FadeType)(GameSystem.GetEnumLength<FadeType>() - 1) : type - 1;
                GameSystem.Log("<color=white>フェードタイプ</color>を" + "<color=cyan>" + type + "</color>" + "に変更しました");
            }
            //フェードタイプ変更(↓)
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                int index = (int)type;
                type = index == GameSystem.GetEnumLength<FadeType>() - 1 ? FadeType.Entire : type + 1;

                GameSystem.Log("<color=white>フェードタイプ</color>を" + "<color=cyan>" + type + "</color>" + "に変更しました");
            }
            //明転後のα値変更(Z)
            else if (Input.GetKeyDown(KeyCode.Z)) {
                Ins.MaxAlpha = Random.Range(0.5f, 1.0f);
                Debug.Log("MaxAlpha(明転時の画面の明るさ)が変更されました : " + Ins.MaxAlpha);
            }
            //暗転後のα値変更(X)
            else if (Input.GetKeyDown(KeyCode.X)) {
                Ins.MinAlpha = Random.Range(0.0f, 0.4f);
                Debug.Log("MinAlpha(暗転時の画面の明るさ)が変更されました : " + Ins.MinAlpha);
            }
            //明るさを直接変更(C)
            else if (Input.GetKeyDown(KeyCode.C)) {
                float ram = Random.Range(0.0f, 1.0f);
                Ins.CurrentAlpha = ram;
                Debug.Log("画面の明るさを変更します　: " + ram);
            }
        }
    }
}
