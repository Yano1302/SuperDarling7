using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Œã‚Å‚·‚©‚à
public class OpenItemManager : MonoBehaviour
{   
    void Start()
    {
        UIManager.Instance.OpenUI(UIType.ItemWindow);
        MapSetting.Instance.CreateMap(1);
    }
}
