
public class StoryController : BaseTextController
{
    public override void TalkEnd()
    {
        base.TalkEnd();
        sceneManager.SceneChange(SCENENAME.RequisitionsScene);
    }
}
