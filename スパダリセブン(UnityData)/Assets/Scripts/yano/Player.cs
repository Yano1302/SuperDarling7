using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //  変数一覧    //
    private Camera m_camera;        //追従用カメラ
    private Rigidbody2D m_rb;       //移動用rigidbody
    [SerializeField]        
    private float m_speed;          //移動スピード

    void Start()
    {
        m_camera = Camera.main;
        m_rb = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        Move();
    }

    private void LateUpdate() {
        m_camera.transform.position = new Vector3(transform.position.x,transform.position.y,-10);
    }


    /// <summary>移動処理を行います</summary>
    void Move() {
        //キーの入力を取得する
        float x = Input.GetAxis("Horizontal");         //左右
        float y = Input.GetAxis("Vertical");           //上下
        //移動する
        m_rb.velocity = new Vector3(x, y) * m_speed;
    }
}
