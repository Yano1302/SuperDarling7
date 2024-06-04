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
    #region Config�ϐ�
    [SerializeField, Header("BGM���i�[����Ă���Resources�t�H���_�̃p�X(Asset/�`)")]
    private string c_FolderPath_BGM = "Audio\\BGM\\Resources"; //BGM���i�[����Ă���t�H���_�̃p�X


    [SerializeField, Header("SE���i�[����Ă���Resources�t�H���_�̃p�X(Asset/�`)")]
    private string c_FolderPath_SE = "Audio\\SE\\Resources";   //SE���i�[����Ă���t�H���_�̃p�X


    [SerializeField,Header("�I�[�f�B�I�̍ő��(BGM����)")]
    private int m_maxSoundOverlap = 10;                    �@�@         //�I�[�f�B�I�̍ő��(BGM����)
    [SerializeField, Header("���̑傫�������i�K�ɕ����邩(BGM)")]
    private int m_divisionScaleNum_BGM = 10;                            //���̑傫�������i�K�ɕ����邩(BGM)
    [SerializeField, Header("���̑傫�������i�K�ɕ����邩(SE)")]
    private int m_divisionScaleNum_SE = 10;                             //���̑傫�������i�K�ɕ����邩(SE)

    [Space(10),Header("----�f�t�H���g�l-------------------------------------------------------------------------------------------------------------------")]
    [SerializeField, Header("�f�t�H���g�l�Ŏg�p����鉹��(0�`1)")]
    private float m_standardVolume = 1.0f;                           //��̉���
    [SerializeField, Header("�f�t�H���g�l�Ŏg�p�����t�F�[�h����(�b)")]
    private float m_standardFadeTime = 1.0f;                        //�f�t�H���g�̃t�F�[�h����

    [Space(10), Header("----�����l------------------------------------------------------------------------------------------------------------------------")]
    [SerializeField, Header("�T�E���h�̒i�K�̏����ݒ�(0�`divisionScaleNum_BGM)")]
    private int m_divisionScale_BGM = 5;                               //�T�E���h�̃X�P�[��(BGM)
    [SerializeField, Header("�T�E���h�̒i�K�̏����ݒ�(0�`divisionScaleNum_SE)")]
    private int m_divisionScale_SE = 5;                                //�T�E���h�̃X�P�[��(SE)
    [SerializeField,Header("�Đ��\�t���O")]
    private bool m_canPlayFlag = true;                              //�Đ��\�t���O
    [SerializeField, Header("�~���[�g���ǂ����̃t���O")]
    private bool m_mute = false;                                    //�~���[�g���ǂ���
    [SerializeField, Header("SE����̏ꍇ�㏑�����邩�ǂ���(���[�v�Đ�����SE�͏���)")]
    private bool m_overwriteSE = true;                              //SE���㏑�����邩

    [Space(10), Header("----�f�o�b�O�p----------------------------------------------------------------------------------------------------------------------")]
    [SerializeField, Header("�Đ�����鉹���Ȃǂ����O�ɕ\�����邩�ǂ���(�G���[���b�Z�[�W�͏���)")]
    private bool PlaySoundLog = true;
    #endregion
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


