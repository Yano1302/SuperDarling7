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
        Dungeon = 5 // 調査テストシーン
    }
    // 会話関係のステータス
    public enum TALKSTATE 
    {
        NOTALK, // 話していない
        TALKING, // 会話中
        NEXTTALK, // 次のセリフ
        LASTTALK // 最後のセリフ
    }

