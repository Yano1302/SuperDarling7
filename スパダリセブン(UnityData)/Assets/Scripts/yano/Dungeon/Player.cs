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
    /// <summary>視野狭窄の設定を取得・変更する</summary>
    public bool VisibilityImage { get { return m_visImage.enabled; } set{ m_visImage.enabled = value; } }
    // プライベート変数・関数 //
    [SerializeField, Header("視界のImageのtransform")]
    private RectTransform m_visRect;
    [SerializeField, Header("プレイヤーのモーション切り替え時間")]
    private float m_motionSwichTime;
    [SerializeField, Header("プレイヤーのモーションスプライト")]
    private Sprite[] m_motionSprites;
   


    private Image m_visImage;               //視野狭窄用イメージ
    private CameraManager m_CamIns;         //カメラマネージャーインスタンス
    private Rigidbody2D m_rb;               //移動用rigidbody
    private static Player m_instance;       //このクラスのインスタンス
    private Vector3 m_direction;            //プレイヤーの向き
    private SpriteRenderer m_playerImage;   //プレイヤーイメージ
    private float m_motionTime = 0;         //モーションの切り替えタイミングを計測する
    private int m_motionIndex = 0;          //モーションのインデックスを管理する

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
                //TODO:視点の切り替えを緩やかにする
                transform.eulerAngles = new Vector3(0, 0, z);  
                m_visRect.eulerAngles = new Vector3(0,0, z);
                
                //モーション設定
                m_motionTime += Time.deltaTime;
                if (m_motionTime >= m_motionSwichTime) {
                    //モーション切り替え計測時間を0付近に戻す
                    m_motionTime -= m_motionSwichTime;
                    //モーション切り替えインデックスを進める
                    m_motionIndex += 1;
                    //インデックスが配列外まで進む場合は0に戻してループさせる
                    m_motionIndex = m_motionSprites.Length <= m_motionIndex? 0 : m_motionIndex;
                    //モーションスプライトを格納
                    m_playerImage.sprite = m_motionSprites[m_motionIndex];
                }
            }           
        }
    }
}
