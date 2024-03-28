using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>UIタイプ</summary>
public enum UIType {
    Close,          //ここは必須
    EscapeButton,   //トラップから抜け出すボタンを表示する
    Timer,          //タイマー表示用UI
    ItemWindow,     //アイテムウィンドウ
    Clear,          //答えが合っていた場合のUI画面(仮置き)
    miss,           //答えが間違っていた場合のUI画面(仮置き)
}

/// <summary>
/// UIの表示を管理するクラスです<br />
/// 列挙子を記載しそれに対応するオブジェクトをUIObjectsにアタッチして使用してください<br />
/// </summary>
/// 

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    //  パブリック変数   //--------------------------------------------------------------------------------------------------------

    /// <summary>falseの間は処理が行われません</summary>
    public bool m_IsAvailable { get; set; } = true;

    /// <summary>falseの間はUI開閉の際のディレイが発生しません</summary>
    public bool m_EnableDelay { get; set; } = false;

    /// <summary>現在のUIタイプ</summary>
    public UIType m_CurrentType { get { return m_UIList[m_UIList.Count - 1]; } }


    //  コンフィグ用変数    //-----------------------------------------------------------------------------------------------------

    [SerializeField,Header("UI表示・非表示処理から再び開閉処理に入るまでの待機時間")]
    private float OCDeley = 0.2f;
    [SerializeField, Header("最初に表示しておくUIタイプを設定します")]
    private UIType[] FirstOpenUIs;


    //  アタッチ用変数   //--------------------------------------------------------------------------------------------------------

    [SerializeField, EnumIndex(typeof(UIType)), Header("UIオブジェクト一覧")]
    private GameObject[] UIObjects;
    //  アタッチ用関数  //--------------------------------------------------------------------------------------------------------
    /// <summary>指定されたUIを閉じます(ボタンアタッチ用)</summary>
    [EnumAction(typeof(UIType))]
    public void CloseUI(int type) { CloseUI((UIType)type); }

    /// <summary>現在開かれている特定のUIまで戻ります(ボタン用)</summary>
    [EnumAction(typeof(UIType))]
    public void ReturnUI(int type) { ReturnUI((UIType)type); }

    //  パブリック関数    //------------------------------------------------------------------------------------------------------

    /// <summary>指定されたUIを開きます</summary>
    public void OpenUI(UIType type) {
        if (CheckCall()) {
            OpenSettings(type);
        }
    }

    /// <summary>指定されたUIを開きます(ボタン用)</summary>
    [EnumAction(typeof(UIType))]
    public void OpenUI(int type) { OpenUI((UIType)type); }


    /// <summary>複数のUIを同時に開きます</summary>
    /// <param name="UITypes">開くUIタイプの配列<br />
    /// ※配列の順番通りにUIを表示していきます。<br />
    /// ただし、既に開かれているUIタイプを配列に入れた場合、そのUIを再表示する処理は行わないので<br />
    /// 画面のレイアウト、Closeの順番等がおかしくなる可能性があります。
    /// </param>
    public void OpenUIs(UIType[] UITypes) {
#if UNITY_EDITOR
        if (UITypes.Length == 0) {
            UnityEngine.Debug.LogError("引数の配列の中身がありません");
            return;
        }
#endif
        if (CheckCall()) {
            for (int i = 0; i < UITypes.Length; i++) {
                OpenSettings(UITypes[i]);
            }
        }
    }

    /// <summary>現在のUIを閉じます</summary>
    public void CloseUI() {
        if (CheckCall()) {
            CloseSetting();
        }
    }

    /// <summary>指定されたUIを閉じます</summary>
    public void CloseUI(UIType type) {
        if (CheckCall()) {
            if (m_UIList.Remove(type)) {
                //削除したい要素を最後尾にずらす
                m_UIList.Add(type);
                CloseSetting();
            }
#if UNITY_EDITOR
            else {
                UnityEngine.Debug.LogError("指定されたUIが開かれていません");
                return;
            }
#endif
        }
    }

    /// <summary>現在開かれている特定のUIまで戻ります<br />
    /// ※そのUIから派生したUIは全て閉じられます</summary>
    public void ReturnUI(UIType returnUI) {
#if UNITY_EDITOR
        if (!m_UIList.Contains(returnUI)) {
            UnityEngine.Debug.LogError("指定されたUIが開かれていません");
            return;
        }
        if (m_CurrentType == returnUI) {
            UnityEngine.Debug.LogWarning("既に指定されたUIの状態です");
            return;
        }
        if (returnUI == UIType.Close) {
            UnityEngine.Debug.LogWarning("全てのUIが閉じられます");
        }
#endif
        if (CheckCall()) {
            while (m_CurrentType != returnUI) {
                CloseSetting();
            }
        }
    }

    /// <summary>全てのUIを閉じます　</summary>
    public void CloseALLUI() {
        if (CheckCall()) {
            while (m_CurrentType != UIType.Close) {
                CloseSetting();
            }
        }
    }




    //  プライベート変数  //-------------------------------------------------------------------------------------------------------

    private List<UIType> m_UIList = null;         //開かれているUI一覧
   
    private GameObject m_TargetUI { get { return GetUIObject(m_CurrentType); } }         //開閉対象のUI

    //  プライベート関数  //-------------------------------------------------------------------------------------------------------
   　protected override void Awake() {
        base.Awake();
        if(m_UIList == null) {
            m_UIList = new List<UIType>() { UIType.Close};
            UnityEngine.Debug.Assert(UIObjects.Length > 1,"UIオブジェクトがアタッチされていません");
            for (int i = 1; i < UIObjects.Length; i++) {
                UnityEngine.Debug.Assert(UIObjects[i] != null, (UIType)i + "に対応するUIオブジェクトがアタッチされていません。");
                UIObjects[i].SetActive(false);
            }
            foreach(var ui in FirstOpenUIs) {
                OpenUI(ui);
            }
        }
    }

    /// <summary>UIオブジェクトを取得します</summary>
    private GameObject GetUIObject(UIType type)
    {
        int index = (int)type;
#if UNITY_EDITOR
        if (type == UIType.Close)
        {
            UnityEngine.Debug.LogWarning("CloseUIは取得できません");
            return null;
        }
        if(UIObjects.Length < index - 1 || UIObjects[index] == null)
        {
            UnityEngine.Debug.LogError("UIがアタッチされていません : "+ type.ToString());
            return null;
        }
#endif
        return UIObjects[index];
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
                UsefulSystem.Instance.WaitCallBack(OCDeley, () => m_IsAvailable = true);
            }
            return true;
        }
