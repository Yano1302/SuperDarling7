using Supadari;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingScript : MonoBehaviour
{
    SceneManager sceneManager; // �V�[���}�l�[�W���[�ϐ�
    [SerializeField]TestTextController testTextController; // �e�X�g�e�L�X�g�R���g���[���[�ϐ�
    [EnumIndex(typeof(SETTINGSTATE))]
    [SerializeField] Slider[] slider = new Slider[3]; // �X���C�_�[�z��
    void Awake()
    {
        // �V�[���}�l�[�W���[���擾
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
        // �e�X���C�_�[��json�̒l��������
        slider[0].value = sceneManager.enviromentalData.m_tInstance.volumeBGM;
        slider[1].value = sceneManager.enviromentalData.m_tInstance.volumeSE;
        slider[2].value = sceneManager.enviromentalData.m_tInstance.textSpeed;
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
                sceneManager.audioManager.BGM_Volume = slider[stateNomber].value; // BGM���ʂ�ύX����
                sceneManager.enviromentalData.m_tInstance.volumeBGM = slider[stateNomber].value; // BGM���ʂ��X�V����
                    break;
            case SETTINGSTATE.SE:
                // ���ʂ̕ύX���Ȃ������ꍇ��SE��炳�Ȃ��悤�Ƀu���C�N
                if (slider[stateNomber].value == sceneManager.enviromentalData.m_tInstance.volumeSE) break; // �����l�Ȃ�u���C�N����
                sceneManager.audioManager.SE_Play("SE_item01", slider[stateNomber].value); // SE��炷
                sceneManager.enviromentalData.m_tInstance.volumeSE = slider[stateNomber].value; // SE���ʂ��X�V����
                break;
            case SETTINGSTATE.TEXTSPEED:
                if (slider[stateNomber].value == sceneManager.enviromentalData.m_tInstance.textSpeed) break; // �����l�Ȃ�u���C�N����
                sceneManager.enviromentalData.m_tInstance.textSpeed = slider[stateNomber].value + 0.1f; // �e�L�X�g�X�s�[�h�v�Z���ɍ��킹�邽�߂�0.1f���Ă���
                testTextController.playerTextSpeed = sceneManager.enviromentalData.m_tInstance.textSpeed; // �e�L�X�g�X�s�[�h���X�V����
                testTextController.OnTalkButtonClicked(); // �e�X�g�e�L�X�g�𗬂�
                break;
        }
    }
}
