using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayer currentPlayer;
    private CHARACTER currentCharacter;
    private Dictionary<CHARACTER, IPlayer> playerDictionary;
    private CHARACTER_STATE currentState;

    private void Start()
    {
        // Get player character object
        playerDictionary = new Dictionary<CHARACTER, IPlayer>();
        playerDictionary[CHARACTER.HERO] = transform.Find("Hero").GetComponent<IPlayer>();
        playerDictionary[CHARACTER.HEROINE] = transform.Find("Heroine").GetComponent<IPlayer>();
        currentPlayer = playerDictionary[CHARACTER.HERO];
        currentCharacter = CHARACTER.HERO;

        // Set initial state
        currentState = CHARACTER_STATE.IDLE;
    }

    // Set player state according to the info
    public void SetPlayerState(CHARACTER character)
    {
        // Initially change character
        ChangeCharacter(character);


    }

    // Change player character
    public void ChangeCharacter(CHARACTER character)
    {
        currentPlayer.ShowCharacter(false);
        currentPlayer = playerDictionary[character];
        currentCharacter = character;
        currentPlayer.ShowCharacter(true);
    }

    // Tag out current player character, tag in stand-by player character
    IEnumerator TagPlayer()
    {
        // Start tagging out
        currentState = CHARACTER_STATE.TAGGING;
        yield return currentPlayer.TagOut();

        // Change player character
        ChangeCharacter(currentCharacter == CHARACTER.HERO ? CHARACTER.HEROINE : CHARACTER.HERO);

        // Start tagging in
        yield return currentPlayer.TagIn();
        currentState = CHARACTER_STATE.IDLE;
    }
}

public interface IPlayer
{
    public void ShowCharacter(bool value);
    public Coroutine TagIn();
    public Coroutine TagOut();
}