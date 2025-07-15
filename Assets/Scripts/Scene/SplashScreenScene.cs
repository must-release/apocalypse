using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;


public class SplashScreenScene : MonoBehaviour, IScene
{
    /****** Public Members ******/

    public bool CanMoveToNextScene { get; private set; } = false;
    public SceneType CurrentSceneType => SceneType.SplashScreenScene;
    public Transform PlayerTransform => null; // Splash screen does not have a player transform

    public async UniTask AsyncInitializeScene()
    {
        await UniTask.CompletedTask;
    }

    public void ActivateScene()
    {
        // This scene does not require any activation logic.
    }

    /****** Private Members ******/

    private IEnumerator Start()
    {
        yield return PlaySplashAnimation();

        CanMoveToNextScene = true;
    }

    private IEnumerator PlaySplashAnimation()
    {
        Debug.Log("Splash animation start");

        yield return new WaitForSeconds(2f);
        
        Debug.Log("Splash animation done");
    }
}