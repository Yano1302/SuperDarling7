
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SolveTextController : BaseTextController
{
    private void Start()
    {
        OnTalkButtonClicked(); // �Q�[���X�^�[�g���ɕ\������
        Button judgeBu = GameObject.FindGameObjectWithTag("Judge").GetComponent<Button>(); // �W���b�W�{�^�����擾
        judgeBu.onClick.AddListener(Judge); // �W���b�W�{�^���ɃW���b�W�֐���ݒ�
    }

    /// <summary>
    /// �I�������A�C�e����˂��t����֐�
    /// </summary>
    public void Judge()
    {
        ItemID selectedID = ItemManager.GetSelectedID; // �I�������A�C�e��ID����

        // �I�������A�C�e��ID�������̃A�C�e��ID���ǂ����𔻒f���A���ɕ\������X�g�[���[��ς���
        if (selectedID == rightID) OnTalkButtonClicked(int.Parse(storyTalks[talkNum].correct));
        else if (selectedID != rightID && missCount < 2)
        {
            OnTalkButtonClicked(int.Parse(storyTalks[talkNum].miss));
            missCount++;
        }
        else OnTalkButtonClicked(int.Parse(storyTalks[talkNum].gameOver));

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
}
