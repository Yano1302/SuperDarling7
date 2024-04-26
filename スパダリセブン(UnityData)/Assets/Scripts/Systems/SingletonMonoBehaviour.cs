using UnityEngine;

/// <summary>
/// MonoBehaviourに対応したシングルトンクラス
/// （例）public class GameManager : SingletonMonoBehaviour<GameManager>
/// </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T m_instance;

    /// <summary>インスタンスを取得します</summary>
    public static T Instance
    {
        get
        {
            m_instance ??= (T)FindObjectOfType(typeof(T));
            Debug.Assert(m_instance != null, typeof(T) + "をアタッチしているGameObjectがありません");
            return m_instance;
        }
    }

    virtual protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる
        if (this != Instance)
        {
            // アタッチされている場合は破棄する
#if UNITY_EDITOR
            Debug.LogWarning("既に"+ typeof(T)+"があるのでオブジェクトが破棄されます");
#endif
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
