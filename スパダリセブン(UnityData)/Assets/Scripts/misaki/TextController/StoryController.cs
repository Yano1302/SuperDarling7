
public class StoryController : BaseTextController
{
    /// --------ŠÖ”ˆê——-------- ///

    #region publicŠÖ”
    /// -------publicŠÖ”------- ///

    public override void TalkEnd()
    {
        base.TalkEnd();
        sceneManager.SceneChange(SCENENAME.RequisitionsScene);
    }

    /// -------publicŠÖ”------- ///
    #endregion

    #region protectedŠÖ”
    /// -----protectedŠÖ”------ ///



    /// -----protectedŠÖ”------ ///
    #endregion

    #region privateŠÖ”
    /// ------privateŠÖ”------- ///

    private void Start()
    {
        OnTalkButtonClicked();
    }

    /// ------privateŠÖ”------- ///
    #endregion

    /// --------ŠÖ”ˆê——-------- ///

}
