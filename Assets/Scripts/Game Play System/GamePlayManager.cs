using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;

    public bool IsCutscenePlaying { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
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