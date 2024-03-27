using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Œã‚Å‚·‚©‚à
public class OpenItemManager : MonoBehaviour
{   
    void Start()
    {
        MapSetting.Instance.CreateMap(1);
        UIManager.Instance.OpenUI(UIType.ItemWindow);
        UIManager.Instance.OpenUI(UIType.Timer);
    }
}
