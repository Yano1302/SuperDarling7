using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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
    private const string c_FolderPath_BGM = "Audio\\BGM\\Resources"; //BGM���i�[����Ă���t�H���_�̃p�X
    private const string c_FolderPath_SE = "Audio\\SE\\Resources";   //SE���i�[����Ă���t�H���_�̃p�X
    private const int c_MaxSoundOverlap = 10;                    �@�@//�I�[�f�B�I�̍ő��(BGM����)s
    private const int c_SoundScale = 10;                             //���̑傫�������i�K�ɕ����邩
    private const float  c_DfaultSoundVolume = 0.5f;                 //SoundVolume�̏����l
    private const float  c_DefaultFadeTime = 2.0f;                  //FadeTime�̃f�t�H���g�l
   
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


//  �ϐ��E�֐��ꗗ    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //  �ݒ�Ȃ�    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>���y���Đ��\���ǂ����̃t���O�ł��B���������Afalse�ɂ������Ɋ��ɍĐ�����Ă��鉹�y�͒�~����܂���</summary>
    public bool CanPlayFlag { get { return m_canPlayFlag; } set { Log("�Đ��\�t���O�� " + value + " �ɕύX����܂�",false); m_canPlayFlag = value; } }

    /// <summary>�t�F�[�h�ɂ����鎞�Ԃ̃f�t�H���g�l���擾�E�ύX���܂�</summary>
    public float DefaultFadeTime { get { return m_defaltTime; } set { m_defaltTime = value; } }

    /// <summary>�T�E���h�̉��ʂ̃f�t�H���g�l��ݒ�E�擾���܂�</summary>
    public float DefaultVolume { get { return m_defaltVolume; } set { m_defaltVolume = value; } }

    /// <summary>�T�E���h�̏o�͊�����ݒ�E�擾���܂��B</summary>
    public int CurrentScale { get; set; }�@//TODO

    //  ���ʐݒ�    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
//BGM
    /// <summary>���݂�BGM�̉��ʂ�ݒ�E�擾���܂��B</summary>
    public float BGM_Volume {
        get {
            if (!m_BGMData.Source.isPlaying) { Log("BGM�͍Đ�����Ă��܂���B", true); } 
            return m_BGMData.Source.volume;
        }
        set {
            if (!m_BGMData.Source.isPlaying) { Log("BGM�͍Đ�����Ă��܂���B�Đ����ɉ��ʐݒ肪�㏑������鋰�ꂪ����܂��B", true); } else { Log("BGM�̉��ʂ�" + value + "�ɕύX����܂��B", false); }
            m_BGMData.Source.volume = value; 
        }
    }

    /// <summary>BGM�̉��ʂ����X�ɕω������܂��BBGM���Đ�����Ă��Ȃ������ꍇ�ɂ͏������s���܂���B
    /// <br/>���܂��A�t�F�[�h�������ɐV�����t�F�[�h�������Ă΂ꂽ�ꍇ�͏㏑������܂��B </summary>
    /// <param name="volume">�ݒ肷�鉹��</param>
    /// <param name="fadeTime">�ݒ肵�����ʂɂȂ�܂ł̎���(�b)�@���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h������ɍs�����s�֐�
    /// <br/>���t�F�[�h���㏑�����ꂽ��A�t�F�[�h�r����BGM����~���ꂽ�ꍇ�ɂ͌Ă΂�܂���B
    /// </param>
    /// <returns>�Ăяo������BGM���Đ�����Ă����true��Ԃ��܂��B</returns>
    public bool BGM_FadeVolume(float volume, float fadeTime = float.NaN, UnityAction action = null) {
        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData.Source.clip.name + "(BGM)�̉��ʂ����X�ɕύX����܂��B \n �ύX��̉��� : " + volume, false);
            StartCoroutine(FadeSound(m_BGMData, fadeTime, volume, action, false,false));
            return true;
        }
        else {
            Log("BGM���Đ�����Ă��Ȃ��̂ŉ��ʂ�ύX���鎖���ł��܂���B", true);
            return false;
        }
    }


