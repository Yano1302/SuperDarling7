using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDebug : MonoBehaviour
{
    MapSetting ins;
    UsefulSystem us;
    private void Start() {
        ins = MapSetting.Instance;
        us = UsefulSystem.Instance;
    }
   
    void Update()
    {
        us.InputAction(KeyCode.Z, () => ins.CreateMap(1)); ;
    }
}
