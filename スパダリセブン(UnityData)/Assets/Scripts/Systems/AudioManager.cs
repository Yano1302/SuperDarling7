using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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
    private const string c_FolderPath_BGM = "Audio\\BGM\\Resources"; //BGMが格納されているフォルダのパス
    private const string c_FolderPath_SE = "Audio\\SE\\Resources";   //SEが格納されているフォルダのパス
    private const int c_MaxSoundOverlap = 10;                    　　//オーディオの最大個数(BGM込み)s
    private const int c_SoundScale = 10;                             //音の大きさを何段階に分けるか
    private const float  c_DfaultSoundVolume = 0.5f;                 //SoundVolumeの初期値
    private const float  c_DefaultFadeTime = 2.0f;                  //FadeTimeのデフォルト値
   
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//


//  変数・関数一覧    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //  設定など    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>音楽が再生可能かどうかのフラグです。※ただし、falseにした時に既に再生されている音楽は停止されません</summary>
    public bool CanPlayFlag { get { return m_canPlayFlag; } set { Log("再生可能フラグが " + value + " に変更されます",false); m_canPlayFlag = value; } }

    /// <summary>フェードにかける時間のデフォルト値を取得・変更します</summary>
    public float DefaultFadeTime { get { return m_defaltTime; } set { m_defaltTime = value; } }

    /// <summary>サウンドの音量のデフォルト値を設定・取得します</summary>
    public float DefaultVolume { get { return m_defaltVolume; } set { m_defaltVolume = value; } }

    /// <summary>サウンドの出力割合を設定・取得します。</summary>
    public int CurrentScale { get; set; }　//TODO

    //  音量設定    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
//BGM
    /// <summary>現在のBGMの音量を設定・取得します。</summary>
    public float BGM_Volume {
        get {
            if (!m_BGMData.Source.isPlaying) { Log("BGMは再生されていません。", true); } 
            return m_BGMData.Source.volume;
        }
        set {
            if (!m_BGMData.Source.isPlaying) { Log("BGMは再生されていません。再生時に音量設定が上書きされる恐れがあります。", true); } else { Log("BGMの音量が" + value + "に変更されます。", false); }
            m_BGMData.Source.volume = value; 
        }
    }

    /// <summary>BGMの音量を徐々に変化させます。BGMが再生されていなかった場合には処理が行われません。
    /// <br/>※また、フェード処理中に新しくフェード処理が呼ばれた場合は上書きされます。 </summary>
    /// <param name="volume">設定する音量</param>
    /// <param name="fadeTime">設定した音量になるまでの時間(秒)　※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="action">フェード処理後に行う実行関数
    /// <br/>※フェードが上書きされたり、フェード途中でBGMが停止された場合には呼ばれません。
    /// </param>
    /// <returns>呼び出し時にBGMが再生されていればtrueを返します。</returns>
    public bool BGM_FadeVolume(float volume, float fadeTime = float.NaN, UnityAction action = null) {
        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData.Source.clip.name + "(BGM)の音量が徐々に変更されます。 \n 変更後の音量 : " + volume, false);
            StartCoroutine(FadeSound(m_BGMData, fadeTime, volume, action, false,false));
            return true;
        }
        else {
            Log("BGMが再生されていないので音量を変更する事ができません。", true);
            return false;
        }
    }


