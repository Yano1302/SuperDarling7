using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideWindow : MonoBehaviour
{
    RectTransform rectTransform; // 自身のRectTransform変数
    RectTransform[] rectTransforms; // RectTransform配列
    public float speacing = 20f; // オブジェクト間のスペースの大きさ
    float maxParentLength = 0; // 親オブジェクトの全長
    public float speed = 1000f;
    float initialLeft;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // RectTransformを代入
        rectTransforms = new RectTransform[transform.childCount]; // 子オブジェクト分の配列を用意する
        // 子オブジェクトのRectTransformを格納する
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            rectTransforms[i] = transform.GetChild(i).GetComponent<RectTransform>();
            maxParentLength += rectTransforms[i].rect.width + speacing;
        }
        initialLeft = rectTransform.offsetMin.x;
    }
    private void FixedUpdate()
    {
        SlideIn();
    }
    void SlideIn()
    {
        float currentLeft = rectTransform.offsetMin.x;
        float newLeft = currentLeft + speed * Time.deltaTime;
        rectTransform.offsetMin= new Vector2(newLeft, rectTransform.offsetMin.y);

    }
    void SlideOut()
    {

    }
}
