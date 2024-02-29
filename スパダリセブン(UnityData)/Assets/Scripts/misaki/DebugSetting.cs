using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetting : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings;    //ゲームの設定データ(追記)
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        Debug.unityLogger.logEnabled = gameSettings.debugLogEnabled;   //デバッグログを非表示にする
    }
}
