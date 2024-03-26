using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catchTrap : MonoBehaviour
{
    public static GameObject CaughtTrap = null;
    private void OnTriggerEnter2D(Collider2D collision) {
      if(collision.gameObject.tag == "Player") {
            CaughtTrap = gameObject;
            collision.transform.position = transform.position;
            Player.Instance.MoveFlag = false;
            UIManager.Instance.OpenUI(UIType.EscapeButton);
        }
    }
}
