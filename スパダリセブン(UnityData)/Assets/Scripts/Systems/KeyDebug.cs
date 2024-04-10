using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// keyCodeから関数を呼び出します。
/// static変数のSetKeyDebugを呼び出して使用してください
/// 設定したUnityActionの順番に応じてキーが割り振られます
/// 
/// actionlist[0] = KeyCode.1,actionlist[1] = KeyCode.2...actionlist[9] = KeyCode.0,
/// 
/// actionlist[10] = KeyCode.Q ... actionlist[19] = KeyCode.P
/// 
/// actionlist[20] = KeyCode.A...
///                     .
///                     .
///                     .
///  の様に左上のキーから順に右方向に割り振られます(最大割り当て数は36個までです　※1～0までの10＋Q～Mまでの26)
/// </summary>
public class KeyDebug:MonoBehaviour {


    /// <summary>デバッグ用クラスを設定します<br />
    /// SetKeyDebug(インスペクター上にあるゲームオブジェクト(第一引数), <br />
    /// new KeyDebugData("インスペクターに表示する名前",実行関数１),　//actionList[0]<br />
    /// new KeyDebugData("インスペクターに表示する名前",実行関数２),  //actionList[1]<br />
    /// new KeyDebugData("インスペクターに表示する名前",実行関数３))  //actionList[2]<br />
    /// の様な形で渡してください。
    /// </summary>
    /// <param name="obj">設定するオブジェクト</param>
    /// <param name="actionList">実行関数一覧</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void AddKeyDebug(string methodName, UnityAction action) {
        if (!m_instance) { CreateInstance(); }
        m_instance.AddAction(methodName, action);      
    }


    /// <summary>対応するキーリストです</summary>
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
                            Debug.Log(action.ToString() + " が実行されます");
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

        //インスペクター上に表示する
        [SerializeField,EnumIndex(typeof(DebugKeyCodes))]
        private string[] ActionListToString;
        //キーコードと対応するデリゲード
        private Dictionary<KeyCode?, UnityAction> m_codeList;
        //インスタンス
        private static  KeyDebug m_instance = null;
        //次に使用するキーコード
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
