
public class StoryController : BaseTextController
{
    /// --------�֐��ꗗ-------- ///

    #region public�֐�
    /// -------public�֐�------- ///

    public override void TalkEnd()
    {
        base.TalkEnd();
        sceneManager.SceneChange(SCENENAME.RequisitionsScene);
    }

    /// -------public�֐�------- ///
    #endregion

    #region protected�֐�
    /// -----protected�֐�------ ///



    /// -----protected�֐�------ ///
    #endregion

    #region private�֐�
    /// ------private�֐�------- ///

    private void Start()
    {
        OnTalkButtonClicked();
    }

    /// ------private�֐�------- ///
    #endregion

    /// --------�֐��ꗗ-------- ///

}
