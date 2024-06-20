using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvGauge : MonoBehaviour
{
    [SerializeField, Header("�Q�[�W�I�u�W�F�N�g")]
    private Image m_img;
    [SerializeField,Header("�Q�[�W�̒��g�I�u�W�F�N�g")]
    private Image m_imgfill;
    [SerializeField, Header("�ۓ��I�u�W�F�N�g")]
    private Image m_waves;
    [SerializeField,Header("�ۓ��A�j���[�V�����̊Ԋu")]
    private float m_waveDirectionTime;
    [SerializeField, Header("�댯�x�̏�����P�Ƃ����ꍇ�̌ۓ��̊Ԋu")]
    private float WaveInterval = 0.25f;             �@�@//�ۓ��̊Ԋu

    //�ۓ��̊Ԋu���v������
    private float m_waveInterval = 0;

    /// <summary>�Q�[�W��\�����܂�</summary>
    public void OpenGauge(float initialRate) {
        SetRate(initialRate);
        m_img.enabled = true;
        m_imgfill.enabled = true;
        m_waves.enabled = true;
    }

    /// <summary>�Q�[�W���\���ɂ��܂�</summary>
    public void CloseGauge() {
        StopCoroutine("Waves");
        m_img.enabled = false;
        m_imgfill.enabled = false;
        m_waves.enabled = false;
    }
    /// <summary>�E�F�[�u���o���X�g�b�v���܂�</summary>
    public void StopWave() { StopCoroutine("Waves"); }

    /// <summary>���݂̌x���x�̊�����ݒ肵�܂�</summary>
    public void SetRate(float rate) {
        m_imgfill.fillAmount = rate;
        //�x���x���X�V���ꂽ�ꍇ�̏���
        if (rate > m_waveInterval && InvManager.Instance.VigilanceFlag) {
            StartCoroutine(Waves(rate));
            m_waveInterval += WaveInterval * Mathf.Floor(rate/m_waveInterval);
        }
    }

    /// <summary>�Q�[�W�̏�Ԃ��O�ɂ��܂�</summary>
    public void ResetGauge() {
        m_waveInterval = 0;
        SetRate(0);
    }
    /// <summary>�Q�[�W�̏�Ԃ��w�肵�������l�ɖ߂��܂�</summary>
    public void ResetGauge(float rate) {
        m_waveInterval = rate;
        SetRate(rate);
    }

    /// <summary>�S���̃A�j���[�V�������s���܂� TODO �A�j���[�V�����ɐ؂�ւ��邩��</summary>
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