//  �ϐ��E�֐��ꗗ    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //  �ݒ�Ȃ�    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>���y���Đ��\���ǂ����̃t���O�ł��B���������Afalse�ɂ������Ɋ��ɍĐ�����Ă��鉹�y�͒�~����܂���</summary>
    public bool CanPlayFlag { get { return m_canPlayFlag; } set { Log("�Đ��\�t���O�� " + value + " �ɕύX����܂�",false); m_canPlayFlag = value; } }

    /// <summary>�t�F�[�h�ɂ����鎞�Ԃ̃f�t�H���g�l���擾�E�ύX���܂�</summary>
    public float DefaultFadeTime { get { return m_standardFadeTime; } set { m_standardFadeTime = value; } }


    public int DivisionScale_BGM {
        get { return m_divisionScale_BGM; }
        set {
            m_divisionScale_BGM = Mathf.Clamp(value, 0, m_divisionScaleNum_BGM);
            SetVolume(m_BGMData, m_BGMData.SetVolume);
            Log("BGM�̃E���h�̏o�͊����� " + m_divisionScale_BGM + " �ɕύX����܂��B", false);
        }
    }


    /// <summary>�T�E���h�̏o�͊�����ݒ�E�擾���܂��B(0 �` DivisionScaleNum)</summary>
    public int DivisionScale_SE { 
        get { return m_divisionScale_SE; }
        set {
            m_divisionScale_SE = Mathf.Clamp(value, 0, m_divisionScaleNum_SE);
            foreach (var sd in m_SEDatas) {
               SetVolume(sd,sd.SetVolume);
            }        
            Log("SE�̃T�E���h�̏o�͊����� " + m_divisionScale_SE + " �ɕύX����܂��B", false);
        }
    }

    /// <summary>�S�Ẳ����̃~���[�g�ݒ���擾�E�ύX���܂��B</summary>
    public bool Mute {
        get { return m_mute; }
        set {
            if (m_mute != value) {
                m_BGMData.Source.mute = value;
                foreach (var sd in m_SEDatas) {
                    sd.Source.mute = value;
                }
                m_mute = value;
                Log("�~���[�g�ݒ肪 " + value + " �ɕύX����܂��B", false);
            }
            else {
                Log("�~���[�g�ݒ�͊��� " + value + " �ł�", true);
            }

        }
    }

    /// <summary>SE������ɒB�����ۂɏ㏑�����邩�ǂ���(���[�v�Đ���SE�͏㏑������܂���)</summary>
    public bool OverWriteSE { get { return OverWriteSE; } set { OverWriteSE = value; } }


//  ���ʐݒ�    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
//BGM
    /// <summary>���݂�BGM�̉��ʂ�ݒ�E�擾���܂��B</summary>
    public float BGM_Volume {
        get {
            if (!m_BGMData.Source.isPlaying) { Log("BGM�͍Đ�����Ă��܂���B", true); } 
            return m_BGMData.Source.volume;
        }
        set {
            Log("BGM�͍Đ�����Ă��܂���B�Đ����ɉ��ʐݒ肪�㏑������鋰�ꂪ����܂��B", true, m_BGMData.Source.isPlaying);
            Log("BGM�̉��ʂ�" + value + "�ɕύX����܂��B", false);
            SetVolume(m_BGMData,value);
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
    public bool SE_SetVolume(string clipName,float volume) {
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.isPlaying && clipName == sd.Source.clip.name) {
                SetVolume(sd, volume);
                Log(sd.Source.clip + "�̉��ʂ�" + volume + "�ɕύX����܂��B", false);            
            }
        };
        bool check = GetUsingSource(action);
        Log("�w�肳�ꂽ��������SE�͍Đ�����Ă��܂���ł����B\n�w�肳�ꂽ������ : " + clipName, true,check);
        return check;
    }
    /// <summary>�Đ�����Ă���SE�̉��ʂ�ύX���܂��B(ID�w��)</summary>
    /// <param name="id">�ύX������SE��ID</param>
    /// <param name="volume">�ύX��̉���</param>
    /// <returns>�w�肳�ꂽID��SE���������ꍇ��true��Ԃ��܂��B</returns>
     public bool SE_SetVolume(int id, float volume) {
        UnityAction<SoundData> action = (SoundData sd) =>
         {
             if (sd.Source.isPlaying) {
                 SetVolume(sd, volume);
                 Log(sd.Source.clip + "�̉��ʂ�" + volume + "�ɕύX����܂��B", false);
             }
         };
        bool check = GetUsingSource(action);
        Log("�w�肳�ꂽID��SE�͍Đ�����Ă��܂���ł����B\nID : " + id, true,check);
        return false;
    }

    /// <summary>�Đ�����Ă���SE�̉��ʂ����X�ɕύX���܂��B(���̎w��)�w�肳�ꂽ�����Ɠ���������SE����������ꍇ�͑S�ĕύX����܂��B
    /// <br/>�w�肳�ꂽSE���Đ�����Ă��Ȃ������ꍇ�͏��������s����܂���B
    /// <br/>�܂��A�w�肳�ꂽ�T�E���h�����Ƀt�F�[�h�������ł���΃t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�����w��(�w�肳�ꂽ��������������ꍇ�͑S�ĕύX����܂�)</param>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="fadeTime">�t�F�[�h����</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�(�K�v�ȏꍇ�̂�) 
    /// <br/>���t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���B
    ///  <br/>�܂��A�������������ꍇ�͌��������񐔕��Ă΂�܂��B
    /// </param>
    /// <param name="StopAction">�t�F�[�h�����O�ɍĐ�����~�����ꍇ��action�����s���邩�ǂ���</param>
    /// <returns>�Đ�����Ă��鉹�������������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_SetVolumeFade(string clipName,float volume,float fadeTime = float.NaN,UnityAction action = null,bool StopAction = false) {
        UnityAction<SoundData> action1 = (SoundData sd) =>
        {
            if (clipName == sd.Source.clip.name) {
                StartCoroutine(FadeSound(sd, fadeTime, volume, action, false, StopAction));
            }
        };    
        bool check = GetUsingSource(action1);
        Log("�w�肳�ꂽSE���݂���܂���ł����B" + "\n������ : " + clipName + "\n", true,check);      
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
        UnityAction<SoundData> action1 = (SoundData sd) =>
        {
            if (id == sd.ID) {
                StartCoroutine(FadeSound(sd, fadeTime, volume, action, false, StopAction));
            }
        };
        bool check = GetUsingSource(action1);
        Log("�w�肳�ꂽSE���݂���܂���ł����B" + "\nID : " + id + "\n", true,check);
        return check;
    }

