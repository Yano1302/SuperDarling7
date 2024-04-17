using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シーンの名称　数字はビルド番号に合わせている
public enum SCENENAME
{
    TitleScene = 0, // タイトル
    RequisitionsScene = 1, // 依頼シーン
    StoryScene = 2, // ストーリーシーン
    InvestigationScene = 3, // 調査シーン
    SolveScene = 4, // 解決シーン
    Dungeon = 5, // 調査テストシーン
    GameOverScene = 6, // ゲームオーバーシーン
    GameClearScene = 7 // ゲームクリアシーン
}
// 会話関係のステータス
public enum TALKSTATE 
{
    NOTALK, // 話していない
    TALKING, // 会話中
    NEXTTALK, // 次のセリフ
    LASTTALK // 最後のセリフ
}
// 環境設定のステータス
public enum SETTINGSTATE
{
    BGM = 0, // BGMの設定
    SE = 1, // SEの設定
    TEXTSPEED = 2 // テキストスピードの設定
}
// ストーリーメニュー画面のスライドのステータス
public enum SLIDESTATE
{
    DEFAULT = 0, // スライドしていない
    SLIDE = 1 // スライド中
}