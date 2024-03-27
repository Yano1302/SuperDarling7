using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : BaseTextController
{
    private Supadari.SceneManager sceneManager; // �X�p�_���̃V�[���}�l�[�W���[�p�ϐ�

    protected override void Awake()
    {
        base.Awake();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    public override void TalkEnd()
    {
        base.TalkEnd();
        sceneManager.SceneChange(EnumList.SCENENAME.Dungeon); // �T����ʂփV�[���J�ڂ���
    }
}
