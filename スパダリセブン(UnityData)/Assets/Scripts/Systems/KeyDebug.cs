using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// keyCode����֐����Ăяo���܂��B
/// static�ϐ���SetKeyDebug���Ăяo���Ďg�p���Ă�������
/// �ݒ肵��UnityAction�̏��Ԃɉ����ăL�[������U���܂�
/// </summary>
public class KeyDebug:MonoBehaviour {

    /// <summary>����U���ė~�����Ȃ��L�[��ݒ�ł��܂�</summary>
    private static readonly KeyCode[] InvalidKey ={
        KeyCode.Return,
    };

    /// <summary>�f�o�b�O�p�N���X��ݒ肵�܂�<br />
    /// SetKeyDebug(�C���X�y�N�^�[��ɂ���Q�[���I�u�W�F�N�g(������), <br />
    /// new KeyDebugData("�C���X�y�N�^�[�ɕ\�����閼�O",���s�֐��P),�@//actionList[0]<br />
    /// new KeyDebugData("�C���X�y�N�^�[�ɕ\�����閼�O",���s�֐��Q),  //actionList[1]<br />
    /// new KeyDebugData("�C���X�y�N�^�[�ɕ\�����閼�O",���s�֐��R))  //actionList[2]<br />
    /// �̗l�Ȍ`�œn���Ă��������B
    /// </summary>
    ///<param name="methodName">�C���X�y�N�^�[��ɕ\������֐�</param>
    ///<param name="action">���s�֐�</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void AddKeyAction(string methodName, UnityAction action) {
        if (!m_instance) { CreateInstance(); }
        m_instance.AddAction(methodName, action);      
    }



//private 
    
    private KeyDebug() { }

    private void Update() {
            CheckKey();
        }

    private void CheckKey() {
            if (Input.anyKeyDown){
                for (int i = 0; i < m_codeList.Count; i++) {
                    var code = m_keyCods[i];
                    if (Input.GetKeyDown(code)){
                        if (m_codeList.TryGetValue(code, out var action)) {
                            Debug.Log(m_actioString[i] + " �����s����܂�");
                            action();
                        }                        
                        break;
                    }
                }             
            }
        }

    private void AddAction(string methodName, UnityAction action) {
        for (int i = 0; i < InvalidKey.Length; i++) {
            if (m_keyCods[m_keyIndex] == InvalidKey[i]) {
                m_actioString[m_keyIndex] = "--------------------------------------------------------------------------------";
                m_keyIndex += m_keyIndex < m_keyCods.Length ? 1 : -m_keyCods.Length;
                i = 0;
            }
        }
        m_codeList.Add(m_keyCods[m_keyIndex], action);
        m_instance.m_actioString[m_keyIndex] = methodName;
        m_keyIndex += m_keyIndex < m_keyCods.Length ? 1 : -m_keyCods.Length;        
    }
    
    //�C���X�^���X���쐬���܂�
    private static void CreateInstance() {
        m_instance = new GameObject("KeyDebug").AddComponent<KeyDebug>();
        int length = UsefulSystem.GetEnumLength<DebugKeyCodes>();
        m_instance.m_actioString = new string[length];
        m_instance.m_codeList = new Dictionary<KeyCode, UnityAction>(length);
        m_instance.m_keyIndex = 0;
        DontDestroyOnLoad(m_instance.gameObject);
        //������₷���悤�Ƀq�G�����L�[�̈�ԏ�Ɉړ�������
        m_instance.gameObject.transform.SetAsFirstSibling();    
    }

//Variable
        //�C���X�y�N�^�[��ɕ\������
        [SerializeField,EnumIndex(typeof(DebugKeyCodes))]
        private string[] m_actioString;
        //�L�[�R�[�h�ƑΉ�����f���Q�[�h
        private Dictionary<KeyCode, UnityAction> m_codeList;
        //�C���X�^���X
        private static  KeyDebug m_instance = null;
        //�L�[�R�[�h�̃C���f�b�N�X
        private int m_keyIndex;

    /// <summary>�Ή�����L�[���X�g�ł� �L�������L�[�͔͂����Đ����ƃA���t�@�x�b�g�݂̂Ɍ��肵�Ă��܂�(Element�\���p)</summary>
    private enum DebugKeyCodes {
        Alpha1,
        Alpha2,
        Alpha3,
        Alpha4,
        Alpha5,
        Alpha6,
        Alpha7,
        Alpha8,
        Alpha9,
        Alpha0,
        Q,
        W,
        E,
        R,
        T,
        Y,
        U,
        I,
        O,
        P,
        A,
        S,
        D,
        F,
        G,
        H,
        J,
        K,
        L,
        Z,
        X,
        C,
        V,
        B,
        N,
        M,
    };

    /// <summary>�Ή�����L�[���X�g�̎���</summary>
    private KeyCode[] m_keyCods = {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Alpha0,
            KeyCode.Q,
            KeyCode.W,
            KeyCode.E,
            KeyCode.R,
            KeyCode.T,
            KeyCode.Y,
            KeyCode.U,
            KeyCode.I,
            KeyCode.O,
            KeyCode.P,
            KeyCode.A,
            KeyCode.S,
            KeyCode.D,
            KeyCode.F,
            KeyCode.G,
            KeyCode.H,
            KeyCode.J,
            KeyCode.K,
            KeyCode.L,
            KeyCode.Z,
            KeyCode.X,
            KeyCode.C,
            KeyCode.V,
            KeyCode.B,
            KeyCode.N,
            KeyCode.M,
        };
 }