//SE
    /// <summary>�Đ�����Ă���SE�̉��ʂ�ύX���܂��B(���̎w��)�w�肳�ꂽ�����Ɠ���������SE����������ꍇ�͑S�ĕύX����܂��B</summary>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="clipName">�T�E���h�̎w��(�����T�E���h���������ꍇ�͂��ׂĕύX����܂�)</param>
    /// <returns>���������������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_Volume(string clipName,float volume) {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {               
                if (sd.Source.isPlaying && clipName == sd.Source.clip.name) {
                    sd.Source.volume = volume;
                    Log(sd.Source.clip + "�̉��ʂ�" + volume + "�ɕύX����܂��B", false);
                    check = true;
                }
                
            }
            if (!check) {
                Log("�w�肳�ꂽ��������SE�͍Đ�����Ă��܂���ł����B\n�w�肳�ꂽ������ : "+clipName,true);
            }
        }
        return check;
    }
    /// <summary>�Đ�����Ă���SE�̉��ʂ�ύX���܂��B(ID�w��)</summary>
    /// <param name="id">�ύX������SE��ID</param>
    /// <param name="volume">�ύX��̉���</param>
    /// <returns>�w�肳�ꂽID��SE���������ꍇ��true��Ԃ��܂��B</returns>
     public bool SE_Volume(int id, float volume) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.Source.isPlaying) {
                    sd.Source.volume = volume;
                    Log(sd.Source.clip + "�̉��ʂ�" + volume + "�ɕύX����܂��B", false);
                    return true;
                }
            }
        }
        Log("�w�肳�ꂽID��SE�͍Đ�����Ă��܂���ł����B\nID : " + id, true);
        return false;
    }

    /// <summary>�Đ�����Ă���SE�̉��ʂ����X�ɕύX���܂��B(���̎w��)�w�肳�ꂽ�����Ɠ���������SE����������ꍇ�͑S�ĕύX����܂��B
    /// <br/>�w�肳�ꂽSE���Đ�����Ă��Ȃ������ꍇ�͏��������s����܂���B
    /// <br/>�܂��A�w�肳�ꂽ�T�E���h�����Ƀt�F�[�h�������ł���΃t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�����w��(�w�肳�ꂽ��������������ꍇ�͑S�ĕύX����܂�)</param>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="fadeTime">�t�F�[�h����</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�) <br/>���t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���B</param>
    /// <param name="StopAction">�t�F�[�h�����O�ɍĐ�����~�����ꍇ��action�����s���邩�ǂ���</param>
    /// <returns>�Đ�����Ă��鉹�������������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_SetVolumeFade(string clipName,float volume,float fadeTime = float.NaN,UnityAction action = null,bool StopAction = false) {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (clipName == sd.Source.clip.name) {
                    StartCoroutine(FadeSound(sd, fadeTime, volume, action,false,StopAction));
                    check = true;
                }
            }
            if (!check) {
                Log("�w�肳�ꂽSE���݂���܂���ł����B" + "\n������ : " + clipName + "\n", true);
            }
        }
        return check;
    }
    /// <summary>�Đ�����Ă���SE�̉��ʂ����X�ɕύX���܂��B(ID�w��)
    /// <br/>�w�肳�ꂽSE���Đ�����Ă��Ȃ������ꍇ�͏��������s����܂���B
    /// <br/>�܂��A�w�肳�ꂽ�T�E���h�����Ƀt�F�[�h�������ł���΃t�F�[�h�������㏑������܂��B</summary>
    /// <param name="id">�ύX������SE��ID</param>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="fadeTime">�t�F�[�h����</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�) <br/>���t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���B</param>
    /// <param name="StopAction">�t�F�[�h�����O�ɍĐ�����~�����ꍇ��action�����s���邩�ǂ���</param>
    /// <returns>�Đ�����Ă��鉹�������������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_SetVolumeFade(int id,float volume, float fadeTime = float.NaN, UnityAction action = null, bool StopAction = false) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (id == sd.ID) {
                    StartCoroutine(FadeSound(sd, fadeTime, volume, action, false, StopAction));
                    return true;
                }
            }
        }
        Log("�w�肳�ꂽSE���݂���܂���ł����B" + "\nID : " + id + "\n", true);
        return false;
    }

