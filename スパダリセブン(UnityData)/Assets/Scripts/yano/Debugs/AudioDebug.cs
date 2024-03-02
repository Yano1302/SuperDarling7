using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AudioDebug :SingletonMonoBehaviour<AudioDebug>
{
    AudioManager ins;
    int ID;
    void Start()
    {
        ins = AudioManager.Instance;
    }
    void Update() {
        DA(KeyCode.Backspace, () => ins.CanPlayFlag = false);
        DA(KeyCode.Return, () => ins.CanPlayFlag = true);

        DA(KeyCode.Z, () => ins.BGM_Play("BGM"));
        DA(KeyCode.X, () => ins.BGM_PlayFade("BGM", action: () => UsefulSystem.LogError("関数を実行します")));

        DA(KeyCode.C, () => ins.BGM_Pause());
        DA(KeyCode.V, () => ins.BGM_Restert());

        DA(KeyCode.B, () => ins.BGM_Stop());
        DA(KeyCode.N, () => ins.BGM_StopFade(action: () => UsefulSystem.LogError("関数を実行します")));


        DA(KeyCode.M, () => ins.BGM_Volume = 0.1f);
        DA(KeyCode.A, () => ins.BGM_Volume = 1.0f);
        DA(KeyCode.S, () => ins.BGM_FadeVolume(0.1f, action: () => UsefulSystem.LogError("関数を実行します")));
        DA(KeyCode.D, () => ins.BGM_FadeVolume(1.0f, action: () => UsefulSystem.LogError("関数を実行します")));


        DA(KeyCode.F, () => ID = ins.SE_Play("BGM"));
        DA(KeyCode.G, () => ID = ins.SE_Play("BGM", IsLoop: true));
        DA(KeyCode.H, () => ID = ins.SE_PlayFade("BGM"));
        DA(KeyCode.J, () => ins.SE_Stop("BGM"));
        DA(KeyCode.K, () => ins.SE_Stop(ID));
        DA(KeyCode.L, () => ins.SE_StopInLoop());
        DA(KeyCode.Q, () => ins.SE_StopNotLoop());
        DA(KeyCode.W, () => ins.SE_SetVolume("BGM", 1.0f));
        DA(KeyCode.E, () => ins.SE_SetVolume(ID, 1.0f));
        DA(KeyCode.R, () => ins.SE_SetVolumeFade("BGM", 1.0f, action: () => UsefulSystem.LogError("関数を実行します")));
        DA(KeyCode.T, () => ins.SE_SetVolumeFade(ID, 1.0f, action: () => UsefulSystem.LogError("関数を実行します")));

        DA(KeyCode.Y, () => ins.SE_StopFade("BGM", action: () => UsefulSystem.LogError("関数を実行します")));
        DA(KeyCode.U, () => ins.SE_StopFade(ID, action: () => UsefulSystem.LogError("関数を実行します")));

        DA(KeyCode.I, () => ins.ALL_SetVolume(1.0f, true));
        DA(KeyCode.O, () => ins.ALL_FadeVolume(0.1f, true));

        DA(KeyCode.P, () => ins.ALL_Pause(true));
        DA(KeyCode.Alpha1, () => ins.ALL_Restert(true));

        DA(KeyCode.Alpha2, () => ins.SE_Pause("BGM"));
        DA(KeyCode.Alpha3, () => ins.SE_Pause(ID));
        DA(KeyCode.Alpha4, () => ins.SE_Restert("BGM"));
        DA(KeyCode.Alpha5, () => ins.SE_Restert(ID));
        DA(KeyCode.Alpha6, () => ins.ALL_StopFade(true, action: () => UsefulSystem.LogError("関数を実行します")));

        DA(KeyCode.UpArrow, () => ins.DivisionScale += 1);
        DA(KeyCode.DownArrow, () => ins.DivisionScale -= 1);

       // DA(KeyCode.LeftArrow, () => ins.SetSpeed -= 1);
       // DA(KeyCode.RightArrow, () => ins.SetSpeed += 1);
    }


    void DA(KeyCode code,UnityAction action) {
        if (Input.GetKeyDown(code)) {
            action();
        }
    }

}
