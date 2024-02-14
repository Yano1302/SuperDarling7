using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// �I�[�f�B�I���ꊇ�Ǘ�����V���O���g���N���X�ł��B�C���X�^���X��AudioManager.Instance�Ŏ擾�ł��܂�<br />
/// SE/BGM�p��Resources�t�H���_���쐬���A�����ɉ������i�[���Ă�������<br />
/// �Đ����͉����t�@�C�����ōs���̂ŁA�����K���Ȃǂ�݂��Ă�������<br />
/// �쐬�҂�PG2�N�̖��m���ł��@�^��_����P�_�A�������Ăق����@�\�A�o�O�񍐓�����΋C�y�ɂ��A����������<br />
/// </summary>

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
// Config�ϐ�  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    private const int    c_MaxSoundOverlap = 10;                     //�I�[�f�B�I�̍ő��(BGM����)
    private const string c_FolderPath_BGM = "Audio\\BGM\\Resources"; //BGM���i�[����Ă���t�H���_�̃p�X
    private const string c_FolderPath_SE = "Audio\\SE\\Resources";   //SE���i�[����Ă���t�H���_�̃p�X

    private const float c_SoundVolume = 0.5f;                        //SoundVolume�̏����l
    private const float c_FadeTimer = 1.0f;                          //FadeTime�̃f�t�H���g�l
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