//ALL
    /// <summary>�Đ����̑S�ẴT�E���h�̉��ʂ�ύX���܂��B</summary>
    /// <param name="volume">�ύX��̉���</param>
    /// <param name="IncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void ALL_SetVolume(float volume, bool IncludingBGM) {
        if (IncludingBGM) {
            BGM_Volume = volume;
        }
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.isPlaying) {
                SetVolume(sd, volume);
                Log(sd.Source.volume + "(SE)�̉��ʂ��ύX����܂��B \n �ύX��̉��� : " + volume, false);
            }
        };
        bool check = GetUsingSource(action);
        Log("SE�͍Đ�����Ă��܂���ł����B", true, check);
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
        }
        if (check) {
            GetUsingSource((SoundData sd) => { StartCoroutine(FadeSound(sd, fadeTime, volume, null, false, false)); });
        }
        else {
            SoundData restSd = null;
            float resttime = 0;
            //�c��b������Ԓ���SE��T��
            foreach (var sd in m_SEDatas) {
                if (sd.Source.isPlaying) {
                    check = true;
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
            }
            //�c��b������Ԓ���SE��action��n��
            StartCoroutine(FadeSound(restSd, fadeTime, volume, action, false, StopAction));
        }
        return check;
    }


//  �Đ����x�ݒ�    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

//BGM
    



//  �Đ��֐�    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //BGM
    /// <summary>BGM���Đ����܂��B����BGM���Đ�����Ă����ꍇ�͂���BGM�͒�~����܂��B</summary>
    /// <param name="clipName">�炵����������(SE�������Đ��\�ł�)</param>
    /// <param name="volume">����(�O�`�P)�@���w�肵�Ȃ��ꍇ��SoundScale���g�p����܂�</param>
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
            SetVolume(m_BGMData, volume);
            m_BGMData.fadeState = FadeState.None;
            m_BGMData.Source.clip = clip;
            m_BGMData.Source.loop = true;
            m_BGMData.Source.Play();
            Log(m_BGMData,true);
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
    /// <param name="endVolume">�I���̉��̑傫��(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundS���g�p����܂�</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐� <br/>���t�F�[�h�r���ŉ�������~���ꂽ�ꍇ��t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���</param>
    public void BGM_PlayFade(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, UnityAction action = null) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }
        SetVolume(m_BGMData, startVolume);
        BGM_Play(clipName, startVolume);
        StartCoroutine(FadeSound(m_BGMData, fadeTime, endVolume, action, false,false));
    }

