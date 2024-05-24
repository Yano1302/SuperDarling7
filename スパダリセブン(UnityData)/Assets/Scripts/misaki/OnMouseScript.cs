using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool eventCheck = true; // OnMouseScriptを動かすかどうかのチェック
    [SerializeField] string storyname = null; // 表示したいテキスト名
    private bool onPointerCheck = false; // マウスが上に載っているかのチェック
    [SerializeField]Image barImage; // バーのイメージ変数
    [SerializeField] float barFluctuationValue = 0.3f; // バーの表示速度

    private void FixedUpdate()
    {
        if (onPointerCheck && barImage && barImage.fillAmount < 1) // バーを表示する許可が出た場合
        {
            barImage.fillAmount += barFluctuationValue; // バーを表示する
        }
        else if (!onPointerCheck && barImage && barImage.fillAmount > 0) // バーを非表示する許可が出た場合
        {
            barImage.fillAmount -= barFluctuationValue; // バーを非表示にする
        }
    }
    /// <summary>
    /// マウスがオブジェクト上に乗った時に１度だけ呼び出す関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventCheck) return; // イベントがないならリターン
        onPointerCheck = true; // バーを表示する許可を出す
    }
    /// <summary>
    /// マウスがオブジェクト上から外れたときに１度だけ呼び出す関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventCheck) return; // イベントがないならリターン
        onPointerCheck = false; // バーを非表示にする許可を出す
    }
    /// <summary>
    /// 非表示になる際に初期化する
    /// </summary>
    private void OnDisable()
    {
        barImage.fillAmount = 0;
        onPointerCheck = false;
    }
}