//ALL
    /// <summary>�Đ����̑S�ẴT�E���h�̉��ʂ�ύX���܂��B</summary>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="IncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void ALL_SetVolume(float volume, bool IncludingBGM) {
        if (IncludingBGM) {
            BGM_Volume = volume;
        }

        if (GetUsingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                if (sd.Source.isPlaying) {
                    sd.Source.volume = volume;
                    Log(sd.Source.volume + "(SE)�̉��ʂ��ύX����܂��B \n �ύX��̉��� : " + volume, false);
                }
            }
        }
        else {
            Log("SE�͍Đ�����Ă��܂���ł����B", true);
        }
    }

    /// <summary>�Đ����̑S�ẴT�E���h�̉��ʂ��t�F�[�h�ύX���܂��B�T�E���h���Đ�����Ă��Ȃ������ꍇ�ɂ͏����͍s���܂���B</summary>
    /// <param name="volume">�t�F�[�h��̉���</param>
    /// <param name="IncludingBGM">BGM���Ώۂɂ��邩</param>
    /// <param name="fadeTime">�t�F�[�h����</param>
    /// <param name="action">�t�F�[�h��̊֐��Ăяo��</param>
    /// <param name="StopAction">IncludingBGM��false�̍ۂɑS�Ă�SE���t�F�[�h�O�ɍĐ��I�������ꍇ��action���N�����邩�ǂ���</param>
    /// <returns>�t�F�[�h�ύX���s������������ꍇ��true��Ԃ��܂��B</returns>
    public bool ALL_FadeVolume(float volume, bool IncludingBGM,float fadeTime = float.NaN, UnityAction action = null, bool StopAction = false) {
        bool check = false;
        if (IncludingBGM && m_BGMData.Source.isPlaying) {
            BGM_FadeVolume(volume, fadeTime, action);
            check = true;
        }

        if (GetUsingSource(out List<SoundData> sds)) {
            check = true;
            if (IncludingBGM && m_BGMData.Source.isPlaying) {
                foreach (var sd in sds) {
                    StartCoroutine(FadeSound(sd, fadeTime, volume, null, false, false));       
                }
            }
            else {
                SoundData restSd = null;
                float resttime = 0;
                //�c��b������Ԓ���SE��T��
                foreach (var sd in sds) {
                    float rt = sd.Source.clip.length - sd.Source.time;
                    if (rt > resttime) {
                        if (restSd != null) { StartCoroutine(FadeSound(restSd, fadeTime, volume, null, false, false)); }
                        restSd = sd;
                        resttime = rt;
                    }
                    else {
                        StartCoroutine(FadeSound(sd, fadeTime, volume, null, false, false));
                    }
                }
                //�c��b������Ԓ���SE��action��n��
                StartCoroutine(FadeSound(restSd, fadeTime, volume, action, false, StopAction));
            }  
        }
        return check;
    }



    //  �Đ��֐�    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //BGM
    /// <summary>BGM���Đ����܂��B����BGM���Đ�����Ă����ꍇ�͂���BGM�͒�~����܂��B</summary>
    /// <param name="clipName">�炵����������(SE�������Đ��\�ł�)</param>
    /// <param name="volume">����(�O�`�P)�@���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    public void BGM_Play(string clipName, float volume = float.NaN) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }
        if(m_BGMData.Source.isPlaying || m_BGMData.Source.time != 0) {
            Log("���ɍĐ�����Ă���BGM���㏑������܂��B", true);
            m_BGMData.Source.Stop();
        }
        //�������擾����
        if (GetClip(out AudioClip clip, clipName, true)) {
            //�f�t�H���g�l�ł���ΐ������l��^����
            m_BGMData.Source.volume = float.IsNaN(volume) ? m_defaltVolume : volume;
            m_BGMData.Source.clip = clip;
            m_BGMData.Source.loop = true;
            m_BGMData.Source.Play();
            Log(clip, true, true, m_BGMData.ID);
        }
        else {
            Debug.Assert(clip != null, "�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
            return;
        }
        
       
       
    }

    /// <summary>BGM���t�F�[�h�C���Đ������܂��B����BGM���Đ�����Ă���ꍇ�A����BGM�͒�~����܂��B
    /// <br/>�܂��ABGM�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�炵���������t�@�C����</param>
    /// <param name="fadeTime">�t�F�[�h�C��(endVolume)�ɂȂ�܂ł̎���(�b)�@���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="startVolume">���߂̉��̑傫��(�O�`�P)</param>
    /// <param name="endVolume">�I���̉��̑傫��(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐� <br/>���t�F�[�h�r���ŉ�������~���ꂽ�ꍇ��t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���</param>
    public void BGM_PlayFade(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, UnityAction action = null) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }
        m_BGMData.Source.volume = startVolume;
        BGM_Play(clipName, startVolume);
        StartCoroutine(FadeSound(m_BGMData, fadeTime, endVolume, action, false,false));
    }

//SE
    /// <summary>SE���Đ����܂��B</summary>
    /// <param name="clipName">�炵����������(BGM�������Đ��\�ł�)</param>
    /// <param name="volume">����(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    /// <param name="IsLoop">���[�v�Đ�������ꍇ��ture</param>
    /// <param name="action">�Đ����ꂽ�������Đ��I���܂��͒�~���ꂽ�ꍇ�Ɋ֐������s���܂��B
    /// <br/>���܂��A���[�v���L���ȏꍇ�̓��[�v�Đ�����~�����܂Ŋ֐��͌Ă΂�܂���B</param>
    /// <returns>�Đ����Ɋ���U��ꂽ���ʗp��ID�̂悤�Ȃ��̂��擾���܂��B�Đ����o���Ȃ������ꍇ��int.MinValue��Ԃ��܂��B</returns>
    public int SE_Play(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //���݉���点���Ԃ���ɒ��ׂ�
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return int.MinValue;
        }
        if (!GetNotUsedSource(out SoundData sd)) {
            return int.MinValue;
        }
        if (GetClip(out var clip, clipName, false)) {
            //���ʂȂǂ̐ݒ�
            //�f�t�H���g�l�ł���ΐ������l��^����
            sd.Source.volume = float.IsNaN(volume) ? m_defaltVolume : volume;
            sd.Source.loop = IsLoop;
            sd.Source.clip = clip;
            sd.Source.Play();
            Log(clip, true, false,sd.ID);
            if (action != null) {
                StartCoroutine(CheckPlaying(sd.Source, action));
            }
            return sd.ID;
        }
        else {
            return int.MinValue;
        }     
    }

    /// <summary>BGM���t�F�[�h�C���Đ������܂��B����BGM���Đ�����Ă���ꍇ�A����BGM�͒�~����܂��B
    /// <br/>�܂��ABGM�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�炵���������t�@�C����</param>
    /// <param name="fadeTime">�t�F�[�h�C��(endVolume)�ɂȂ�܂ł̎���(�b)�@���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="startVolume">���߂̉��̑傫��(�O�`�P)</param>
    /// <param name="endVolume">�I���̉��̑傫��(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundVolume���g�p����܂�</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐� <br/>���t�F�[�h�r���ŉ�������~���ꂽ�ꍇ��t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���</param>
    /// <returns>�Đ����Ɋ���U��ꂽ���ʗp��ID�̂悤�Ȃ��̂��擾���܂��B�Đ����o���Ȃ������ꍇ��int.MinValue��Ԃ��܂�</returns>
    public int SE_PlayFade(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, bool IsLoop = false,UnityAction action = null) {
        //���݉���点���Ԃ���ɒ��ׂ�
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return int.MinValue;
        }
        if (!GetNotUsedSource(out SoundData sd)) {
            return int.MinValue;
        }
        if (GetClip(out var clip, clipName, false)) {
            //���ʂȂǂ̐ݒ�
            sd.Source.volume = startVolume;
            sd.Source.loop = IsLoop;
            sd.Source.clip = clip;
            sd.Source.Play();
            Log(clip, true, false,sd.ID);
            StartCoroutine(FadeSound(sd, fadeTime, endVolume, action, false, false));
            return sd.ID;
        }
        else {
            return int.MinValue;
        }
    }

    //  ��~�֐�    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

