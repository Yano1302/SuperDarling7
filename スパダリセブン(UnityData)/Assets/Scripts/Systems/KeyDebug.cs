using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// keyCode����֐����Ăяo���܂��B
/// static�ϐ���SetKeyDebug���Ăяo���Ďg�p���Ă�������
/// �ݒ肵��UnityAction�̏��Ԃɉ����ăL�[������U���܂�
/// 
/// actionlist[0] = KeyCode.1,actionlist[1] = KeyCode.2...actionlist[9] = KeyCode.0,
/// 
/// actionlist[10] = KeyCode.Q ... actionlist[19] = KeyCode.P
/// 
/// actionlist[20] = KeyCode.A...
///                     .
///                     .
///                     .
///  �̗l�ɍ���̃L�[���珇�ɉE�����Ɋ���U���܂�(�ő劄�蓖�Đ���36�܂łł��@��1�`0�܂ł�10�{Q�`M�܂ł�26)
/// </summary>
public class KeyDebug:MonoBehaviour {


    /// <summary>�f�o�b�O�p�N���X��ݒ肵�܂�<br />
    /// SetKeyDebug(�C���X�y�N�^�[��ɂ���Q�[���I�u�W�F�N�g(������), <br />
    /// new KeyDebugData("�C���X�y�N�^�[�ɕ\�����閼�O",���s�֐��P),�@//actionList[0]<br />
    /// new KeyDebugData("�C���X�y�N�^�[�ɕ\�����閼�O",���s�֐��Q),  //actionList[1]<br />
    /// new KeyDebugData("�C���X�y�N�^�[�ɕ\�����閼�O",���s�֐��R))  //actionList[2]<br />
    /// �̗l�Ȍ`�œn���Ă��������B
    /// </summary>
    /// <param name="obj">�ݒ肷��I�u�W�F�N�g</param>
    /// <param name="actionList">���s�֐��ꗗ</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void AddKeyDebug(string methodName, UnityAction action) {
        if (!m_instance) { CreateInstance(); }
        m_instance.AddAction(methodName, action);      
    }


    /// <summary>�Ή�����L�[���X�g�ł�</summary>
    private enum DebugKeyCodes{
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



//private 

    private void Update() {
            CheckKey();
        }

    private void CheckKey() {
            if (Input.anyKeyDown){
                for (int i = 0; i < m_codeList.Count; i++) {
                    var code = m_keyCods[i];
                    if (Input.GetKeyDown((KeyCode)code)){
                        if (m_codeList.TryGetValue((KeyCode)code, out var action)) {
                            Debug.Log(action.ToString() + " �����s����܂�");
                            action();
                        }                        
                        break;
                    }
                }             
            }
        }

    private void AddAction(string methodName, UnityAction action) {
        m_codeList.Add(m_keyCods[m_next], action);
        m_instance.ActionListToString[m_next] = methodName;
        m_next = m_next > UsefulSystem.GetEnumLength<DebugKeyCodes>() ? m_next++ : 0;
    }
       
    private static void CreateInstance() {
        m_instance = new GameObject("KeyDebug").AddComponent<KeyDebug>();
        int length = UsefulSystem.GetEnumLength<DebugKeyCodes>();
        m_instance.ActionListToString = new string[length];
        m_instance.m_codeList = new Dictionary<KeyCode?, UnityAction>(length);
        m_instance.m_next = 0;
        DontDestroyOnLoad(m_instance.gameObject);
        m_instance.gameObject.transform.SetAsFirstSibling();
    }

//Variable

        //�C���X�y�N�^�[��ɕ\������
        [SerializeField,EnumIndex(typeof(DebugKeyCodes))]
        private string[] ActionListToString;
        //�L�[�R�[�h�ƑΉ�����f���Q�[�h
        private Dictionary<KeyCode?, UnityAction> m_codeList;
        //�C���X�^���X
        private static  KeyDebug m_instance = null;
        //���Ɏg�p����L�[�R�[�h
        private int m_next;

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
