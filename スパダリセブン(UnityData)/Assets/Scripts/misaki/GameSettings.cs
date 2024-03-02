using UnityEngine;
//ゲームの設定用ScriptableObject
[CreateAssetMenu(menuName = "ScriptableObject/GameSettings", fileName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("デバッグログを表示したい場合はチェックをつける")]
    public bool debugLogEnabled; //デバッグログを表示するか
}
