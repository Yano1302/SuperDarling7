using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimerManager : SingletonMonoBehaviour<TimerManager>
{
    /// <summary>タイマーのフラグを設定します</summary>
    public bool TimerFlag { get { return _Timer.TimeFlag; } set { _Timer.TimeFlag = value; } }

    public void SetTimer(float time) {
        OpenTimer();
        _Timer = Timer.SetTimer(m_timer.gameObject, time, TimeUp);
        _Timer.SecondAction = SetTimerUI;
        SetTimerUI();
    }

    public void OpenTimer() {
        UIManager.Instance.OpenUI(UIType.Timer);
        m_timer.enabled = true;
        m_text.enabled = true;
    }

    public void CloseTimer(bool timerStop) {
        if (timerStop) {
            UIManager.Instance.CloseUI(UIType.Timer);
        }
        else {
           m_timer.enabled = false;
           m_text.enabled = false;
        }
    }


    [SerializeField, Header("時間を表示するUI")]
    private Image m_timer;
    [SerializeField, Header("時間を表示するUIテキスト")]
    private Text m_text;
    
 
    private Timer _Timer;
    


    private void SetTimerUI() {
        string mstr = _Timer.Minutes < 10 ? "0" + _Timer.Minutes : _Timer.Minutes.ToString();
        string sstr = _Timer.IntSeconds < 10 ? " :  0" + _Timer.IntSeconds : " : " + _Timer.IntSeconds.ToString();
        m_text.text = mstr + sstr;
    } 

    private void TimeUp() {
        DisplayManager.Instance.FadeOut(
            FadeType.Entire, 
            () => {
                Supadari.SceneManager.Instance.SceneChange(SCENENAME.SolveScene);
            });
    }

}
