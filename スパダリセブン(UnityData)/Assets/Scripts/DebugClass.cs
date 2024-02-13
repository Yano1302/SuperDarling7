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
            //タイムスケール変更(V)
            else if (Input.GetKeyDown(KeyCode.V)) {
                float t = Random.Range(0.0f, 3.0f);
                Time.timeScale = t;
                GameSystem.Log("タイムスケールが" + t + "に変更されます");
            }
            //オーディオを鳴らす　B
            else if (Input.GetKeyDown(KeyCode.B)) {
                Debug.Log("SEを鳴らします");
                AudioManager.Instance.PlaySound("SE",0.5f,false,action: () => { Debug.Log("デバッグログメッセージです"); });
            }
            //BGMを鳴らす　N
            else if (Input.GetKeyDown(KeyCode.N)) {
                AudioManager.Instance.BGM_Play("BGM");
            }
            //オーディオに間違った情報を渡す M
            else if (Input.GetKeyDown(KeyCode.M)) {
                AudioManager.Instance.PlaySound("ああああああああああああああああ");
            }
            //オーディオのPlayBGMでSEを流す A
            else if (Input.GetKeyDown(KeyCode.A)) {
                AudioManager.Instance.BGM_Play("SE");
            }
            //オーディオのPlaySoundでBGMを流す S
            else if (Input.GetKeyDown(KeyCode.S)) {
                AudioManager.Instance.PlaySound("BGM");
            }
            //BGMをフェードイン再生する D
            else if (Input.GetKeyDown(KeyCode.D)) {
                float s = Random.Range(1.0f, 10.0f);
                Debug.Log(s + "秒間かけフェード再生します");
                AudioManager.Instance.BGM_Play_Fadein("BGM", s, action: () => { Debug.Log("デバッグログメッセージです"); });
            }
            //BGMを停止する　F
            else if (Input.GetKeyDown(KeyCode.F)) {
                AudioManager.Instance.BGM_Stop();
            }
            //BGMをフェード停止する　G
            else if (Input.GetKeyDown(KeyCode.G)) {
                float s = Random.Range(1.0f, 5.0f);
                AudioManager.Instance.BGM_Stop_FadeOut(action: () => { Debug.Log("デバッグログメッセージです"); });
            }
            //ＢＧＭの音量を変更する H
            else if (Input.GetKeyDown(KeyCode.H)) {
                float volume = Random.Range(0.1f, 1.0f);
                Debug.Log("BGMの音量が" + volume + "に変更されます");
                AudioManager.Instance.BGM_Volume = volume;
            }
            //ＢＧＭの音量を徐々に変更する J
            else if (Input.GetKeyDown(KeyCode.J)) {
                float volume = Random.Range(0.1f, 1.0f);
                Debug.Log("BGMの音量が" + volume + "に変更されます");
                AudioManager.Instance.BGM_SetVolume_Fade(volume);
            }
            //鳴っている音を全て停止させる K
            else if (Input.GetKeyDown(KeyCode.K)) {
                AudioManager.Instance.StopAllSound(true);
            }
            //BGMをポーズ L
            else if (Input.GetKeyDown(KeyCode.L)) {
                AudioManager.Instance.BGM_Pause();
            }
            //ポーズされているBGMを再開　Q
            else if (Input.GetKeyDown(KeyCode.Q)) {
                AudioManager.Instance.BGM_Restert();
            }
            //全ての音源をポーズ　W
            else if (Input.GetKeyDown(KeyCode.W)) {
                AudioManager.Instance.PauseAllSound(true);
            }
            //全ての音源をポーズから再生　E
            else if (Input.GetKeyDown(KeyCode.E)) {
                AudioManager.Instance.PauseAllSound(true);
            }
        }
    }
}