//SE
    /// <summary>SE���Đ����܂��B</summary>
    /// <param name="clipName">�炵����������(BGM�������Đ��\�ł�)</param>
    /// <param name="volume">����(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundScale���g�p����܂�</param>
    /// <param name="IsLoop">���[�v�Đ�������ꍇ��ture</param>
    /// <param name="action">�Đ����ꂽ�������Đ��I���܂��͒�~���ꂽ�ꍇ�Ɋ֐������s���܂��B
    /// <br/>���܂��A���[�v���L���ȏꍇ�̓��[�v�Đ�����~�����܂Ŋ֐��͌Ă΂�܂���B</param>
    /// <returns>�Đ����Ɋ���U��ꂽ���ʗp��ID�̂悤�Ȃ��̂��擾���܂��B�Đ����o���Ȃ������ꍇ��int.MinValue��Ԃ��܂��B</returns>
    public int SE_Play(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //���݉���点���Ԃ���ɒ��ׂ�
        int id = int.MinValue;
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return id;
        }
        UnityAction<SoundData> action1 = (SoundData sd) =>
        {
            if (GetClip(out var clip, clipName, false)) {
                //���ʂȂǂ̐ݒ�
                //�f�t�H���g�l�ł���ΐ������l��^����
                SetVolume(sd, volume); // ���ǋL�@sd�ɕύX���܂���
                sd.Source.loop = IsLoop;
                sd.Source.clip = clip;
                sd.fadeState = FadeState.None;
                sd.Source.Play();
                id = sd.ID;
                Log(sd, true);
                if (action != null) { StartCoroutine(CheckPlaying(sd.Source, action)); }
            }
        };
        GetNotUsedSource(action1);
        return id;
    }

    /// <summary>BGM���t�F�[�h�C���Đ������܂��B����BGM���Đ�����Ă���ꍇ�A����BGM�͒�~����܂��B
    /// <br/>�܂��ABGM�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�炵���������t�@�C����</param>
    /// <param name="fadeTime">�t�F�[�h�C��(endVolume)�ɂȂ�܂ł̎���(�b)�@���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="startVolume">���߂̉��̑傫��(�O�`�P)</param>
    /// <param name="endVolume">�I���̉��̑傫��(�O�`�P) ���w�肵�Ȃ��ꍇ��SoundScale���g�p����܂�</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐� <br/>���t�F�[�h�r���ŉ�������~���ꂽ�ꍇ��t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ă΂�܂���</param>
    /// <returns>�Đ����Ɋ���U��ꂽ���ʗp��ID�̂悤�Ȃ��̂��擾���܂��B�Đ����o���Ȃ������ꍇ��int.MinValue��Ԃ��܂�</returns>
    public int SE_PlayFade(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, bool IsLoop = false,UnityAction action = null) {
        int id = int.MaxValue;
        //���݉���点���Ԃ���ɒ��ׂ�
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return id;
        }
        UnityAction<SoundData> action1 = (SoundData sd) =>
        {
            if (GetClip(out var clip, clipName, false)) {
                //���ʂȂǂ̐ݒ�
                SetVolume(sd,startVolume);
                sd.Source.loop = IsLoop;
                sd.Source.clip = clip;
                sd.Source.Play();
                id = sd.ID;
                Log(sd, true);
                StartCoroutine(FadeSound(sd, fadeTime, endVolume, action, false, false));
            }
        };
        GetNotUsedSource(action1);
        return id;
    }

    //  ��~�֐�    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

