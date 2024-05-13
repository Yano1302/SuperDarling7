using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Threading;


public enum InvBackGroundType {
   A, B, C,
}

public class InvManager : MonoBehaviour
{
    /// <summary>探索パート～調査パート間のみ取得できるインスタンス</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "シーンが違うのでインスタンスを取得できません。"); return m_instance; } }
    /// <summary>この調査パートに配置されたアイテムの中で取得しているアイテム数</summary>
    public static int GetItemNum { get; set; } = 0;

    public void Open(InvBackGroundType type) {
        Debug.Assert(!m_isOpen,"探索パートが既に開かれています"); 
        m_currentInvType = type;  m_invObj[(int)m_currentInvType].SetActive(true); 
        m_backBtn.SetActive(true);
        m_isOpen = true; 
    }

    public void Close() {
        if (!m_click) {
            m_invObj[(int)m_currentInvType].SetActive(false);
            m_backBtn.SetActive(false);
            m_isOpen = false;
            Player.Instance.MoveFlag = true;
            m_currentInvType = 0;
        }
    }

    private static InvManager m_instance;
    private InvManager() { }

    [SerializeField, Header("戻るボタン")]
    private GameObject m_backBtn;

    [SerializeField,Header("探索パート背景用ImageObjct"),EnumIndex(typeof(InvBackGroundType))]
    private GameObject[] m_invObj;
   

    private InvBackGroundType  m_currentInvType;
    private bool m_isOpen = false;              //開いているかのフラグ
    private bool m_click = false;               //クリック抑制
    private PointerEventData pointData;         //クリック検知用

    private void Awake() {
        GetItemNum = 0;
        m_instance = GetComponent<InvManager>(); 
        pointData = new PointerEventData(EventSystem.current);      
    }

    private void Update() {
        CheckClick();
    }
    private void CheckClick() {
        //画面が開かれていて、かつクリック処理を行っていない場合、クリックされた判定を行う
        if (m_isOpen && !m_click && Input.GetKeyDown(KeyCode.Mouse0)) {
            //クリック処理中
            m_click = true; 
            //別スレッドからメインスレッドを参照できるようにする
            SynchronizationContext MainThread = SynchronizationContext.Current;
            Task.Run(() =>
            {
                MainThread.Post(__ =>
                {
                    //RaycastAllの結果格納用のリスト作成
                    List<RaycastResult> RayResult = new List<RaycastResult>();

                    //PointerEvenDataに、マウスの位置をセット
                    pointData.position = Input.mousePosition;
                    //RayCast（スクリーン座標）
                    EventSystem.current.RaycastAll(pointData, RayResult);

                    foreach (RaycastResult result in RayResult) {
                        //アイテムの取得処理
                        if (result.gameObject.TryGetComponent<ItemObject>(out var item)) {
                            //クリック処理中
                            m_click = true;
                            //取得するアイテムのメッセージを取得する
                            var ins = ItemManager.Instance;
                            ins.GetItemMessage(item.ID,ItemManager.ItemMessageType.Investigation,out string message);
                            item.GetItem();

                            if (GetItemNum == ins.GetTotalItemNum(MapSetting.Instance.StageNumber)) {
                                Close();
                                Supadari.SceneManager.Instance.SceneChange(SCENENAME.SolveScene);
                            }
                            break;
                        }
                    }
                    //処理終了
                }, new Action(() => m_click = false));
            });
        }
    }



}
