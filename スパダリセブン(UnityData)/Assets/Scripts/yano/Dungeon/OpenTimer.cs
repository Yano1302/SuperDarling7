using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenTimer : MonoBehaviour
{
    [SerializeField, Header("時間を表示するUIテキスト")]
    private Text m_text;

    private Timer _Timer;


    //TODO　timerをどこかから設定する？/一旦CreateMapから呼び出します
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
                //TODO　仕様が決まり次第追記する
                UsefulSystem.Instance.EndGame();
            });
    }

}
