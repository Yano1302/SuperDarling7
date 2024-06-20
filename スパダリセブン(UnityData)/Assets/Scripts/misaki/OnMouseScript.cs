using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class OnMouseScript : DebugSetting, IPointerEnterHandler, IPointerExitHandler
{
    /// --------関数一覧-------- ///

    #region public関数
    /// -------public関数------- ///

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

    /// -------public関数------- ///
    #endregion

    #region protected関数
    /// -----protected関数------ ///



    /// -----protected関数------ ///
    #endregion

    #region private関数
    /// ------private関数------- ///

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
    /// 非表示になる際に初期化する
    /// </summary>
    private void OnDisable()
    {
        barImage.fillAmount = 0;
        onPointerCheck = false;
    }

    /// ------private関数------- ///
    #endregion

    /// --------関数一覧-------- ///
}
public partial class OnMouseScript
{
    /// --------変数一覧-------- ///

    #region public変数
    /// -------public変数------- ///



    /// -------public変数------- ///
    #endregion

    #region protected変数
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    #endregion

    #region private変数
    /// ------private変数------- ///

    private bool onPointerCheck = false; // マウスが上に載っているかのチェック
    [SerializeField] private bool eventCheck = true; // OnMouseScriptを動かすかどうかのチェック

    [SerializeField] private float barFluctuationValue = 0.3f; // バーの表示速度

    [SerializeField] private string storyname = null; // 表示したいテキスト名

    [SerializeField] private Image barImage; // バーのイメージ変数

    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}