using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO 後ですかも
public class OpenItemManager : MonoBehaviour
{   
    void Start()
    {
        //マップ生成
        MapSetting.Instance.CreateMap(1);
        //アイテムウィンドウの表示(アイテムウィンドウの状態の更新も必要かも？)
        var UIins = UIManager.Instance;
        UIins.OpenUI(UIType.ItemWindow);
        UIins.OpenUI(UIType.Timer);
    }
}
