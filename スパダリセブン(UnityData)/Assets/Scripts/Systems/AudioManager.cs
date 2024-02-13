using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// オーディオを一括管理するシングルトンクラスです。インスタンスはAudioManager.Instanceで取得できます<br />
/// SE/BGM用のResourcesフォルダを作成し、そこに音源を格納してください<br />
/// 再生等は音源ファイル名で行うので、命名規則などを設けてください<br />
/// 作成者はPG2年の矢野洋平です　疑問点や改善点、バグ報告等あれば気軽にご連絡ください<br />
/// </summary>

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
// Config変数  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    private const int    c_MaxSoundOverlap = 10;                                                              //オーディオの最大個数(BGM込み)
    private const string c_FolderPath_BGM = "C:\\Users\\yano3\\Desktop\\Content_3\\スパダリセブン(UnityData)\\Assets\\Audio\\BGM\\Resources"; //BGMが格納されているフォルダのパス
    private const string c_FolderPath_SE = "C:\\Users\\yano3\\Desktop\\Content_3\\スパダリセブン(UnityData)\\Assets\\Audio\\SE\\Resources";   //SEが格納されているフォルダのパス

    private const float c_SoundVolume = 0.5f;                                                                  //SoundVolumeの初期値
    private const float c_FadeTimer = 1.0f;                                                                    //FadeTimeのデフォルト値
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


