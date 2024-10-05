using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroineController : MonoBehaviour, IPlayer
{
    public bool IsLoaded {get; set;}
    // Show or hide character object
    public void ShowCharacter(bool value)
    {
        gameObject.SetActive(value);
    }

    public void RotateUpperBody(float rotateAngle)
    {
        
    }

    public void RotateUpperBody(Vector3 target)
    {
        
    }

    public float Attack()
    {
        return 0;
    }
}
