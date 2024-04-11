using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 岬作成のセーブ・ロードするクラスのスクリプトです
// セーブ・ロードしたい変数はここに記載してください
[Serializable]
public class MasterData // jsonデータとして保存するclass
{
    // 証拠？
    //public GameObject[] evidences = new GameObject[10];
    // シーン名
    public SCENENAME scenename = SCENENAME.TitleScene;
    // セーブしたことがあるか
    public bool haveSaved = false;
}

