using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingDot : MonoBehaviour
{
    public AimingDot NextDot { get; set; }

    private void InactiveNextDots()
    {
        NextDot?.InactiveNextDots();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.isTrigger == false)
        {
            gameObject.SetActive(false);
            NextDot?.InactiveNextDots();
        }
    }
}
