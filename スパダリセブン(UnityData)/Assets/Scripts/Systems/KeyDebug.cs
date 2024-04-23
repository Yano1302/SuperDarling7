using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// keyCodeから関数を呼び出します。
/// static変数のSetKeyDebugを呼び出して使用してください
/// 設定したUnityActionの順番に応じてキーが割り振られます
/// </summary>
public class KeyDebug:MonoBehaviour {

    /// <summary>割り振られて欲しくないキーを設定できます</summary>
    private static readonly KeyCode[] InvalidKey ={
        KeyCode.Return,
    };

    /// <summary>デバッグ用クラスを設定します<br />
    /// SetKeyDebug(インスペクター上にあるゲームオブジェクト(第一引数), <br />
    /// new KeyDebugData("インスペクターに表示する名前",実行関数１),　//actionList[0]<br />
    /// new KeyDebugData("インスペクターに表示する名前",実行関数２),  //actionList[1]<br />
    /// new KeyDebugData("インスペクターに表示する名前",実行関数３))  //actionList[2]<br />
    /// の様な形で渡してください。
    /// </summary>
    ///<param name="methodName">インスペクター上に表示する関数</param>
    ///<param name="action">実行関数</param>
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
                            Debug.Log(m_actioString[i] + " が実行されます");
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
    
    //インスタンスを作成します
    private static void CreateInstance() {
        m_instance = new GameObject("KeyDebug").AddComponent<KeyDebug>();
        int length = UsefulSystem.GetEnumLength<DebugKeyCodes>();
        m_instance.m_actioString = new string[length];
        m_instance.m_codeList = new Dictionary<KeyCode, UnityAction>(length);
        m_instance.m_keyIndex = 0;
        DontDestroyOnLoad(m_instance.gameObject);
        //分かりやすいようにヒエラルキーの一番上に移動させる
        m_instance.gameObject.transform.SetAsFirstSibling();    
    }

//Variable
        //インスペクター上に表示する
        [SerializeField,EnumIndex(typeof(DebugKeyCodes))]
        private string[] m_actioString;
        //キーコードと対応するデリゲード
        private Dictionary<KeyCode, UnityAction> m_codeList;
        //インスタンス
        private static  KeyDebug m_instance = null;
        //キーコードのインデックス
        private int m_keyIndex;

    /// <summary>対応するキーリストです 記号や特殊キーはは避けて数字とアルファベットのみに限定しています(Element表示用)</summary>
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

    /// <summary>対応するキーリストの実数</summary>
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
