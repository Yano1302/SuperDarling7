
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SolveTextController : BaseTextController
{
    private void Start()
    {
        OnTalkButtonClicked(); // ゲームスタート時に表示する
        Button judgeBu = GameObject.FindGameObjectWithTag("Judge").GetComponent<Button>(); // ジャッジボタンを取得
        judgeBu.onClick.AddListener(Judge); // ジャッジボタンにジャッジ関数を設定
    }

    /// <summary>
    /// 選択したアイテムを突き付ける関数
    /// </summary>
    public void Judge()
    {
        ItemID selectedID = ItemManager.GetSelectedID; // 選択したアイテムIDを代入

        // 選択したアイテムIDが正解のアイテムIDかどうかを判断し、次に表示するストーリーを変える
        if (selectedID == rightID) OnTalkButtonClicked(int.Parse(storyTalks[talkNum].correct));
        else if (selectedID != rightID && missCount < 2)
        {
            OnTalkButtonClicked(int.Parse(storyTalks[talkNum].miss));
            missCount++;
        }
        else OnTalkButtonClicked(int.Parse(storyTalks[talkNum].gameOver));

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
}
