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
///  ゲーム制作で使いそうなシステムを記載していくクラスです。<br />
///  クラスのインスタンスはGameSystem.Instanceから取得できます
///  各自追加したい機能があれば記述してください。<br /> 
///  
///  作成者はPG2年の矢野洋平です　疑問点や改善点、バグ報告等あれば気軽にご連絡ください<br />
///  
/// <br />---以下記述の際の注意事項---------------<br />
///  記述したい場合は競合を防ぐ為に一応報告を行ってから記述してください。<br />
///  また、可能な限りアタッチは使用せずに記載してください。<br />
///
///  メンバ変数の宣言や実行処理関数を分ける等の場合には下の方にある
///  ゲームシステムの作成の箇所で記述をお願いします。<br />
///  また、記載の際にsummaryコメント等を出来る限り残してください。
///  
/// </summary>
public class UsefulSystem : SingletonMonoBehaviour<UsefulSystem>
{
    //  関数一覧    //

    //  Static関数    //-------------------------------------------------------------------------------------------------------------------------------

    /// <summary>ファイル名(拡張子込み)からファイルパスを検索します</summary>
    /// <param name="FileName">ファイル名(拡張子込み)</param>
    /// <returns>ファイルパスが見つかった場合にパスを返します</returns>
    public static string FindFilePath(string FileName)
    {
        var paths = Directory.GetFiles(Application.dataPath, FileName, SearchOption.AllDirectories);
        if (paths != null && paths.Length > 0)
        {
            return paths[0].Replace("\\", "/").Replace(Application.dataPath, "Assets");
        }
        LogError("ファイルが見つかりませんでした   ファイル名 : " + FileName);
        return null;
    }

    /// <summary>テキストファイルを一行ずつ読み込みます</summary>
    /// <param name="FilePath">テキストファイルのパス</param>
    /// <returns>テキストファイルを読み込んだList</returns>
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

