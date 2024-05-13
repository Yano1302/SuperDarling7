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
    /// <summary>�T���p�[�g�`�����p�[�g�Ԃ̂ݎ擾�ł���C���X�^���X</summary>
    public static InvManager Instance { get { Debug.Assert(m_instance, "�V�[�����Ⴄ�̂ŃC���X�^���X���擾�ł��܂���B"); return m_instance; } }
    /// <summary>���̒����p�[�g�ɔz�u���ꂽ�A�C�e���̒��Ŏ擾���Ă���A�C�e����</summary>
    public static int GetItemNum { get; set; } = 0;

    public void Open(InvBackGroundType type) {
        Debug.Assert(!m_isOpen,"�T���p�[�g�����ɊJ����Ă��܂�"); 
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

    [SerializeField, Header("�߂�{�^��")]
    private GameObject m_backBtn;

    [SerializeField,Header("�T���p�[�g�w�i�pImageObjct"),EnumIndex(typeof(InvBackGroundType))]
    private GameObject[] m_invObj;
   

    private InvBackGroundType  m_currentInvType;
    private bool m_isOpen = false;              //�J���Ă��邩�̃t���O
    private bool m_click = false;               //�N���b�N�}��
    private PointerEventData pointData;         //�N���b�N���m�p

    private void Awake() {
        GetItemNum = 0;
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
                            //�N���b�N������
                            m_click = true;
                            //�擾����A�C�e���̃��b�Z�[�W���擾����
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
                    //�����I��
                }, new Action(() => m_click = false));
            });
        }
    }



}
