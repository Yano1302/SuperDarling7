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
        _Timer = Timer.SetTimer(gameObject, 100,TimeUp);
        _Timer.SecondAction = SetTimerUI;
    }

    private void SetTimerUI() {
        m_text.text = "0"+_Timer.minutes + " : "+_Timer.seconds;
    } 

    private void TimeUp() {
        Display.Instance.FadeOut(
            FadeType.Entire, 
            () => {
                //TODO�@�d�l�����܂莟��ǋL����
                UsefulSystem.Instance.EndGame();
            });
    }

}
