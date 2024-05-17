using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Supadari;
using SceneManager = Supadari.SceneManager;

public class Goal : MonoBehaviour
{
    [SerializeField] private InvType m_invType;

    private void OnCollisionEnter2D(Collision2D collision) {
        // ウィンドウを開く処理
        if (collision.gameObject.CompareTag("Player")) {
            var ins = Player.Instance;
            ins.MoveFlag = false;
            ins.VisibilityImage = false;
            InvManager.Instance.Open(m_invType);
        }
    }
}