//BGM
    /// <summary>BGM���~�����܂�</summary>
    public void BGM_Stop() {
        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData.Source.clip, false, true,m_BGMData.ID);
        }
        else if (m_BGMData.Source.time != 0) {
            Log("�|�[�Y����BGM����~����܂��B",true);
        }
        else {
            Log("BGM�͍Đ�����Ă��܂���ł���", true);
        }
        m_BGMData.Source.Stop();
    }

    /// <summary>BGM���t�F�[�h�A�E�g������~���܂��B <br/>BGM�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="fadeTime">�t�F�[�h�A�E�g�܂ł̎���(�b) ���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h�A�E�g��ɍs�����s�֐�(�s���K�v���Ȃ��ꍇ��null��n���Ă�������)</param>
    /// <param name="endVolume">��~���钼�O�̉���(�O�`�P)</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�
    /// <br/>���t�F�[�h���㏑�����ꂽ��A�t�F�[�h�r����BGM����~���ꂽ�ꍇ�ɂ͌Ă΂�܂���B</param>
    public void BGM_StopFade(float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null) {

        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData.Source.clip, false,true,m_BGMData.ID);
            StartCoroutine(FadeSound(m_BGMData, fadeTime, endVolume, action, true,false));
        }
        else {
            Log("BGM�͍Đ�����Ă��܂���ł���", true);
            return;
        }

    }


