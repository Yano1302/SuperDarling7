using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : MonoBehaviour
{
    private SpriteRenderer m_sr;
    private CircleCollider2D m_ccol;

    private void Start() {
        m_sr = gameObject.GetComponent<SpriteRenderer>();
        m_sr.enabled = false;
        m_ccol = gameObject.GetComponent<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            AudioManager.Instance.SE_Play("SE_dungeon03");
            m_sr.enabled = true;
            Player pins = Player.Instance;
            pins.MoveFlag = false;
            pins.ResetPositon();
            DisplayManager dins = DisplayManager.Instance;
            dins.FadeOut(FadeType.CurrentFadeType, () =>{
                dins.FadeIn(FadeType.CurrentFadeType, () =>
                {
                    pins.ResetPositon();
                    m_ccol.radius = 0.5537109f;
                    Destroy(this);
                }); 
            });
           
        }   
    }
}
