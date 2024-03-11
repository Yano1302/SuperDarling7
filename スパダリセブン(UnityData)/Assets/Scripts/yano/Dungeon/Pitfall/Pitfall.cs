using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : MonoBehaviour
{
    private SpriteRenderer m_sr;
    private CircleCollider2D m_ccol;

    private void Start() {
        m_sr = gameObject.GetComponent<SpriteRenderer>();
        m_ccol = gameObject.GetComponent<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            m_ccol.radius = 0.55f;
            Destroy(this);
        }   
    }
}
