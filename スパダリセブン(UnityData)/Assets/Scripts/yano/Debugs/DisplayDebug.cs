
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class DisplayDebug : SingletonMonoBehaviour<DisplayDebug>
{
    private DisplayManager Ins;
    [SerializeField]
    private FadeType type;

    private void Start() {
        Ins = DisplayManager.Instance;
        type = 0;
    }

    //デバッグ用
    private void Update() {
        DA(KeyCode.Return, () => Ins.FadeIn(type));
        DA(KeyCode.Backspace, () => Ins.FadeOut(type));


        DA(KeyCode.UpArrow, () => ChangeType(true));
        DA(KeyCode.DownArrow, () => ChangeType(false));

        DA(KeyCode.Z, () => { float f = Random.Range(0.0f, 1.0f); Log("ランダム値 : "+f); Ins.CurrentAlpha = f;});
        
    }
      
      
    
         

    void DA(KeyCode code, UnityAction action) {
        if (Input.GetKeyDown(code)) {
            action();
        }
    }

    void ChangeType(bool Up) {
        if (Up) {
            type = type == FadeType.Entire ? (FadeType)(UsefulSystem.GetEnumLength<FadeType>() - 1) : type - 1;
            UsefulSystem.Log("<color=white>フェードタイプ</color>を" + "<color=cyan>" + type + "</color>" + "に変更しました");
        }
        else {
            int index = (int)type;
            type = index == UsefulSystem.GetEnumLength<FadeType>() - 1 ? FadeType.Entire : type + 1;

            UsefulSystem.Log("<color=white>フェードタイプ</color>を" + "<color=cyan>" + type + "</color>" + "に変更しました");
        }
    }

    void Log(object o) {Debug.Log(o);}
}

