using UnityEngine;
//ゲームの設定用ScriptableObject
[CreateAssetMenu(menuName = "ScriptableObject/DebugSettings", fileName = "DebugSettings")]
public class DebugSettings : ScriptableObject
{
    [Header("デバッグログを表示したい場合はチェックをつける")]
    public bool debugLogEnabled; //デバッグログを表示するか
}