//SE
    /// <summary>再生されているSEの音量を変更します。(名称指定)指定された音源と同じ音源のSEが複数ある場合は全て変更されます。</summary>
    /// <param name="volume">変更後の音量</param>
    /// <param name="clipName">サウンドの指定(同じサウンドがあった場合はすべて変更されます)</param>
    /// <returns>音源が見つかった場合はtrueを返します</returns>
    public bool SE_Volume(string clipName,float volume) {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {               
                if (sd.Source.isPlaying && clipName == sd.Source.clip.name) {
                    sd.Source.volume = volume;
                    Log(sd.Source.clip + "の音量が" + volume + "に変更されます。", false);
                    check = true;
                }
                
            }
            if (!check) {
                Log("指定された音源名のSEは再生されていませんでした。\n指定された音源名 : "+clipName,true);
            }
        }
        return check;
    }
    /// <summary>再生されているSEの音量を変更します。(ID指定)</summary>
    /// <param name="id">変更したいSEのID</param>
    /// <param name="volume">変更後の音量</param>
    /// <returns>指定されたIDのSEがあった場合はtrueを返します。</returns>
     public bool SE_Volume(int id, float volume) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.Source.isPlaying) {
                    sd.Source.volume = volume;
                    Log(sd.Source.clip + "の音量が" + volume + "に変更されます。", false);
                    return true;
                }
            }
        }
        Log("指定されたIDのSEは再生されていませんでした。\nID : " + id, true);
        return false;
    }

    /// <summary>再生されているSEの音量を徐々に変更します。(名称指定)指定された音源と同じ音源のSEが複数ある場合は全て変更されます。
    /// <br/>指定されたSEが再生されていなかった場合は処理が実行されません。
    /// <br/>また、指定されたサウンドが既にフェード処理中であればフェード処理が上書きされます。</summary>
    /// <param name="clipName">音源指定(指定された音源が複数ある場合は全て変更されます)</param>
    /// <param name="volume">変更後の音量</param>
    /// <param name="fadeTime">フェード時間</param>
    /// <param name="action">フェード後に行う実行関数(必要な場合のみ) <br/>※フェードが上書きされた場合には呼ばれません。</param>
    /// <param name="StopAction">フェード完了前に再生が停止した場合にactionを実行するかどうか</param>
    /// <returns>再生されている音源が見つかった場合はtrueを返します</returns>
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
                Log("指定されたSEがみつかりませんでした。" + "\n音源名 : " + clipName + "\n", true);
            }
        }
        return check;
    }
    /// <summary>再生されているSEの音量を徐々に変更します。(ID指定)
    /// <br/>指定されたSEが再生されていなかった場合は処理が実行されません。
    /// <br/>また、指定されたサウンドが既にフェード処理中であればフェード処理が上書きされます。</summary>
    /// <param name="id">変更したいSEのID</param>
    /// <param name="volume">変更後の音量</param>
    /// <param name="fadeTime">フェード時間</param>
    /// <param name="action">フェード後に行う実行関数(必要な場合のみ) <br/>※フェードが上書きされた場合には呼ばれません。</param>
    /// <param name="StopAction">フェード完了前に再生が停止した場合にactionを実行するかどうか</param>
    /// <returns>再生されている音源が見つかった場合はtrueを返します</returns>
    public bool SE_SetVolumeFade(int id,float volume, float fadeTime = float.NaN, UnityAction action = null, bool StopAction = false) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (id == sd.ID) {
                    StartCoroutine(FadeSound(sd, fadeTime, volume, action, false, StopAction));
                    return true;
                }
            }
        }
        Log("指定されたSEがみつかりませんでした。" + "\nID : " + id + "\n", true);
        return false;
    }

