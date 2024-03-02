using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeBtn : MonoBehaviour
{
    enum BtnSide { Left, Right }

    //�A�^�b�`�p
    [SerializeField] private BtnSide Side = BtnSide.Right;
    [SerializeField] private RectTransform Rect;
    [SerializeField] private Text m_text;
    //Escape����ݒ�
    [HideInInspector] public int RestPressNum { get { return m_CurrentPresNum; } set { m_CurrentPresNum = value; if (m_CurrentPresNum <= 0) { clear = true; Rect.gameObject.SetActive(false); } } }
    [HideInInspector] public KeyCode Key { get { return m_key; } set { m_key = value; m_text.text = value.ToString(); } }
    [HideInInspector] public bool clear = false;

    //�v���C�x�[�g�ϐ�
    private KeyCode m_key;
    private int m_CurrentPresNum;


   public void Set(int pressNum) {
        RestPressNum = pressNum;
        clear = false;
        //97 ~ 122 ���@A~Z�͈̔�
        Key = (KeyCode)Random.Range(97, 122);
        Vector3 sidePos = Side == BtnSide.Right ? Vector3.right : Vector3.left * 2;
        Rect.position = Camera.main.WorldToScreenPoint((Player.Instance.gameObject.transform.position + sidePos));
    }
}
