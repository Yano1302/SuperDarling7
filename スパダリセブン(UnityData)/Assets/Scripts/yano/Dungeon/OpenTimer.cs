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
        UsefulSystem.Log("タイムアップです。ゲームが終了されます。");
        
        DisplayManager.Instance.FadeOut(
            FadeType.Entire, 
            () => {
                //TODO　仕様が決まり次第追記する
                UsefulSystem.Instance.EndGame();
            });
    }

}
