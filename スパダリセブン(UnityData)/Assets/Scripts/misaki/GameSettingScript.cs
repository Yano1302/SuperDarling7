using Supadari;
using System;
using UnityEngine;
using UnityEngine.UI;

public partial class GameSettingScript : MonoBehaviour
{
    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

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
                sceneManager.enviromentalData.TInstance.volumeBGM = slider[stateNomber].value; // BGM���ʂ��X�V����
                break;
            case SETTINGSTATE.SE:
                // ���ʂ̕ύX���Ȃ������ꍇ��SE��炳�Ȃ��悤�Ƀu���C�N
                if (slider[stateNomber].value == sceneManager.enviromentalData.TInstance.volumeSE) break; // �����l�Ȃ�u���C�N����
                sceneManager.audioManager.SE_Play("SE_item01", slider[stateNomber].value); // SE��炷
                sceneManager.enviromentalData.TInstance.volumeSE = slider[stateNomber].value; // SE���ʂ��X�V����
                break;
            case SETTINGSTATE.TEXTSPEED:
                if (slider[stateNomber].value == sceneManager.enviromentalData.TInstance.textSpeed) break; // �����l�Ȃ�u���C�N����
                sceneManager.enviromentalData.TInstance.textSpeed = slider[stateNomber].value + 0.1f; // �e�L�X�g�X�s�[�h�v�Z���ɍ��킹�邽�߂�0.1f���Ă���
                testTextController.playerTextSpeed = sceneManager.enviromentalData.TInstance.textSpeed; // �e�L�X�g�X�s�[�h���X�V����
                testTextController.OnTalkButtonClicked(""); // �e�X�g�e�L�X�g�𗬂�
                break;
        }
    }

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///



    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///

    private void Awake()
    {
        // �V�[���}�l�[�W���[���擾
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
        // �e�X���C�_�[��json�̒l��������
        slider[0].value = sceneManager.enviromentalData.TInstance.volumeBGM;
        slider[1].value = sceneManager.enviromentalData.TInstance.volumeSE;
        slider[2].value = sceneManager.enviromentalData.TInstance.textSpeed;
    }

    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///
}
public partial class GameSettingScript
{
    /// --------�ϐ��ꗗ-------- ///

    #region public�ϐ�
    /// -------public�ϐ�------- ///



    /// -------public�ϐ�------- ///
    #endregion

    #region protected�ϐ�
    /// -----protected�ϐ�------ ///



    /// -----protected�ϐ�------ ///
    #endregion

    #region private�ϐ�
    /// ------private�ϐ�------- ///

    private SceneManager sceneManager; // �V�[���}�l�[�W���[�ϐ�

    [SerializeField] private TestTextController testTextController; // �e�X�g�e�L�X�g�R���g���[���[�ϐ�

    [EnumIndex(typeof(SETTINGSTATE))]
    [SerializeField] private Slider[] slider = new Slider[3]; // �X���C�_�[�z��

    /// ------private�ϐ�------- ///
    #endregion

    #region �v���p�e�B
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    #endregion

    /// --------�ϐ��ꗗ-------- ///
}