using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

/// <summary>
/// SetTimer�ŃC���X�^���X���쐬(�Ώۂ̃I�u�W�F�N�g��AddComponent)����܂��B
/// �Ώۂ̃I�u�W�F�N�g����A�N�e�B�u�ɂȂ����ꍇ�A�������͎��Ԑ؂�ɂȂ����ꍇ�ɂ��̃N���X�͎����I�ɔj������܂��B
/// TODO : �K�v�ȋ@�\������ΐ����ǉ����܂�
/// </summary>
public class Timer:MonoBehaviour
{
// public  static //
    /// <summary>�^�C�}�[��ݒ肵�܂��B</summary>
    /// <param name="manager">�^�C�}�[���Ǘ�����I�u�W�F�N�g</param>
    /// <param name="timer">�ݒ肷��b��</param>
    /// <param name="timeUpAction">���Ԑ؂�̏ꍇ�ɋN�����֐�</param>
    /// <returns>Timer�N���X�̃C���X�^���X</returns>
    public static Timer SetTimer(GameObject manager,float timer,UnityAction timeUpAction) {
        var instance = manager.AddComponent<Timer>();
        instance.TimeUpAction = timeUpAction;
        instance.RemainingTime = timer;
        instance.TimeFlag = true;
        return instance;
    }
    /// <summary>�^�C�}�[��ݒ肵�܂��B</summary>
    /// <param name="manager">�^�C�}�[���Ǘ�����I�u�W�F�N�g</param>
    /// <param name="timer">�ݒ肷��b��</param>
    /// <param name="timeUpAction">���Ԑ؂�̏ꍇ�ɋN�����֐�</param>
    /// <param name="timeMoves">�^�C�}�[��i�߂邩�ǂ����̃t���O</param>
    /// <param name="timerSpeed">�^�C�}�[�̐i�ޑ��x</param>
    /// <param name="deltaAction">���t���[�����Ƃɍs���֐�</param>
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
    /// <summary> ���c�莞��(�b)</summary>
    public float RemainingTime { get; set; } = 999;
    /// <summary>�c�莞�Ԃ𕪐��ŕԂ��܂�</summary>
    public int Minutes { get { return (int)(RemainingTime / 60); } }
 �@ /// <summary>�c�莞��(�b���̂� :  0 �`�@59) </summary>
    public int IntSeconds { get { return (int)Mathf.Ceil(RemainingTime % 60); } }
    /// <summary>�c�莞��(�b���̂� :  0.000000 �`�@59.99999) </summary>
    public float FloatSeconds { get { return RemainingTime % 60; }  }
    
    /// <summary> ���̕ϐ���true�̏ꍇ�Ƀ^�C�}�[���i�݂܂� </summary>
    public bool TimeFlag { get; set; } = false;
    /// <summary>�^�C�}�[���؂ꂽ�ꍇ�̏���</summary>
    public UnityAction TimeUpAction { get; set; }
    /// <summary>�P�b���Ƃɍs���֐�(����1�b��TimeSpeed�̉e�����󂯂܂��B�����x���}�C�i�X�ɂ����ꍇ�ł�����ɓ����͂��ł�) </summary>
    public UnityAction SecondAction { get; set; } = null;   
    /// <summary>�^�C�}�[�̐i�ޑ��x���w��ł��܂�(�{��)�B���x���}�C�i�X�ɂ����ꍇ�͎c�莞�Ԃ������Ă����܂��B </summary>
    public float TimeSpeed { get { return m_timeSpeed; } set { m_timeSpeed = value; UsefulSystem.DebugAction(() => { if (value < 0) Debug.LogWarning("���X�Ɏc�莞�Ԃ������Ă����܂��B"); }); } }




// private //
    private float m_timeSpeed = 1.0f; //�^�C�}�[�̑��x
    private float m_count = 0.0f;     //�P�b�o�߂��J�E���g����

    private void Update() {
        if (RemainingTime <= 0) {
            TimeUpAction?.Invoke();
            Log("�^�C�}�[��0�ɂȂ�܂���");
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
        Log("�^�C�}�[���j������܂��B");
        Destroy(this);
    }

    //Other Fanction 
    [Conditional("UNITY_EDITOR")]
    private void Log(string message) {Debug.Log(message);}



}
