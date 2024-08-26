using System.Collections;
using System.Collections.Generic;
using CharacterEums;
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

    // Play Cutscene
    public void PlayCutscene() { StartCoroutine(CutsceneCoroutine()); }
    IEnumerator CutsceneCoroutine()
    {
        IsCutscenePlaying = true;
        Debug.Log("Playing cutscene");
        yield return new WaitForSeconds(3);
        IsCutscenePlaying = false;
    }

    // Initialize Player transform, info
    public void InitializePlayerCharacter(Transform player, CHARACTER character)
    {
        CharacterManager.Instance.SetPlayerCharacter(player, character);
    }

    // Control player character
    public void ControlPlayerCharacter(ControlInfo controlInfo)
    {

    }
}
