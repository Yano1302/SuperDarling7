using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision) {
        //TODO SceneManager“I‚È‚Ì‚ªo—ˆ‚½‚ç’u‚«Š·‚¦‚é
        DisplayManager.Instance.FadeOut(FadeType.Entire,() => SceneManager.LoadScene("SolveScene"));
    }
}
