using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    [Header("�E���̃{�^��")]
    [SerializeField] private EscapeBtn Rbtn;
    [Header("�����̃{�^��")]
    [SerializeField] private EscapeBtn Lbtn;

    private Player pIns;
    private UsefulSystem _USins;
    private const int c_pressNum = 10;    //�A�ŉ񐔁@TODO�@�ǂ�������A�ŉ񐔂�ݒ肷��H
    private bool swing = false;

    private void OnEnable() {
        pIns = pIns ?? Player.Instance;
        _USins = _USins ?? UsefulSystem.Instance;
        StartCoroutine(Push());
    }

    private IEnumerator Push() {
        //����OnEnable�̏�����D�悷�邽�߂P�t���[���ҋ@����
        yield return null;
        //�{�^���̕\��������
        Rbtn.gameObject.SetActive(true);
        Lbtn.gameObject.SetActive(true);
        Rbtn.Set(c_pressNum);
        Lbtn.Set(c_pressNum);
        //�A�Ŕ�����s��
        while (!Rbtn.clear || !Lbtn.clear) {
            KeyCheck(Rbtn);
            KeyCheck(Lbtn);
            yield return null;
        }
        Player.Instance.CanMove = true;
        UIManager.Instance.CloseUI(UIType.EscapeButton);
        Destroy(catchTrap.CaughtTrap);
    }

    private void KeyCheck(EscapeBtn eb) {
        if (Input.GetKeyDown(eb.Key)) {
            eb.SetColor(Color.red);
            eb.RestPressNum--;
            if (!swing) {
                swing = true;
                float x = Random.Range(0.01f, 0.1f);
                float y = Random.Range(-0.1f, 0.1f);
                pIns.gameObject.transform.position += new Vector3(x,y,0);
                _USins.WaitCallBack(0.1f, () => pIns.gameObject.transform.position -= new Vector3(x*2,y*2, 0));
                _USins.WaitCallBack(0.1f, () => { pIns.gameObject.transform.position += new Vector3(x,y, 0); swing = false; if (eb != null) { eb.SetColor(Color.blue); } });
                
            }
        }
    }
}