//SE
    /// <summary>�w�肳�ꂽ�T�E���h���~�����܂��B</summary>
    /// <param name="clipName">�T�E���h���̎w��</param>
    /// <returns>�Ăяo�����Ɏw�肳�ꂽSE���Đ����ł����true��Ԃ��܂�</returns>
    public bool SE_Stop(string clipName) {
        bool check = GetUsingSource(out var sds);
        if (check) {
            foreach (var sd in sds) {
                if (sd.Source.clip.name == clipName) {
                    Log(sd.Source.clip, false, false,sd.ID);
                    sd.Source.Stop();
                }
            }
        }
        if (!check) {
            Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@������ : " + clipName, true);
        }
        return check;
    }
    /// <summary>�w�肳�ꂽ�T�E���h���~�����܂�</summary>
    /// <param name="id">�T�E���hID�w��</param>
    /// <returns>�Ăяo�����Ɏw�肳�ꂽID��SE���Đ����ł����true��Ԃ��܂�</returns>
    public bool SE_Stop(int id) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.ID == id) {
                    Log(sd.Source.clip, false, false,sd.ID);
                    sd.Source.Stop();
                    return true;
                }
            }
        }
        Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@ID : " + id, true);
        return false;
    }

    /// <summary>SE���t�F�[�h�A�E�g������~���܂��B�w�肳�ꂽSE���Ȃ������ꍇ�ɂ͏����͎��s����܂���B
    /// <br/>�w�肳�ꂽSE�����������Đ�����Ă����ꍇ�͑S�Ē�~����܂��B
    /// <br/>SE�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�T�E���h���̎w��</param>
    /// <param name="fadeTime">�t�F�[�h�A�E�g�܂ł̎���(�b) ���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h�A�E�g��ɍs�����s�֐�(�s���K�v���Ȃ��ꍇ��null��n���Ă�������)</param>
    /// <param name="endVolume">��~���钼�O�̉���(�O�`�P)</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�<br/>���t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ăяo����܂���B</param>
    /// <param name="StopAction">�t�F�[�h�����O�ɍĐ�����~�����ꍇ��action�����s���邩�ǂ���</param>
    public bool SE_StopFade(string clipName,float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null,bool StopAction = false) {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.Source.clip.name == clipName) {
                    Log(sd.Source.clip, false, false, sd.ID);
                    StartCoroutine(FadeSound(sd, fadeTime, endVolume, action, true, StopAction));
                    check = true;
                }
            }
        }
        if (!check) {
            Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@������ : "+clipName, true);
        }
        return check;
    }
    /// <summary>SE���t�F�[�h�A�E�g������~���܂��B�w�肳�ꂽSE���Ȃ������ꍇ�ɂ͏����͎��s����܂���B
    /// <br/>SE�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="id">�T�E���hID�w��</param>
    /// <param name="fadeTime">�t�F�[�h�A�E�g�܂ł̎���(�b) ���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h�A�E�g��ɍs�����s�֐�(�s���K�v���Ȃ��ꍇ��null��n���Ă�������)</param>
    /// <param name="endVolume">��~���钼�O�̉���(�O�`�P)</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�<br/>���t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ăяo����܂���B</param>
    /// <param name="StopAction">�t�F�[�h�����O�ɍĐ�����~�����ꍇ��action�����s���邩�ǂ���</param>
    public bool SE_StopFade(int id, float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null, bool StopAction = false) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.ID == id) {
                    Log(sd.Source.clip, false, false, sd.ID);
                    StartCoroutine(FadeSound(sd, fadeTime, endVolume, action, true, StopAction));
                    return true;
                }
            }
        }
        Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@ID : " + id, true);
        return false;
    }

    /// <summary>���[�v�Đ�����SE�����ׂĒ�~���܂�</summary>
    /// <returns>���[�v�Đ�����SE���������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_StopInLoop() {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds ) {
                if (sd.Source.isPlaying && sd.Source.loop) {
                    check = true;
                    Log(sd.Source.clip,false,false,sd.ID);
                    sd.Source.Stop();
                }
            }
            if (!check) {
                Log("���[�v�Đ�����Ă���SE�͂���܂���ł����B", true);
            }
        }
        return check;
    }

    /// <summary>���[�v���ł͂Ȃ��Đ�����SE��S�Ē�~�����܂��B </summary>
    /// <returns>���[�v�ł͂Ȃ��Đ�����SE���������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_StopNotLoop()
    {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.Source.isPlaying && !sd.Source.loop) {
                    check = true;
                    Log(sd.Source.clip, false,false,sd.ID);
                    sd.Source.Stop();
                }
            }
            if (!check) {
                Log("���[�v�Đ��ȊO�̍Đ�����Ă���SE�͂���܂���ł����B", true);
            }
        }
        return check;
    }


//ALL
    /// <summary>�Đ�����Ă���T�E���h��S�Ē�~�����܂��B</summary>
    /// <returns>�Ăяo�����_�ōĐ�����Ă���T�E���h���������ꍇ��true��Ԃ��܂��B
    /// <br/>IsIncludingBGM��true�̏ꍇ��BGM�݂̂��Đ�����Ă����ꍇ�ł�true��Ԃ��܂�</returns>
    public void ALL_Stop(bool IsIncludingBGM) {
        if (IsIncludingBGM) {
            BGM_Stop();
        }
        if (GetUsingSource(out var sds)) {
            foreach (var ad in sds) {
                ad.Source.Stop();
            }
        }
    }

    /// <summary>�Đ����̑S�Ẳ������t�F�[�h��~�����܂��B</summary>
    /// <param name="IncludingBGM">BGM���Ώۂɂ��邩</param>
    /// <param name="fadeTime">�t�F�[�h����</param>
    /// <param name="action">�t�F�[�h��̊֐��Ăяo��</param>
    /// <param name="StopAction">IncludingBGM��false�̍ۂɑS�Ă�SE���t�F�[�h�O�ɍĐ��I�������ꍇ��action���N�����邩�ǂ���</param>
    /// <returns>�t�F�[�h�I�����鉹�����������ꍇ�ɂ�true��Ԃ��܂��B</returns>
    public bool ALL_StopFade(bool IncludingBGM, float fadeTime = float.NaN,float endVolume = 0.05f, UnityAction action = null, bool StopAction = false) {
        bool check = false;
        if (IncludingBGM && m_BGMData.Source.isPlaying) {
            BGM_StopFade(fadeTime,endVolume,action);
            check = true;
        }
        if (GetUsingSource(out List<SoundData> sds)) {
            check = true;
            if (IncludingBGM && m_BGMData.Source.isPlaying) {
                foreach (var sd in sds) {
                    StartCoroutine(FadeSound(sd, fadeTime, endVolume, null, true, false));
                }
            }
            else {
                SoundData restSd = null;
                float resttime = 0;
                //�c��b������Ԓ���SE��T��
                foreach (var sd in sds) {
                    float rt = sd.Source.clip.length - sd.Source.time;
                    if (rt > resttime) {
                        if (restSd != null) { StartCoroutine(FadeSound(restSd, fadeTime, endVolume, null, true, false)); }
                        restSd = sd;
                        resttime = rt;
                    }
                    else {
                        StartCoroutine(FadeSound(sd, fadeTime, endVolume, null, true, false));
                    }
                }
                //�c��b������Ԓ���SE��action��n��
                StartCoroutine(FadeSound(restSd, fadeTime,endVolume, action, true, StopAction));
            }
        }
        return check;
    }

   
    //  �ꎞ��~�E�ĊJ   //---------------------------------------------------------------------------------------------------------------------------------------------------------------

