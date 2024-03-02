using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    //  �p�u���b�N�ϐ�  //

    /// <summary>�^�[�Q�b�g�ɒǏ]���邩�ǂ����̃t���O</summary>
    public bool IsTraking { get; set; } = false;
   
    /// <summary>�Ǐ]����^�[�Q�b�g���w�肵�܂��B�V�[�����Ƀ^�[�Q�b�g��ݒ肵�Ă��������B
    /// <br/>�܂��^�[�Q�b�g���w�肵���ۂɎ����ŒǏ]�t���O��ON�ɂȂ�܂��B</summary>
    public Transform SetTarget { set { m_target = value; IsTraking = true; } }




    //  �v���C�x�[�g�ϐ�    //
    private Transform m_target;      //�^�[�Q�b�g�̃g�����X�t�H�[��
    private Camera m_mainCamera;     //���C���J����
    private const float posZ = -10;  //�J������Z���W    

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
