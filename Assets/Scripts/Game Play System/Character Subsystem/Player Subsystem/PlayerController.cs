using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CHARACTER currentCharacter;
    
    public void SetPlayerState(CHARACTER character)
    {
        currentCharacter = character;
    }
}
