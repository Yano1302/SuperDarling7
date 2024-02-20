using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ウィンドウの表示を管理するクラスです<br />
/// 列挙子を記載しそれに対応するオブジェクトをWindowObjectsにアタッチして使用してください<br />
/// </summary>
public class WindowManager : SingletonMonoBehaviour<WindowManager>
{

    /// <summary>ウィンドウタイプ</summary>
    public enum WindowType
    {
        Close,          //ここは必須
        MainWindow,
        Window1, 
        Window2,         
    }


    //  パブリック変数   //--------------------------------------------------------------------------------------------------------

    /// <summary>falseの間は処理が行われません</summary>
    public bool m_IsAvailable { get; set; } = true;

    /// <summary>falseの間はウィンドウ開閉の際のディレイが発生しません</summary>
    public bool m_EnableDelay { get; set; } = false;

    /// <summary>現在のウィンドウタイプ</summary>
    public WindowType m_CurrentType { get { return m_WindowList[m_WindowList.Count - 1]; } }


    //  コンフィグ用変数    //-----------------------------------------------------------------------------------------------------

    [SerializeField,Header("ウィンドウ開閉処理から再び開閉処理に入るまでの待機時間")]
    private float c_OCDeley = 0.2f;                     


    //  アタッチ用変数   //--------------------------------------------------------------------------------------------------------

    [SerializeField, Header("ポーズ画面用の背景オブジェクト\n(必要ない場合はアタッチしなくてもかまいません)")]
    private GameObject BackGround;

    [SerializeField, EnumIndex(typeof(WindowType)), Header("ウィンドウオブジェクト一覧")]
    private GameObject[] WindowObjects;
    

    //  プライベート変数  //-------------------------------------------------------------------------------------------------------

    private List<WindowType> m_WindowList = new List<WindowType>() { WindowType.Close };         //開かれているウィンドウ一覧
   
    private GameObject m_TargetWindow { get { return GetWindowObject(m_CurrentType); } }         //開閉対象のウィンドウ


    //  記述関数    //-------------------------------------------------------------------------------------------------------------

    //ウィンドウを開く実際の処理を記述します(演出面など)　※OpenWindows等からは複数回(開くウィンドウ個数回)呼ばれます
    private void Open()
    {
       m_TargetWindow.SetActive(true);
      
    }

    //ウィンドウを閉じる実際の処理を記述します(演出面など) ※AllCloseWindow等からは複数回(閉じるウィンドウ個数回)呼ばれます
    private void Close()
    {
        m_TargetWindow.SetActive(false);
    }


    //  アタッチ等で外部から呼び出す関数    //-------------------------------------------------------------------------------------

    /// <summary>指定されたウィンドウを開きます</summary>
    public void OpenWindow(WindowType type)
    {
        if (CheckCall())
        {
            OpenSettings(type);
        }
    }

    /// <summary>指定されたウィンドウを開きます(ボタン用)</summary>
    [EnumAction(typeof(WindowType))]
    public void OpenWindow(int type){ OpenWindow((WindowType)type); }


    /// <summary>複数のウィンドウを同時に開きます</summary>
    /// <param name="windowTypes">開くウィンドウタイプの配列<br />
    /// ※配列の順番通りにウィンドウを表示していきます。<br />
    /// ただし、既に開かれているウィンドウタイプを配列に入れた場合、そのウィンドウを再表示する処理は行わないので<br />
    /// 画面のレイアウト、Closeの順番等がおかしくなる可能性があります。
    /// </param>
    public void OpenWindows(WindowType[] windowTypes)
    {
#if UNITY_EDITOR
        if(windowTypes.Length == 0)
        {
            Debug.LogError("引数の配列の中身がありません");
            return;
        }
#endif
        if (CheckCall())
        {
            for(int i = 0; i < windowTypes.Length; i++)
            {
                OpenSettings(windowTypes[i]);
            }
        }
    }


    /// <summary>現在のウィンドウを閉じます</summary>
    public void CloseWindow()
    {
        if(CheckCall())
        {
            CloseSetting();
        }
    }


    /// <summary>指定されたウィンドウを閉じます</summary>
    public void CloseWindow(WindowType type)
    {
        if (CheckCall())
        {
            if (m_WindowList.Remove(type))
            {
                //削除したい要素を最後尾にずらす
                m_WindowList.Add(type);
                CloseSetting();
            }
#if UNITY_EDITOR
            else
            {
                    Debug.LogError("指定されたウィンドウが開かれていません");
                    return;
            }
#endif
        }
    }

    /// <summary>指定されたウィンドウを閉じます(ボタン用)</summary>
    [EnumAction(typeof(WindowType))]
    public void CloseWindow(int type){ CloseWindow((WindowType)type); }


