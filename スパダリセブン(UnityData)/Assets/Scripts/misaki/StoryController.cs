using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : BaseTextController
{
    private Supadari.SceneManager sceneManager; // スパダリのシーンマネージャー用変数

    protected override void Awake()
    {
        base.Awake();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    public override void TalkEnd()
    {
        base.TalkEnd();
        sceneManager.SceneChange(EnumList.SCENENAME.Dungeon); // 探索画面へシーン遷移する
    }
}
