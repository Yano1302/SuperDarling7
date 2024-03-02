using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    [Header("右側のボタン")]
    [SerializeField] private EscapeBtn Rbtn;
    [Header("左側のボタン")]
    [SerializeField] private EscapeBtn Lbtn;

    private Player pIns;
    private const int c_pressNum = 10;    //連打回数
    private bool swing = false;

    private void OnEnable() {
        pIns = pIns ?? Player.Instance;
        Rbtn.gameObject.SetActive(true);
        Lbtn.gameObject.SetActive(true);
        Rbtn.Set(c_pressNum);
        Lbtn.Set(c_pressNum);
        StartCoroutine(Push());
    }

    private IEnumerator Push() {
        while (!Rbtn.clear || !Lbtn.clear) {
            KeyCheck(Rbtn);
            KeyCheck(Lbtn);
            yield return null;
        }
        Player.Instance.CanMoving = true;
        UIManager.Instance.CloseUI(UIType.EscapeButton);
        Destroy(catchTrap.CaughtTrap);
    }

    private void KeyCheck(EscapeBtn eb) {
        if (Input.GetKeyDown(eb.Key)) { 
            eb.RestPressNum--;
            if (!swing) {
                UsefulSystem usi = UsefulSystem.Instance;
                swing = true;
                float x = Random.Range(0.01f, 0.1f);
                float y = Random.Range(-0.1f, 0.1f);
                pIns.gameObject.transform.position += new Vector3(x,y,0);
                usi.WaitCallBack(0.1f, () => pIns.gameObject.transform.position -= new Vector3(x*2,y*2, 0));
                usi.WaitCallBack(0.1f, () => { pIns.gameObject.transform.position += new Vector3(x,y, 0); swing = false; });
                
            }
        }
    }
}
