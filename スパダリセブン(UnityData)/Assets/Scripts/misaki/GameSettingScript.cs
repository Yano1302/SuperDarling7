using Supadari;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingScript : MonoBehaviour
{
    AudioManager audioManager; // �I�[�f�B�I�}�l�[�W���[�ϐ�
    SceneManager sceneManager; // �V�[���}�l�[�W���[�ϐ�
    [EnumIndex(typeof(SETTINGSTATE))]
    [SerializeField] Slider[] slider = new Slider[3];
    void Awake()
    {
        // �I�[�f�B�I�}�l�[�W���[�ƃV�[���}�l�[�W���[���擾
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
    }
    [EnumAction(typeof(SETTINGSTATE))]
    public void Value(int stateNomber)
    {
        SETTINGSTATE settingState = (SETTINGSTATE)stateNomber; // int�^��Enum�ɕϊ�
        if (slider[stateNomber].value % 1 != 0) // �����łȂ��ꍇ
        {
            slider[stateNomber].value = (float)Math.Round(slider[stateNomber].value, 1, MidpointRounding.AwayFromZero); // �l�̌ܓ����Đ����ɂ���
        }
        // �e��ݒ�̃e�X�g�𗬂��A�ݒ��ς���
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
                sceneManager.saveData.m_tInstance.textSpeed = slider[stateNomber].value + 0.1f; // �e�L�X�g�X�s�[�h�v�Z���ɍ��킹�邽�߂�0.1f���Ă���
                break;
        }
    }
}
