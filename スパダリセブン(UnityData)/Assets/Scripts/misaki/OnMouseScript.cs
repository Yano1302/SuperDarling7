using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

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

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///



    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///

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
    /// ��\���ɂȂ�ۂɏ���������
    /// </summary>
    private void OnDisable()
    {
        barImage.fillAmount = 0;
        onPointerCheck = false;
    }

    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///
}
public partial class OnMouseScript
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

    private bool onPointerCheck = false; // �}�E�X����ɍڂ��Ă��邩�̃`�F�b�N
    [SerializeField] private bool eventCheck = true; // OnMouseScript�𓮂������ǂ����̃`�F�b�N

    [SerializeField] private float barFluctuationValue = 0.3f; // �o�[�̕\�����x

    [SerializeField] private string storyname = null; // �\���������e�L�X�g��

    [SerializeField] private Image barImage; // �o�[�̃C���[�W�ϐ�

    /// ------private�ϐ�------- ///
    #endregion

    #region �v���p�e�B
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    #endregion

    /// --------�ϐ��ꗗ-------- ///
}