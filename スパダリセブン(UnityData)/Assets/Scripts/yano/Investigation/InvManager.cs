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
    public static InvManager Instance { get { Debug.Assert(m_instance, "�V�[�����Ⴄ�̂ŃC���X�^���X���擾�ł��܂���B"); return m_instance; } }


    public void Open(InvType type) {
        Debug.Assert(!m_isOpen,"�T���p�[�g�����ɊJ����Ă��܂�"); 
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

    [SerializeField,Header("�T���p�[�g�w�i�pImageObjct"),EnumIndex(typeof(InvType))]
    private GameObject[] m_invObj;
    [SerializeField, Header("�߂�{�^��")]
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
        //��ʂ��J����Ă��āA���N���b�N�������s���Ă��Ȃ��ꍇ�A�N���b�N���ꂽ������s��
        if (m_isOpen && !m_click && Input.GetKeyDown(KeyCode.Mouse0)) {
            //�N���b�N������
            m_click = true; 
            //�ʃX���b�h���烁�C���X���b�h���Q�Ƃł���悤�ɂ���
            SynchronizationContext MainThread = SynchronizationContext.Current;
            Task.Run(() =>
            {
                MainThread.Post(__ =>
                {
                    //RaycastAll�̌��ʊi�[�p�̃��X�g�쐬
                    List<RaycastResult> RayResult = new List<RaycastResult>();

                    //PointerEvenData�ɁA�}�E�X�̈ʒu���Z�b�g
                    pointData.position = Input.mousePosition;
                    //RayCast�i�X�N���[�����W�j
                    EventSystem.current.RaycastAll(pointData, RayResult);

                    foreach (RaycastResult result in RayResult) {
                        //�A�C�e���̎擾����
                        if (result.gameObject.TryGetComponent<ItemObject>(out var item)) {
                            ItemManager.Instance.AddItem(item.ID);
                            Destroy(item.gameObject);
                        }
                    }
                    m_click = false;    //�����I��
                }, null);
            });
        }
    }
}
