using Supadari;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingScript : MonoBehaviour
{
    SceneManager sceneManager; // シーンマネージャー変数
    [SerializeField]TestTextController testTextController; // テストテキストコントローラー変数
    [EnumIndex(typeof(SETTINGSTATE))]
    [SerializeField] Slider[] slider = new Slider[3]; // スライダー配列
    void Awake()
    {
        // シーンマネージャーを取得
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
        // 各スライダーにjsonの値を代入する
        slider[0].value = sceneManager.enviromentalData.TInstance.volumeBGM;
        slider[1].value = sceneManager.enviromentalData.TInstance.volumeSE;
        slider[2].value = sceneManager.enviromentalData.TInstance.textSpeed;
    }
    [EnumAction(typeof(SETTINGSTATE))]
    public void Value(int stateNomber)
    {
        SETTINGSTATE settingState = (SETTINGSTATE)stateNomber; // int型をEnumに変換
        if (slider[stateNomber].value % 1 != 0) // 整数でない場合
        {
            slider[stateNomber].value = (float)Math.Round(slider[stateNomber].value, 1, MidpointRounding.AwayFromZero); // 四捨五入して整数にする
        }
        // 各種設定のテストを流し、設定を変える
        switch (settingState)
        {
            case SETTINGSTATE.BGM:
                sceneManager.audioManager.BGM_Volume = slider[stateNomber].value; // BGM音量を変更する
                sceneManager.enviromentalData.TInstance.volumeBGM = slider[stateNomber].value; // BGM音量を更新する
                    break;
            case SETTINGSTATE.SE:
                // 音量の変更がなかった場合はSEを鳴らさないようにブレイク
                if (slider[stateNomber].value == sceneManager.enviromentalData.TInstance.volumeSE) break; // 同じ値ならブレイクする
                sceneManager.audioManager.SE_Play("SE_item01", slider[stateNomber].value); // SEを鳴らす
                sceneManager.enviromentalData.TInstance.volumeSE = slider[stateNomber].value; // SE音量を更新する
                break;
            case SETTINGSTATE.TEXTSPEED:
                if (slider[stateNomber].value == sceneManager.enviromentalData.TInstance.textSpeed) break; // 同じ値ならブレイクする
                sceneManager.enviromentalData.TInstance.textSpeed = slider[stateNomber].value + 0.1f; // テキストスピード計算式に合わせるために0.1fしている
                testTextController.playerTextSpeed = sceneManager.enviromentalData.TInstance.textSpeed; // テキストスピードを更新する
                testTextController.OnTalkButtonClicked(""); // テストテキストを流す
                break;
        }
    }
}
