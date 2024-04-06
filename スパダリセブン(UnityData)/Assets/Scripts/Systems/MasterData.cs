using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 岬作成のセーブ・ロードするクラスのスクリプトです
// セーブ・ロードしたい変数はここに記載してください
[Serializable]
public class MasterData // jsonデータとして保存するclass
{
    // 下記は例です
    /*
    public float[] rhythmRanking = new float[10]; // リズムランキング
    public string rhythmHighScoreRank = "D"; // リズムハイスコアランク
    */
    // 証拠？
    //public GameObject[] evidences = new GameObject[10];
    // シーン番号？
    public int sceneNum = 0;
    // BGM音量
    public float volumeBGM = 0.5f;
    // SE音量
    public float volumeSE = 0.5f;
    // テキストスピード
    public float textSpeed = 0.5f;

}

