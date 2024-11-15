using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroineController : MonoBehaviour, IPlayer
{
    public bool IsLoaded {get; set;}
    public bool IsAiming { set{}}

    private void Start() { StartCoroutine(LoadWeaponsAndDots()); }
    public IEnumerator LoadWeaponsAndDots()
    {
        yield return null;
        IsLoaded = true;
    }

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

    public void Aim(bool value)
    {

    }

    public float Attack()
    {
        return 0;
    }
}
