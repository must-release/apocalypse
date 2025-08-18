using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IScene
{
    bool        CanMoveToNextScene  { get; }
    SceneType   CurrentSceneType    { get; }
    Transform   PlayerTransform     { get; }
    
    AD.Camera.ICamera[] GetSceneCameras();
    UniTask AsyncInitializeScene();
    void ActivateScene();
}
