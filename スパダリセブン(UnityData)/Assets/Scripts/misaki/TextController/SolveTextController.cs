using UnityEngine;
using UnityEngine.UI;

public class SolveTextController : BaseTextController
{
    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

    /// <summary>
    /// �I�������A�C�e����˂��t����֐�
    /// </summary>
    public void Judge()
    {
        if (TalkState != TALKSTATE.Question) return; // �A�C�e���I����Ԃł͂Ȃ��ꍇ�̓��^�[������

        sceneManager.audioManager.SE_Play("SE", sceneManager.enviromentalData.TInstance.volumeSE); // SE��炷

        ItemID selectedID = ItemManager.GetSelectedID; // �I�������A�C�e��ID����

        // �I�������A�C�e��ID�������̃A�C�e��ID���ǂ����𔻒f���A���ɕ\������X�g�[���[��ς���
        if (selectedID == rightID) OnTalkButtonClicked(int.Parse(storyTalks[talkNum].correct));
        else if (selectedID != rightID && MissCount < 2)
        {
            OnTalkButtonClicked(int.Parse(storyTalks[talkNum].miss));
            MissCount++;
        }
        else
        {
            OnTalkButtonClicked(int.Parse(storyTalks[talkNum].gameOver));
            MissCount++;
        }

        itemWindow.WinSlide(); // �A�C�e���E�B���h�E�����܂�
    }

    public override void TalkEnd()
    {
        Debug.Log("��b���I��");
        TalkState = TALKSTATE.NOTALK; // ��b�X�e�[�^�X��b���Ă��Ȃ��ɕύX
        if (talkAuto) OnAutoModeCllicked(); // �I�[�g���[�h���I���ł���΃I�t�ɂ���
        sceneManager.SceneChange(storyTalks[talkNum].transition + "Scene"); // �Q�[���N���A���Q�[���I�[�o�[�V�[���ɑJ��
        talkNum = default; // ���Z�b�g����
    }

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///



    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///

    private void Start()
    {
        OnTalkButtonClicked(); // �Q�[���X�^�[�g���ɕ\������
        Button judgeBu = GameObject.FindGameObjectWithTag("Judge").GetComponent<Button>(); // �W���b�W�{�^�����擾
        judgeBu.onClick.AddListener(Judge); // �W���b�W�{�^���ɃW���b�W�֐���ݒ�
    }

    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///
}
