using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 岬作成のデバッグログを表示するかどうかを決めるスクリプトです
// このスクリプトを継承すれば、継承したスクリプトに記載したデバッグログを表示の有無をProject上で設定できます
// Project上のScriptableObjects/GameSettingsのチェックの有無でデバッグログを表示するかしないかが決まります
public class DebugSetting : MonoBehaviour
{
    [SerializeField] private DebugSettings debugSettings;    //ゲームの設定データ(追記)
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        Debug.unityLogger.logEnabled = debugSettings.debugLogEnabled;   //デバッグログを非表示にする
    }
}
