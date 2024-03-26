using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Supadari;
using SceneManager = Supadari.SceneManager;

public class Goal : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision) {
        // TODO ‰¼’u‚«
        Player.Instance.MoveFlag = false;
        UIManager.Instance.CloseUI(UIType.Timer);
        SceneManager.Instance.SceneChange(4);
    }
}
