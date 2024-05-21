using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvWarning : MonoBehaviour
{
    private float timer;
    private void OnEnable() {
        timer = 0;
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer >= 3) {
            timer = 0;
            InvManager.Instance.ResetVigilance();
        }
    }
}
