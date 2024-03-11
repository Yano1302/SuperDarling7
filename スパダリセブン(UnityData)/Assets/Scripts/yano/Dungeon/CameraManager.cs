using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    //  パブリック変数  //

    /// <summary>ターゲットに追従するかどうかのフラグ</summary>
    public bool IsTraking { get; set; } = false;
   
    /// <summary>追従するターゲットを指定します。シーン毎にターゲットを設定してください。
    /// <br/>またターゲットを指定した際に自動で追従フラグがONになります。</summary>
    public Transform SetTarget { set { m_target = value; IsTraking = true; } }




    //  プライベート変数    //
    private Transform m_target;      //ターゲットのトランスフォーム
    private Camera m_mainCamera;     //メインカメラ
    private const float posZ = -10;  //カメラのZ座標    

    protected override void Awake() {
        base.Awake();
        m_target = null;
        m_mainCamera = Camera.main;
    }

 
    private void LateUpdate()
    {
        if(m_target != null) {
            m_mainCamera.transform.position = new Vector3(m_target.position.x, m_target.position.y, posZ);
        }     
    }
}