//ALL
    /// <summary>再生中の全てのサウンドの音量を変更します。</summary>
    /// <param name="volume">変更後の音量</param>
    /// <param name="IncludingBGM">BGMを含めるかどうか</param>
    public void ALL_SetVolume(float volume, bool IncludingBGM) {
        if (IncludingBGM) {
            BGM_Volume = volume;
        }

        if (GetUsingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                if (sd.Source.isPlaying) {
                    sd.Source.volume = volume;
                    Log(sd.Source.volume + "(SE)の音量が変更されます。 \n 変更後の音量 : " + volume, false);
                }
            }
        }
        else {
            Log("SEは再生されていませんでした。", true);
        }
    }

    /// <summary>再生中の全てのサウンドの音量をフェード変更します。サウンドが再生されていなかった場合には処理は行われません。</summary>
    /// <param name="volume">フェード後の音量</param>
    /// <param name="IncludingBGM">BGMも対象にするか</param>
    /// <param name="fadeTime">フェード時間</param>
    /// <param name="action">フェード後の関数呼び出し</param>
    /// <param name="StopAction">IncludingBGMがfalseの際に全てのSEがフェード前に再生終了した場合にactionを起動するかどうか</param>
    /// <returns>フェード変更を行う音源がある場合にtrueを返します。</returns>
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
                //残り秒数が一番長いSEを探す
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
                //残り秒数が一番長いSEにactionを渡す
                StartCoroutine(FadeSound(restSd, fadeTime, volume, action, false, StopAction));
            }  
        }
        return check;
    }



    //  再生関数    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //BGM
    /// <summary>BGMを再生します。既にBGMが再生されていた場合はそのBGMは停止されます。</summary>
    /// <param name="clipName">鳴らしたい音源名(SE音源も再生可能です)</param>
    /// <param name="volume">音量(０～１)　※指定しない場合はSoundVolumeが使用されます</param>
    public void BGM_Play(string clipName, float volume = float.NaN) {
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }
        if(m_BGMData.Source.isPlaying || m_BGMData.Source.time != 0) {
            Log("既に再生されているBGMが上書きされます。", true);
            m_BGMData.Source.Stop();
        }
        //音源を取得する
        if (GetClip(out AudioClip clip, clipName, true)) {
            //デフォルト値であれば正しい値を与える
            m_BGMData.Source.volume = float.IsNaN(volume) ? m_defaltVolume : volume;
            m_BGMData.Source.clip = clip;
            m_BGMData.Source.loop = true;
            m_BGMData.Source.Play();
            Log(clip, true, true, m_BGMData.ID);
        }
        else {
            Debug.Assert(clip != null, "音源ファイル名が間違っているかファイルが存在しません\nファイル名 : " + clipName);
            return;
        }
        
       
       
    }

    /// <summary>BGMをフェードイン再生させます。既にBGMが再生されている場合、そのBGMは停止されます。
    /// <br/>また、BGMが既にフェード処理中であった場合、フェード処理が上書きされます。</summary>
    /// <param name="clipName">鳴らしたい音源ファイル名</param>
    /// <param name="fadeTime">フェードイン(endVolume)になるまでの時間(秒)　※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="startVolume">初めの音の大きさ(０～１)</param>
    /// <param name="endVolume">終わりの音の大きさ(０～１) ※指定しない場合はSoundVolumeが使用されます</param>
    /// <param name="action">フェード後に行う実行関数 <br/>※フェード途中で音源が停止された場合やフェードが上書きされた場合には呼ばれません</param>
    public void BGM_PlayFade(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, UnityAction action = null) {
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }
        m_BGMData.Source.volume = startVolume;
        BGM_Play(clipName, startVolume);
        StartCoroutine(FadeSound(m_BGMData, fadeTime, endVolume, action, false,false));
    }

//SE
    /// <summary>SEを再生します。</summary>
    /// <param name="clipName">鳴らしたい音源名(BGM音源も再生可能です)</param>
    /// <param name="volume">音量(０～１) ※指定しない場合はSoundVolumeが使用されます</param>
    /// <param name="IsLoop">ループ再生をする場合はture</param>
    /// <param name="action">再生された音源が再生終了または停止された場合に関数を実行します。
    /// <br/>※また、ループが有効な場合はループ再生が停止されるまで関数は呼ばれません。</param>
    /// <returns>再生時に割り振られた識別用のIDのようなものを取得します。再生が出来なかった場合はint.MinValueを返します。</returns>
    public int SE_Play(string clipName, float volume = float.NaN, bool IsLoop = false, UnityAction action = null) {
        //現在音を鳴らせる状態か先に調べる
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return int.MinValue;
        }
        if (!GetNotUsedSource(out SoundData sd)) {
            return int.MinValue;
        }
        if (GetClip(out var clip, clipName, false)) {
            //音量などの設定
            //デフォルト値であれば正しい値を与える
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

    /// <summary>BGMをフェードイン再生させます。既にBGMが再生されている場合、そのBGMは停止されます。
    /// <br/>また、BGMが既にフェード処理中であった場合、フェード処理が上書きされます。</summary>
    /// <param name="clipName">鳴らしたい音源ファイル名</param>
    /// <param name="fadeTime">フェードイン(endVolume)になるまでの時間(秒)　※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="startVolume">初めの音の大きさ(０～１)</param>
    /// <param name="endVolume">終わりの音の大きさ(０～１) ※指定しない場合はSoundVolumeが使用されます</param>
    /// <param name="action">フェード後に行う実行関数 <br/>※フェード途中で音源が停止された場合やフェードが上書きされた場合には呼ばれません</param>
    /// <returns>再生時に割り振られた識別用のIDのようなものを取得します。再生が出来なかった場合はint.MinValueを返します</returns>
    public int SE_PlayFade(string clipName, float fadeTime = float.NaN, float startVolume = 0.1f, float endVolume = float.NaN, bool IsLoop = false,UnityAction action = null) {
        //現在音を鳴らせる状態か先に調べる
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return int.MinValue;
        }
        if (!GetNotUsedSource(out SoundData sd)) {
            return int.MinValue;
        }
        if (GetClip(out var clip, clipName, false)) {
            //音量などの設定
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

    //  停止関数    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

//BGM
    /// <summary>BGMを停止させます</summary>
    public void BGM_Stop() {
        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData.Source.clip, false, true,m_BGMData.ID);
        }
        else if (m_BGMData.Source.time != 0) {
            Log("ポーズ中のBGMが停止されます。",true);
        }
        else {
            Log("BGMは再生されていませんでした", true);
        }
        m_BGMData.Source.Stop();
    }

    /// <summary>BGMをフェードアウトさせ停止します。 <br/>BGMが既にフェード処理中であった場合、フェード処理が上書きされます。</summary>
    /// <param name="fadeTime">フェードアウトまでの時間(秒) ※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="action">フェードアウト後に行う実行関数(行う必要がない場合はnullを渡してください)</param>
    /// <param name="endVolume">停止する直前の音量(０～１)</param>
    /// <param name="action">フェード後に行う実行関数
    /// <br/>※フェードが上書きされたり、フェード途中でBGMが停止された場合には呼ばれません。</param>
    public void BGM_StopFade(float fadeTime = float.NaN, float endVolume = 0.05f, UnityAction action = null) {

        if (m_BGMData.Source.isPlaying) {
            Log(m_BGMData.Source.clip, false,true,m_BGMData.ID);
            StartCoroutine(FadeSound(m_BGMData, fadeTime, endVolume, action, true,false));
        }
        else {
            Log("BGMは再生されていませんでした", true);
            return;
        }

    }


