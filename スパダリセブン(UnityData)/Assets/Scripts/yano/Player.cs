using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //  �ϐ��ꗗ    //
    private Camera m_camera;        //�Ǐ]�p�J����
    private Rigidbody2D m_rb;       //�ړ��prigidbody
    [SerializeField]        
    private float m_speed;          //�ړ��X�s�[�h

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


    /// <summary>�ړ��������s���܂�</summary>
    void Move() {
        //�L�[�̓��͂��擾����
        float x = Input.GetAxis("Horizontal");         //���E
        float y = Input.GetAxis("Vertical");           //�㉺
        //�ړ�����
        m_rb.velocity = new Vector3(x, y) * m_speed;
    }
}
