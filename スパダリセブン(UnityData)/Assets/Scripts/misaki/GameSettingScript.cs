using Supadari;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingScript : MonoBehaviour
{
    SceneManager sceneManager; // シーンマネージャー変数
    [EnumIndex(typeof(SETTINGSTATE))]
    [SerializeField] Slider[] slider = new Slider[3];
    void Awake()
    {
        // シーンマネージャーを取得
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
        // 各スライダーにjsonの値を代入する
        slider[0].value = sceneManager.enviromentalData.m_tInstance.volumeBGM;
        slider[1].value = sceneManager.enviromentalData.m_tInstance.volumeSE;
        slider[2].value = sceneManager.enviromentalData.m_tInstance.textSpeed;
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
                sceneManager.audioManager.BGM_Volume = slider[stateNomber].value;
                sceneManager.enviromentalData.m_tInstance.volumeBGM = slider[stateNomber].value;
                    break;
            case SETTINGSTATE.SE:
                // 音量の変更がなかった場合はSEを鳴らさないようにブレイク
                if (slider[stateNomber].value == sceneManager.enviromentalData.m_tInstance.volumeSE) break; 
                sceneManager.audioManager.SE_Play("SE_item01", slider[stateNomber].value);
                sceneManager.enviromentalData.m_tInstance.volumeSE = slider[stateNomber].value;
                break;
            case SETTINGSTATE.TEXTSPEED:
                sceneManager.enviromentalData.m_tInstance.textSpeed = slider[stateNomber].value + 0.1f; // テキストスピード計算式に合わせるために0.1fしている
                break;
        }
    }
}
