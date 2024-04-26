using UnityEngine;

/// <summary>
/// MonoBehaviour�ɑΉ������V���O���g���N���X
/// �i��jpublic class GameManager : SingletonMonoBehaviour<GameManager>
/// </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T m_instance;

    /// <summary>�C���X�^���X���擾���܂�</summary>
    public static T Instance
    {
        get
        {
            m_instance ??= (T)FindObjectOfType(typeof(T));
            Debug.Assert(m_instance != null, typeof(T) + "���A�^�b�`���Ă���GameObject������܂���");
            return m_instance;
        }
    }

    virtual protected void Awake()
    {
        // ����GameObject�ɃA�^�b�`����Ă��邩���ׂ�
        if (this != Instance)
        {
            // �A�^�b�`����Ă���ꍇ�͔j������
#if UNITY_EDITOR
            Debug.LogWarning("����"+ typeof(T)+"������̂ŃI�u�W�F�N�g���j������܂�");
#endif
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
