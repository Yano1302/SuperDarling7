using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSubject : MonoBehaviour
{
    /// --------変数一覧-------- ///

    /// private変数 ///
    [Header("通知を出したいスクリプト")]
    [SerializeField] GameClearController gameClearController;

    /// protected変数 ///


    /// public変数 ///


    /// --------変数一覧-------- ///
    /// --------関数一覧-------- ///
    /// private関数 ///


    /// protected関数 ///


    /// public関数 ///

    /// <summary>
    /// アニメーション終了通知を出す関数
    /// </summary>
    public void Notice()
    {
        gameClearController.AnimEnd = true;
    }

    /// --------関数一覧-------- ///
}
