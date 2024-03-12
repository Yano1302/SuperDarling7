using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenTimer : MonoBehaviour
{
    [SerializeField, Header("���Ԃ�\������UI�e�L�X�g")]
    private Text m_text;

    private Timer _Timer;


    //TODO�@timer���ǂ�������ݒ肷��H/��UCreateMap����Ăяo���܂�
    private void OnEnable() {
        _Timer = Timer.SetTimer(gameObject, 30,TimeUp);
        _Timer.SecondAction = SetTimerUI;
        SetTimerUI();
    }

    private void SetTimerUI() {
        string mstr = _Timer.minutes < 10 ? "0" + _Timer.minutes : _Timer.minutes.ToString();
        string sstr = _Timer.seconds < 10 ? " :  0" + _Timer.seconds : " : " + _Timer.seconds.ToString();
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
