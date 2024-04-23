using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //  Config変数    //
    [SerializeField]
    private float m_speed = 1.0f;       //移動スピード
    [SerializeField]
    private bool isMoving = true;       //移動可能フラグ

    //  パブリック変数・関数 //

    /// <summary>インスタンスを取得します</summary>
    public static Player Instance { get { return m_instance; } }
    /// <summary>移動可能かどうかを設定します。</summary>
    public bool MoveFlag { get { return isMoving; }set { isMoving = value; if (!value) { m_rb.velocity = Vector2.zero; } } }
    /// <summary>プレイヤーの向きを取得します</summary>
    public Vector2 Direction { get { return m_direction; } }
    /// <summary></summary>
    public bool VisibilityImage { get { return m_visImage.enabled; } set{ m_visImage.enabled = value; } }
    // プライベート変数・関数 //
    [SerializeField, Header("視界のImageのtransform")]
    private RectTransform m_visRect;

    private Image m_visImage;
    private CameraManager m_CamIns;         //カメラマネージャーインスタンス
    private Rigidbody2D m_rb;               //移動用rigidbody
    private static Player m_instance;       //このクラスのインスタンス
    private Vector3 m_direction;            //プレイヤーの向き

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

    /// <summary>移動処理を行います</summary>
    void Move() {
        if (MoveFlag) {
            //キーの入力を取得する
            m_direction.x = Input.GetAxis("Horizontal");
            m_direction.y = Input.GetAxis("Vertical");
            //移動する
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