#if UNITY_EDITOR
        if (!m_EnableDelay)
        {
            UnityEngine.Debug.LogWarning("UIの開閉処理の際のディレイが無効化されています");
        }
        if (!m_IsAvailable)
        {
            UnityEngine.Debug.LogWarning("m_IsAvailableがfalseなので処理が行われません");
        }
#endif
        return false;
    }

    /// <summary>画面を開く際の準備を行います</summary>
    private void OpenSettings(UIType type)
    {
#if UNITY_EDITOR
        if (type == UIType.Close)
        {
            UnityEngine.Debug.LogError("CloseUIは開く事が出来ません。\n呼び出し元の引数(ボタン、スクリプト等)を確認してください");
            return;
        }
#endif
        //UIが開かれていないかチェックする
        if (!m_UIList.Contains(type))
        {
            //現在のUI情報を追加、更新する
            m_UIList.Add(type);
            //一番前にUIオブジェクトを持ってくる
            GetUIObject(type).transform.SetAsLastSibling();
            //UIを表示する
            m_TargetUI.SetActive(true);
        }
#if UNITY_EDITOR
        else
        {
            UnityEngine.Debug.LogWarning("既にそのUIは開かれています : " + type.ToString());
            return;
        }
#endif
    }

    /// <summary>画面を閉じる際の準備を行います</summary>
    private void CloseSetting()
    {
        if (m_CurrentType != UIType.Close)
        {
            m_TargetUI.SetActive(false);               //現在のUIフラグを変更する
            m_UIList.RemoveAt(m_UIList.Count - 1); //閉じられたUIを配列から取り除く             
        }
#if UNITY_EDITOR
        else
        {
            UnityEngine.Debug.LogWarning("既に全てのUIが閉じられています");
        }
#endif
    }

}