//  変数・関数一覧    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //  設定など    //

    /// <summary>音楽が再生可能かどうかのフラグです。※ただし、falseにした時に既に再生されている音楽は停止されません</summary>
    public bool CanPlay { get; set; } = true;

    /// <summary>フェードにかける時間のデフォルト値を取得・変更します</summary>
    public float DefaultFadeTime { get { return m_defaltTime; } set { m_defaltTime = value; } }


    //  音量設定    //

    /// <summary>サウンドの音量のデフォルト値を設定・取得します</summary>
    public float DefaultVolume { get { return m_defaltVolume; } set { m_defaltVolume = value; } }

    /// <summary>現在のBGMの音量を設定・取得します。</summary>
    public float BGM_Volume {
        get {
            GameSystem.Action(() => { if (!m_BGMSource.isPlaying) { Log("BGMは再生されていません。", true); } else { Log("現在のBGMに設定されている音量は" + m_BGMSource.volume + "です。", false); } }); 
            return m_BGMSource.volume;
        }
        set {
            GameSystem.Action(() => { if (!m_BGMSource.isPlaying) { Log("BGMは再生されていません。再生時に音量設定が上書きされる恐れがあります。", true); } else { Log("BGMの音量が" + value+"に変更されます。",false); } });
            m_BGMSource.volume = value; 
        }
    }

    /// <summary>〇BGMの音量を徐々に変化させます   <br/>※フェード実行中フェード処理以外からBGMの音量が変化した場合フェード処理は停止されます。<br/>※渡された関数も呼ばれません </summary>
    /// <param name="volume">設定する音量</param>
    /// <param name="fadeTime">設定した音量になるまでの時間(秒)　※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="action">フェード後に行う実行関数(必要な場合のみ)</param>
    public void BGM_SetVolume_Fade(float volume, float fadeTime = float.NaN, UnityAction action = null) {
        if (m_BGMSource.isPlaying) {
            Log(m_BGMSource.clip.name + "の音量が徐々に変更されます。 \n 変更後の音量 : " + volume, false);
            //デフォルト値であれば正しい値に変更する
            fadeTime = float.IsNaN(fadeTime) ? m_defaltTime : fadeTime;
            StartCoroutine(FadeSound(m_BGMSource, fadeTime, volume, action, false));
        }
        else {
            Log("BGMが再生されていないので音量を変更する事ができません。", true);
            return;
        }
    }

    /// <summary>〇現在PlaySoundから再生されているサウンドの音量を変更します</summary>
    /// <param name="volume"></param>
    /// <param name="clipName">サウンドの音源指定(指定がない場合は全ての音量が変更されます)</param>
    /// <returns>音量を変更したオーディオソースを格納したリストを返します</returns>
    public List<AudioSource> SetVolume(float volume,string clipName = null) {
        if (GetUsingSource(out var sources)) {
            bool check = false;
            //指定なしの場合
            UnityAction<AudioSource> action =
                clipName == null ? (AudioSource s) => {
                    check = true;
                    s.volume = volume;
                    Log(s.name + "の音量が変更されます。\n変更後の音量 : " + volume, false); 
                }
            //指定ありの場合
            : (AudioSource s) => {
                if (s.clip.name == clipName) {
                    check = true;
                    s.volume = volume;
                    Log(s.name + "の音量が変更されます。\n変更後の音量 : " + volume, false);
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
                Log("指定された音源は再生されていませんでした。 : " + clipName, true);
                return null;
            }
            return sources;
        }
        else {
            Log("再生されている音源は見つかりませんでした。",true);
            return null;
        }
    }
    


    public void SetVolume_Fade(float volume,string clipName,float fadeTime = float.NaN,UnityAction action = null) {

    }   

    //  再生関数    //

    /// <summary>BGMを再生します。既にBGMが再生されていた場合はそのBGMは停止されます。</summary>
    /// <param name="clipName">鳴らしたい音源ファイル名(拡張子不要)</param>
    /// <param name="volume">音量(０～１)　※指定しない場合はSoundVolumeが使用されます</param>
    /// <returns>再生に使用しているオーディオソースを返します</returns>
    public void BGM_Play(string clipName, float volume = float.NaN) {
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }
        //音源を取得する
        bool check = ClipList_BGM.TryGetValue(clipName, out AudioClip clip);
        //音源を取得できなかった場合はSE音源から取得する
        if (!check) {
            check = ClipList_SE.TryGetValue(clipName, out clip);
            //SE音源からも取得できなかった場合
            if (!check) {
                Debug.Assert(clip != null, "音源ファイル名が間違っているかファイルが存在しません\nファイル名 : " + clipName);
                return;
            }
            Log("BGMとしてSE音源が再生されます", true);
        }
        //デフォルト値であれば正しい値を与える
        m_BGMSource.volume = float.IsNaN(volume)? m_defaltVolume : volume;
        m_BGMSource.clip = clip;
        m_BGMSource.loop = true;
        m_BGMSource.Play();
        Log(clip, true);
    }

    /// <summary>BGMをフェードイン再生させます。既にBGMが再生されている場合、そのBGMは停止されます。 <br/>※フェード実行中フェード処理以外からBGMの音量が変化した場合フェード処理は停止されます。<br/>※渡された関数も呼ばれません </summary>
    /// <param name="clipName">鳴らしたい音源ファイル名</param>
    /// <param name="fadeTime">フェードイン(endVolume)になるまでの時間(秒)　※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="startVolume">初めの音の大きさ(０～１)</param>
    /// <param name="endVolume">終わりの音の大きさ(０～１) ※指定しない場合はSoundVolumeが使用されます</param>
    /// <param name="action">フェード後に行う実行関数(必要な場合のみ)</param>
    public void BGM_Play_Fadein(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, UnityAction action = null) {
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }
        //デフォルト値であれば正しい値を与える
        fadeTime = float.IsNaN(fadeTime) ?m_defaltTime : fadeTime;
        endVolume = float.IsNaN(endVolume) ? m_defaltVolume : endVolume;
        m_BGMSource.volume = startVolume;
        BGM_Play(clipName, startVolume);
        StartCoroutine(FadeSound(m_BGMSource, fadeTime, endVolume, action, false));
    }

    /// <summary>サウンドを再生します。主にSEを再生する場合に使用してください</summary>
    /// <param name="clipName">鳴らしたい音源ファイル名(拡張子不要)</param>
    /// <param name="volume">音量(０～１) ※指定しない場合はSoundVolumeが使用されます</param>
    /// <param name="IsLoop">ループ再生をする場合はture</param>
    /// <param name="action">再生された音源が停止された場合に関数を実行します。(必要な場合のみ)
    /// <br/>※音源が最後まで再生されず停止された場合にも関数が実行されます。注意してください。
    /// <br/>※また、ループが有効な場合はループ再生が停止されるまで関数は呼ばれません。</param>
    /// <returns>再生に使用しているオーディオソースを返します。再生が出来ない場合はnullを返します。</returns>
    public AudioSource PlaySound(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //現在音を鳴らせる状態か先に調べる
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return null;
        }
        if (!GetNotUsedSource(out AudioSource ads)) {
            return null;
        }
        //音源を取得する
        bool check = ClipList_SE.TryGetValue(clipName, out AudioClip clip);
        //音源を取得できなかった場合はBGM音源から取得する
        if (!check) {
            check = ClipList_BGM.TryGetValue(clipName, out clip);
            //BGM音源からも取得できなかった場合はnullを返す
            if (!check) {
                GameSystem.LogError("音源ファイル名が間違っているかファイルが存在しません\nファイル名 : " + clipName);
                return null;
            }
            Log("PlaySound関数からBGM音源が再生されます。", true);
        }
        //音量などの設定
        //デフォルト値であれば正しい値を与える
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


    //  停止関数    //
    /// <summary>BGMを停止させます</summary>
    public void BGM_Stop() {
        if (m_BGMSource.isPlaying) {
            Log(m_BGMSource.clip, false);
        }
        else {
            Log("BGMは再生されていませんでした", true);
        }
        m_BGMSource.Stop();
    }

    /// <summary>BGMをフェードアウトさせ停止します。 <br/>※フェード実行中フェード処理以外からBGMの音量が変化した場合フェード処理は停止されます。。<br/>※渡された関数も呼ばれません</summary>
    /// <param name="fadeTime">フェードアウトまでの時間(秒) ※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="action">フェードアウト後に行う実行関数(行う必要がない場合はnullを渡してください)</param>
    /// <param name="endVolume">停止する直前の音量(０～１)</param>
    /// <param name="action">フェード後に行う実行関数(必要な場合のみ)</param>
    public void BGM_Stop_FadeOut(float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null) {

        if (m_BGMSource.isPlaying) {
            Log(m_BGMSource.clip.name + "がフェードアウトされます", false);
            //デフォルト値であれば正しい値に変更する
            fadeTime = float.IsNaN(fadeTime) ? m_defaltTime : fadeTime;
            StartCoroutine(FadeSound(m_BGMSource, fadeTime, endVolume, action, true));
        }
        else {
            Log("BGMは再生されていませんでした", true);
            return;
        }

    }

    /// <summary>再生されているサウンドを全て停止させます。</summary>
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
                    Log("BGM以外に再生されている音源はありませんでした。", true);
                }
                else {
                    Log("再生されている音源はありませんでした", true);
                }
            });
        }
    }

    /// <summary>PlaySoudから再生されたループ中のサウンドの再生をすべて停止します</summary>
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
                Log("ループ再生されている音源はありませんでした。", true);
            }
        }
        else {
            Log("再生されている音源は見つかりませんでした。",true);
        }

    }


    //  一時停止・再開   //

    /// <summary>BGMをポーズします</summary>
    public void BGM_Pause() {
        if (m_BGMSource.isPlaying) {
            m_BGMSource.Pause();
            Log("BGMがポーズされます", false);
        }
        else {
            Log("BGMは再生されていません", true);
        }

    }

    /// <summary>ポーズ中のBGMを再開します</summary>
    public void BGM_Restert() {
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }

        if (!m_BGMSource.isPlaying && m_BGMSource.time != 0) {
            m_BGMSource.UnPause();
            Log("BGMのポーズ状態が解除されます", false);
        }
        else {
            GameSystem.LogError("BGMはポーズされていません。");
        }

    }

    /// <summary>〇再生されているサウンドを全てポーズします</summary>
    /// <param name="IsIncludingBGM">BGMも含めるかどうか</param>
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
                    Log("BGM以外に再生されている音源はありませんでした。", true);
                }
                else if (m_BGMSource.isPlaying) {
                    Log("BGM以外に再生されている音源はありませんでした。(BGMはポーズされません)", true);
                }
                else {
                    Log("再生されている音源はありませんでした", true);
                }
            });
        }
    }

    /// <summary>全てのポーズされている音源を再開させます</summary>
    /// <param name="IsIncludingBGM">BGMも含めるかどうか</param>
    public void RestertAllSound(bool IsIncludingBGM) {
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }


        if (IsIncludingBGM) {
            if(!m_BGMSource.isPlaying && m_BGMSource.time != 0) {
                m_BGMSource.Play();
            }
            else {
                Log("BGMは中断されていません",true);
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
                Log("中断されているサウンドはありません。",true);
            }
        }     
    }


  
    
    //   〇ループ中ではないサウンドを全て停止させます。
    /// <summary>
    /// 〇ループ中ではないサウンドを全て停止させます。
    /// </summary>
    /// <param name="FadeTime">フェードアウトまでの時間(すぐ消したい場合は０)</param>
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
    //          Debug.LogWarning("再生されている音源はありませんでした");
    //  }
    //  
    //

    //
    //   〇指定した音源のサウンドが再生されていた場合それを停止します
    /// <summary>
    /// 指定した音源のサウンドが再生されていた場合それを停止します(BGMは除く)
    /// </summary>
    /// <param name="clip">停止させる音源</param>
    /// <param name="FadeTime">フェードアウトアウトまでの時間(すぐ消したい場合は０)</param>
    /// <param name="action">フェードが終わった際に関数を呼び出したい場合は使用してください。必要なければnullを渡してください</param>
    /// <returns>再生を停止させた場合trueを返し、指定した音源が再生されていなかった場合はfalseを返します</returns>
    //
    //  public void Stop_Select_Sound(AudioClip clip, float FadeTime,UnityAction action = null)
    //  .
    //  {
    //      int check = -1;
    //      //フェードアウト処理
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
    //          Debug.LogWarning(clip.name + "は再生されていませんでした。");
    //      }
    //  }
    //  
    //
    //   〇指定した音源のサウンドが再生されていた場合、そのサウンドの音量を変更します。同じ音源のサウンドが複数見つかった場合は全て変更されます。
    /// <summary>
    /// 〇指定した音源のサウンドが再生されていた場合、そのサウンドの音量を変更します。同じ音源のサウンドが複数見つかった場合は全て変更されます。
    /// </summary>
    /// <param name="clip">変更したいサウンドの音源</param>
    /// <param name="SetVolume">変更後の音量</param>
    /// <param name="FadeTime">この時間をかけて徐々に変更します(すぐ変更したい場合は０)</param>
    /// <param name="action">フェードが終わった際に関数を呼び出したい場合は使用してください。必要なければnullを渡してください</param>
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
    //          Debug.LogWarning(clip.name + "は再生されていませんでした。");
    //  }
    //  
    //
    //   〇現在再生されているサウンドがあるか調べます
    /// <summary>
    /// 〇現在再生されているサウンドがあるか調べます
    /// </summary>
    /// <returns>再生されている音源がある場合trueを返します</returns>
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
    //   〇今再生されている音源をすべてコンソール上に表示します(デバッグ用)
    /// <summary>
    /// 〇今再生されている音源をすべてコンソール上に表示します(デバッグ用)
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
    //          Debug.Log("再生されている音源はありませんでした。");
    //  }
    //  
    //

    //  プライベート変数  //------------------------------------------------------------------------------------------------------------------------------

    //使用するオーディオソース
    private AudioSource[] m_SoundSource;                       
    private AudioSource m_BGMSource;
    //音源リスト
    private Dictionary<string, AudioClip> ClipList_BGM;     //BGMのリスト
    private Dictionary<string, AudioClip> ClipList_SE;      //SEのリスト
    //その他
    private float m_defaltVolume = c_SoundVolume;           //デフォルトのvolume
    private float m_defaltTime = c_FadeTimer;               //デフォルトのフェード時間
    [SerializeField, TooltipAttribute("再生される音源などをログに表示するかどうか(エラーメッセージは除く)")] private bool PlaySoundLog = true;
    //  プライベート関数  //------------------------------------------------------------------------------------------------------------------------------

    //  初期化関数   //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        //オーディオソースの作成----------------------------------------------------
        if (m_SoundSource == null) {
            m_BGMSource = gameObject.AddComponent<AudioSource>();
            m_SoundSource = new AudioSource[c_MaxSoundOverlap - 1];
            for (int i = 0; i < m_SoundSource.Length; ++i) {
                m_SoundSource[i] = gameObject.AddComponent<AudioSource>();
            }
            //ファイルの読み込み----------------------------------------------------
            ClipList_BGM = new Dictionary<string, AudioClip>();
            ClipList_SE = new Dictionary<string, AudioClip>();
            SetAudioClip(ClipList_BGM,c_FolderPath_BGM);
            SetAudioClip(ClipList_SE,c_FolderPath_SE);
        }
        //--------------------------------------------------------------------------
    }

    //  実行関数    //-----------------------------------------------------------------------------

    /// <summary>使われていないオーディオソースの取得</summary>
    private bool GetNotUsedSource(out AudioSource s)
    {
        s = null;
      for (int i = 0; i < m_SoundSource.Length; ++i) {
          if (!m_SoundSource[i].isPlaying && m_SoundSource[i].time == 0) {;
                s = m_SoundSource[i];
                return true;
          }
      }
        GameSystem.LogError("一度に鳴らせる音の最大数を超えています");
        return false;
    }

    /// <summary>使われているオーディオソースの取得</summary>
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

    /// <summary> フォルダのパスからフォルダ内の全ての音源をロードし、リストに追加する</summary>
    private void SetAudioClip(Dictionary<string,AudioClip> dic, string path) {

        string[] fileNames = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            .Where(s => !s.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase)).ToArray();
        Debug.Assert(fileNames != null, "フォルダが見つかりませんでした");
        for(int i = 0; i < fileNames.Length; i++) {
            //プレハブ名だけを抜き取る
            string[] part = fileNames[i].Split('\\');            
            int index = part[part.Length - 1].IndexOf('.');
            fileNames[i] = part[part.Length - 1].Remove(index);
        }
        foreach (string name in fileNames) {
            AudioClip audio = (AudioClip)Resources.Load(name);
            dic[name] = audio;
        }
    }

    /// <summary>音のフェードイン、アウトを行う</summary>
    private IEnumerator FadeSound(AudioSource s,float time,float ev, UnityAction action, bool stop) {
        float sv = s.volume - 0.001f;
        s.volume = sv;
        float vo = sv;
        float t = 0;
        float startTime = Time.time;
        yield return null;
        while (true) {
            if (vo != s.volume) {
                Log("外部からフェード対象の音量が操作されたのでフェードが停止されます。フェード開始時に渡された関数は実行されません", true);
                break;
            }

            if (!s.isPlaying) {
                if(s.time == 0) {
                    Log("フェード中に再生が停止されました。フェード開始時に渡された関数は実行されません", true);
                    break;
                }
            }
            else {
                vo = Mathf.SmoothStep(sv, ev, t);
                s.volume = vo;
                t += Time.deltaTime / time;
            }         
            //ループを抜ける条件
            if (t >= 1) {
                if (s.isPlaying) {
                    Log(s.clip.name + "のフェードが完了しました", false);
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
            //次のループまで1フレーム待機する
            yield return null;
        }
    }

    /// <summary>音源が再生終了するまで監視する</summary>
    private IEnumerator CheckPlaying(AudioSource s,UnityAction action) {
        while (true) {
            if (!s.isPlaying && s.time == 0) {
                Log(s.clip.name + "の再生が終了しました。関数の実行に移ります",false);
                action.Invoke();
                yield break;    
            }
            yield return null;
        }
    }

    //  デバッグ用   //----------------------------------------------------------------------------

    /// <summary>ログの表示が有効な場合に音源名をログに表示する</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(AudioClip clip,bool play) { 
        if (PlaySoundLog) {
            string str = play ? "<color=cyan>" + clip.name + "</color>" + "が再生されます" : "<color=cyan>" + clip.name + "</color>" + "が停止されます";
            Debug.Log(str); 
        } 
    }

    /// <summary>ログの表示が有効な場合にメッセージをログに表示する</summary>
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






