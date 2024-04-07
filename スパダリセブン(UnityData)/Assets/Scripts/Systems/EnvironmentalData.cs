using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 環境設定を記載するスクリプトです
[Serializable]
public class EnvironmentalData // jsonデータとして保存するclass
{
    // BGM音量
    public float volumeBGM = 0.5f;
    // SE音量
    public float volumeSE = 0.5f;
    // テキストスピード
    public float textSpeed = 0.5f;
}