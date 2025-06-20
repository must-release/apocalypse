using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IScene
{
    bool        CanMoveToNextScene  { get; }
    SceneType   CurrentSceneType    { get; }
    Transform   PlayerTransform { get; }

    UniTask AsyncInitializeScene();
    void ActivateScene();
}
