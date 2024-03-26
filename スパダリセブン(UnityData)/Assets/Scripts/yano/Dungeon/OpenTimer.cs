using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenTimer : MonoBehaviour
{
    [SerializeField, Header("���Ԑݒ�(�b)")]
    private float DefaultTimer = 60f;
    [SerializeField, Header("���Ԃ�\������UI�e�L�X�g")]
    private Text m_text;
    
    private Timer _Timer;


    //TODO�@timer���ǂ�������ݒ肷��H/��UCreateMap����Ăяo���܂�
    private void OnEnable() {
        _Timer = Timer.SetTimer(gameObject, DefaultTimer,TimeUp);
        _Timer.SecondAction = SetTimerUI;
        SetTimerUI();
    }

    private void SetTimerUI() {
        string mstr = _Timer.Minutes < 10 ? "0" + _Timer.Minutes : _Timer.Minutes.ToString();
        string sstr = _Timer.IntSeconds < 10 ? " :  0" + _Timer.IntSeconds : " : " + _Timer.IntSeconds.ToString();
        m_text.text = mstr + sstr;
    } 

    private void TimeUp() {
        UsefulSystem.Log("�^�C���A�b�v�ł��B�Q�[�����I������܂��B");
        
        DisplayManager.Instance.FadeOut(
            FadeType.Entire, 
            () => {
                //TODO�@�d�l�����܂莟��ǋL����
                UsefulSystem.Instance.EndGame();
            });
    }

}
