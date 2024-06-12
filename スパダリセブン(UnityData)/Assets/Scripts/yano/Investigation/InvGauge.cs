using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvGauge : MonoBehaviour
{
    [SerializeField, Header("ゲージオブジェクト")]
    private Image m_img;
    [SerializeField,Header("ゲージの中身オブジェクト")]
    private Image m_imgfill;
    [SerializeField, Header("鼓動オブジェクト")]
    private Image m_waves;
    [SerializeField,Header("鼓動アニメーションの間隔")]
    private float m_waveDirectionTime;
    [SerializeField, Header("危険度の上限を１とした場合の鼓動の間隔")]
    private float WaveInterval = 0.25f;             　　//鼓動の間隔

    //鼓動の間隔を計測する
    private float m_waveInterval = 0;

    /// <summary>ゲージを表示します</summary>
    public void OpenGauge(float initialRate) {
        SetRate(initialRate);
        m_img.enabled = true;
        m_imgfill.enabled = true;
        m_waves.enabled = true;
    }

    /// <summary>ゲージを非表示にします</summary>
    public void CloseGauge() {
        StopCoroutine("Waves");
        m_img.enabled = false;
        m_imgfill.enabled = false;
        m_waves.enabled = false;
    }
    /// <summary>ウェーブ演出をストップします</summary>
    public void StopWave() { StopCoroutine("Waves"); }

    /// <summary>現在の警戒度の割合を設定します</summary>
    public void SetRate(float rate) {
        m_imgfill.fillAmount = rate;
        //警戒度が更新された場合の処理
        if (rate > m_waveInterval && InvManager.Instance.VigilanceFlag) {
            StartCoroutine(Waves(rate));
            m_waveInterval += WaveInterval * Mathf.Floor(rate/m_waveInterval);
        }
    }

    /// <summary>ゲージの状態を０にします</summary>
    public void ResetGauge() {
        m_waveInterval = 0;
        SetRate(0);
    }
    /// <summary>ゲージの状態を指定した初期値に戻します</summary>
    public void ResetGauge(float rate) {
        m_waveInterval = rate;
        SetRate(rate);
    }

    /// <summary>心臓のアニメーションを行います TODO アニメーションに切り替えるかも</summary>
    private IEnumerator Waves(float rate) {
        float time = 0;
        if (rate >= 1) { AudioManager.Instance.SE_Play("SE_survey01"); }
        else { AudioManager.Instance.SE_Play("SE_survey02"); }

        WaitForSeconds wait = new WaitForSeconds(Time.deltaTime);
        while (time < 0.5) {
            float t = Time.deltaTime / m_waveDirectionTime;
            Color c = m_waves.color;
            c.a += t * 2;
            m_waves.color = c;
            time += t;
            yield return wait;
        }
        while (time < 1) {
            float t = Time.deltaTime / m_waveDirectionTime;
            Color c = m_waves.color;
            c.a -= t * 2;
            m_waves.color = c;
            time += t;
            yield return wait;
        }
        Color col = m_waves.color;
        col.a = 0;
        m_waves.color = col;
    }
}