//BGM
    /// <summary>BGM���~�����܂�</summary>
    public void BGM_Stop() {
        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData,false);
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
            Log(m_BGMData,false);
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
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.clip.name == clipName) {
                Log(sd, false);
                sd.Source.Stop();
            }
        };
        bool check = GetUsingSource(action);
        Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@������ : " + clipName, true, check);
        return check;
    }
    /// <summary>�w�肳�ꂽ�T�E���h���~�����܂�</summary>
    /// <param name="id">�T�E���hID�w��</param>
    /// <returns>�Ăяo�����Ɏw�肳�ꂽID��SE���Đ����ł����true��Ԃ��܂�</returns>
    public bool SE_Stop(int id) {
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.ID == id) {
                Log(sd, false);
                sd.Source.Stop();
            }
        };
        bool check = GetUsingSource(action);
        Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@ID : " + id, true,check);
        return check;
    }

    /// <summary>SE���t�F�[�h�A�E�g������~���܂��B�w�肳�ꂽSE���Ȃ������ꍇ�ɂ͏����͎��s����܂���B
    /// <br/>�w�肳�ꂽSE�����������Đ�����Ă����ꍇ�͑S�Ē�~����܂��B
    /// <br/>SE�����Ƀt�F�[�h�������ł������ꍇ�A�t�F�[�h�������㏑������܂��B</summary>
    /// <param name="clipName">�T�E���h���̎w��</param>
    /// <param name="fadeTime">�t�F�[�h�A�E�g�܂ł̎���(�b) ���w�肵�Ȃ��ꍇ��FadeTime���g�p����܂�</param>
    /// <param name="action">�t�F�[�h�A�E�g��ɍs�����s�֐�(�s���K�v���Ȃ��ꍇ��null��n���Ă�������)</param>
    /// <param name="endVolume">��~���钼�O�̉���(�O�`�P)</param>
    /// <param name="action">�t�F�[�h��ɍs�����s�֐�
    /// <br/>���t�F�[�h���㏑�����ꂽ�ꍇ�ɂ͌Ăяo����܂���B
    /// <br/>�܂��A�w�肳�ꂽSE���������������ꍇ�͌��������񐔕��Ă΂�܂��B
    /// </param>
    /// <param name="StopAction">�t�F�[�h�����O�ɍĐ�����~�����ꍇ��action�����s���邩�ǂ���</param>
    public bool SE_StopFade(string clipName,float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null,bool StopAction = false) {
        UnityAction<SoundData> action1 = (SoundData sd) =>
        {
            if (sd.Source.clip.name == clipName) {
                Log(sd, false);
                StartCoroutine(FadeSound(sd, fadeTime, endVolume, action, true, StopAction));
            }
        };
        bool check = GetUsingSource(action1);
        Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@������ : " + clipName, true,check);
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
        UnityAction<SoundData> action1 = (SoundData sd) =>
        {
            if (sd.ID == id) {
                Log(sd, false);
                StartCoroutine(FadeSound(sd, fadeTime, endVolume, action, true, StopAction));
            }
        };
        bool check = GetUsingSource(action1);
        Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B�@ID : " + id, true,check);
        return check;
    }

    /// <summary>���[�v�Đ�����SE�����ׂĒ�~���܂�</summary>
    /// <returns>���[�v�Đ�����SE���������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_StopInLoop() {
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.isPlaying && sd.Source.loop) {
                Log(sd, false);
                sd.Source.Stop();
            }
        };
       bool check = GetUsingSource(action);
        Log("���[�v�Đ�����Ă���SE�͂���܂���ł����B", true,check);
        return check;       
    }

    /// <summary>���[�v���ł͂Ȃ��Đ�����SE��S�Ē�~�����܂��B </summary>
    /// <returns>���[�v�ł͂Ȃ��Đ�����SE���������ꍇ��true��Ԃ��܂�</returns>
    public bool SE_StopNotLoop()
    {
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.isPlaying && !sd.Source.loop) {
                Log(sd, false);
                sd.Source.Stop();
            }
        };
        bool check = GetUsingSource(action);
        Log("���[�v�Đ��ȊO�̍Đ�����Ă���SE�͂���܂���ł����B", true, check);
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
        UnityAction<SoundData> action = (SoundData sd) => { sd.Source.Stop(); };
        GetUsingSource(action);
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
        if (check) {
            GetUsingSource((SoundData sd) => { StartCoroutine(FadeSound(sd, fadeTime, endVolume, null, true, false)); });
        }
        else {
                SoundData restSd = null;
                float resttime = 0;
                //�c��b������Ԓ���SE��T��
                foreach (var sd in m_SEDatas) {
                if (sd.Source.isPlaying) {
                    check = true;
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
                StartCoroutine(FadeSound(restSd, fadeTime, endVolume, action, true, StopAction));
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
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.clip.name == clipName) {
                Log("\n" + clipName + "���|�[�Y����܂��B" + "\nID : " + sd.ID, false);
                sd.Source.Pause();
            }
        };
        bool check = GetUsingSource(action);
        Log("�w�肳�ꂽ��������SE�͍Đ�����Ă��܂���ł����B\n������ : " + clipName, true,check);
        return check;
    }
    /// <summary>�w�肳�ꂽSE���|�[�Y���܂��B</summary>
    /// <param name="clipName">����ID�w��</param>
    /// <returns>�w�肳�ꂽ�������Đ�����Ă����ꍇ��true��Ԃ��܂�</returns>
    public bool SE_Pause(int id) {
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.ID == id) {
                Log("\n" + sd.Source.clip.name + "���|�[�Y����܂��B" + "\nID : " + sd.ID, false);
                sd.Source.Pause();
            }
        };
        bool check = GetUsingSource(action);
        Log("�w�肳�ꂽID��SE�͍Đ�����Ă��܂���ł����B\nID : " + id, true,check);
        return check;
    }

    /// <summary>�w�肳�ꂽ��������SE���|�[�Y�������܂��B</summary>
    /// <param name="clipName">�������̎w��</param>
    /// <returns>�|�[�Y���������������������true��Ԃ��܂��B</returns>
    public bool SE_Restert(string clipName) {
        if (!CanPlayFlag) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return false;
        }
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.Source.clip.name == clipName) {
                Log("\n" + clipName + "�̃|�[�Y����������܂��B" + "\nID : " + sd.ID, false);
                sd.Source.UnPause();
            }
        };
        bool check = GetPausingSource(action);
        Log("�w�肳�ꂽ��������SE�̓|�[�Y����Ă��܂���ł����B\n������ : " + clipName, true,check);
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
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            if (sd.ID == id) {
                Log("\n" + sd.Source.clip.name + "�̃|�[�Y����������܂��B" + "\nID : " + sd.ID, false);
                sd.Source.UnPause();
            }
        };
        bool check = GetPausingSource(action);
        Log("�w�肳�ꂽID��SE�̓|�[�Y����Ă��܂���ł����B    ID : " + id, true, check);
        return check;
    }


