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
/// �쐬�҂�PG2�N�̖��m���ł��@�^��_����P�_�A�o�O�񍐓�����΋C�y�ɂ��A����������<br />
/// </summary>

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
// Config�ϐ�  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    private const int    c_MaxSoundOverlap = 10;                                                              //�I�[�f�B�I�̍ő��(BGM����)
    private const string c_FolderPath_BGM = "C:\\Users\\yano3\\Desktop\\Content_3\\�X�p�_���Z�u��(UnityData)\\Assets\\Audio\\BGM\\Resources"; //BGM���i�[����Ă���t�H���_�̃p�X
    private const string c_FolderPath_SE = "C:\\Users\\yano3\\Desktop\\Content_3\\�X�p�_���Z�u��(UnityData)\\Assets\\Audio\\SE\\Resources";   //SE���i�[����Ă���t�H���_�̃p�X

    private const float c_SoundVolume = 0.5f;                                                                  //SoundVolume�̏����l
    private const float c_FadeTimer = 1.0f;                                                                    //FadeTime�̃f�t�H���g�l
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

    /// <summary>�ZBGM�̉��ʂ����X�ɕω������܂�   <br/>���t�F�[�h���s���t�F�[�h�����ȊO����BGM�̉��ʂ��ω������ꍇ�t�F�[�h�����͒�~����܂��B<br/>���n���ꂽ�֐����Ă΂�܂��� </summary>
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

    /// <summary>�Z����PlaySound����Đ�����Ă���T�E���h�̉��ʂ�ύX���܂�</summary>
    /// <param name="volume"></param>
    /// <param name="clipName">�T�E���h�̉����w��(�w�肪�Ȃ��ꍇ�͑S�Ẳ��ʂ��ύX����܂�)</param>
    /// <returns>���ʂ�ύX�����I�[�f�B�I�\�[�X���i�[�������X�g��Ԃ��܂�</returns>
    public List<AudioSource> SetVolume(float volume,string clipName = null) {
        if (GetUsingSource(out var sources)) {
            bool check = false;
            //�w��Ȃ��̏ꍇ
            UnityAction<AudioSource> action =
                clipName == null ? (AudioSource s) => {
                    check = true;
                    s.volume = volume;
                    Log(s.name + "�̉��ʂ��ύX����܂��B\n�ύX��̉��� : " + volume, false); 
                }
            //�w�肠��̏ꍇ
            : (AudioSource s) => {
                if (s.clip.name == clipName) {
                    check = true;
                    s.volume = volume;
                    Log(s.name + "�̉��ʂ��ύX����܂��B\n�ύX��̉��� : " + volume, false);
                }
                else {
                    sources.Remove(s);
                }
            };

            foreach(var source in sources) {
                if (source.isPlaying) {
                  action(source);
                }
            }
            if (!check) {
                Log("�w�肳�ꂽ�����͍Đ�����Ă��܂���ł����B : " + clipName, true);
                return null;
            }
            return sources;
        }
        else {
            Log("�Đ�����Ă��鉹���͌�����܂���ł����B",true);
            return null;
        }
    }
    


    public void SetVolume_Fade(float volume,string clipName,float fadeTime = float.NaN,UnityAction action = null) {

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
    /// <returns>�Đ��Ɏg�p���Ă���I�[�f�B�I�\�[�X��Ԃ��܂��B�Đ����o���Ȃ��ꍇ��null��Ԃ��܂��B</returns>
    public AudioSource PlaySound(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //���݉���点���Ԃ���ɒ��ׂ�
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return null;
        }
        if (!GetNotUsedSource(out AudioSource ads)) {
            return null;
        }
        //�������擾����
        bool check = ClipList_SE.TryGetValue(clipName, out AudioClip clip);
        //�������擾�ł��Ȃ������ꍇ��BGM��������擾����
        if (!check) {
            check = ClipList_BGM.TryGetValue(clipName, out clip);
            //BGM����������擾�ł��Ȃ������ꍇ��null��Ԃ�
            if (!check) {
                GameSystem.LogError("�����t�@�C�������Ԉ���Ă��邩�t�@�C�������݂��܂���\n�t�@�C���� : " + clipName);
                return null;
            }
            Log("PlaySound�֐�����BGM�������Đ�����܂��B", true);
        }
        //���ʂȂǂ̐ݒ�
        //�f�t�H���g�l�ł���ΐ������l��^����
        ads.volume = float.IsNaN(volume) ? m_defaltVolume : volume;
        ads.loop = IsLoop;
        ads.clip = clip;
        ads.Play();
        Log(clip, true);
        if (action != null) {
            StartCoroutine(CheckPlaying(ads, action));
        }
        return ads;
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

    /// <summary>�Đ�����Ă���T�E���h��S�Ē�~�����܂��B</summary>
    public void StopAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (IsIncludingBGM) {
            check = m_BGMSource.isPlaying;
            BGM_Stop();
        }
        if (GetUsingSource(out List<AudioSource> adsList)) {
            foreach (var ads in adsList) {
                ads.Stop();
            }
        }
        else {
            GameSystem.Action(() =>
            {
                if (check) {
                    Log("BGM�ȊO�ɍĐ�����Ă��鉹���͂���܂���ł����B", true);
                }
                else {
                    Log("�Đ�����Ă��鉹���͂���܂���ł���", true);
                }
            });
        }
    }

    /// <summary>PlaySoud����Đ����ꂽ���[�v���̃T�E���h�̍Đ������ׂĒ�~���܂�</summary>
    public void StopLoopSound() {
        if (GetUsingSource(out List<AudioSource> adsList)) {
            bool check = false;
            foreach (var ads in adsList) {
                if (ads.isPlaying && ads.loop) {
                    check = true;
                    Log(ads.clip,false);
                    ads.Stop();
                }
            }
            if (!check) {
                Log("���[�v�Đ�����Ă��鉹���͂���܂���ł����B", true);
            }
        }
        else {
            Log("�Đ�����Ă��鉹���͌�����܂���ł����B",true);
        }

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

    /// <summary>�Z�Đ�����Ă���T�E���h��S�ă|�[�Y���܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void PauseAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (IsIncludingBGM) {
            check = m_BGMSource.isPlaying;
            BGM_Pause();
        }

        if (GetUsingSource(out List<AudioSource> adsList)) {
            check = true;
            foreach (var ads in adsList) {
                ads.Stop();
            }
        }
        else {
            GameSystem.Action(() =>
            {
                if (check) {
                    Log("BGM�ȊO�ɍĐ�����Ă��鉹���͂���܂���ł����B", true);
                }
                else if (m_BGMSource.isPlaying) {
                    Log("BGM�ȊO�ɍĐ�����Ă��鉹���͂���܂���ł����B(BGM�̓|�[�Y����܂���)", true);
                }
                else {
                    Log("�Đ�����Ă��鉹���͂���܂���ł���", true);
                }
            });
        }
    }

    /// <summary>�S�Ẵ|�[�Y����Ă��鉹�����ĊJ�����܂�</summary>
    /// <param name="IsIncludingBGM">BGM���܂߂邩�ǂ���</param>
    public void RestertAllSound(bool IsIncludingBGM) {
        if (!CanPlay) {
            Log("���ݍĐ��t���O���I�t�ɂȂ��Ă��邽�߁A�Đ��ł��܂���B", true);
            return;
        }


        if (IsIncludingBGM) {
            if(!m_BGMSource.isPlaying && m_BGMSource.time != 0) {
                m_BGMSource.Play();
            }
            else {
                Log("BGM�͒��f����Ă��܂���",true);
            }
        }
        bool check = false;
        if (GetUsingSource(out List<AudioSource> adsList)) {
            foreach (var ads in adsList) {
                if (!ads.isPlaying && ads.time != 0) {
                    Log(ads.clip,true);
                    ads.Play();
                    check = true;
                }
            }

            if (!check) {
                Log("���f����Ă���T�E���h�͂���܂���B",true);
            }
        }     
    }


  
    
    //   �Z���[�v���ł͂Ȃ��T�E���h��S�Ē�~�����܂��B
    /// <summary>
    /// �Z���[�v���ł͂Ȃ��T�E���h��S�Ē�~�����܂��B
    /// </summary>
    /// <param name="FadeTime">�t�F�[�h�A�E�g�܂ł̎���(�������������ꍇ�͂O)</param>
    //endregion
    //  public void Stop_NotInLoop_Sound(float FadeTime,UnityAction action = null)
    //  .
    //  {
    //      int check = -1;
    //      for (int i = 0; i < source.Length; ++i)
    //      {
    //          if (source[i].isPlaying && !source[i].loop)
    //          {
    //              if (0 <= check)
    //              { Task task = AsyncFadeOutSound(source[check], FadeTime, null); }
    //              check = i;
    //          }    
    //      }
    //      if (0 <= check)
    //      { Task task = AsyncFadeOutSound(source[check], FadeTime, action); }
    //      else
    //          Debug.LogWarning("�Đ�����Ă��鉹���͂���܂���ł���");
    //  }
    //  
    //

    //
    //   �Z�w�肵�������̃T�E���h���Đ�����Ă����ꍇ������~���܂�
    /// <summary>
    /// �w�肵�������̃T�E���h���Đ�����Ă����ꍇ������~���܂�(BGM�͏���)
    /// </summary>
    /// <param name="clip">��~�����鉹��</param>
    /// <param name="FadeTime">�t�F�[�h�A�E�g�A�E�g�܂ł̎���(�������������ꍇ�͂O)</param>
    /// <param name="action">�t�F�[�h���I������ۂɊ֐����Ăяo�������ꍇ�͎g�p���Ă��������B�K�v�Ȃ����null��n���Ă�������</param>
    /// <returns>�Đ����~�������ꍇtrue��Ԃ��A�w�肵���������Đ�����Ă��Ȃ������ꍇ��false��Ԃ��܂�</returns>
    //
    //  public void Stop_Select_Sound(AudioClip clip, float FadeTime,UnityAction action = null)
    //  .
    //  {
    //      int check = -1;
    //      //�t�F�[�h�A�E�g����
    //      for (int i = 0; i < source.Length; ++i)
    //      {
    //          if (source[i].isPlaying && source[i].clip == clip)
    //          {      
    //              if (0 <= check)
    //              { Task task = AsyncFadeOutSound(source[check], FadeTime, null); }
    //              check = i;
    //          }
    //      }
    //
    //      if (0 <= check)
    //      {
    //          Task task = AsyncFadeOutSound(source[check], FadeTime,action);
    //      }
    //      else
    //      {
    //          Debug.LogWarning(clip.name + "�͍Đ�����Ă��܂���ł����B");
    //      }
    //  }
    //  
    //
    //   �Z�w�肵�������̃T�E���h���Đ�����Ă����ꍇ�A���̃T�E���h�̉��ʂ�ύX���܂��B���������̃T�E���h���������������ꍇ�͑S�ĕύX����܂��B
    /// <summary>
    /// �Z�w�肵�������̃T�E���h���Đ�����Ă����ꍇ�A���̃T�E���h�̉��ʂ�ύX���܂��B���������̃T�E���h���������������ꍇ�͑S�ĕύX����܂��B
    /// </summary>
    /// <param name="clip">�ύX�������T�E���h�̉���</param>
    /// <param name="SetVolume">�ύX��̉���</param>
    /// <param name="FadeTime">���̎��Ԃ������ď��X�ɕύX���܂�(�����ύX�������ꍇ�͂O)</param>
    /// <param name="action">�t�F�[�h���I������ۂɊ֐����Ăяo�������ꍇ�͎g�p���Ă��������B�K�v�Ȃ����null��n���Ă�������</param>
    //
    //  public void Set_Volume_Select_Sound(AudioClip clip, float SetVolume, float FadeTime,UnityAction action = null)
    //  .
    //  {
    //      int check = -1;
    //      for (int i = 0; i < source.Length; ++i)
    //      {
    //          if (source[i].isPlaying && source[i].clip == clip)
    //          {
    //              if (0 <= check)
    //              { Task task = AsyncChangeVolume(source[check], SetVolume, FadeTime, null); }
    //              check = i;
    //          }
    //      }
    //
    //      if(0 <= check)
    //      {
    //          Task task = AsyncChangeVolume(source[check], SetVolume, FadeTime,action);
    //      }
    //      else
    //          Debug.LogWarning(clip.name + "�͍Đ�����Ă��܂���ł����B");
    //  }
    //  
    //
    //   �Z���ݍĐ�����Ă���T�E���h�����邩���ׂ܂�
    /// <summary>
    /// �Z���ݍĐ�����Ă���T�E���h�����邩���ׂ܂�
    /// </summary>
    /// <returns>�Đ�����Ă��鉹��������ꍇtrue��Ԃ��܂�</returns>
    //
    //  public bool Check_IsPlaying_Sound()
    //  .
    //  {
    //      for (int i = 0; i < source.Length; ++i)
    //      {
    //          if (source[i].isPlaying)
    //              return true;
    //      }
    //      return false;
    //  }
    //  
    //
    //   �Z���Đ�����Ă��鉹�������ׂăR���\�[����ɕ\�����܂�(�f�o�b�O�p)
    /// <summary>
    /// �Z���Đ�����Ă��鉹�������ׂăR���\�[����ɕ\�����܂�(�f�o�b�O�p)
    /// </summary>
    //    
    //  [Conditional("UNITY_EDITOR")]
    //  public void Display_IsPlaying_ClipName()
    //  .
    //  {
    //      bool find = false;
    //      for (int i = 0; i < source.Length; ++i)
    //      {
    //          if (source[i].isPlaying)
    //          {
    //              find = true;
    //              Debug.Log(source[i].clip.name);
    //          }
    //      }
    //      if (!find)
    //          Debug.Log("�Đ�����Ă��鉹���͂���܂���ł����B");
    //  }
    //  
    //

    //  �v���C�x�[�g�ϐ�  //------------------------------------------------------------------------------------------------------------------------------

    //�g�p����I�[�f�B�I�\�[�X
    private AudioSource[] m_SoundSource;                       
    private AudioSource m_BGMSource;
    //�������X�g
    private Dictionary<string, AudioClip> ClipList_BGM;     //BGM�̃��X�g
    private Dictionary<string, AudioClip> ClipList_SE;      //SE�̃��X�g
    //���̑�
    private float m_defaltVolume = c_SoundVolume;           //�f�t�H���g��volume
    private float m_defaltTime = c_FadeTimer;               //�f�t�H���g�̃t�F�[�h����
    [SerializeField, TooltipAttribute("�Đ�����鉹���Ȃǂ����O�ɕ\�����邩�ǂ���(�G���[���b�Z�[�W�͏���)")] private bool PlaySoundLog = true;
    //  �v���C�x�[�g�֐�  //------------------------------------------------------------------------------------------------------------------------------

    //  �������֐�   //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        //�I�[�f�B�I�\�[�X�̍쐬----------------------------------------------------
        if (m_SoundSource == null) {
            m_BGMSource = gameObject.AddComponent<AudioSource>();
            m_SoundSource = new AudioSource[c_MaxSoundOverlap - 1];
            for (int i = 0; i < m_SoundSource.Length; ++i) {
                m_SoundSource[i] = gameObject.AddComponent<AudioSource>();
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
    private bool GetNotUsedSource(out AudioSource s)
    {
        s = null;
      for (int i = 0; i < m_SoundSource.Length; ++i) {
          if (!m_SoundSource[i].isPlaying && m_SoundSource[i].time == 0) {;
                s = m_SoundSource[i];
                return true;
          }
      }
        GameSystem.LogError("��x�ɖ点�鉹�̍ő吔�𒴂��Ă��܂�");
        return false;
    }

    /// <summary>�g���Ă���I�[�f�B�I�\�[�X�̎擾</summary>
    private bool GetUsingSource(out List<AudioSource> s) {
        s = new List<AudioSource> ();
        bool check = false;
        for (int i = 0; i < m_SoundSource.Length; ++i) {
            if (m_SoundSource[i].isPlaying) {
                s.Add (m_SoundSource[i]);
                check = true;
            }
        }
        return check;
    }

    /// <summary> �t�H���_�̃p�X����t�H���_���̑S�Ẳ��������[�h���A���X�g�ɒǉ�����</summary>
    private void SetAudioClip(Dictionary<string,AudioClip> dic, string path) {

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






