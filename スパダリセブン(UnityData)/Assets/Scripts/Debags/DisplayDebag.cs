using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class DisplayDebag : SingletonMonoBehaviour<DisplayDebag>
{
    private Display Ins;
    private FadeType type;

    private void Start() {
        Ins = Display.Instance;
        type = 0;
    }

    //デバッグ用
    private void Update() {
        DA(KeyCode.Return, () => Ins.FadeIn(type));
        DA(KeyCode.Backspace, () => Ins.FadeOut(type));


        DA(KeyCode.UpArrow, () => ChangeType(true));
        DA(KeyCode.DownArrow, () => ChangeType(false));


        DA(KeyCode.Z, () => SetAfterAlpha(true));
        DA(KeyCode.X, () => SetAfterAlpha(false));
        DA(KeyCode.C, () => SetAlpha());
    }
      
      
    
         

    void DA(KeyCode code, UnityAction action) {
        if (Input.GetKey(code)) {
            action();
        }
    }


    void ChangeType(bool Up) {
        if (Up) {
            type = type == FadeType.Entire ? (FadeType)(GameSystem.GetEnumLength<FadeType>() - 1) : type - 1;
            GameSystem.Log("<color=white>フェードタイプ</color>を" + "<color=cyan>" + type + "</color>" + "に変更しました");
        }
        else {
            int index = (int)type;
            type = index == GameSystem.GetEnumLength<FadeType>() - 1 ? FadeType.Entire : type + 1;

            GameSystem.Log("<color=white>フェードタイプ</color>を" + "<color=cyan>" + type + "</color>" + "に変更しました");
        }
    }

    void SetAfterAlpha(bool fadeIn) {
        if (fadeIn) {
            Ins.MaxAlpha = Random.Range(0.5f, 1.0f);
            Debug.Log("MaxAlpha(明転時の画面の明るさ)が変更されました : " + Ins.MaxAlpha);
        }
        else {
            Ins.MinAlpha = Random.Range(0.0f, 0.4f);
            Debug.Log("MinAlpha(暗転時の画面の明るさ)が変更されました : " + Ins.MinAlpha);
        }
    }

    void SetAlpha() {
        float ram = Random.Range(0.0f, 1.0f);
        Ins.CurrentAlpha = ram;
        Debug.Log("画面の明るさを変更します　: " + ram);
    }
}

