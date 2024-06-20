using UnityEngine;
using UnityEngine.UI;

public class SolveTextController : BaseTextController
{
    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///

    /// <summary>
    /// 選択したアイテムを突き付ける関数
    /// </summary>
    public void Judge()
    {
        if (TalkState != TALKSTATE.Question) return; // アイテム選択状態ではない場合はリターンする

        sceneManager.audioManager.SE_Play("SE", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす

        ItemID selectedID = ItemManager.GetSelectedID; // 選択したアイテムIDを代入

        // 選択したアイテムIDが正解のアイテムIDかどうかを判断し、次に表示するストーリーを変える
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

        itemWindow.WinSlide(); // アイテムウィンドウをしまう
    }

    public override void TalkEnd()
    {
        Debug.Log("会話を終了");
        TalkState = TALKSTATE.NOTALK; // 会話ステータスを話していないに変更
        if (talkAuto) OnAutoModeCllicked(); // オートモードがオンであればオフにする
        sceneManager.SceneChange(storyTalks[talkNum].transition + "Scene"); // ゲームクリアかゲームオーバーシーンに遷移
        talkNum = default; // リセットする
    }

    /// -------public関数------- ///
    #endregion

    #region protected関数
    /// -----protected関数------ ///



    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///

    private void Start()
    {
        OnTalkButtonClicked(); // ゲームスタート時に表示する
        Button judgeBu = GameObject.FindGameObjectWithTag("Judge").GetComponent<Button>(); // ジャッジボタンを取得
        judgeBu.onClick.AddListener(Judge); // ジャッジボタンにジャッジ関数を設定
    }

    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///
}
