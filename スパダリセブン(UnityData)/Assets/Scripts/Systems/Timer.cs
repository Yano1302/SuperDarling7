using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

/// <summary>
/// SetTimerでインスタンスが作成(対象のオブジェクトにAddComponent)されます。
/// 対象のオブジェクトが非アクティブになった場合、もしくは時間切れになった場合にこのクラスは自動的に破棄されます。
/// TODO : 必要な機能があれば随時追加します
/// </summary>
public class Timer:MonoBehaviour
{
// public  static //
    /// <summary>タイマーを設定します。</summary>
    /// <param name="manager">タイマーを管理するオブジェクト</param>
    /// <param name="timer">設定する秒数</param>
    /// <param name="timeUpAction">時間切れの場合に起こす関数</param>
    /// <returns>Timerクラスのインスタンス</returns>
    public static Timer SetTimer(GameObject manager,float timer,UnityAction timeUpAction) {
        var instance = manager.AddComponent<Timer>();
        instance.TimeUpAction = timeUpAction;
        instance.RemainingTime = timer;
        instance.TimeFlag = true;
        return instance;
    }
    /// <summary>タイマーを設定します。</summary>
    /// <param name="manager">タイマーを管理するオブジェクト</param>
    /// <param name="timer">設定する秒数</param>
    /// <param name="timeUpAction">時間切れの場合に起こす関数</param>
    /// <param name="timeMoves">タイマーを進めるかどうかのフラグ</param>
    /// <param name="timerSpeed">タイマーの進む速度</param>
    /// <param name="deltaAction">毎フレームごとに行う関数</param>
    /// <returns></returns>
    public static Timer SetTimer(GameObject manager, float timer, UnityAction timeUpAction, bool timeMoves, float timerSpeed,UnityAction deltaAction) {
        var instance = manager.AddComponent<Timer>();
        instance.TimeUpAction = timeUpAction;
        instance.RemainingTime = timer;
        instance.TimeSpeed = timerSpeed;
        instance.TimeFlag = timeMoves;
        return instance;
    }

    //  Public //
    /// <summary> 総残り時間(秒)</summary>
    public float RemainingTime { get; set; } = 999;
    /// <summary>残り時間を分数で返します</summary>
    public int Minutes { get { return (int)(RemainingTime / 60); } }
 　 /// <summary>残り時間(秒数のみ :  0 ～　59) </summary>
    public int IntSeconds { get { return (int)Mathf.Ceil(RemainingTime % 60); } }
    /// <summary>残り時間(秒数のみ :  0.000000 ～　59.99999) </summary>
    public float FloatSeconds { get { return RemainingTime % 60; }  }
    
    /// <summary> この変数がtrueの場合にタイマーが進みます </summary>
    public bool TimeFlag { get; set; } = false;
    /// <summary>タイマーが切れた場合の処理</summary>
    public UnityAction TimeUpAction { get; set; }
    /// <summary>１秒ごとに行う関数(この1秒はTimeSpeedの影響を受けます。※速度をマイナスにした場合でも正常に動くはずです) </summary>
    public UnityAction SecondAction { get; set; } = null;   
    /// <summary>タイマーの進む速度を指定できます(倍率)。速度をマイナスにした場合は残り時間が増えていきます。 </summary>
    public float TimeSpeed { get { return m_timeSpeed; } set { m_timeSpeed = value; UsefulSystem.DebugAction(() => { if (value < 0) Debug.LogWarning("徐々に残り時間が増えていきます。"); }); } }




// private //
    private float m_timeSpeed = 1.0f; //タイマーの速度
    private float m_count = 0.0f;     //１秒経過をカウントする

    private void Update() {
        if (RemainingTime <= 0) {
            TimeUpAction?.Invoke();
            Log("タイマーが0になりました");
            Destroy(this);
        }
        if (TimeFlag) {
            RemainingTime -= Time.deltaTime * TimeSpeed;
            m_count += Mathf.Abs(Time.deltaTime * TimeSpeed);
            if(m_count >= 1) {
                SecondAction?.Invoke();
                m_count -= 1;
            }          
        }
    }


    private void OnDisable() {
        Log("タイマーが破棄されます。");
        Destroy(this);
    }

    //Other Fanction 
    [Conditional("UNITY_EDITOR")]
    private void Log(string message) {Debug.Log(message);}



}
