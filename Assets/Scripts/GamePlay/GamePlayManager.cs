using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour, IAsyncLoadObject
{
    /****** Public Members ******/

    public static GamePlayManager Instance { get; private set; } = null;

    public bool IsCutscenePlaying { get; private set; } = false;

    public bool IsLoaded => _isPoolLoaded;

    public void PlayCutscene() { StartCoroutine(CutsceneCoroutine()); }

    // Initialize Player transform, info
    public void InitializePlayerCharacter(Transform player, PlayerType character)
    {
        CharacterManager.Instance.SetPlayerCharacter(player, character);

        CameraController.Instance.AttachCamera(player, 0.1f);
    }

    public void ControlPlayerCharacter(ControlInfo controlInfo)
    {
        CharacterManager.Instance.ExecutePlayerControl(controlInfo);
    }

    public void ProcessGameOver()
    {
        GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.GameOver));
    }


    /****** Private Members ******/

    private bool _isPoolLoaded  = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(AsyncLoadObjectPool());
    }

    private IEnumerator AsyncLoadObjectPool()
    {
        yield return PooInitializer.AsyncLoadPool();

        _isPoolLoaded = true;
    }

    private IEnumerator CutsceneCoroutine()
    {
        IsCutscenePlaying = true;
        Debug.Log("Playing cutscene");
        yield return new WaitForSeconds(3);
        IsCutscenePlaying = false;
    }
}
