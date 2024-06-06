using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Supadari;
using SceneManager = Supadari.SceneManager;

public class Goal : MonoBehaviour
{
    public InvType InvType;     //MapManager����i�[�����

    private void OnCollisionEnter2D(Collision2D collision) {
        // �E�B���h�E���J������
        if (collision.gameObject.CompareTag("Player")) {
            var ins = Player.Instance;
            ins.MoveFlag = false;
            ins.VisibilityImage = false;
            InvManager.Instance.Open(InvType);
        }
    }
}
