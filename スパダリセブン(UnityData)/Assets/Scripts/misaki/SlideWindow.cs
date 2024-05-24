using Supadari;
using UnityEngine;

public class SlideWindow : MonoBehaviour
{
    RectTransform rectTransform; // 自身のRectTransform変数
    RectTransform[] rectTransforms; // RectTransform配列
    [SerializeField] UIManager uiManager; // UIManager変数
    [SerializeField] AudioManager audioManager; // AudioManager変数
    [SerializeField] SceneManager sceneManager; // SceneManager変数
    public float speacing = 20f; // オブジェクト間のスペースの大きさ
    float maxParentLength = 0; // 親オブジェクトの全長
    float parentOffsetMin_x = 0; // 親オブジェクトのleft初期値
    public float speed = 1000f;
    float[] phase; // 次フェーズに進める目標値
    float phasePoint = 0; // 次フェーズに進むかどうかの判定で使うフロート値
    SLIDESTATE slideState; // SLIDESTATE変数
    bool openMenu = false; // メニューを開いているかどうか
    public bool OpenCheck() => openMenu; // openMenuゲッター関数

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // RectTransformを代入
        parentOffsetMin_x = rectTransform.offsetMin.x; // 親オブジェクト
        rectTransforms = new RectTransform[transform.childCount]; // 子オブジェクト分の配列を用意する
        // 子オブジェクトのRectTransformを格納し、子オブジェクトの横幅を全て足して親オブジェクトの全長を算出する
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            rectTransforms[i] = transform.GetChild(i).GetComponent<RectTransform>();
            maxParentLength += rectTransforms[i].rect.width + speacing;
        }
        phase = new float[transform.childCount]; // フェーズの要素数を子オブジェクト数にする
        for (int i = 0; i < phase.Length; i++)
        {
            // 基準となるフェーズ目標値を算出する　親オブジェクトの全長/子オブジェクト数
            if (i == 0)
            {
                phase[0] = maxParentLength / transform.childCount;
                continue;
            }
            // 要素数1以降はphase[0]を(i+1)倍して代入する 例 phase[4]の場合はphase[0]*5倍となる
            phase[i] = phase[0] * (i + 1);
        }
        slideState = SLIDESTATE.DEFAULT; // slideStateをデフォルトにする
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>(); // AudioManagerを代入
    }

    private void FixedUpdate()
    {
        if (!openMenu) SlideIn(); // メニューをしまう場合
        else if (openMenu) SlideOut(); // メニューを出す場合
    }
    /// <summary>
    /// メニューをしまう関数
    /// </summary>
    void SlideIn()
    {
        // フェーズポイントが親オブジェクトの全長以上になった または スライドステータスがデフォルトの場合
        if (phasePoint >= maxParentLength || slideState == SLIDESTATE.DEFAULT)
        {
            if (slideState == SLIDESTATE.DEFAULT) return; // デフォルトの場合はリターンして関数を動かさない
            // 親オブジェクトの全長を最小に変更
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMax.x  , rectTransform.offsetMin.y);
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                rectTransforms[i].position = new Vector2(rectTransform.position.x, rectTransforms[i].position.y);
            }
            phasePoint = default; // フェーズポイントをリセット
            slideState = SLIDESTATE.DEFAULT; // デフォルトにする
            return;
        }
        float currentLeft = rectTransform.offsetMin.x; // 親オブジェクトの現在のLeft値を代入
        float move = speed * Time.deltaTime; // 移動値を算出
        float newLeft = currentLeft + move; // 移動値を足したLeft値を算出
        phasePoint += move; // フェーズポイントに移動値を加算
        // フェーズポイントによって子オブジェクトを動かす
        PhaseSlide(move);
        // 親オブジェクトのLeftを変更して全長を縮める
        rectTransform.offsetMin = new Vector2(newLeft, rectTransform.offsetMin.y);
    }
    /// <summary>
    /// メニューを出す関数
    /// </summary>
    void SlideOut()
    {
        // フェーズポイントが親オブジェクトの全長以上になった または スライドステータスがデフォルトの場合
        if (phasePoint >= maxParentLength || slideState == SLIDESTATE.DEFAULT)
        {
            if (slideState == SLIDESTATE.DEFAULT) return; // デフォルトの場合はリターンして関数を動かさない
            // 親オブジェクトの全長を最大(全子オブジェクトの幅合計値)に変更
            rectTransform.offsetMin = new Vector2(-(maxParentLength - rectTransform.offsetMax.x), rectTransform.offsetMin.y);
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                if (i == 0)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x + phase[2], rectTransforms[i].position.y);
                }
                else if(i == 1)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x + phase[1], rectTransforms[i].position.y);
                }
                else if (i == 2)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x + phase[0], rectTransforms[i].position.y);
                }
                else if (i == 3)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x, rectTransforms[i].position.y);
                }
            }
            phasePoint = default; // フェーズポイントをリセット
            slideState = SLIDESTATE.DEFAULT; // デフォルトにする
            return; 
        }
        float currentLeft = rectTransform.offsetMin.x; // 親オブジェクトの現在のLeft値を代入
        float move = speed * Time.deltaTime; // 移動値を算出
        float newLeft = currentLeft - move; // 移動値を足したLeft値を算出
        phasePoint += move; // フェーズポイントに移動値を加算
        // フェーズポイントによって子オブジェクトを動かす
        PhaseSlide(move);
        // 親オブジェクトのLeftを変更して全長を縮める
        rectTransform.offsetMin = new Vector2(newLeft, rectTransform.offsetMin.y);
    }
    /// <summary>
    /// 子オブジェクトをスライドインさせる関数
    /// </summary>
    /// <param name="value">動かしたい子オブジェクトの要素数</param>
    /// <param name="move">移動値</param>
    void SlideChildren(int value, float move)
    {
        // value以下の子オブジェクトをスライドさせる
        for (int i = -1; i < value; i++)
        {
            if (!openMenu) // メニューを開く場合
            {
                rectTransforms[i + 1].position = new Vector2(rectTransforms[i + 1].position.x - move, rectTransforms[i + 1].position.y);
            }
            else // メニューをしまう場合
            {
                rectTransforms[i + 1].position = new Vector2(rectTransforms[i + 1].position.x + move, rectTransforms[i + 1].position.y);
            }
        }
    }
    /// <summary>
    /// 子オブジェクトをスライドインさせるかのチェック関数
    /// </summary>
    /// <param name="move">移動値</param>
    void PhaseSlide(float move)
    {
        // フェーズポイントがどのフェーズまで到達しているかのチェック
        for (int i = phase.Length - 1; i >= 0; i--)
        {
            if (phasePoint > phase[i])
            {
                // チェックが通ったらフェーズ配列の要素数分の子オブジェクトを動かす
                SlideChildren(i, move);
                return;
            }
        }
    }
    /// <summary>
    /// ストーリーメニューを開くまたは閉じる関数
    /// </summary>
    public void StoryMenuButton()
    {
        // スライドステータスがデフォルトかをチェック
        if (slideState == SLIDESTATE.DEFAULT)
        {
            openMenu = !openMenu; // 反転して関数を動かす
            slideState = SLIDESTATE.SLIDE; // スライド中に変更
        }
    }
    /// <summary>
    /// メニューのボタン配置を初期化する関数
    /// </summary>
    public void InitializeStoryMenu()
    {
        if (!openMenu) return; // メニューを開いていないならリターン
        // 開いていない状態にする
        openMenu = false;
        rectTransform.offsetMin = new Vector2(parentOffsetMin_x, rectTransform.offsetMin.y);
        
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            rectTransforms[i].localPosition = new Vector2(rectTransforms[i].localPosition.x - rectTransforms[i].localPosition.x, rectTransforms[i].localPosition.y);
        }

    }
    /// <summary>
    /// セーブスロットを開く関数
    /// </summary>
    public void SaveButton()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす
        uiManager.OpenUI(UIType.SaveSlot); // セーブスロットを表示
    }
    /// <summary>
    /// ロードスロットを開く関数
    /// </summary>
    public void LoadButton()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE); // SEを鳴らす
        uiManager.OpenUI(UIType.LoadSlot); // ロードスロットを表示
    }
}
