using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : MonoBehaviour
{
    public static GamePlayController Instance;

    public bool IsCutscenePlaying { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void PlayCutscene()
    {
        StartCoroutine(CutsceneCoroutine());
    }
    IEnumerator CutsceneCoroutine()
    {
        IsCutscenePlaying = true;
        Debug.Log("Playing cutscene");
        yield return new WaitForSeconds(3);
        IsCutscenePlaying = false;
    }
}
