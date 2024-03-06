using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// フェードイン・アウト用のキャンパスにアタッチすること
// 上記のキャンパスはタグで「fade」としておくことを推奨
// キャンパスはプレハブ化し、ヒエラルキー上には存在させないこと
// スクリプト「SceneScript」と共に使用する
// 
public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    public static bool isFadeInstance = false; // FadeCanvas召喚フラグ
    public bool isFadeIn = false;              // フェードインするフラグ
    public bool isFadeOut = false;             // フェードアウトするフラグ
    public float alpha = 0.0f;                 // 透過率
    public float fadeSpeed = 0.2f;             // フェードにかかる時間

    protected override void Awake()
    {
        base.Awake(); // デバッグログを表示するか否かスクリプタブルオブジェクトのGameSettingsを参照
        if (!isFadeInstance)           // FadeCanvas召喚フラグがfalseなら
        {
            isFadeInstance = true;     // FadeCanvas召喚フラグがtrueにする
        }
        else                           // FadeCanvas召喚フラグがtrueなら
        {
            Destroy(this);             // 起動時以外は重複しないようにするため召喚したCanvasを消す
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)                                                                      // フェードインフラグがtrueなら
        {
            alpha -= Time.deltaTime / fadeSpeed;                                           // 徐々にalpha値を減らす fadeSpeedで速度調整可能                Debug.Log(alpha);
            Debug.Log(alpha);
            if (alpha <= 0.0f)                                                             // 透明になったらフェードイン終了
            {
                isFadeIn = false;                                                          // フェードインフラグをfalseにする
                alpha = 0.0f;                                                              // alpha値がマイナスにならないように0を代入
            }
            this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha); // 暗転用Imageのalphaに代入し徐々に透明にする
        }

        else if (isFadeOut)                                                                   // フェードアウトフラグがtrueなら
        {
            alpha += Time.deltaTime / fadeSpeed;                                             // 徐々にalpha値を増やす fadeSpeedで速度調整可能
            Debug.Log(alpha);
            if (alpha >= 1.0f)                                                               // 真っ黒になったらフェードアウト終了
            {
                isFadeOut = false;                                                           // フェードアウトフラグをfalseにする
                isFadeIn = true;                                                             // フェードインフラグをtrueにする
                alpha = 1.0f;                                                                // alpha値が1以上にならないように1を代入
            }

            this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha); // 暗転用Imageのalphaに代入し徐々に黒にする
        }
    }

    public void fadeIn()   // フェードイン関数
    {
        isFadeIn = true;   // フェードインフラグをtrueにする
        isFadeOut = false; // フェードアウトフラグをfalseにして誤発動を防ぐ
    }

    public void fadeOut() // フェードアウト関数
    {
        isFadeOut = true; // フェードアウトフラグをtrueにする
        isFadeIn = false; // フェードインフラグをfalseにして誤発動を防ぐ
    }
}

