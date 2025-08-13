using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleScene : MonoBehaviour, IScene
{
    public bool CanMoveToNextScene => true;
    public SceneType CurrentSceneType => SceneType.TitleScene;
    public Transform PlayerTransform => null; // Title scene does not have a player transform

    public async UniTask AsyncInitializeScene()
    {
        await UniTask.CompletedTask;
    }

    public void ActivateScene() { }
    
    public ICamera[] GetSceneCameras()
    {
        return new ICamera[0]; // No cameras in title scene
    }
}
