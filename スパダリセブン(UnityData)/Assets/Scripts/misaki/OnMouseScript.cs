using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool eventCheck=true;
    [SerializeField] RequisitionController requisitionController; // RequisitionController�p�ϐ�
    [SerializeField] string storyname = null; // �\���������e�L�X�g��

    /// <summary>
    /// �}�E�X���I�u�W�F�N�g��ɏ�������ɂP�x�����Ăяo���֐�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventCheck) return;
        // ��x��b���I��点��
        requisitionController.TalkEnd();
        // storyname�̕��͂�\������
        requisitionController.OnTalkButtonClicked(storyname);
    }
    /// <summary>
    /// �}�E�X���I�u�W�F�N�g�ォ��O�ꂽ�Ƃ��ɂP�x�����Ăяo���֐�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventCheck) return;
        // ��x��b���I��点��
        requisitionController.TalkEnd();
        // default�̕��͂�\������
        requisitionController.OnTalkButtonClicked(requisitionController.storynum);
    }
}