//ALL
    /// <summary>�Đ�����Ă���T�E���h��S�ă|�[�Y���܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void ALL_Pause(bool IsIncludingBGM) {
        if (IsIncludingBGM) {
            BGM_Pause();
        }
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            sd.Source.Pause();
            Log(sd.Source.clip.name + "(SE)���|�[�Y����܂�", false);
        };
        GetUsingSource(action);
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
        UnityAction<SoundData> action = (SoundData sd) =>
        {
            Log(sd, false);
            sd.Source.UnPause();
            Log(sd.Source.clip.name + "(SE)���|�[�Y��������܂�", false);
        };
        GetPausingSource(action);
    }


//  ���̑�   //---------------------------------------------------------------------------------------------------------------

    /// <summary>�w�肳�ꂽ�������擾���܂�</summary>
    /// <returns>��������Ԃ��܂��B������Ȃ������ꍇ��null��Ԃ��܂��B</returns>
    public AudioClip GetAudioClip(string clipName) { GetClip(out var clip, clipName, true); return clip;}

    /// <summary>���ݍĐ�����Ă���SE�����擾���܂��B</summary>
    public int GetPlaySENum() {
        int num = 0;
        foreach(var sd in m_SEDatas) {
            if (sd.Source.isPlaying) num++;
        }
        return num;
    }

    /// <summary>(�f�o�b�O�p)���Đ�����Ă��鉹���Ƃ���ID�����ׂăR���\�[����ɕ\�����܂�</summary>    
    [Conditional("UNITY_EDITOR")]
    public void IsPlayingSoundLog(){
        if (m_BGMData.Source.isPlaying){
            Debug.Log("�Đ�����Ă���BGM : " + m_BGMData.Source.clip.name);
        }
        GetUsingSource((SoundData sd) => { Debug.Log("�Đ�����Ă���T�E���h : " + sd.Source.clip.name + "�g�p���Ă���ID : " + sd.ID); });
      }
      

    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------

    //�I�[�f�B�I�\�[�X���Ǘ�����N���X
    private class SoundData {
        public AudioSource Source;
        public int ID;
        public FadeState fadeState;
        public float SetVolume;
    }
    //�t�F�[�h��Ԃ��Ǘ�����񋓌^
    private enum FadeState {
        None,
        Fading,
        Pause,
    }
    //�g�p����I�[�f�B�I�\�[�X�Ǘ��N���X
    private SoundData[] m_SEDatas;
    private SoundData m_BGMData;
    //�������X�g
    private static Dictionary<string, AudioClip> ClipList_BGM;     //BGM�̃��X�g
    private static Dictionary<string, AudioClip> ClipList_SE;      //SE�̃��X�g
    //���̑�   
    private int m_IDN = 1;                                         //ID�z�z�p�̔ԍ�

  
   
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
            m_BGMData.Source.loop = true;
            SetVolume(m_BGMData, m_standardVolume);
            m_SEDatas = new SoundData[m_maxSoundOverlap - 1];
            for (int i = 0; i < m_SEDatas.Length; ++i) {
                m_SEDatas[i] = new SoundData();
                m_SEDatas[i].Source = gameObject.AddComponent<AudioSource>();
                SetVolume(m_SEDatas[i],m_standardVolume);
                m_SEDatas[i].ID = int.MinValue;
                m_SEDatas[i].fadeState = FadeState.None;  
            }
            //�t�@�C���̓ǂݍ���----------------------------------------------------
            ClipList_BGM = new Dictionary<string, AudioClip>();
            ClipList_SE = new Dictionary<string, AudioClip>();
            SetAudioClip(ClipList_BGM,c_FolderPath_BGM);
            SetAudioClip(ClipList_SE,c_FolderPath_SE);
            //�f�t�H���g�l�̊i�[----------------------------------------------------      
            m_divisionScale_SE = Mathf.Clamp(m_divisionScale_SE,0,m_divisionScaleNum_SE);
            m_divisionScale_BGM = Mathf.Clamp(m_divisionScale_SE,0,m_divisionScaleNum_BGM);
        }
        //--------------------------------------------------------------------------
    }

    //  ���s�֐�    //-----------------------------------------------------------------------------

    /// <summary>�g���Ă��Ȃ��I�[�f�B�I�\�[�X�̎擾(id������U����)</summary>
    private bool GetNotUsedSource(UnityAction<SoundData> action)
    {
        if (!m_overwriteSE) {
            for (int i = 0; i < m_SEDatas.Length; ++i) {
                if (!m_SEDatas[i].Source.isPlaying && m_SEDatas[i].Source.time == 0) {
                    m_SEDatas[i].ID = GetID();
                    action(m_SEDatas[i]);
                    return true;
                }
            }
            Log("��x�ɖ点�鉹�̍ő吔�𒴂��Ă��܂�", true);
            return false;
        }
        else {
            SoundData d = null;
            for (int i = 0; i < m_SEDatas.Length; ++i) {
                if (!m_SEDatas[i].Source.isPlaying) {
                    if (m_SEDatas[i].Source.time == 0) {
                        m_SEDatas[i].ID = GetID();
                        action(m_SEDatas[i]);
                        return true;
                    }
                }
                else {
                    if (!m_SEDatas[i].Source.loop) {
                        d = d == null ? m_SEDatas[i]: d.ID > m_SEDatas[i].ID ? d = m_SEDatas[i] : d;
                    }
                }
            }
            if (d != null) {
                Log("SE���㏑������܂��B" + "�㏑�������SE�̖��O�F " + d.Source.clip.name + "  ID : " + d.ID, true);
                d.ID = GetID();
                action(d);
                return true;
            }
            else {
                Log("�㏑���ł���SE������܂���ł����B", true);
                return false;
            }
        }
    }

    /// <summary>�|�[�Y���̃I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetPausingSource(UnityAction<SoundData> action) {
        bool check = false;
        for (int i = 0; i < m_SEDatas.Length; ++i) {
            if (!m_SEDatas[i].Source.isPlaying && m_SEDatas[i].Source.time != 0) {
                action(m_SEDatas[i]);
                check = true;
            }
        }
        return check;
    }

    /// <summary>�g���Ă���I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetUsingSource(UnityAction<SoundData> action,string noneLog = null) {
        bool check = false;
        for (int i = 0; i < m_SEDatas.Length; ++i) {
            if (m_SEDatas[i].Source.isPlaying) {
                action(m_SEDatas[i]);
                check = true;
            }
        }
        UsefulSystem.DebugAction(() => {
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
        time = float.IsNaN(time) ? m_standardFadeTime : time;
        ev = float.IsNaN(ev) ?  m_standardVolume: ev;
        //�t�F�[�h���d�����Ă���ꍇ�͏㏑������
        if (s.fadeState != FadeState.None) {
            s.fadeState = FadeState.None;
            yield return null;
            s.fadeState = s.fadeState == FadeState.None ? FadeState.Fading : FadeState.Pause;
        }
        else {
            s.fadeState = FadeState.Fading;
        }
        float sv = s.SetVolume;
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
                Log("�t�F�[�h�������㏑������܂��B", true);
                break;
            }
            //�S�Ă̏����𖞂������ꍇ�̂݃t�F�[�h��i�߂�
            else if(s.fadeState == FadeState.Fading){
                SetVolume(s,Mathf.Lerp(sv, ev, t));
                Debug.Log(Mathf.Lerp(sv, ev, t));
                t += Time.deltaTime / time;
            }         
            //���[�v�𔲂������
            if (t >= 1) {
                if (s.Source.isPlaying) {
                    Log(s.Source.clip.name + "�̃t�F�[�h���������܂���", false);
                    SetVolume(s, ev);
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
                UsefulSystem.LogError("�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
            }
#if UNITY_EDITOR
            string str = IsPrioritizeBGM ? "SE������BGM�Ƃ��Ďg�p����܂��B" : "BGM������SE�Ƃ��Ďg�p����܂��B";
            Log(str, true);
#endif
        }
        return check;
    }

    /// <summary>�T�E���h�̉��ʂ��擾���܂��B </summary>
    private void SetVolume(SoundData sd,float volume) {
        CheckNaN(ref volume, m_standardVolume);
        if(sd == m_BGMData)
            sd.Source.volume = volume * ((float)m_divisionScale_SE / m_divisionScaleNum_BGM);
        else 
            sd.Source.volume = volume * ((float)m_divisionScale_SE / m_divisionScaleNum_SE);

        sd.SetVolume = volume;
    }
    
    /// <summary>NaN�ł����value���i�[���܂�</summary>
    private void CheckNaN(ref float num,float value) {
        num = float.IsNaN(num)? value : num;
    }

    //  �f�o�b�O�p   //----------------------------------------------------------------------------

    /// <summary>���O�̕\�����L���ȏꍇ�ɉ����������O�ɕ\������</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(SoundData d,bool playing) { 
        if (PlaySoundLog) {
            string type = d.ID == 0 ? "(BGM)" : "(SE)";
            string id = d.ID != 0 ? "<color=cyan>ID : " + d.ID + "</color>" : "";
            string str = playing ? "<color=cyan>" + d.Source.clip.name + type + "</color>" + "���Đ�����܂�    " + id : "<color=cyan>" + d.Source.clip.name + type + "</color>" + "����~����܂�" + id;
            Debug.Log(str); 
        } 
    }

    /// <summary>���O�̕\�����L���ȏꍇ�Ƀ��b�Z�[�W�����O�ɕ\������</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(string message,bool warning,bool ignore = false) {
        if (PlaySoundLog && !ignore) {
            if (warning) {
                Debug.LogWarning("<color=yellow>"+ message + "</color>");
            }
            else {
                Debug.Log(message);
            }
        }
    }
}






