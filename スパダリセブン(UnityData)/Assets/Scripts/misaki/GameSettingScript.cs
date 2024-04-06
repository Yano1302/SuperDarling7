using Supadari;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingScript : MonoBehaviour
{
    AudioManager audioManager; // オーディオマネージャー変数
    SceneManager sceneManager; // シーンマネージャー変数
    [EnumIndex(typeof(SETTINGSTATE))]
    [SerializeField] Slider[] slider = new Slider[3];
    void Awake()
    {
        // オーディオマネージャーとシーンマネージャーを取得
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
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
                audioManager.BGM_Volume = slider[stateNomber].value;
                sceneManager.saveData.m_tInstance.volumeBGM = slider[stateNomber].value;
                    break;
            case SETTINGSTATE.SE:
                audioManager.SE_Play("SE", slider[stateNomber].value);
                sceneManager.saveData.m_tInstance.volumeSE = slider[stateNomber].value;
                break;
            case SETTINGSTATE.TEXTSPEED:
                sceneManager.saveData.m_tInstance.textSpeed = slider[stateNomber].value + 0.1f; // テキストスピード計算式に合わせるために0.1fしている
                break;
        }
    }
}
