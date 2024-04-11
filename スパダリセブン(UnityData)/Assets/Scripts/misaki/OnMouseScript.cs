using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool eventCheck = true;
    [SerializeField] RequisitionController requisitionController; // RequisitionController用変数
    [SerializeField] string storyname = null; // 表示したいテキスト名
    private Supadari.SceneManager sceneManager; // SceneManager変数
    bool onPointerCheck = false; // マウスが上に載っているかのチェック
    [SerializeField]Image barImage; // バーのイメージ変数
    [SerializeField] float barFluctuationValue = 0.3f;
    private void Start()
    {
        // シーンマネージャーを取得
        sceneManager=GameObject.FindGameObjectWithTag("SceneManager").GetComponent<Supadari.SceneManager>();
    }
    private void FixedUpdate()
    {
        if (onPointerCheck && barImage && barImage.fillAmount < 1)
        {
            barImage.fillAmount += barFluctuationValue;
        }
        else if(!onPointerCheck && barImage && barImage.fillAmount>0)
        {
            barImage.fillAmount -= barFluctuationValue;
        }
    }
    /// <summary>
    /// マウスがオブジェクト上に乗った時に１度だけ呼び出す関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventCheck) return; // イベントがないならリターン
        SCENENAME sceneName = sceneManager.CheckSceneName; // 現在のシーンを取得
        if (sceneName == SCENENAME.RequisitionsScene)
        {
            if (!eventCheck || requisitionController.TalkState == TALKSTATE.LASTTALK) return;
            // 一度会話を終わらせる
            requisitionController.TalkEnd();
            // storynameの文章を表示する
            requisitionController.OnTalkButtonClicked(storyname);
        }
        else
        {
            onPointerCheck = true;
        }
    }
    /// <summary>
    /// マウスがオブジェクト上から外れたときに１度だけ呼び出す関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventCheck) return; // イベントがないならリターン
        SCENENAME sceneName = sceneManager.CheckSceneName; // 現在のシーンを取得
        if (sceneName == SCENENAME.RequisitionsScene)
        {
            if (requisitionController.TalkState == TALKSTATE.LASTTALK) return;
            // 一度会話を終わらせる
            requisitionController.TalkEnd();
            // defaultの文章を表示する
            requisitionController.OnTalkButtonClicked(requisitionController.storynum);
        }
        else
        {
            onPointerCheck = false;
        }
    }
}
