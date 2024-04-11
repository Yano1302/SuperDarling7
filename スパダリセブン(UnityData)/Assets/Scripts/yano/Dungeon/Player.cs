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
    /// <summary></summary>
    public bool VisibilityImage { get { return m_visImage.enabled; } set{ m_visImage.enabled = value; } }
    // �v���C�x�[�g�ϐ��E�֐� //
    [SerializeField, Header("���E��Image��transform")]
    private RectTransform m_visRect;

    private Image m_visImage;
    private CameraManager m_CamIns;         //�J�����}�l�[�W���[�C���X�^���X
    private Rigidbody2D m_rb;               //�ړ��prigidbody
    private static Player m_instance;       //���̃N���X�̃C���X�^���X
    private Vector3 m_direction;            //�v���C���[�̌���

    private void Awake() {
        m_instance = GetComponent<Player>();
        m_rb = GetComponent<Rigidbody2D>();
        m_visImage = m_visRect.gameObject.GetComponent<Image>();
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
                transform.eulerAngles = new Vector3(0, 0, z);  
                m_visRect.eulerAngles = new Vector3(0,0, z);
            }           
        }
    }
}
