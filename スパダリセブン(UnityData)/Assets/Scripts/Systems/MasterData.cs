using System;

// 岬作成のセーブ・ロードするクラスのスクリプトです
// セーブ・ロードしたい変数はここに記載してください
[Serializable]
public class MasterData // jsonデータとして保存するclass
{
    /// --------変数一覧-------- ///

    #region public変数
    /// -------public変数------- ///

    // シーン名
    public SCENENAME scenename = SCENENAME.TitleScene;
    // セーブしたことがあるか
    public bool haveSaved = false;

    /// -------public変数------- ///
    #endregion

    #region protected変数
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    #endregion

    #region private変数
    /// ------private変数------- ///



    /// ------private変数------- ///
    #endregion

    #region プロパティ
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    #endregion

    /// --------変数一覧-------- ///
}

