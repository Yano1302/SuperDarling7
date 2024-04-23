using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Threading;


public enum InvType {
   A, B, C,
}

public class InvManager : MonoBehaviour
{
    public static InvManager Instance { get { Debug.Assert(m_instance, "シーンが違うのでインスタンスを取得できません。"); return m_instance; } }


    public void Open(InvType type) {
        Debug.Assert(!m_isOpen,"探索パートが既に開かれています"); 
        m_currentInvType = type;  m_invObj[(int)m_currentInvType].SetActive(true); 
        m_backBtn.SetActive(true);
        m_isOpen = true; 
    }

    public void Close() {
        m_invObj[(int)m_currentInvType].SetActive(false);
        m_backBtn.SetActive(false);
        m_isOpen = false; 
        Player.Instance.MoveFlag = true;
        m_currentInvType = 0;
    }


  


    private static InvManager m_instance;

    [SerializeField,Header("探索パート背景用ImageObjct"),EnumIndex(typeof(InvType))]
    private GameObject[] m_invObj;
    [SerializeField, Header("戻るボタン")]
    private GameObject m_backBtn;

    private InvType  m_currentInvType;
    private bool m_isOpen = false;
    private bool m_click = false;
    private PointerEventData pointData;

    private void Awake() {
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
                            ItemManager.Instance.AddItem(item.ID);
                            Destroy(item.gameObject);
                        }
                    }
                    m_click = false;    //処理終了
                }, null);
            });
        }
    }
}
