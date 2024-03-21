using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool eventCheck=true;
    [SerializeField] RequisitionController requisitionController; // RequisitionController用変数
    [SerializeField] string storyname = null; // 表示したいテキスト名

    /// <summary>
    /// マウスがオブジェクト上に乗った時に１度だけ呼び出す関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventCheck) return;
        // 一度会話を終わらせる
        requisitionController.TalkEnd();
        // storynameの文章を表示する
        requisitionController.OnTalkButtonClicked(storyname);
    }
    /// <summary>
    /// マウスがオブジェクト上から外れたときに１度だけ呼び出す関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventCheck) return;
        // 一度会話を終わらせる
        requisitionController.TalkEnd();
        // defaultの文章を表示する
        requisitionController.OnTalkButtonClicked(requisitionController.storynum);
    }
}