//SE
    /// <summary>指定されたサウンドを停止させます。</summary>
    /// <param name="clipName">サウンド名称指定</param>
    /// <returns>呼び出し時に指定されたSEが再生中であればtrueを返します</returns>
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
            Log("指定された音源は再生されていませんでした。　音源名 : " + clipName, true);
        }
        return check;
    }
    /// <summary>指定されたサウンドを停止させます</summary>
    /// <param name="id">サウンドID指定</param>
    /// <returns>呼び出し時に指定されたIDのSEが再生中であればtrueを返します</returns>
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
        Log("指定された音源は再生されていませんでした。　ID : " + id, true);
        return false;
    }

    /// <summary>SEをフェードアウトさせ停止します。指定されたSEがなかった場合には処理は実行されません。
    /// <br/>指定されたSE音源が複数再生されていた場合は全て停止されます。
    /// <br/>SEが既にフェード処理中であった場合、フェード処理が上書きされます。</summary>
    /// <param name="clipName">サウンド名称指定</param>
    /// <param name="fadeTime">フェードアウトまでの時間(秒) ※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="action">フェードアウト後に行う実行関数(行う必要がない場合はnullを渡してください)</param>
    /// <param name="endVolume">停止する直前の音量(０～１)</param>
    /// <param name="action">フェード後に行う実行関数<br/>※フェードが上書きされた場合には呼び出されません。</param>
    /// <param name="StopAction">フェード完了前に再生が停止した場合にactionを実行するかどうか</param>
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
            Log("指定された音源は再生されていませんでした。　音源名 : "+clipName, true);
        }
        return check;
    }
    /// <summary>SEをフェードアウトさせ停止します。指定されたSEがなかった場合には処理は実行されません。
    /// <br/>SEが既にフェード処理中であった場合、フェード処理が上書きされます。</summary>
    /// <param name="id">サウンドID指定</param>
    /// <param name="fadeTime">フェードアウトまでの時間(秒) ※指定しない場合はFadeTimeが使用されます</param>
    /// <param name="action">フェードアウト後に行う実行関数(行う必要がない場合はnullを渡してください)</param>
    /// <param name="endVolume">停止する直前の音量(０～１)</param>
    /// <param name="action">フェード後に行う実行関数<br/>※フェードが上書きされた場合には呼び出されません。</param>
    /// <param name="StopAction">フェード完了前に再生が停止した場合にactionを実行するかどうか</param>
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
        Log("指定された音源は再生されていませんでした。　ID : " + id, true);
        return false;
    }

    /// <summary>ループ再生中のSEをすべて停止します</summary>
    /// <returns>ループ再生中のSEがあった場合はtrueを返します</returns>
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
                Log("ループ再生されているSEはありませんでした。", true);
            }
        }
        return check;
    }

    /// <summary>ループ中ではない再生中のSEを全て停止させます。 </summary>
    /// <returns>ループではない再生中のSEがあった場合はtrueを返します</returns>
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
                Log("ループ再生以外の再生されているSEはありませんでした。", true);
            }
        }
        return check;
    }


