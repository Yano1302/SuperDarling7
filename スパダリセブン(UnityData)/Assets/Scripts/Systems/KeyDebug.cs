using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class KeyDebugData {
    private KeyDebugData() { }
    public KeyDebugData(string methodName, UnityAction action) { this.methodName = methodName; this.action = action; }
    public UnityAction action;
    public string methodName;
}

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
        public static void SetKeyDebug(GameObject obj,params KeyDebugData[]actionList) {
            Debug.Assert(actionList.Length <= DebugKeyCodes.Length,"�n�����֐����Ή�����L�[��葽���̂ŃL�[������U���Ă��Ȃ��֐�������܂��B");
            obj.AddComponent<KeyDebug>().SetAction(actionList);
        }


    /// <summary>�Ή�����L�[���X�g�ł�</summary>
    private static readonly KeyCode[] DebugKeyCodes = {
            KeyCode.Alpha1,   //actionlist[0]
            KeyCode.Alpha2,   //actionlist[1]         
            KeyCode.Alpha3,   //actionlist[2]
            KeyCode.Alpha4,   //actionlist[3]
            KeyCode.Alpha5,   //actionlist[4]
            KeyCode.Alpha6,   //actionlist[5]
            KeyCode.Alpha7,   //actionlist[6]
            KeyCode.Alpha8,   //actionlist[7]
            KeyCode.Alpha9,   //actionlist[8]
            KeyCode.Alpha0,   //actionlist[9]
            KeyCode.Q,        //actionlist[10]
            KeyCode.W,        //actionlist[11]
            KeyCode.E,        //actionlist[12]
            KeyCode.R,        //actionlist[13]
            KeyCode.T,        //actionlist[14]
            KeyCode.Y,        //actionlist[15]
            KeyCode.U,        //actionlist[16]
            KeyCode.I,        //actionlist[17]
            KeyCode.O,        //actionlist[18]
            KeyCode.P,        //actionlist[19]
            KeyCode.A,        //actionlist[20]
            KeyCode.S,        //actionlist[21]
            KeyCode.D,        //actionlist[22]
            KeyCode.F,        //actionlist[23]
            KeyCode.G,        //actionlist[24]
            KeyCode.H,        //actionlist[25]
            KeyCode.J,        //actionlist[26]
            KeyCode.K,        //actionlist[27]
            KeyCode.L,        //actionlist[28]
            KeyCode.Z,        //actionlist[29]
            KeyCode.X,        //actionlist[30]
            KeyCode.C,        //actionlist[31]
            KeyCode.V,        //actionlist[32]
            KeyCode.B,        //actionlist[33]
            KeyCode.N,        //actionlist[34]
            KeyCode.M,        //actionlist[35]
        };




//private 
    //Function
    public void SetAction(KeyDebugData[] actionList) {        
            m_codeList = new Dictionary<KeyCode?, UnityAction>(actionList.Length);
            ActionListToString = new string[actionList.Length];
            for (int i = 0; i < actionList.Length; i++) {
                m_codeList.Add(DebugKeyCodes[i],actionList[i].action);
            string keyName = i < 10 ? ((i + 1) % 10).ToString() : DebugKeyCodes[i].ToString();
                ActionListToString[i] = keyName + " : " + actionList[i].methodName;
            }
        }

        private void Update() {
            CheckKey();
        }

        private void CheckKey() {
            if (!m_running && Input.anyKeyDown){
                for (int i = 0; i < m_codeList.Count; i++) {
                    var code = DebugKeyCodes[i];
                    if (Input.GetKeyDown(code)){
                        if (m_codeList.TryGetValue(code, out var action)) {
                            Debug.Log(action.ToString() + " �����s����܂�");
                            action();
                        }                        
                        break;
                    }
                }             
            }
        }
       
       

//Variable

        //�C���X�y�N�^�[��ɕ\������
        [SerializeField]
        private string[] ActionListToString;
        //�L�[�R�[�h�ƑΉ�����f���Q�[�h
        private Dictionary<KeyCode?, UnityAction> m_codeList;
        //�������s���Ă��Ȃ������肷��
        private bool m_running = false;
    }
