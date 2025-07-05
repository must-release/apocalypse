
public interface IPartObject
{
    ICompositeObject CreateCompositeObjectFrame();
    bool IsPartOfSameCompositeObject(IPartObject partObject);
}
