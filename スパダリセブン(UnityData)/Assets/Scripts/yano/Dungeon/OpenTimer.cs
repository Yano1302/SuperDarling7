using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenTimer : MonoBehaviour
{
    [SerializeField, Header("時間設定(秒)")]
    private float DefaultTimer = 60f;
    [SerializeField, Header("時間を表示するUIテキスト")]
    private Text m_text;
    
    private Timer _Timer;


    //TODO　timerをどこかから設定する？/一旦CreateMapから呼び出します
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
        UsefulSystem.Log("タイムアップです。ゲームが終了されます。");
        
        DisplayManager.Instance.FadeOut(
            FadeType.Entire, 
            () => {
                //TODO　仕様が決まり次第追記する
                UsefulSystem.Instance.EndGame();
            });
    }

}
