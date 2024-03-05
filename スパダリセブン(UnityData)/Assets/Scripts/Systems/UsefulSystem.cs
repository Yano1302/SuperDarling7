using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;




/// <summary>
///  �Q�[������Ŏg�������ȃV�X�e�����L�ڂ��Ă����N���X�ł��B<br />
///  �N���X�̃C���X�^���X��GameSystem.Instance����擾�ł��܂�
///  �e���ǉ��������@�\������΋L�q���Ă��������B<br /> 
///  
///  �쐬�҂�PG2�N�̖��m���ł��@�^��_����P�_�A�o�O�񍐓�����΋C�y�ɂ��A����������<br />
///  
/// <br />---�ȉ��L�q�̍ۂ̒��ӎ���---------------<br />
///  �L�q�������ꍇ�͋�����h���ׂɈꉞ�񍐂��s���Ă���L�q���Ă��������B<br />
///  �܂��A�\�Ȍ���A�^�b�`�͎g�p�����ɋL�ڂ��Ă��������B<br />
///
///  �����o�ϐ��̐錾����s�����֐��𕪂��铙�̏ꍇ�ɂ͉��̕��ɂ���
///  �Q�[���V�X�e���̍쐬�̉ӏ��ŋL�q�����肢���܂��B<br />
///  �܂��A�L�ڂ̍ۂ�summary�R�����g�����o�������c���Ă��������B
///  
/// </summary>
public class UsefulSystem : SingletonMonoBehaviour<UsefulSystem>
{
    //  �֐��ꗗ    //

    //  Static�֐�    //-------------------------------------------------------------------------------------------------------------------------------

    /// <summary>�t�@�C����(�g���q����)����t�@�C���p�X���������܂�</summary>
    /// <param name="FileName">�t�@�C����(�g���q����)</param>
    /// <returns>�t�@�C���p�X�����������ꍇ�Ƀp�X��Ԃ��܂�</returns>
    public static string FindFilePath(string FileName)
    {
        var paths = Directory.GetFiles(Application.dataPath, FileName, SearchOption.AllDirectories);
        if (paths != null && paths.Length > 0)
        {
            return paths[0].Replace("\\", "/").Replace(Application.dataPath, "Assets");
        }
        LogError("�t�@�C����������܂���ł���   �t�@�C���� : " + FileName);
        return null;
    }

    /// <summary>�e�L�X�g�t�@�C������s���ǂݍ��݂܂�</summary>
    /// <param name="FilePath">�e�L�X�g�t�@�C���̃p�X</param>
    /// <returns>�e�L�X�g�t�@�C����ǂݍ���List</returns>
    public static List<string> Reader_TextFile(string FilePath)
    {
        StreamReader sr = new StreamReader(FilePath);
        List<string> lines = new List<string>();
        while (sr.Peek() != -1)
        {
            lines.Add(sr.ReadLine());
        }
        sr.Close();
        return lines;
    }

