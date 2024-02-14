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
/// 作成者はPG2年の矢野洋平です　疑問点や改善点、実装してほしい機能、バグ報告等あれば気軽にご連絡ください<br />
/// </summary>

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
// Config変数  //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    private const int    c_MaxSoundOverlap = 10;                     //オーディオの最大個数(BGM込み)
    private const string c_FolderPath_BGM = "Audio\\BGM\\Resources"; //BGMが格納されているフォルダのパス
    private const string c_FolderPath_SE = "Audio\\SE\\Resources";   //SEが格納されているフォルダのパス

    private const float c_SoundVolume = 0.5f;                        //SoundVolumeの初期値
    private const float c_FadeTimer = 1.0f;                          //FadeTimeのデフォルト値
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

    /// <summary>BGMの音量を徐々に変化させます   <br/>※フェード実行中フェード処理以外からBGMの音量が変化した場合フェード処理は停止されます。<br/>※渡された関数も呼ばれません </summary>
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

    public bool SetAllVolume();

    /// <summary>現在PlaySoundから再生されているサウンドの音量を変更します</summary>
    /// <param name="volume"></param>
    /// <param name="clipName">サウンドの指定(同じサウンドがあった場合はすべて変更されます)</param>
    /// <returns>音源が見つかった場合はtrueを返します</returns>
    public bool  SetVolume(string clipName,float volume) {
        bool check = GetUsingSource(out var sds);
            foreach(var sd in sds) {
                if (sd.Source.isPlaying) {
                  sd.Source.volume = volume;
                Log(sd.Source.clip + "の音量が" + volume + "に変更されます。", false);
                }
            }
            if (!check) {
                Log("指定された音源は再生されていませんでした。 : " + clipName, true);
            }
        return check;
    }

    /// <summary>BGMを除く再生されている音源の音量を徐々に変更します。</summary>
    /// <param name="clipName">音源指定(指定された音源が複数ある場合は全て変更されます)</param>
    /// <param name="volume">変更後の音量</param>
    /// <param name="fadeTime">フェード時間</param>
    /// <param name="action">フェード後に行う実行関数(必要な場合のみ) 
    /// <br/>※フェード実行中フェード処理以外からBGMの音量が変化した場合フェード処理は停止されます。※渡された関数も呼ばれません
    /// <br/>またフェードが完了する前に再生が終了した場合も呼ばれません。再生終了を取得したい場合はフェード時間を短くするか、PlaySoundに関数を渡しつつ実行し、SetVolumeFadeからフェードを実装してください。
    /// </param>
    /// <returns>再生されている音源が見つかった場合はtrueを返します</returns>
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
    /// <returns>再生時に割り振られたIDを取得します。再生が出来なかった場合はint.MinValueを返します</returns>
    public int PlaySound(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //現在音を鳴らせる状態か先に調べる
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return int.MinValue;
        }
        if (!GetNotUsedSource(out SoundData sd)) {
            return int.MinValue;
        }
        //音源を取得する
        bool check = ClipList_SE.TryGetValue(clipName, out AudioClip clip);
        //音源を取得できなかった場合はBGM音源から取得する
        if (!check) {
            check = ClipList_BGM.TryGetValue(clipName, out clip);
            //BGM音源からも取得できなかった場合はnullを返す
            if (!check) {
                GameSystem.LogError("音源ファイル名が間違っているかファイルが存在しません\nファイル名 : " + clipName);
                return int.MinValue;
            }
            Log("PlaySound関数からBGM音源が再生されます。", true);
        }
        //音量などの設定
        //デフォルト値であれば正しい値を与える
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

    /// <summary>指定されたサウンドを停止させます</summary>
    /// <param name="clipName">サウンド指定</param>
    /// <returns>指定されたサウンドが再生中であればtrueを返します</returns>
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

    /// <summary>再生されているサウンドを全て停止させます。</summary>
    /// <returns>呼び出し時点で再生されているサウンドがあった場合はtrueを返します。
    /// <br/>IsIncludingBGMがtrueの場合はBGMのみが再生されていた場合でもtrueを返します</returns>
    public bool StopAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (IsIncludingBGM) {
            check = m_BGMSource.isPlaying;
            BGM_Stop();
        }
        string str = check ? "BGM以外に再生されている音源はありませんでした。":null;     
        if (GetUsingSource(out var sds, str)) {
            foreach (var ad in sds) {
                ad.Source.Stop();
                check = true;
            }
        }
        return check;
    }

    /// <summary>PlaySoudから再生されたループ中のサウンドの再生をすべて停止します</summary>
    /// <returns>PlaySoundからループ再生中のサウンドがあった場合はtrueを返します</returns>
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
                Log("ループ再生されている音源はありませんでした。", true);
            }
        }
        return check;
    }

    /// <summary>ループ中ではないサウンドを全て停止させます。 </summary>
    /// <returns>ループではない再生中のサウンドがあった場合はtrueを返します</returns>
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
                Log("ループ再生されていない音源はありませんでした。", true);
            }
        }
        return check;
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

    /// <summary>再生されているサウンドを全てポーズします</summary>
    /// <param name="IsIncludingBGM">BGMも含めるかどうか</param>
    /// <returns>再生されているサウンドがあった場合trueを返します
    /// </br>IsIncludingBGMがtrueの場合はBGMのみが再生されていた場合もtrueを返します。</returns>
    public bool PauseAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (IsIncludingBGM) {
            check = m_BGMSource.isPlaying;
            BGM_Pause();
        }
        string str = check ? "BGM以外に再生されている音源はありませんでした。" : m_BGMSource.isPlaying ? "BGM以外に再生されている音源はありませんでした。(BGMはポーズされません)" : null;
        if (GetUsingSource(out var sds,str)) {
            check = true;
            foreach (var sd in sds) {
                sd.Source.Stop();
            }
        }
        return check;
    }

    /// <summary>全てのポーズされている音源を再開させます</summary>
    /// <param name="IsIncludingBGM">BGMも含めるかどうか</param>
    /// <returns>ポーズ中の音源があった場合はtrueを返します
    /// <br/>IsIncludingがtrueの場合はBGMのみがポーズ中だった場合でもtrueを返します
    /// <br/>ただしCanPlay(再生可能フラグ)がfalseだった場合はポーズ中の音源があった場合でもfalseを返します。</returns>
    public bool RestertAllSound(bool IsIncludingBGM) {
        bool check = false;
        if (!CanPlay) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return　check;
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
                Log("BGMはポーズ中ではありませんでした。",true);
            }
            return c2;
        }
        else if (check) {
            string str = IsIncludingBGM ? "BGMのみがポーズから再開されます":"BGM以外のポーズ中の音源はありません。(ポーズは解除されません)";
            Log(str, true);
        }
        else{
            Log("ポーズ中の音源はありませんでした。",true);
        }
        return check;
    }



    //  その他   //

    /// <summary>指定された音源を取得します</summary>
    /// <returns>音源情報を返します。見つからなかった場合はnullを返します。</returns>
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
            GameSystem.LogError("音源ファイル名が間違っているかファイルが存在しません\nファイル名 : " + clipName);
            return null;
        }
    }


    //現在再生されている音源数を取得します。


    /// <summary>(デバッグ用)今再生されている音源をすべてコンソール上に表示します</summary>    
    [Conditional("UNITY_EDITOR")]
    public void IsPlayingSoundLog(){
        if (m_BGMSource.isPlaying){
            Debug.Log("再生されているBGM : " + m_BGMSource.clip.name);
        }

         if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                Debug.Log("再生されているサウンド : " + sd.Source.clip.name);
            }
         }       
      }
      
    




    //  プライベート変数  //------------------------------------------------------------------------------------------------------------------------------

    //オーディオソースを管理する構造体
    private struct SoundData {
        public AudioSource Source;
        public int ID;
    }
    //使用するオーディオソース
    private SoundData[] m_SDatas;                       
    private AudioSource m_BGMSource = null;
    //音源リスト
    private Dictionary<string, AudioClip> ClipList_BGM;     //BGMのリスト
    private Dictionary<string, AudioClip> ClipList_SE;      //SEのリスト
    //その他
    private float m_defaltVolume = c_SoundVolume;           //デフォルトのvolume
    private float m_defaltTime = c_FadeTimer;               //デフォルトのフェード時間
    private int m_IDN = int.MinValue + 1;                   //ID配布用の番号
    [SerializeField, TooltipAttribute("再生される音源などをログに表示するかどうか(エラーメッセージは除く)")] private bool PlaySoundLog = true;
    //  プライベート関数  //------------------------------------------------------------------------------------------------------------------------------

    //  初期化関数   //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        //オーディオソースの作成----------------------------------------------------
        if (m_BGMSource == null) {
            m_BGMSource = gameObject.AddComponent<AudioSource>();
            m_SDatas = new SoundData[c_MaxSoundOverlap - 1];
            for (int i = 0; i < m_SDatas.Length; ++i) {
                m_SDatas[i].Source = gameObject.AddComponent<AudioSource>();
                m_SDatas[i].ID = int.MinValue;
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
    private bool GetNotUsedSource(out SoundData s)
    {
      for (int i = 0; i < m_SDatas.Length; ++i) {
          if (!m_SDatas[i].Source.isPlaying && m_SDatas[i].Source.time == 0) {
                m_SDatas[i].ID = GetID();
                s = m_SDatas[i];
                return true;
          }
      }
        Log("一度に鳴らせる音の最大数を超えています",true);
        s.ID = int.MinValue;
        s.Source = null;
        return false;
    }
    /// <summary>使われていないオーディオソースの取得(全て)</summary>
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

    /// <summary>使われているオーディオソースの取得</summary>
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
               noneLog = noneLog ?? "再生されている音源はありませんでした。";
                Log(noneLog,true);
            } 
        });

        return check;
    }

    /// <summary> フォルダのパスからフォルダ内の全ての音源をロードし、リストに追加する</summary>
    private void SetAudioClip(Dictionary<string,AudioClip> dic, string path) {

        path = Application.dataPath + "/" + path;
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

    /// <summary>サウンドを識別するIDを取得します(簡易的)</summary>
    private int GetID(){
        int id = m_IDN == int.MinValue ? int.MinValue + 1 : m_IDN;
        m_IDN++;
        return id;
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