//ALL
    /// <summary>再生されているサウンドを全て停止させます。</summary>
    /// <returns>呼び出し時点で再生されているサウンドがあった場合はtrueを返します。
    /// <br/>IsIncludingBGMがtrueの場合はBGMのみが再生されていた場合でもtrueを返します</returns>
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

    /// <summary>再生中の全ての音源をフェード停止させます。</summary>
    /// <param name="IncludingBGM">BGMも対象にするか</param>
    /// <param name="fadeTime">フェード時間</param>
    /// <param name="action">フェード後の関数呼び出し</param>
    /// <param name="StopAction">IncludingBGMがfalseの際に全てのSEがフェード前に再生終了した場合にactionを起動するかどうか</param>
    /// <returns>フェード終了する音源があった場合にはtrueを返します。</returns>
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
                //残り秒数が一番長いSEを探す
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
                //残り秒数が一番長いSEにactionを渡す
                StartCoroutine(FadeSound(restSd, fadeTime,endVolume, action, true, StopAction));
            }
        }
        return check;
    }

   
    //  一時停止・再開   //---------------------------------------------------------------------------------------------------------------------------------------------------------------

//BGM
    /// <summary>BGMをポーズします</summary>
    public void BGM_Pause() {
        if (m_BGMData.Source.isPlaying) {
            m_BGMData.Source.Pause();
            Log("BGMがポーズされます", false);
        }
        else {
            Log("BGMは再生されていません", true);
        }

    }

    /// <summary>ポーズ中のBGMを再開します</summary>
    public void BGM_Restert() {
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }

        if (!m_BGMData.Source.isPlaying && m_BGMData.Source.time != 0) {
            m_BGMData.Source.UnPause();
            Log("BGMのポーズ状態が解除されます", false);
        }
        else {
            Log("BGMはポーズされていません。",true);
        }

    }

//SE
    /// <summary>指定されたSEをポーズします。同じ音源のSEが複数ある場合には全てポーズされます。</summary>
    /// <param name="clipName">音源名称指定</param>
    /// <returns>指定された音源が再生されていた場合はtrueを返します</returns>
    public bool SE_Pause(string clipName) {
        bool check = false;
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if(sd.Source.clip.name == clipName) {
                    check = true;
                    Log("\n" + clipName + "がポーズされます。" + "\nID : " + sd.ID, false);
                    sd.Source.Pause();
                }
            }
        }
        if(!check) Log("指定された音源名のSEは再生されていませんでした。\n音源名 : " + clipName,true);
        return check;
    }
    /// <summary>指定されたSEをポーズします。</summary>
    /// <param name="clipName">音源ID指定</param>
    /// <returns>指定された音源が再生されていた場合はtrueを返します</returns>
    public bool SE_Pause(int id) {
        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                if (sd.ID == id) {
                    Log("\n" + sd.Source.clip.name + "がポーズされます。" + "\nID : " + sd.ID, false);
                    sd.Source.Pause();
                    return true;
                }
            }
        }
        Log("指定されたIDのSEは再生されていませんでした。\nID : " + id, true);
        return false;
    }

    /// <summary>指定された音源名のSEをポーズ解除します。</summary>
    /// <param name="clipName">音源名称指定</param>
    /// <returns>ポーズを解除した音源があればtrueを返します。</returns>
    public bool SE_Restert(string clipName) {
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return false;
        }
        bool check = false;
        if (GetPausingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                if (sd.Source.clip.name == clipName) {
                    check = true;
                    Log("\n" + clipName + "のポーズが解除されます。" + "\nID : " + sd.ID, false);
                    sd.Source.UnPause();
                }
            }
        }
        if (!check)Log("指定された音源名のSEはポーズされていませんでした。\n音源名 : " + clipName, true);
        return check;
    }
    /// <summary>指定された音源名のSEをポーズ解除します。</summary>
    /// <param name="id">音源ID指定</param>
    /// <returns>ポーズを解除した音源があればtrueを返します。</returns>
    public bool SE_Restert(int id) {
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return false;
        }
        if (GetPausingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                if (sd.ID == id) {
                    Log("\n" + sd.Source.clip.name + "のポーズが解除されます。" + "\nID : " + sd.ID, false);
                    sd.Source.UnPause();
                    return true;
                }
            }
        }
        Log("指定されたIDのSEはポーズされていませんでした。    ID : " + id, true);
        return false;
    }


