using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeBtn : MonoBehaviour
{
    enum BtnSide { Left, Right }

    //アタッチ用
    [SerializeField] private BtnSide Side = BtnSide.Right;
    [SerializeField] private RectTransform Rect;
    [SerializeField] private Text m_text;
    [SerializeField] private Image m_image;
    //Escapeから設定
    [HideInInspector] public int RestPressNum { get { return m_CurrentPresNum; } set { m_CurrentPresNum = value; if (m_CurrentPresNum <= 0) { clear = true; Rect.gameObject.SetActive(false); } } }
    [HideInInspector] public KeyCode Key { get { return m_key; } set { m_key = value; m_text.text = value.ToString(); } }
    [HideInInspector] public bool clear = false;

    //プライベート変数
    private KeyCode m_key;
    private int m_CurrentPresNum;


   public void Set(int pressNum) {
        RestPressNum = pressNum;
        clear = false;
        m_image.color = Color.blue;
        //97 ~ 122 が　A~Zの範囲
        Key = (KeyCode)Random.Range(97, 122);
        Vector3 sidePos = Side == BtnSide.Right ? new Vector3(2, 0, 0) : new Vector3(-2, 0, 0);
        Rect.position = Camera.main.WorldToScreenPoint((Player.Instance.gameObject.transform.position + sidePos));
    }

    public void SetColor(Color color) { m_image.color = color; }
}
