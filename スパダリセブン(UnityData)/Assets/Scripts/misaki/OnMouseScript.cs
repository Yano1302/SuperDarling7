using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool eventCheck = true;
    [SerializeField] RequisitionController requisitionController; // RequisitionController�p�ϐ�
    [SerializeField] string storyname = null; // �\���������e�L�X�g��
    private Supadari.SceneManager sceneManager; // SceneManager�ϐ�
    bool onPointerCheck = false; // �}�E�X����ɍڂ��Ă��邩�̃`�F�b�N
    [SerializeField]Image barImage; // �o�[�̃C���[�W�ϐ�
    [SerializeField] float barFluctuationValue = 0.3f; // �o�[�̕\�����x
    private void Start()
    {
        // �V�[���}�l�[�W���[���擾
        sceneManager=GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    private void FixedUpdate()
    {
        if (onPointerCheck && barImage && barImage.fillAmount < 1) // �o�[��\�����鋖���o���ꍇ
        {
            barImage.fillAmount += barFluctuationValue; // �o�[��\������
        }
        else if (!onPointerCheck && barImage && barImage.fillAmount > 0) // �o�[���\�����鋖���o���ꍇ
        {
            barImage.fillAmount -= barFluctuationValue; // �o�[���\���ɂ���
        }
    }
    /// <summary>
    /// �}�E�X���I�u�W�F�N�g��ɏ�������ɂP�x�����Ăяo���֐�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventCheck) return; // �C�x���g���Ȃ��Ȃ烊�^�[��
        SCENENAME sceneName = sceneManager.CheckSceneName; // ���݂̃V�[�����擾
        if (sceneName == SCENENAME.RequisitionsScene)
        {
            if (!eventCheck || requisitionController.TalkState == TALKSTATE.LASTTALK) return;
            // ��x��b���I��点��
            requisitionController.TalkEnd();
            // storyname�̕��͂�\������
            requisitionController.OnTalkButtonClicked(storyname);
        }
        else
        {
            onPointerCheck = true; // �o�[��\�����鋖���o��
        }
    }
    /// <summary>
    /// �}�E�X���I�u�W�F�N�g�ォ��O�ꂽ�Ƃ��ɂP�x�����Ăяo���֐�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventCheck) return; // �C�x���g���Ȃ��Ȃ烊�^�[��
        SCENENAME sceneName = sceneManager.CheckSceneName; // ���݂̃V�[�����擾
        if (sceneName == SCENENAME.RequisitionsScene)
        {
            if (requisitionController.TalkState == TALKSTATE.LASTTALK) return;
            // ��x��b���I��点��
            requisitionController.TalkEnd();
            // default�̕��͂�\������
            requisitionController.OnTalkButtonClicked(requisitionController.storynum);
        }
        else
        {
            onPointerCheck = false; // �o�[���\���ɂ��鋖���o��
        }
    }
}