//BGM
    /// <summary>BGM���|�[�Y���܂�</summary>
    public void BGM_Pause() {
        if (m_BGMData.Source.isPlaying) {
            m_BGMData.Source.Pause();
            Log("BGM���|�[�Y����܂�", false);
        }
        else {
            Log("BGM�͍Đ�����Ă��܂���", true);
        }

    }

    /// <summary>�|�[�Y����BGM���ĊJ���܂�</summary>
    public void BGM_Restert() {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }

        if (!m_BGMData.Source.isPlaying && m_BGMData.Source.time != 0) {
            m_BGMData.Source.UnPause();
            Log("BGM�̃|�[�Y��Ԃ���������܂�", false);
        }
        else {
            Log("BGM�̓|�[�Y����Ă��܂���B",true);
        }

    }

//SE
    /// <summary>�w�肳�ꂽSE���|�[�Y���܂��B����������SE����������ꍇ�ɂ͑S�ă|�[�Y����܂��B</summary>
    /// <param name="clipName">�������̎w��</param>
    /// <returns>�w�肳�ꂽ�������Đ�����Ă����ꍇ��true��Ԃ��܂�</returns>
    public bool SE_Pause(string clipName) {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if(sd.Source.clip.name == clipName) {
                    check = true;
                    Log("\n" + clipName + "���|�[�Y����܂��B" + "\nID : " + sd.ID, false);
                    sd.Source.Pause();
                }
            }
        }
        if(!check) Log("�w�肳�ꂽ��������SE�͍Đ�����Ă��܂���ł����B\n������ : " + clipName,true);
        return check;
    }
    /// <summary>�w�肳�ꂽSE���|�[�Y���܂��B</summary>
    /// <param name="clipName">����ID�w��</param>
    /// <returns>�w�肳�ꂽ�������Đ�����Ă����ꍇ��true��Ԃ��܂�</returns>
    public bool SE_Pause(int id) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.ID == id) {
                    Log("\n" + sd.Source.clip.name + "���|�[�Y����܂��B" + "\nID : " + sd.ID, false);
                    sd.Source.Pause();
                    return true;
                }
            }
        }
        Log("�w�肳�ꂽID��SE�͍Đ�����Ă��܂���ł����B\nID : " + id, true);
        return false;
    }

    /// <summary>�w�肳�ꂽ��������SE���|�[�Y�������܂��B</summary>
    /// <param name="clipName">�������̎w��</param>
    /// <returns>�|�[�Y���������������������true��Ԃ��܂��B</returns>
    public bool SE_Restert(string clipName) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return false;
        }
        bool check = false;
        if (GetPausingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                if (sd.Source.clip.name == clipName) {
                    check = true;
                    Log("\n" + clipName + "�̃|�[�Y����������܂��B" + "\nID : " + sd.ID, false);
                    sd.Source.UnPause();
                }
            }
        }
        if (!check)Log("�w�肳�ꂽ��������SE�̓|�[�Y����Ă��܂���ł����B\n������ : " + clipName, true);
        return check;
    }
    /// <summary>�w�肳�ꂽ��������SE���|�[�Y�������܂��B</summary>
    /// <param name="id">����ID�w��</param>
    /// <returns>�|�[�Y���������������������true��Ԃ��܂��B</returns>
    public bool SE_Restert(int id) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return false;
        }
        if (GetPausingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                if (sd.ID == id) {
                    Log("\n" + sd.Source.clip.name + "�̃|�[�Y����������܂��B" + "\nID : " + sd.ID, false);
                    sd.Source.UnPause();
                    return true;
                }
            }
        }
        Log("�w�肳�ꂽID��SE�̓|�[�Y����Ă��܂���ł����B    ID : " + id, true);
        return false;
    }