    /// <summary>Vector3の要素数同士を掛け合わせます</summary>
    /// <returns>掛け合わせたVector3</returns>
    public static Vector3 Mul(in Vector3 a, in Vector3 b) { return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z); }

    /// <summary>列挙型の変数をint型にキャストします。ビットフラグを使用している場合は列挙型自体に[Flags]属性を付けてください</summary>
    /// <param name="type">キャストしたい変数</param>
    /// <returns>キャスト後の数字</returns>
    public static int EnumCast<T>(T type) where T : Enum
    {
        if (CheckAttribute<FlagsAttribute,T>(type)){ return GetIndexForBitFlag(type); }
        else{ return CastTo<int>.From(type); }
    }

    /// <summary>列挙型の項目数を取得します</summary>
    /// <typeparam name="T">列挙型の型</typeparam>
    /// <returns>列挙型の項目数</returns>
    public static int GetEnumLength<T>() where T : Enum { return Enum.GetValues(typeof(T)).Length; }

    //  デバッグ用   //--------------------------------------------------------------------------------------------------------------------------------

    /// <summary> エディタ上でのみ有効なログを表示します</summary>
    [Conditional("UNITY_EDITOR")] public static void Log(object o) { Debug.Log(o); }
    /// <summary> エディタ上でのみ有効なWarningログを表示します</summary>
    [Conditional("UNITY_EDITOR")] public static void LogWarning(object o) { Debug.LogWarning(o); }
    /// <summary> エディタ上でのみ有効なErrorログを表示します</summary>
    [Conditional("UNITY_EDITOR")] public static void LogError(object o) { Debug.LogError(o); }
    /// <summary>エディタ上でのみ有効な関数を実行します</summary>
    [Conditional("UNITY_EDITOR")] public static void DebugAction(UnityAction action) { action(); }


    //  Public関数    //--------------------------------------------------------------------------------------------------------------------------------

    /// <summary> 一定時間待機したあと、渡された関数を実行します。
    /// <br />※呼び出し時のタイムスケールは無視されます</summary>
    /// <param name="waitSeconds">待機秒数</param>
    /// <param name="action">実行する関数</param>
    public void WaitCallBack(float waitSeconds, UnityAction action) 
    {
        Debug.Assert(0 < waitSeconds, "タイマーが既に０以下です");
        StartCoroutine(WaitCall(waitSeconds, action)); 
    }

    /// <summary>一定時間待機したあと、渡された関数を実行します。
    /// <br />呼び出し元でTimer[index]内の数字を増減させるとタイマーに反映されます(増やすと時間が延びる)
    /// <br />※呼び出し時のタイムスケールは無視されます</summary>
    /// <param name="Timer">タイマーを格納している配列</param>
    /// <param name="index">配列のインデックス</param>
    /// <param name="action">実行する関数</param>
    public void WaitCallBack(float[] Timer, int index, UnityAction action)
    {
        Debug.Assert(index < Timer.Length, "インデックスがタイマー配列の範囲外です");
        Debug.Assert(0 < Timer[index], "タイマーが既に０以下です");

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

    /// <summary>ゲームを終了します</summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    public void InputAction(KeyCode code, UnityAction action) {
        if (Input.GetKeyDown(code)) {
            action();
        }
    }

    //MonoBehaviourの機能の委託-------------------------------------------------------------------------------------------------------------------------


    /// <summary>渡されたオブジェクトを生成します</summary>
    /// <param name="obj">生成したいゲームオブジェクト情報</param>
    /// <returns>生成後のゲームオブジェクト</returns>
    public GameObject Mono_Instantiate(GameObject obj) { return Instantiate(obj); }

    /// <summary>渡されたオブジェクトを生成します</summary>
    /// <param name="obj">生成したいゲームオブジェクト情報</param>
    /// <param name="vector">生成したい位置</param>
    /// <param name="rotate">生成したい角度</param>
    /// <returns>生成したゲームオブジェクト</returns>
    public GameObject Mono_Instantiate(GameObject obj, in Vector3 vector, in Vector3 rotate)
    {
        Quaternion q = Quaternion.Euler(rotate);
        return Instantiate(obj, vector, q);
    }

    /// <summary>ゲームオブジェクトを破棄します</summary>
    /// <param name="obj"></param>
    public void Mono_Destroy(GameObject obj) { Destroy(obj); }

    /// <summary>ゲームオブジェクトを破棄します</summary>
    /// <param name="obj">破壊したいゲームオブジェクト</param>
    /// <param name="t">破壊するまでの時間</param>
    public void Mono_Destroy(GameObject obj, float t) { Destroy(obj, t); }

    /// <summary>渡されたオブジェクトのコンポーネントを取得します</summary>
    /// <typeparam name="T">取得したいコンポーネント</typeparam>
    /// <param name="obj">取得したいコンポーネントを所持しているオブジェクト</param>
    /// <returns>取得したコンポーネント</returns>
    public T Mono_GetComponent<T>(GameObject obj) { return obj.GetComponent<T>(); }

    /// <summary>オブジェクトにコンポーネントを設定します</summary>
    /// <typeparam name="T">設定したいコンポーネント</typeparam>
    /// <param name="obj">コンポーネントを設定したいオブジェクト</param>
    /// <returns>設定したコンポーネント</returns>
    public T Mono_AddComponent<T>(GameObject obj) where T : Component { return obj.AddComponent<T>(); }


    //--------------------------------------------------------------------------------------------------------------------------------------------------

    #region private変数・関数

    //  Private関数・変数    //-------------------------------------------------------------------------------------------------------------------------

    /// <summary>遅延実行関数</summary>
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

    /// <summary>指定された属性があるかどうか調べます</summary>
    /// <typeparam name="Attribute">調べる属性</typeparam>
    /// <typeparam name="C">調べる型</typeparam>
    /// <returns>指定された属性を持っていた場合はtrue</returns>
    private static bool CheckAttribute<Attribute,C>(C checkType) where Attribute : System.Attribute
    {
        System.Attribute attr = System.Attribute.GetCustomAttribute(checkType.GetType(), typeof(Attribute));
        bool check = attr != null ? true : false;
        return check;        
    }

    /// <summary>ビットフラグを使用した列挙型のインデックス番号を取得します</summary>
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
        Debug.LogError("一致する値が見つかりませんでした : " + type);
        return 0;
    }

    /// <summary>キャスト用クラス</summary>
    /// <typeparam name="T">キャスト後の型</typeparam>
    private static class CastTo<T>
    {
        private static class Cache<S>
        {
            static Cache()
            {
                // 次のラムダ式を式木で構築
                // (S s) => (T)s
                var p = Expression.Parameter(typeof(S));
                var c = Expression.ConvertChecked(p, typeof(T));
                Caster = Expression.Lambda<Func<S, T>>(c, p).Compile();
            }
            internal static readonly Func<S, T> Caster;
        }

        /// <summary>キャストします </summary>
        /// <param name="source">キャスト元の型を持った変数(型推論用)</param>
        /// <returns></returns>
        public static T From<S>(S source)
        {
            return Cache<S>.Caster(source);
        }
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------
    #endregion
}