//ALL
    /// <summary>再生されているサウンドを全てポーズします</summary>
    /// <param name="IsIncludingBGM">BGMも含めるかどうか</param>
    public void ALL_Pause(bool IsIncludingBGM) {
        if (IsIncludingBGM) {
            BGM_Pause();
        }

        if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                sd.Source.Pause();
                Log(sd.Source.clip.name + "(SE)がポーズされます", false);
            }
        }
    }

    /// <summary>全てのポーズされている音源を再開させます</summary>
    /// <param name="IsIncludingBGM">BGMも含めるかどうか</param>
    public void ALL_Restert(bool IsIncludingBGM) {
        if (!CanPlayFlag) {
            Log("現在再生フラグがオフになっているため、再生できません。", true);
            return;
        }

        if (IsIncludingBGM) {
            BGM_Restert();
        }

        if (GetPausingSource(out List<SoundData> sds)) {
            foreach (var sd in sds) {
                    Log(sd.Source.clip, true, false, sd.ID);
                    sd.Source.UnPause();
                    Log(sd.Source.clip.name + "(SE)がポーズ解除されます", false);               
            }
        }
    }


    //  その他   //---------------------------------------------------------------------------------------------------------------

    /// <summary>指定された音源を取得します</summary>
    /// <returns>音源情報を返します。見つからなかった場合はnullを返します。</returns>
    public AudioClip GetAudioClip(string clipName) { GetClip(out var clip, clipName, true); return clip;}

    //現在再生されている音源数を取得します。TODO


    /// <summary>(デバッグ用)今再生されている音源とそのIDをすべてコンソール上に表示します</summary>    
    [Conditional("UNITY_EDITOR")]
    public void IsPlayingSoundLog(){
        if (m_BGMData.Source.isPlaying){
            Debug.Log("再生されているBGM : " + m_BGMData.Source.clip.name);
        }

         if (GetUsingSource(out var sds)) {
            foreach (var sd in sds) {
                Debug.Log("再生されているサウンド : " + sd.Source.clip.name +"使用しているID : "+sd.ID);
            }
         }       
      }
      
    




    //  プライベート変数  //------------------------------------------------------------------------------------------------------------------------------

    //オーディオソースを管理するクラス
    private class SoundData {
        public AudioSource Source;
        public int ID;
        public FadeState fadeState;
    }
    //使用するオーディオソース
    private SoundData[] m_SEDatas;
    private SoundData m_BGMData;
    //音源リスト
    private static Dictionary<string, AudioClip> ClipList_BGM;     //BGMのリスト
    private static Dictionary<string, AudioClip> ClipList_SE;      //SEのリスト
    //その他
    private float m_defaltVolume;                                  //デフォルトのvolume
    private float m_defaltTime;                                    //デフォルトのフェード時間
    private int m_IDN = 1;                                         //ID配布用の番号
    private bool m_canPlayFlag = true;                             //再生可能フラグ

    [SerializeField, TooltipAttribute("再生される音源などをログに表示するかどうか(エラーメッセージは除く)")] private bool PlaySoundLog = true;
    //フェード状態を管理する列挙型
    private enum FadeState {
        None,
        Fading,
        Pause,
    }
    //  プライベート関数  //------------------------------------------------------------------------------------------------------------------------------

    //  初期化関数   //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        //オーディオソースの作成----------------------------------------------------
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
            //ファイルの読み込み----------------------------------------------------
            ClipList_BGM = new Dictionary<string, AudioClip>();
            ClipList_SE = new Dictionary<string, AudioClip>();
            SetAudioClip(ClipList_BGM,c_FolderPath_BGM);
            SetAudioClip(ClipList_SE,c_FolderPath_SE);
            //デフォルト値の格納----------------------------------------------------
            m_defaltVolume = c_DfaultSoundVolume;
            m_defaltTime = c_DefaultFadeTime;
        }
        //--------------------------------------------------------------------------
    }

    //  実行関数    //-----------------------------------------------------------------------------

    /// <summary>使われていないオーディオソースの取得</summary>
    private bool GetNotUsedSource(out SoundData s)
    {
      for (int i = 0; i < m_SEDatas.Length; ++i) {
          if (!m_SEDatas[i].Source.isPlaying && m_SEDatas[i].Source.time == 0) {
                m_SEDatas[i].ID = GetID();
                s = m_SEDatas[i];
                return true;
          }
      }
        Log("一度に鳴らせる音の最大数を超えています",true);
        s = null;
        return false;
    }

    /// <summary>ポーズ中のオーディオソースの取得</summary>
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

    /// <summary>使われているオーディオソースの取得</summary>
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
               noneLog = noneLog ?? "再生されているSEはありませんでした。";
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
    private IEnumerator FadeSound(SoundData s,float time,float ev, UnityAction action, bool stop,bool stopToAction) {

        //デフォルト値であれば正しい値に変更する
        time = float.IsNaN(time) ? m_defaltTime : time;
        ev = float.IsNaN(ev) ? m_defaltVolume : ev;

        //フェードが重複している場合は上書きする
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
            //ポーズor再生停止になっていないか確認する
            if (!s.Source.isPlaying) {
                if(s.Source.time == 0) {
                    Log("フェードが終わる前にに再生が停止されました。", true);
                    //強制終了時に関数を起動しない設定の場合はnullにする
                    action = stopToAction ?  action : null;
                    break;
                }
            }
            //フェードが上書きされてないかチェックする
            else if (s.fadeState == FadeState.None) {
                Log("フェード中に新しくフェード処理が呼ばれました。フェード処理が上書きされます。", true);
                break;
            }
            //全ての条件を満たした場合のみフェードを進める
            else if(s.fadeState == FadeState.Fading){
                s.Source.volume = Mathf.SmoothStep(sv, ev, t);
                t += Time.deltaTime / time;
            }         
            //ループを抜ける条件
            if (t >= 1) {
                if (s.Source.isPlaying) {
                    Log(s.Source.clip.name + "のフェードが完了しました", false);
                    s.Source.volume = ev;
                    if (stop) {
                        s.Source.Stop();
                    }
                }
                break;
            }
          
            //問題なければ次のループまで1フレーム待機する
            yield return null;
        }

        //フェードが上書きされてない場合のみ呼ばれる
        if (s.fadeState != FadeState.None) {
            s.fadeState = s.fadeState == FadeState.Fading ? FadeState.None : s.fadeState;
            action?.Invoke();
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

    /// <summary>サウンドを識別するIDのようなものを取得します</summary>
    private int GetID(){
        int id = m_IDN == int.MaxValue ? 1 : m_IDN;
        m_IDN = id + 1;
        return id;
    }

    /// <summary>指定されたサウンドがあるか調べ、見つかった場合は取得します/summary>
    private bool GetClip(out AudioClip clip,string clipName,bool IsPrioritizeBGM) {
        //音源を取得する
        clip = null;
        bool check = IsPrioritizeBGM? ClipList_BGM.TryGetValue(clipName, out clip):ClipList_SE.TryGetValue(clipName, out clip);
        //音源を取得できなかった場合はBGM音源から取得する
        if (!check) {
            check = !IsPrioritizeBGM ? ClipList_BGM.TryGetValue(clipName, out clip) : ClipList_SE.TryGetValue(clipName, out clip);
            //BGM音源からも取得できなかった場合はnullを返す
            if (!check) {
                GameSystem.LogError("音源ファイル名が間違っているかファイルが存在しません\nファイル名 : " + clipName);
            }
#if UNITY_EDITOR
            string str = IsPrioritizeBGM ? "SE音源がBGMとして使用されます。" : "BGM音源がSEとして使用されます。";
            Log(str, true);
#endif
        }
        return check;
    }

    //  デバッグ用   //----------------------------------------------------------------------------

    /// <summary>ログの表示が有効な場合に音源名をログに表示する</summary>
    [Conditional("UNITY_EDITOR")]
    private void Log(AudioClip clip,bool playing,bool BGM,int ID) { 
        if (PlaySoundLog) {
            string type = BGM ? "(BGM)" : "(SE)";
            string id = ID != 0 ? "<color=cyan>ID : " + ID + "</color>" : "";
            string str = playing ? "\n<color=cyan>" + clip.name + type + "</color>" + "が再生されます    " + id : "color=cyan>" + clip.name + type + "</color>" + "が停止されます" + id;
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






