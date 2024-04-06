using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{       void Awake(){
            JsonSettings<MasterData> m = new JsonSettings<MasterData>("MasterData","kari");
        }
    [EnumAction(typeof(SETTINGSTATE))]
    public void Value(SETTINGSTATE settingState)
    {
 
        Slider slider = GetComponent<Slider>(); // Slider���擾
        if (slider.value % 1 == 0) return; // �����Ȃ烊�^�[��
        if (settingState != SETTINGSTATE.TEXTSPEED)
        {
            slider.value = (float)Math.Round(slider.value, 1, MidpointRounding.AwayFromZero); // �l�̌ܓ����Đ����ɂ���
        }
        
        switch(settingState)
        {
            case SETTINGSTATE.BGM:
                    break;
            case SETTINGSTATE.SE:
                break;
            case SETTINGSTATE.TEXTSPEED:
                break;

        }
    }
}