//ALL
    /// <summary>�Đ�����Ă���T�E���h��S�ă|�[�Y���܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void ALL_Pause(bool IsIncludingBGM) {
        if (IsIncludingBGM) {
            BGM_Pause();
        }

        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                sd.Source.Pause();
                Log(sd.Source.clip.name + "(SE)���|�[�Y����܂�", false);
            }
        }
    }

    /// <summary>�S�Ẵ|�[�Y����Ă��鉹�����ĊJ�����܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void ALL_Restert(bool IsIncludingBGM) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }

        if (IsIncludingBGM) {
            BGM_Restert();
        }

        if (GetPausingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                    Log(sd.Source.clip, true, false, sd.ID);
                    sd.Source.UnPause();
                    Log(sd.Source.clip.name + "(SE)���|�[�Y��������܂�", false);               
            }
        }
    }


    //  ���̑�   //---------------------------------------------------------------------------------------------------------------

    /// <summary>�w�肳�ꂽ�������擾���܂�</summary>
    /// <returns>��������Ԃ��܂��B������Ȃ������ꍇ��null��Ԃ��܂��B</returns>
    public AudioClip GetAudioClip(string clipName) { GetClip(out var clip, clipName, true); return clip;}

    //���ݍĐ�����Ă��鉹�������擾���܂��BTODO


    /// <summary>(�f�o�b�O�p)���Đ�����Ă��鉹���Ƃ���ID�����ׂăR���\�[����ɕ\�����܂�</summary>    
    [Conditional("UNITY_EDITOR")]
    public void IsPlayingSoundLog(){
        if (m_BGMData.Source.isPlaying){
            Debug.Log("�Đ�����Ă���BGM : " + m_BGMData.Source.clip.name);
        }

         if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                Debug.Log("�Đ�����Ă���T�E���h : " + sd.Source.clip.name +"�g�p���Ă���ID : "+sd.ID);
            }
         }       
      }
      
    




    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------

    //�I�[�f�B�I�\�[�X���Ǘ�����N���X
    private class SoundData {
        public AudioSource Source;
        public int ID;
        public FadeState fadeState;
    }
    //�g�p����I�[�f�B�I�\�[�X
    private SoundData[] m_SEDatas;
    private SoundData m_BGMData;
    //�������X�g
    private static Dictionary<string, AudioClip> ClipList_BGM;     //BGM�̃��X�g
    private static Dictionary<string, AudioClip> ClipList_SE;      //SE�̃��X�g
    //���̑�
    private float m_defaltVolume;                                  //�f�t�H���g��volume
    private float m_defaltTime;                                    //�f�t�H���g�̃t�F�[�h����
    private int m_IDN = 1;                                         //ID�z�z�p�̔ԍ�
    private bool m_canPlayFlag = true;                             //�Đ��\�t���O

    [SerializeField, TooltipAttribute("�Đ�����鉹���Ȃǂ����O�ɕ\�����邩�ǂ���(�G���[���b�Z�[�W�͏���)")] private bool PlaySoundLog = true;
    //�t�F�[�h��Ԃ��Ǘ�����񋓌^
    private enum FadeState {
        None,
        Fading,
        Pause,
    }
    //  �v���C�x�[�g�֐�  //------------------------------------------------------------------------------------------------------------------------------

    //  �������֐�   //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        //�I�[�f�B�I�\�[�X�̍쐬----------------------------------------------------
        if (ClipList_BGM == null && ClipList_SE == null) {
            m_BGMData = new SoundData();
            m_BGMData.Source = gameObject.AddComponent<AudioSource>();
            m_BGMData.ID = 0;
            m_BGMData.fadeState = FadeState.None;
            m_SEDatas = new SoundData[c_MaxSoundOverlap - 1];
            for (int i = 0; i < m_SEDatas.Length; ++i) {
                m_SEDatas[i] = new SoundData();
                m_SEDatas[i].Source = gameObject.AddComponent<AudioSource>();
                m_SEDatas[i].ID = int.MinValue;
                m_SEDatas[i].fadeState = FadeState.None;
            }
            //�t�@�C���̓ǂݍ���----------------------------------------------------
            ClipList_BGM = new Dictionary<string, AudioClip>();
            ClipList_SE = new Dictionary<string, AudioClip>();
            SetAudioClip(ClipList_BGM,c_FolderPath_BGM);
            SetAudioClip(ClipList_SE,c_FolderPath_SE);
            //�f�t�H���g�l�̊i�[----------------------------------------------------
            m_defaltVolume = c_DfaultSoundVolume;
            m_defaltTime = c_DefaultFadeTime;
        }
        //--------------------------------------------------------------------------
    }

    //  ���s�֐�    //-----------------------------------------------------------------------------

    /// <summary>�g���Ă��Ȃ��I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetNotUsedSource(out SoundData s)
    {
      for (int i = 0; i < m_SEDatas.Length; ++i) {
          if (!m_SEDatas[i].Source.isPlaying && m_SEDatas[i].Source.time == 0) {
                m_SEDatas[i].ID = GetID();
                s = m_SEDatas[i];
                return true;
          }
      }
        Log("��x�ɖ点�鉹�̍ő吔�𒴂��Ă��܂�",true);
        s = null;
        return false;
    }

    /// <summary>�|�[�Y���̃I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetPausingSource(out List<SoundData> s) {
        bool check = false;
        s = new List<SoundData>();
        for (int i = 0; i < m_SEDatas.Length; ++i) {
            if (!m_SEDatas[i].Source.isPlaying && m_SEDatas[i].Source.time != 0) {
                s.Add(m_SEDatas[i]);
                check = true;
            }
        }
        return check;
    }

    /// <summary>�g���Ă���I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetUsingSource(out List<SoundData> s,string noneLog = null) {
        s = new List<SoundData> ();
        bool check = false;
        for (int i = 0; i < m_SEDatas.Length; ++i) {
            if (m_SEDatas[i].Source.isPlaying) {
                s.Add (m_SEDatas[i]);
                check = true;
            }
        }
        GameSystem.Action(() => {
            if (!check) {
               noneLog = noneLog ?? "�Đ�����Ă���SE�͂���܂���ł����B";
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
    private IEnumerator FadeSound(SoundData s,float time,float ev, UnityAction action, bool stop,bool stopToAction) {

        //�f�t�H���g�l�ł���ΐ������l�ɕύX����
        time = float.IsNaN(time) ? m_defaltTime : time;
        ev = float.IsNaN(ev) ? m_defaltVolume : ev;

        //�t�F�[�h���d�����Ă���ꍇ�͏㏑������
        if (s.fadeState != FadeState.None) {
            s.fadeState = FadeState.None;
            yield return null;
            s.fadeState = s.fadeState == FadeState.None ? FadeState.Fading : FadeState.Pause;
        }
        else {
            s.fadeState = FadeState.Fading;
        }
        float sv = s.Source.volume;
        float t = 0;
        yield return null;
        while (true) {
            //�|�[�Yor�Đ���~�ɂȂ��Ă��Ȃ����m�F����
            if (!s.Source.isPlaying) {
                if(s.Source.time == 0) {
                    Log("�t�F�[�h���I���O�ɂɍĐ�����~����܂����B", true);
                    //�����I�����Ɋ֐����N�����Ȃ��ݒ�̏ꍇ��null�ɂ���
                    action = stopToAction ?  action : null;
                    break;
                }
            }
            //�t�F�[�h���㏑������ĂȂ����`�F�b�N����
            else if (s.fadeState == FadeState.None) {
                Log("�t�F�[�h���ɐV�����t�F�[�h�������Ă΂�܂����B�t�F�[�h�������㏑������܂��B", true);
                break;
            }
            //�S�Ă̏����𖞂������ꍇ�̂݃t�F�[�h��i�߂�
            else if(s.fadeState == FadeState.Fading){
                s.Source.volume = Mathf.SmoothStep(sv, ev, t);
                t += Time.deltaTime / time;
            }         
            //���[�v�𔲂������
            if (t >= 1) {
                if (s.Source.isPlaying) {
                    Log(s.Source.clip.name + "�̃t�F�[�h���������܂���", false);
                    s.Source.volume = ev;
                    if (stop) {
                        s.Source.Stop();
                    }
                }
                break;
            }
          
            //���Ȃ���Ύ��̃��[�v�܂�1�t���[���ҋ@����
            yield return null;
        }

        //�t�F�[�h���㏑������ĂȂ��ꍇ�̂݌Ă΂��
        if (s.fadeState != FadeState.None) {
            s.fadeState = s.fadeState == FadeState.Fading ? FadeState.None : s.fadeState;
            action?.Invoke();
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

    /// <summary>�T�E���h�����ʂ���ID�̂悤�Ȃ��̂��擾���܂�</summary>
    private int GetID(){
        int id = m_IDN == int.MaxValue ? 1 : m_IDN;
        m_IDN = id + 1;
        return id;
    }

    /// <summary>�w�肳�ꂽ�T�E���h�����邩���ׁA���������ꍇ�͎擾���܂�/summary>
    private bool GetClip(out AudioClip clip,string clipName,bool IsPrioritizeBGM) {
        //�������擾����
        clip = null;
        bool check = IsPrioritizeBGM? ClipList_BGM.TryGetValue(clipName, out clip):ClipList_SE.TryGetValue(clipName, out clip);
        //�������擾�ł��Ȃ������ꍇ��BGM��������擾����
        if (!check) {
            check = !IsPrioritizeBGM ? ClipList_BGM.TryGetValue(clipName, out clip) : ClipList_SE.TryGetValue(clipName, out clip);
            //BGM����������擾�ł��Ȃ������ꍇ��null��Ԃ�
            if (!check) {
                GameSystem.LogError("�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
            }
#if UNITY_EDITOR
            string str = IsPrioritizeBGM ? "SE������BGM�Ƃ��Ďg�p����܂��B" : "BGM������SE�Ƃ��Ďg�p����܂��B";
            Log(str, true);
#endif
        }
        return check;
    }

    //  �f�o�b�O�p   //----------------------------------------------------------------------------

    /// <summary>���O�̕\�����L���ȏꍇ�ɉ����������O�ɕ\������</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(AudioClip clip,bool playing,bool BGM,int ID) { 
        if (PlaySoundLog) {
            string type = BGM ? "(BGM)" : "(SE)";
            string id = ID != 0 ? "<color=cyan>ID : " + ID + "</color>" : "";
            string str = playing ? "\n<color=cyan>" + clip.name + type + "</color>" + "���Đ�����܂�    " + id : "color=cyan>" + clip.name + type + "</color>" + "����~����܂�" + id;
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






