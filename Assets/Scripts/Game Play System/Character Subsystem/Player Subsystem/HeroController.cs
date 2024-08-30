using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour, IPlayer
{
    // Show or hide character object
    public void ShowCharacter(bool value)
    {
        gameObject.SetActive(value);
    }

    public Coroutine TagIn()
    {
        return null;
    }   
    public Coroutine TagOut()
    {
        return null;
    }
}
