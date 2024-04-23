
public class StoryController : BaseTextController
{
    private void Start()
    {
        OnTalkButtonClicked();
    }
    public override void TalkEnd()
    {
        base.TalkEnd();
        sceneManager.SceneChange(SCENENAME.RequisitionsScene);
    }
}