    /// <summary>現在開かれている特定のウィンドウまで戻ります<br />
    /// ※そのウィンドウから派生したウィンドウは全て閉じられます</summary>
    public void ReturnWindow(WindowType returnWindow)
    {
#if UNITY_EDITOR
        if (!m_WindowList.Contains(returnWindow))
        {
            Debug.LogError("指定されたウィンドウが開かれていません");
            return;
        }
        if(m_CurrentType == returnWindow)
        {
            Debug.LogWarning("既に指定されたウィンドウの状態です");
            return;
        }
        if (returnWindow == WindowType.Close)
        {
            Debug.LogWarning("全てのウィンドウが閉じられます");
        }
#endif
            if (CheckCall())
            {
                while (m_CurrentType != returnWindow)
                {
                    CloseSetting();
                }
            }
    }

    /// <summary>現在開かれている特定のウィンドウまで戻ります(ボタン用)</summary>
    [EnumAction(typeof(WindowType))]
    public void ReturnWindow(int type) { ReturnWindow((WindowType)type); }


    /// <summary>全てのウィンドウを閉じます　</summary>
    public void CloseALLWindow()
    {
        if (CheckCall())
        {
            while (m_CurrentType != WindowType.Close)
            {
                CloseSetting();
            }
        }
    }


    //  その他関数   //------------------------------------------------------------------------------------------------------------

    /// <summary>ウィンドウオブジェクトを取得します</summary>
    private GameObject GetWindowObject(WindowType type)
    {
        int index = (int)type;
#if UNITY_EDITOR
        if (type == WindowType.Close)
        {
            Debug.LogWarning("Closeウィンドウは取得できません");
            return null;
        }
        if(WindowObjects.Length < index - 1 || WindowObjects[index] == null)
        {
            Debug.LogError("ウィンドウがアタッチされていません : "+type.ToString());
            return null;
        }
#endif
        return WindowObjects[index];
    }

    /// <summary>呼び出し可能かどうか調べます</summary>
    /// /// <returns>可能であればtrueを返します</returns>
    private bool CheckCall()
    {
        //処理を行う
        if (m_IsAvailable)
        {
            //処理中のフラグを立てる
            if (m_EnableDelay)
            {
                m_IsAvailable = false;
                UsefulSystem.Instance.WaitCallBack(c_OCDeley, () => m_IsAvailable = true);
            }
            return true;
        }
#if UNITY_EDITOR
        if (!m_EnableDelay)
        {
            Debug.LogWarning("ウィンドウの開閉処理の際のディレイが無効化されています");
        }
        if (!m_IsAvailable)
        {
            Debug.LogWarning("m_IsAvailableがfalseなので処理が行われません");
        }
#endif
        return false;
    }

    /// <summary>バックグラウンドを表示、非表示にします</summary>
    private void SetBackGround(bool setActive)
    {
        if (BackGround != null)
        {
            BackGround.SetActive(setActive);
            //バックグラウンドを一番後ろに持っていく
            BackGround.transform.SetAsFirstSibling();
        }
    }

    /// <summary>画面を開く際の準備を行います</summary>
    private void OpenSettings(WindowType type)
    {
#if UNITY_EDITOR
        if (type == WindowType.Close)
        {
            Debug.LogError("Closeウィンドウは開く事が出来ません。\n呼び出し元の引数(ボタン、スクリプト等)を確認してください");
            return;
        }
#endif
        //ウィンドウが開かれていないかチェックする
        if (!m_WindowList.Contains(type))
        {
            //ウィンドウが閉じられているならバックグラウンドを出す処理も行う
            if (m_CurrentType == WindowType.Close)
            {
                SetBackGround(true);
            }
            //現在のウィンドウ情報を追加、更新する
            m_WindowList.Add(type);
            //一番前にウィンドウオブジェクトを持ってくる
            GetWindowObject(type).transform.SetAsLastSibling();
            //ウィンドウを開く
            Open();
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning("既にそのウィンドウは開かれています : " + type.ToString());
            return;
        }
#endif
    }

    /// <summary>画面を閉じる際の準備を行います</summary>
    private void CloseSetting()
    {
        if (m_CurrentType != WindowType.Close)
        {
                Close();                                       //現在のウィンドウフラグを変更する
                m_WindowList.RemoveAt(m_WindowList.Count - 1); //閉じられたウィンドウを配列から取り除く             
                if (m_CurrentType == WindowType.Close)
                {
                    //全てのウィンドウが閉じられているのでバックグラウンドを閉じる
                    SetBackGround(false);
                }
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning("既に全てのウィンドウが閉じられています");
        }
#endif
    }

}