using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool eventCheck = true; // OnMouseScript�𓮂������ǂ����̃`�F�b�N
    [SerializeField] string storyname = null; // �\���������e�L�X�g��
    private bool onPointerCheck = false; // �}�E�X����ɍڂ��Ă��邩�̃`�F�b�N
    [SerializeField]Image barImage; // �o�[�̃C���[�W�ϐ�
    [SerializeField] float barFluctuationValue = 0.3f; // �o�[�̕\�����x

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
        onPointerCheck = true; // �o�[��\�����鋖���o��
    }
    /// <summary>
    /// �}�E�X���I�u�W�F�N�g�ォ��O�ꂽ�Ƃ��ɂP�x�����Ăяo���֐�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventCheck) return; // �C�x���g���Ȃ��Ȃ烊�^�[��
        onPointerCheck = false; // �o�[���\���ɂ��鋖���o��
    }
    /// <summary>
    /// ��\���ɂȂ�ۂɏ���������
    /// </summary>
    private void OnDisable()
    {
        barImage.fillAmount = 0;
        onPointerCheck = false;
    }
}
