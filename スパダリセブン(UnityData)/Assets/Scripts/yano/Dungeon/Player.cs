using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //  Config�ϐ�    //
    [SerializeField]
    private float m_speed = 1.0f;       //�ړ��X�s�[�h
    [SerializeField]
    private bool isMoving = true;       //�ړ��\�t���O

    //  �p�u���b�N�ϐ��E�֐� //

    /// <summary>�C���X�^���X���擾���܂�</summary>
    public static Player Instance { get { return m_instance; } }
    /// <summary>�ړ��\���ǂ�����ݒ肵�܂��B</summary>
    public bool MoveFlag { get { return isMoving; }set { isMoving = value; if (!value) { m_rb.velocity = Vector2.zero; } } }
    /// <summary>�v���C���[�̌������擾���܂�</summary>
    public Vector2 Direction { get { return m_direction; } }
    /// <summary>���싷��̐ݒ���擾�E�ύX����</summary>
    public bool VisibilityImage { get { return m_visImage.enabled; } set{ m_visImage.enabled = value; } }
    // �v���C�x�[�g�ϐ��E�֐� //
    [SerializeField, Header("���E��Image��transform")]
    private RectTransform m_visRect;
    [SerializeField, Header("�v���C���[�̃��[�V�����؂�ւ�����")]
    private float m_motionSwichTime;
    [SerializeField, Header("�v���C���[�̃��[�V�����X�v���C�g")]
    private Sprite[] m_motionSprites;
   


    private Image m_visImage;               //���싷��p�C���[�W
    private CameraManager m_CamIns;         //�J�����}�l�[�W���[�C���X�^���X
    private Rigidbody2D m_rb;               //�ړ��prigidbody
    private static Player m_instance;       //���̃N���X�̃C���X�^���X
    private Vector3 m_direction;            //�v���C���[�̌���
    private SpriteRenderer m_playerImage;   //�v���C���[�C���[�W
    private float m_motionTime = 0;         //���[�V�����̐؂�ւ��^�C�~���O���v������
    private int m_motionIndex = 0;          //���[�V�����̃C���f�b�N�X���Ǘ�����

    private void Awake() {
        m_instance = GetComponent<Player>();
        m_rb = GetComponent<Rigidbody2D>();
        m_visImage = m_visRect.gameObject.GetComponent<Image>();
        m_playerImage = gameObject.GetComponent<SpriteRenderer>();
        m_playerImage.sprite = m_motionSprites[m_motionIndex];
    }

    private void Start()
    {       
        m_CamIns = Camera.main.GetComponent<CameraManager>();
        m_CamIns.SetTarget = transform;
    }

    private void FixedUpdate() {
        Move();
    }

    /// <summary>�ړ��������s���܂�</summary>
    void Move() {
        if (MoveFlag) {     
            //�L�[�̓��͂��擾����
            m_direction.x = Input.GetAxis("Horizontal");
            m_direction.y = Input.GetAxis("Vertical");
            //�ړ�����
            m_rb.velocity = m_direction.normalized * m_speed;
            if (m_direction.magnitude > 0) {
                float z = Vector2.Angle(Vector2.up, m_direction);
                z = m_direction.x < 0 ? z : -z;
                //TODO:���_�̐؂�ւ����ɂ₩�ɂ���
                transform.eulerAngles = new Vector3(0, 0, z);  
                m_visRect.eulerAngles = new Vector3(0,0, z);
                
                //���[�V�����ݒ�
                m_motionTime += Time.deltaTime;
                if (m_motionTime >= m_motionSwichTime) {
                    //���[�V�����؂�ւ��v�����Ԃ�0�t�߂ɖ߂�
                    m_motionTime -= m_motionSwichTime;
                    //���[�V�����؂�ւ��C���f�b�N�X��i�߂�
                    m_motionIndex += 1;
                    //�C���f�b�N�X���z��O�܂Ői�ޏꍇ��0�ɖ߂��ă��[�v������
                    m_motionIndex = m_motionSprites.Length <= m_motionIndex? 0 : m_motionIndex;
                    //���[�V�����X�v���C�g���i�[
                    m_playerImage.sprite = m_motionSprites[m_motionIndex];
                }
            }           
        }
    }
}
