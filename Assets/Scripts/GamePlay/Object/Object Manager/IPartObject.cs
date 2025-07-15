
public interface IPartObject : IStageElement
{
    ICompositeObject CreateCompositeObjectFrame();
    bool IsPartOfSameCompositeObject(IPartObject partObject);
}
