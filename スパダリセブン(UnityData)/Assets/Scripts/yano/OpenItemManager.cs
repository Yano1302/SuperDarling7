using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO ��ł�����
public class OpenItemManager : MonoBehaviour
{   
    void Start()
    {
        //�}�b�v����
        MapSetting.Instance.CreateMap(1);
        //�A�C�e���E�B���h�E�̕\��(�A�C�e���E�B���h�E�̏�Ԃ̍X�V���K�v�����H)
        var UIins = UIManager.Instance;
        UIins.OpenUI(UIType.ItemWindow);
        UIins.OpenUI(UIType.Timer);
    }
}
