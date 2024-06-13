using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SceneManager = Supadari.SceneManager;

//各調査パートを管理するクラスオブジェクト
public class InvPart : MonoBehaviour {
    // MapManagerから格納されます--------------------------------------------------------------------------------
    /// <summary>自身のInvPartを取得します。設定はMapManagerからSetUpInvPart経由で格納されます</summary>
    public InvType InvPartType { get { return m_InvType; } }
    /// <summary>このInvPartを呼び出すゴールオブジェクトを設定します。MapManagerから呼ばれます</summary>
    public void SetGoal(Goal g) {m_goalList ??= new List<Goal>();  m_goalList.Add(g);}
  
    //------------------------------------------------------------------------------------------------------------

    /// <summary>このパートがクリアされているか</summary>
    public bool ClearFlag { get; private set; } = false;

    /// <summary>自身の調査パートの種類をInvManagerから設定します</summary>---------------------------------------
    public void SetUpInvPart(InvType type, GameObject itemCol, InvGauge invGage, GameObject warning) {
        //自身の名前を調査パート名に変更
        gameObject.name = type.ToString();
        //設定されたInvTypeから自身のテクスチャを取得する
        m_img = GetComponent<Image>();
        m_img.sprite = InvTextureholder.Instance.GetSprite(type);
        m_img.enabled = false;
        //その他必要な情報を格納する
        m_InvType = type;
        m_InvGauge = invGage;
        m_warningMessage = warning;
        //設定されたInvTypeから対応するCSVファイルを読み込み、必要な情報を格納する
        InitialiseInvPart(itemCol);
    }

   
    /// <summary>この調査パートを開く処理を行います</summary>
    public void Open() {
        //マウス座標を初期化する
        m_vig.MouseVec = Input.mousePosition;
        //フラグ設定
        m_InvGauge.OpenGauge(m_vig.Rate);
        m_img.enabled = true;
        foreach (var i in m_itemObj) {
            if (i != null) {
                i.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>この調査パートを閉じる処理を行います</summary>
    public void Close() {
        foreach (var i in m_itemObj) {
            if (i != null) {
                i.gameObject.SetActive(false);
            }
        }
    }

    //現在の警戒度を０にします
    public void ResetVigilance() {
        m_vig.CurrentVigilance = 0;
        m_InvGauge.ResetGauge();
    }

    //-------------------------------------------------------------------------------------------------------------

    /// <summary>残りのアイテムの個数を確認し、全て取得している場合にItemManagerに報告します</summary>
    public void CheckItemNum() {
       if(transform.childCount == 0) {
            ClearFlag = true;                                     //クリアフラグを立てる
            foreach (var g in m_goalList) Destroy(g.gameObject); //このInvPartを呼び出すオブジェクトを全て破棄する
            InvManager.Instance.CheckClear();                   //ItemManagerに全体のクリアを確認させる
        }
    }

    /// <summary>構造体の初期化とアイテム配置を行います</summary>
    private void InitialiseInvPart(GameObject itemCol) {
        //以下初期化
        CSVSetting data = new CSVSetting(m_InvType.ToString());
        m_itemObj = new List<ItemObject>();
        m_getItemCount = 0;
        //最大値を設定する
        data.GetData((int)InvDataCSVIndex.MaxVigilance, 1, out float max);
        m_vig.MaxVigilance = max;
        //警戒度を初期化する
        data.GetData((int)InvDataCSVIndex.InitialiseVigilance, 1, out float initial);
        m_vig.CurrentVigilance = initial;
        //時間経過による上昇量を設定する
        data.GetData((int)InvDataCSVIndex.AddTimeValue, 1, out float time);
        m_vig.AddTimeVigilance = time;
        //マウス操作による上昇量を設定する
        data.GetData((int)InvDataCSVIndex.AddMoseValue, 1, out float mouse);
        m_vig.AddMouseVigilance = mouse;
        //猶予時間を設定する
        data.GetData((int)InvDataCSVIndex.GraceTime, 1, out float grace);
        m_vig.GraceTime = grace;
        //アイテムオブジェクトを生成する
        int length = data.GetLength(1);
        var itemManager = ItemManager.Instance;
        for (int i = (int)InvDataCSVIndex.ItemStart; i < length; i++) {
            if (!data.GetData(i, 1, out string name)) {
                break;
            }
            //アイテムを生成する
            var obj = Instantiate(itemCol);
            //アイテムをInvPartの子にする
            obj.transform.SetParent(transform);
            //生成したアイテムオブジェクトを取得する
            var itemObj = obj.AddComponent<ItemObject>();
            //ID等を割り振る
            var id = itemManager.GetItemID(name);
            itemObj.ID = id;
            itemObj.name = name;
            m_itemObj.Add(itemObj);
            //座標を設定する
            var rect = obj.GetComponent<RectTransform>();
            data.GetData(i, 2, out float x);
            data.GetData(i, 3, out float y);
            Vector2 vec = new Vector2(x, y);
            rect.position = vec;
        }
    }

    /// <summary>警戒度を管理する構造体</summary>
    private struct Vigilance {
        public InvManager invManager;            //InvManagerインスタンス
        public float MaxVigilance;               // 最大警戒度
        public float CurrentVigilance;           // 現在の警戒度  
        public float AddTimeVigilance;           //時間経過による上昇量
        public float AddMouseVigilance;          //マウスによる上昇量
        public Vector2 MouseVec;                 //マウス座標を保存します  
        public float GraceTime;                 // 警戒度最大時に動いても良い猶予時間 岬追記
        public float ReprieveGameOver;          // 上記を判定する為の変数　岬追記

        /// <summary>現在の警戒度の割合を取得します</summary>
        public float Rate { get { return CurrentVigilance / MaxVigilance; } }
        /// <summary>現在の警戒度が最大警戒度以上の場合にtrueを返します</summary>
        public bool IsOver { get { return CurrentVigilance >= MaxVigilance; } }
        //警戒度上昇フラグ
        public bool VigilanceFlag { get { invManager ??= InvManager.Instance; return invManager; } }

    }

    //このパートにあるアイテムオブジェクト
    private List<ItemObject> m_itemObj;
    //自身のImg情報
    private Image m_img;
    //警戒度用構造体
    private Vigilance m_vig;
    //調査パートの種類
    private InvType m_InvType;
    //警戒度ゲージ
    private InvGauge m_InvGauge;
    //危険メッセージ
    private GameObject m_warningMessage;
    //取得しているアイテムの数
    private int m_getItemCount;
    //このパートを呼び出すゴールオブジェクト一覧
    private List<Goal> m_goalList;



    //Update関数
    private void Update() {
        if (m_vig.VigilanceFlag) {
            AddVigilance(m_vig.AddTimeVigilance * Time.deltaTime);
            CheckMouseMove();
        }
    }


    /// <summary>マウスが動いたかどうかを調べます</summary>
    /// <returns>ゲームオーバーの場合にはfalseを返します</returns>
    private void CheckMouseMove() {
        Vector2 pos = Input.mousePosition;
        //マウスが動かされている場合
        if (m_vig.MouseVec != pos) {
            //最大警戒度を超えている場合の処理
            if (m_vig.IsOver) {
                m_vig.ReprieveGameOver += Time.deltaTime;
                if (m_vig.ReprieveGameOver > m_vig.GraceTime) OverVigilance();
            }
            else {
                m_vig.MouseVec = pos;
                AddVigilance(m_vig.AddMouseVigilance * Time.deltaTime);
            }
        }
    }

    /// <summary>警戒度を追加します</summary>
    /// <returns>警戒度が最大値以上の場合にtrueを返します</returns>
    private bool AddVigilance(float addValue) {
        m_vig.CurrentVigilance += m_vig.IsOver ? 0 : addValue;
        m_InvGauge.SetRate(m_vig.Rate);
        m_warningMessage.SetActive(m_vig.IsOver);
        return m_vig.IsOver;
    }

    /// <summary>警戒度を設定します</summary>
    /// <returns>警戒度が最大値以上の場合にtrueを返します</returns>
    private bool SetVigilance(float value) {
        m_vig.CurrentVigilance = value;
        m_InvGauge.SetRate(m_vig.Rate);
        m_warningMessage.SetActive(m_vig.IsOver);
        return m_vig.IsOver;
    }

    /// <summary>警戒度がマックスの際にマウスを動かしてしまった場合の処理</summary>
    private void OverVigilance() {
        if (m_vig.VigilanceFlag) {
            m_warningMessage.SetActive(false);
            m_InvGauge.StopWave();
            SceneManager.Instance.SceneChange(SCENENAME.GameOverScene, () =>
            {
                UIManager.Instance.CloseUI(UIType.Timer);
                m_vig.invManager.SetMouseIcon(null);
            });
        }
    }

    //CSVのインデックスを管理します
    private enum InvDataCSVIndex {
        MaxVigilance = 0,
        InitialiseVigilance = 1,
        AddTimeValue = 2,
        AddMoseValue = 3,
        GraceTime = 4,
        ItemStart = 5,
    }

}