//  �ϐ��E�֐��ꗗ    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //  �ݒ�Ȃ�    //

    /// <summary>���y���Đ��\���ǂ����̃t���O�ł��B���������Afalse�ɂ������Ɋ��ɍĐ�����Ă��鉹�y�͒�~����܂���</summary>
    public bool CanPlay { get; set; } = true;

    /// <summary>�t�F�[�h�ɂ����鎞�Ԃ̃f�t�H���g�l���擾�E�ύX���܂�</summary>
    public float DefaultFadeTime { get { return m_defaltTime; } set { m_defaltTime = value; } }



    //  ���ʐݒ�    //

    /// <summary>�T�E���h�̉��ʂ̃f�t�H���g�l��ݒ�E�擾���܂�</summary>
    public float DefaultVolume { get { return m_defaltVolume; } set { m_defaltVolume = value; } }

    /// <summary>���݂�BGM�̉��ʂ�ݒ�E�擾���܂��B</summary>
    public float BGM_Volume {
        get {
            GameSystem.Action(() => { if (!m_BGMSource.isPlaying) { Log("BGM�͍Đ�����Ă��܂���B", true); } else { Log("���݂�BGM�ɐݒ肳��Ă��鉹�ʂ�" + m_BGMSource.volume + "�ł��B", false); } }); 
            return m_BGMSource.volume;
        }
        set {
            GameSystem.Action(() => { if (!m_BGMSource.isPlaying) { Log("BGM�͍Đ�����Ă��܂���B�Đ����ɉ��ʐݒ肪�㏑������鋰�ꂪ����܂��B", true); } else { Log("BGM�̉��ʂ�" + value+"�ɕύX����܂��B",false); } });
            m_BGMSource.volume = value; 
        }
    }

    /// <summary>BGM�̉��ʂ����X�ɕω������܂�   <br/>���t�F�[�h���s���t�F�[�h�����ȊO����BGM�̉��ʂ��ω������ꍇ�t�F�[�h�����͒�~����܂��B<br/>���n���ꂽ�֐����Ă΂�܂��� </summary>
    /// <param name="volume">�ݒ肷�鉹��</param>
    /// <param name="fadeTime">�ݒ肵�����ʂɂȂ�܂ł̎���(�b)�@���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�)</param>
    public void BGM_SetVolume_Fade(float volume, float fadeTime = float.NaN, UnityAction action = null) {
        if (m_BGMSource.isPlaying) {
            Log(m_BGMSource.clip.name + "�̉��ʂ����X�ɕύX����܂��B \n �ύX��̉��� : " + volume, false);
            //�f�t�H���g�l�ł���ΐ������l�ɕύX����
            fadeTime = float.IsNaN(fadeTime) ? m_defaltTime : fadeTime;
            StartCoroutine(FadeSound(m_BGMSource, fadeTime, volume, action, false));
        }
        else {
            Log("BGM���Đ�����Ă��Ȃ��̂ŉ��ʂ�ύX���鎖���ł��܂���B", true);
            return;
        }
    }

    public bool SetAllVolume();

    /// <summary>����PlaySound����Đ�����Ă���T�E���h�̉��ʂ�ύX���܂�</summary>
    /// <param name="volume"></param>
    /// <param name="clipName">�T�E���h�̎w��(�����T�E���h���������ꍇ�͂��ׂĕύX����܂�)</param>
    /// <returns>���������������ꍇ��true��Ԃ��܂�</returns>
    public bool  SetVolume(string clipName,float volume) {
        bool check = GetUsingSource(out var sds);
            foreach(var sd in sds) {
                if (sd.Source.isPlaying) {
                  sd.Source.volume = volume;
                Log(sd.Source.clip + "�̉��ʂ�" + volume + "�ɕύX����܂��B", false);
                }
            }
            if (!check) {
                Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B : " + clipName, true);
            }
        return check;
    }

    /// <summary>BGM�������Đ�����Ă��鉹���̉��ʂ����X�ɕύX���܂��B</summary>
    /// <param name="clipName">�����w��(�w�肳�ꂽ��������������ꍇ�͑S�ĕύX����܂�)</param>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="fadeTime">�t�F�[�h����</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�) 
    /// <br/>���t�F�[�h���s���t�F�[�h�����ȊO����BGM�̉��ʂ��ω������ꍇ�t�F�[�h�����͒�~����܂��B���n���ꂽ�֐����Ă΂�܂���
    /// <br/>�܂��t�F�[�h����������O�ɍĐ����I�������ꍇ���Ă΂�܂���B�Đ��I�����擾�������ꍇ�̓t�F�[�h���Ԃ�Z�����邩�APlaySound�Ɋ֐���n�����s���ASetVolumeFade����t�F�[�h���������Ă��������B
    /// </param>
    /// <returns>�Đ�����Ă��鉹�������������ꍇ��true��Ԃ��܂�</returns>
    public bool SetVolume_Fade(string clipName,float volume,float fadeTime = float.NaN,UnityAction action = null) {
        if (GetUsingSource(out var sds)) {
            bool check = false;
            foreach (var sd in sds) {
                if (clipName == sd.Source.clip.name) {
                    StartCoroutine(FadeSound(sd.Source, fadeTime, volume, action, false));
                    check = true;
                }
            }
            return check;
        }
        return false;
    }   



    //  �Đ��֐�    //

    /// <summary>BGM���Đ����܂��B����BGM���Đ�����Ă����ꍇ�͂���BGM�͒�~����܂��B</summary>
    /// <param name="clipName">�炵���������t�@�C����(�g���q�s�v)</param>
    /// <param name="volume">����(�O�`�P)�@���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    /// <returns>�Đ��Ɏg�p���Ă���I�[�f�B�I�\�[�X��Ԃ��܂�</returns>
    public void BGM_Play(string clipName, float volume = float.NaN) {
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }
        //�������擾����
        bool check = ClipList_BGM.TryGetValue(clipName, out AudioClip clip);
        //�������擾�ł��Ȃ������ꍇ��SE��������擾����
        if (!check) {
            check = ClipList_SE.TryGetValue(clipName, out clip);
            //SE����������擾�ł��Ȃ������ꍇ
            if (!check) {
                Debug.Assert(clip != null, "�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
                return;
            }
            Log("BGM�Ƃ���SE�������Đ�����܂�", true);
        }
        //�f�t�H���g�l�ł���ΐ������l��^����
        m_BGMSource.volume = float.IsNaN(volume)? m_defaltVolume : volume;
        m_BGMSource.clip = clip;
        m_BGMSource.loop = true;
        m_BGMSource.Play();
        Log(clip, true);
    }

    /// <summary>BGM���t�F�[�h�C���Đ������܂��B����BGM���Đ�����Ă���ꍇ�A����BGM�͒�~����܂��B <br/>���t�F�[�h���s���t�F�[�h�����ȊO����BGM�̉��ʂ��ω������ꍇ�t�F�[�h�����͒�~����܂��B<br/>���n���ꂽ�֐����Ă΂�܂��� </summary>
    /// <param name="clipName">�炵���������t�@�C����</param>
    /// <param name="fadeTime">�t�F�[�h�C��(endVolume)�ɂȂ�܂ł̎���(�b)�@���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="startVolume">���߂̉��̑傫��(�O�`�P)</param>
    /// <param name="endVolume">�I���̉��̑傫��(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�)</param>
    public void BGM_Play_Fadein(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, UnityAction action = null) {
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }
        //�f�t�H���g�l�ł���ΐ������l��^����
        fadeTime = float.IsNaN(fadeTime) ?m_defaltTime : fadeTime;
        endVolume = float.IsNaN(endVolume) ? m_defaltVolume : endVolume;
        m_BGMSource.volume = startVolume;
        BGM_Play(clipName, startVolume);
        StartCoroutine(FadeSound(m_BGMSource, fadeTime, endVolume, action, false));
    }

    /// <summary>�T�E���h���Đ����܂��B���SE���Đ�����ꍇ�Ɏg�p���Ă�������</summary>
    /// <param name="clipName">�炵���������t�@�C����(�g���q�s�v)</param>
    /// <param name="volume">����(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    /// <param name="IsLoop">���[�v�Đ�������ꍇ��ture</param>
    /// <param name="action">�Đ����ꂽ��������~���ꂽ�ꍇ�Ɋ֐������s���܂��B(�K�v�ȏꍇ�̂�)
    /// <br/>���������Ō�܂ōĐ����ꂸ��~���ꂽ�ꍇ�ɂ��֐������s����܂��B���ӂ��Ă��������B
    /// <br/>���܂��A���[�v���L���ȏꍇ�̓��[�v�Đ�����~�����܂Ŋ֐��͌Ă΂�܂���B</param>
    /// <returns>�Đ����Ɋ���U��ꂽID���擾���܂��B�Đ����o���Ȃ������ꍇ��int.MinValue��Ԃ��܂�</returns>
    public int PlaySound(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //���݉���点���Ԃ���ɒ��ׂ�
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return int.MinValue;
        }
        if (!GetNotUsedSource(out SoundData sd)) {
            return int.MinValue;
        }
        //�������擾����
        bool check = ClipList_SE.TryGetValue(clipName, out AudioClip clip);
        //�������擾�ł��Ȃ������ꍇ��BGM��������擾����
        if (!check) {
            check = ClipList_BGM.TryGetValue(clipName, out clip);
            //BGM����������擾�ł��Ȃ������ꍇ��null��Ԃ�
            if (!check) {
                GameSystem.LogError("�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
                return int.MinValue;
            }
            Log("PlaySound�֐�����BGM�������Đ�����܂��B", true);
        }
        //���ʂȂǂ̐ݒ�
        //�f�t�H���g�l�ł���ΐ������l��^����
        sd.Source.volume = float.IsNaN(volume) ? m_defaltVolume : volume;
        sd.Source.loop = IsLoop;
        sd.Source.clip = clip;
        sd.Source.Play();
        Log(clip, true);
        if (action != null) {
            StartCoroutine(CheckPlaying(sd.Source, action));
        }
        return sd.ID;
    }

   


    //  ��~�֐�    //
    /// <summary>BGM���~�����܂�</summary>
    public void BGM_Stop() {
        if (m_BGMSource.isPlaying) {
            Log(m_BGMSource.clip, false);
        }
        else {
            Log("BGM�͍Đ�����Ă��܂���ł���", true);
        }
        m_BGMSource.Stop();
    }

    /// <summary>BGM���t�F�[�h�A�E�g������~���܂��B <br/>���t�F�[�h���s���t�F�[�h�����ȊO����BGM�̉��ʂ��ω������ꍇ�t�F�[�h�����͒�~����܂��B�B<br/>���n���ꂽ�֐����Ă΂�܂���</summary>
    /// <param name="fadeTime">�t�F�[�h�A�E�g�܂ł̎���(�b) ���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h�A�E�g��ɍs�����s�֐�(�s���K�v���Ȃ��ꍇ��null��n���Ă�������)</param>
    /// <param name="endVolume">��~���钼�O�̉���(�O�`�P)</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�)</param>
    public void BGM_Stop_FadeOut(float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null) {

        if (m_BGMSource.isPlaying) {
            Log(m_BGMSource.clip.name + "���t�F�[�h�A�E�g����܂�", false);
            //�f�t�H���g�l�ł���ΐ������l�ɕύX����
            fadeTime = float.IsNaN(fadeTime) ? m_defaltTime : fadeTime;
            StartCoroutine(FadeSound(m_BGMSource, fadeTime, endVolume, action, true));
        }
        else {
            Log("BGM�͍Đ�����Ă��܂���ł���", true);
            return;
        }

    }

    /// <summary>�w�肳�ꂽ�T�E���h���~�����܂�</summary>
    /// <param name="clipName">�T�E���h�w��</param>
    /// <returns>�w�肳�ꂽ�T�E���h���Đ����ł����true��Ԃ��܂�</returns>
    public bool StopSound(string clipName) {
        bool check = GetUsingSource(out var sds);
        if (check) {
            foreach (var sd in sds) {
                if (sd.Source.clip.name == clipName) {
                    sd.Source.Stop();
                }
            }
        }
        return check;
    }

    /// <summary>�Đ�����Ă���T�E���h��S�Ē�~�����܂��B</summary>
    /// <returns>�Ăяo�����_�ōĐ�����Ă���T�E���h���������ꍇ��true��Ԃ��܂��B
    /// <br/>IsIncludingBGM��true�̏ꍇ��BGM�݂̂��Đ�����Ă����ꍇ�ł�true��Ԃ��܂�</returns>
    public bool StopAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (IsIncludingBGM) {
            check = m_BGMSource.isPlaying;
            BGM_Stop();
        }
        string str = check ? "BGM�ȊO�ɍĐ�����Ă��鉹���͂���܂���ł����B":null;     
        if (GetUsingSource(out var sds, str)) {
            foreach (var ad in sds) {
                ad.Source.Stop();
                check = true;
            }
        }
        return check;
    }

    /// <summary>PlaySoud����Đ����ꂽ���[�v���̃T�E���h�̍Đ������ׂĒ�~���܂�</summary>
    /// <returns>PlaySound���烋�[�v�Đ����̃T�E���h���������ꍇ��true��Ԃ��܂�</returns>
    public bool StopLoopSound() {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var ad in sds ) {
                if (ad.Source.isPlaying && ad.Source.loop) {
                    check = true;
                    Log(ad.Source.clip,false);
                    ad.Source.Stop();
                }
            }
            if (!check) {
                Log("���[�v�Đ�����Ă��鉹���͂���܂���ł����B", true);
            }
        }
        return check;
    }

    /// <summary>���[�v���ł͂Ȃ��T�E���h��S�Ē�~�����܂��B </summary>
    /// <returns>���[�v�ł͂Ȃ��Đ����̃T�E���h���������ꍇ��true��Ԃ��܂�</returns>
    public bool StopNotLoopSound()
    {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.Source.isPlaying && !sd.Source.loop) {
                    check = true;
                    Log(sd.Source.clip, false);
                    sd.Source.Stop();
                }
            }
            if (!check) {
                Log("���[�v�Đ�����Ă��Ȃ������͂���܂���ł����B", true);
            }
        }
        return check;
    }
    
   


    //  �ꎞ��~�E�ĊJ   //

    /// <summary>BGM���|�[�Y���܂�</summary>
    public void BGM_Pause() {
        if (m_BGMSource.isPlaying) {
            m_BGMSource.Pause();
            Log("BGM���|�[�Y����܂�", false);
        }
        else {
            Log("BGM�͍Đ�����Ă��܂���", true);
        }

    }

    /// <summary>�|�[�Y����BGM���ĊJ���܂�</summary>
    public void BGM_Restert() {
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }

        if (!m_BGMSource.isPlaying && m_BGMSource.time != 0) {
            m_BGMSource.UnPause();
            Log("BGM�̃|�[�Y��Ԃ���������܂�", false);
        }
        else {
            GameSystem.LogError("BGM�̓|�[�Y����Ă��܂���B");
        }

    }

    /// <summary>�Đ�����Ă���T�E���h��S�ă|�[�Y���܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    /// <returns>�Đ�����Ă���T�E���h���������ꍇtrue��Ԃ��܂�
    /// </br>IsIncludingBGM��true�̏ꍇ��BGM�݂̂��Đ�����Ă����ꍇ��true��Ԃ��܂��B</returns>
    public bool PauseAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (IsIncludingBGM) {
            check = m_BGMSource.isPlaying;
            BGM_Pause();
        }
        string str = check ? "BGM�ȊO�ɍĐ�����Ă��鉹���͂���܂���ł����B" : m_BGMSource.isPlaying ? "BGM�ȊO�ɍĐ�����Ă��鉹���͂���܂���ł����B(BGM�̓|�[�Y����܂���)" : null;
        if (GetUsingSource(out var sds,str)) {
            check = true;
            foreach (var sd in sds) {
                sd.Source.Stop();
            }
        }
        return check;
    }

    /// <summary>�S�Ẵ|�[�Y����Ă��鉹�����ĊJ�����܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    /// <returns>�|�[�Y���̉������������ꍇ��true��Ԃ��܂�
    /// <br/>IsIncluding��true�̏ꍇ��BGM�݂̂��|�[�Y���������ꍇ�ł�true��Ԃ��܂�
    /// <br/>������CanPlay(�Đ��\�t���O)��false�������ꍇ�̓|�[�Y���̉������������ꍇ�ł�false��Ԃ��܂��B</returns>
    public bool RestertAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return�@check;
        }

        if (!m_BGMSource.isPlaying && m_BGMSource.time != 0) {
            check = true;
            if (IsIncludingBGM) {
                m_BGMSource.Play();
            }
        }
        bool c2 = GetNotUsedSource(out List<SoundData> sds);
        if (c2) {
            foreach (var sd in sds) {
                if (!sd.Source.isPlaying && sd.Source.time != 0) {
                    Log(sd.Source.clip,true);
                    sd.Source.Play();
                    check = true;
                }
            }
            if (IsIncludingBGM && !check) {
                Log("BGM�̓|�[�Y���ł͂���܂���ł����B",true);
            }
            return c2;
        }
        else if (check) {
            string str = IsIncludingBGM ? "BGM�݂̂��|�[�Y����ĊJ����܂�":"BGM�ȊO�̃|�[�Y���̉����͂���܂���B(�|�[�Y�͉�������܂���)";
            Log(str, true);
        }
        else{
            Log("�|�[�Y���̉����͂���܂���ł����B",true);
        }
        return check;
    }



    //  ���̑�   //

    /// <summary>�w�肳�ꂽ�������擾���܂�</summary>
    /// <returns>��������Ԃ��܂��B������Ȃ������ꍇ��null��Ԃ��܂��B</returns>
    public AudioClip GetAudioClip(string clipName) {
        bool check = ClipList_BGM.TryGetValue(clipName, out AudioClip audioClip);
        if (check) {
            return audioClip;
        }
        
        check = ClipList_SE.TryGetValue(clipName,out audioClip);
        if (check) {
            return audioClip;
        }
        else {
            GameSystem.LogError("�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
            return null;
        }
    }


    //���ݍĐ�����Ă��鉹�������擾���܂��B


    /// <summary>(�f�o�b�O�p)���Đ�����Ă��鉹�������ׂăR���\�[����ɕ\�����܂�</summary>    
    [Conditional("UNITY_EDITOR")]
    public void IsPlayingSoundLog(){
        if (m_BGMSource.isPlaying){
            Debug.Log("�Đ�����Ă���BGM : " + m_BGMSource.clip.name);
        }

         if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                Debug.Log("�Đ�����Ă���T�E���h : " + sd.Source.clip.name);
            }
         }       
      }
      
    




    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------

    //�I�[�f�B�I�\�[�X���Ǘ�����\����
    private struct SoundData {
        public AudioSource Source;
        public int ID;
    }
    //�g�p����I�[�f�B�I�\�[�X
    private SoundData[] m_SDatas;                       
    private AudioSource m_BGMSource = null;
    //�������X�g
    private Dictionary<string, AudioClip> ClipList_BGM;     //BGM�̃��X�g
    private Dictionary<string, AudioClip> ClipList_SE;      //SE�̃��X�g
    //���̑�
    private float m_defaltVolume = c_SoundVolume;           //�f�t�H���g��volume
    private float m_defaltTime = c_FadeTimer;               //�f�t�H���g�̃t�F�[�h����
    private int m_IDN = int.MinValue + 1;                   //ID�z�z�p�̔ԍ�
    [SerializeField, TooltipAttribute("�Đ�����鉹���Ȃǂ����O�ɕ\�����邩�ǂ���(�G���[���b�Z�[�W�͏���)")] private bool PlaySoundLog = true;
    //  �v���C�x�[�g�֐�  //------------------------------------------------------------------------------------------------------------------------------

    //  �������֐�   //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        //�I�[�f�B�I�\�[�X�̍쐬----------------------------------------------------
        if (m_BGMSource == null) {
            m_BGMSource = gameObject.AddComponent<AudioSource>();
            m_SDatas = new SoundData[c_MaxSoundOverlap - 1];
            for (int i = 0; i < m_SDatas.Length; ++i) {
                m_SDatas[i].Source = gameObject.AddComponent<AudioSource>();
                m_SDatas[i].ID = int.MinValue;
            }
            //�t�@�C���̓ǂݍ���----------------------------------------------------
            ClipList_BGM = new Dictionary<string, AudioClip>();
            ClipList_SE = new Dictionary<string, AudioClip>();
            SetAudioClip(ClipList_BGM,c_FolderPath_BGM);
            SetAudioClip(ClipList_SE,c_FolderPath_SE);
        }
        //--------------------------------------------------------------------------
    }

    //  ���s�֐�    //-----------------------------------------------------------------------------

    /// <summary>�g���Ă��Ȃ��I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetNotUsedSource(out SoundData s)
    {
      for (int i = 0; i < m_SDatas.Length; ++i) {
          if (!m_SDatas[i].Source.isPlaying && m_SDatas[i].Source.time == 0) {
                m_SDatas[i].ID = GetID();
                s = m_SDatas[i];
                return true;
          }
      }
        Log("��x�ɖ点�鉹�̍ő吔�𒴂��Ă��܂�",true);
        s.ID = int.MinValue;
        s.Source = null;
        return false;
    }
    /// <summary>�g���Ă��Ȃ��I�[�f�B�I�\�[�X�̎擾(�S��)</summary>
    private bool GetNotUsedSource(out List<SoundData> s) {
        bool check = false;
        s = new List<SoundData> ();
        for (int i = 0; i < m_SDatas.Length; ++i) {
            if (!m_SDatas[i].Source.isPlaying && m_SDatas[i].Source.time == 0) {
                m_SDatas[i].ID = GetID();
                s.Add(m_SDatas[i]);
                check = true;
            }
        }
        return check;
    }

    /// <summary>�g���Ă���I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetUsingSource(out List<SoundData> s,string noneLog = null) {
        s = new List<SoundData> ();
        bool check = false;
        for (int i = 0; i < m_SDatas.Length; ++i) {
            if (m_SDatas[i].Source.isPlaying) {
                s.Add (m_SDatas[i]);
                check = true;
            }
        }
        GameSystem.Action(() => {
            if (!check) {
               noneLog = noneLog ?? "�Đ�����Ă��鉹���͂���܂���ł����B";
                Log(noneLog,true);
            } 
        });

        return check;
    }

    /// <summary> �t�H���_�̃p�X����t�H���_���̑S�Ẳ��������[�h���A���X�g�ɒǉ�����</summary>
    private void SetAudioClip(Dictionary<string,AudioClip> dic, string path) {

        path = Application.dataPath + "/" + path;
        string[] fileNames = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            .Where(s => !s.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase)).ToArray();
        Debug.Assert(fileNames != null, "�t�H���_��������܂���ł���");
        for(int i = 0; i < fileNames.Length; i++) {
            //�v���n�u�������𔲂����
            string[] part = fileNames[i].Split('\\');            
            int index = part[part.Length - 1].IndexOf('.');
            fileNames[i] = part[part.Length - 1].Remove(index);
        }
        foreach (string name in fileNames) {
            AudioClip audio = (AudioClip)Resources.Load(name);
            dic[name] = audio;
        }
    }

    /// <summary>���̃t�F�[�h�C���A�A�E�g���s��</summary>
    private IEnumerator FadeSound(AudioSource s,float time,float ev, UnityAction action, bool stop) {
        float sv = s.volume - 0.001f;
        s.volume = sv;
        float vo = sv;
        float t = 0;
        float startTime = Time.time;
        yield return null;
        while (true) {
            if (vo != s.volume) {
                Log("�O������t�F�[�h�Ώۂ̉��ʂ����삳�ꂽ�̂Ńt�F�[�h����~����܂��B�t�F�[�h�J�n���ɓn���ꂽ�֐��͎��s����܂���", true);
                break;
            }

            if (!s.isPlaying) {
                if(s.time == 0) {
                    Log("�t�F�[�h���ɍĐ�����~����܂����B�t�F�[�h�J�n���ɓn���ꂽ�֐��͎��s����܂���", true);
                    break;
                }
            }
            else {
                vo = Mathf.SmoothStep(sv, ev, t);
                s.volume = vo;
                t += Time.deltaTime / time;
            }         
            //���[�v�𔲂������
            if (t >= 1) {
                if (s.isPlaying) {
                    Log(s.clip.name + "�̃t�F�[�h���������܂���", false);
                    if (stop) {
                        s.Stop();
                    }
                    else {
                        s.volume = ev;
                    }
                }
                action?.Invoke();
                break;
            }
            //���̃��[�v�܂�1�t���[���ҋ@����
            yield return null;
        }
    }

    /// <summary>�������Đ��I������܂ŊĎ�����</summary>
    private IEnumerator CheckPlaying(AudioSource s,UnityAction action) {
        while (true) {
            if (!s.isPlaying && s.time == 0) {
                Log(s.clip.name + "�̍Đ����I�����܂����B�֐��̎��s�Ɉڂ�܂�",false);
                action.Invoke();
                yield break;    
            }
            yield return null;
        }
    }

    /// <summary>�T�E���h�����ʂ���ID���擾���܂�(�ȈՓI)</summary>
    private int GetID(){
        int id = m_IDN == int.MinValue ? int.MinValue + 1 : m_IDN;
        m_IDN++;
        return id;
    }

    //  �f�o�b�O�p   //----------------------------------------------------------------------------

    /// <summary>���O�̕\�����L���ȏꍇ�ɉ����������O�ɕ\������</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(AudioClip clip,bool play) { 
        if (PlaySoundLog) {
            string str = play ? "<color=cyan>" + clip.name + "</color>" + "���Đ�����܂�" : "<color=cyan>" + clip.name + "</color>" + "����~����܂�";
            Debug.Log(str); 
        } 
    }

    /// <summary>���O�̕\�����L���ȏꍇ�Ƀ��b�Z�[�W�����O�ɕ\������</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(string message,bool warning) {
        if (PlaySoundLog) {
            if (warning) {
                Debug.LogWarning("<color=yellow>"+message+"</color>");
            }
            else {
                Debug.Log(message);
            }
        }
    }
}