    /// <summary>Vector3�̗v�f�����m���|�����킹�܂�</summary>
    /// <returns>�|�����킹��Vector3</returns>
    public static Vector3 Mul(in Vector3 a, in Vector3 b) { return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z); }

    /// <summary>�񋓌^�̕ϐ���int�^�ɃL���X�g���܂��B�r�b�g�t���O���g�p���Ă���ꍇ�͗񋓌^���̂�[Flags]������t���Ă�������</summary>
    /// <param name="type">�L���X�g�������ϐ�</param>
    /// <returns>�L���X�g��̐���</returns>
    public static int EnumCast<T>(T type) where T : Enum
    {
        if (CheckAttribute<FlagsAttribute,T>(type)){ return GetIndexForBitFlag(type); }
        else{ return CastTo<int>.From(type); }
    }

    /// <summary>�񋓌^�̍��ڐ����擾���܂�</summary>
    /// <typeparam name="T">�񋓌^�̌^</typeparam>
    /// <returns>�񋓌^�̍��ڐ�</returns>
    public static int GetEnumLength<T>() where T : Enum { return Enum.GetValues(typeof(T)).Length; }

    //  �f�o�b�O�p   //--------------------------------------------------------------------------------------------------------------------------------

    /// <summary> �G�f�B�^��ł̂ݗL���ȃ��O��\�����܂�</summary>
    [Conditional("UNITY_EDITOR")] public static void Log(object o) { Debug.Log(o); }
    /// <summary> �G�f�B�^��ł̂ݗL����Warning���O��\�����܂�</summary>
    [Conditional("UNITY_EDITOR")] public static void LogWarning(object o) { Debug.LogWarning(o); }
    /// <summary> �G�f�B�^��ł̂ݗL����Error���O��\�����܂�</summary>
    [Conditional("UNITY_EDITOR")] public static void LogError(object o) { Debug.LogError(o); }
    /// <summary>�G�f�B�^��ł̂ݗL���Ȋ֐������s���܂�</summary>
    [Conditional("UNITY_EDITOR")] public static void DebugAction(UnityAction action) { action(); }


    //  Public�֐�    //--------------------------------------------------------------------------------------------------------------------------------

    /// <summary> ��莞�ԑҋ@�������ƁA�n���ꂽ�֐������s���܂��B
    /// <br />���Ăяo�����̃^�C���X�P�[���͖�������܂�</summary>
    /// <param name="waitSeconds">�ҋ@�b��</param>
    /// <param name="action">���s����֐�</param>
    public void WaitCallBack(float waitSeconds, UnityAction action) 
    {
        Debug.Assert(0 < waitSeconds, "�^�C�}�[�����ɂO�ȉ��ł�");
        StartCoroutine(WaitCall(waitSeconds, action)); 
    }

    /// <summary>��莞�ԑҋ@�������ƁA�n���ꂽ�֐������s���܂��B
    /// <br />�Ăяo������Timer[index]���̐����𑝌�������ƃ^�C�}�[�ɔ��f����܂�(���₷�Ǝ��Ԃ����т�)
    /// <br />���Ăяo�����̃^�C���X�P�[���͖�������܂�</summary>
    /// <param name="Timer">�^�C�}�[���i�[���Ă���z��</param>
    /// <param name="index">�z��̃C���f�b�N�X</param>
    /// <param name="action">���s����֐�</param>
    public void WaitCallBack(float[] Timer, int index, UnityAction action)
    {
        Debug.Assert(index < Timer.Length, "�C���f�b�N�X���^�C�}�[�z��͈̔͊O�ł�");
        Debug.Assert(0 < Timer[index], "�^�C�}�[�����ɂO�ȉ��ł�");

        Func<Task> AsyncFunc = async () =>
        {
            if(Time.timeScale == 0)
            {
                while (0 <= Timer[index])
                {
                    await Task.Delay((int)(Time.unscaledDeltaTime * 1000));
                    Timer[index] -= Time.unscaledDeltaTime;
                }             
            }
            else
            {
                while (0 <= Timer[index])
                {
                    await Task.Delay((int)(Time.deltaTime / Time.timeScale * 1000));
                    Timer[index] -= Time.deltaTime;
                }
            }
            action();
        };
        AsyncFunc();
    }

    /// <summary>�Q�[�����I�����܂�</summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }

    public void InputAction(KeyCode code, UnityAction action) {
        if (Input.GetKeyDown(code)) {
            action();
        }
    }

    //MonoBehaviour�̋@�\�̈ϑ�-------------------------------------------------------------------------------------------------------------------------


    /// <summary>�n���ꂽ�I�u�W�F�N�g�𐶐����܂�</summary>
    /// <param name="obj">�����������Q�[���I�u�W�F�N�g���</param>
    /// <returns>������̃Q�[���I�u�W�F�N�g</returns>
    public GameObject Mono_Instantiate(GameObject obj) { return Instantiate(obj); }

    /// <summary>�n���ꂽ�I�u�W�F�N�g�𐶐����܂�</summary>
    /// <param name="obj">�����������Q�[���I�u�W�F�N�g���</param>
    /// <param name="vector">�����������ʒu</param>
    /// <param name="rotate">�����������p�x</param>
    /// <returns>���������Q�[���I�u�W�F�N�g</returns>
    public GameObject Mono_Instantiate(GameObject obj, in Vector3 vector, in Vector3 rotate)
    {
        Quaternion q = Quaternion.Euler(rotate);
        return Instantiate(obj, vector, q);
    }

    /// <summary>�Q�[���I�u�W�F�N�g��j�����܂�</summary>
    /// <param name="obj"></param>
    public void Mono_Destroy(GameObject obj) { Destroy(obj); }

    /// <summary>�Q�[���I�u�W�F�N�g��j�����܂�</summary>
    /// <param name="obj">�j�󂵂����Q�[���I�u�W�F�N�g</param>
    /// <param name="t">�j�󂷂�܂ł̎���</param>
    public void Mono_Destroy(GameObject obj, float t) { Destroy(obj, t); }

    /// <summary>�n���ꂽ�I�u�W�F�N�g�̃R���|�[�l���g���擾���܂�</summary>
    /// <typeparam name="T">�擾�������R���|�[�l���g</typeparam>
    /// <param name="obj">�擾�������R���|�[�l���g���������Ă���I�u�W�F�N�g</param>
    /// <returns>�擾�����R���|�[�l���g</returns>
    public T Mono_GetComponent<T>(GameObject obj) { return obj.GetComponent<T>(); }

    /// <summary>�I�u�W�F�N�g�ɃR���|�[�l���g��ݒ肵�܂�</summary>
    /// <typeparam name="T">�ݒ肵�����R���|�[�l���g</typeparam>
    /// <param name="obj">�R���|�[�l���g��ݒ肵�����I�u�W�F�N�g</param>
    /// <returns>�ݒ肵���R���|�[�l���g</returns>
    public T Mono_AddComponent<T>(GameObject obj) where T : Component { return obj.AddComponent<T>(); }


    //--------------------------------------------------------------------------------------------------------------------------------------------------

    #region private�ϐ��E�֐�

    //  Private�֐��E�ϐ�    //-------------------------------------------------------------------------------------------------------------------------

    /// <summary>�x�����s�֐�</summary>
    private IEnumerator WaitCall(float waitTime, UnityAction action)
    {
        if (Time.timeScale <= 0)
        {
            float timer = 0;
            while (timer < waitTime)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            action();
        }
        else
        {
            yield return new WaitForSeconds(waitTime / Time.timeScale);
            action();
        }
    }

    /// <summary>�w�肳�ꂽ���������邩�ǂ������ׂ܂�</summary>
    /// <typeparam name="Attribute">���ׂ鑮��</typeparam>
    /// <typeparam name="C">���ׂ�^</typeparam>
    /// <returns>�w�肳�ꂽ�����������Ă����ꍇ��true</returns>
    private static bool CheckAttribute<Attribute,C>(C checkType) where Attribute : System.Attribute
    {
        System.Attribute attr = System.Attribute.GetCustomAttribute(checkType.GetType(), typeof(Attribute));
        bool check = attr != null ? true : false;
        return check;        
    }

    /// <summary>�r�b�g�t���O���g�p�����񋓌^�̃C���f�b�N�X�ԍ����擾���܂�</summary>
    private static int GetIndexForBitFlag<T>(T t) where T : Enum
    {
        int length = Enum.GetNames(typeof(T)).Length;
        int type = CastTo<int>.From(t);

        if(type == 0)
            return 0;

        for (int i = 0; i < length; i++)
        {
            int index = (int)Mathf.Pow(2,i);
            if (index == type)
            {
                return index;
            }
        }
        Debug.LogError("��v����l��������܂���ł��� : " + type);
        return 0;
    }

    /// <summary>�L���X�g�p�N���X</summary>
    /// <typeparam name="T">�L���X�g��̌^</typeparam>
    private static class CastTo<T>
    {
        private static class Cache<S>
        {
            static Cache()
            {
                // ���̃����_�������؂ō\�z
                // (S s) => (T)s
                var p = Expression.Parameter(typeof(S));
                var c = Expression.ConvertChecked(p, typeof(T));
                Caster = Expression.Lambda<Func<S, T>>(c, p).Compile();
            }
            internal static readonly Func<S, T> Caster;
        }

        /// <summary>�L���X�g���܂� </summary>
        /// <param name="source">�L���X�g���̌^���������ϐ�(�^���_�p)</param>
        /// <returns></returns>
        public static T From<S>(S source)
        {
            return Cache<S>.Caster(source);
        }
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------
    #endregion
}