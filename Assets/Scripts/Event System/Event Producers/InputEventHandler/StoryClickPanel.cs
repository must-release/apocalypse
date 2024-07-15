using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* EventLister which checks mouse click and turn 'Attack' on in the input handler */

public class StoryClickPanel : MonoBehaviour, IPointerDownHandler
{
    // Called when panel is clicked
    public void OnPointerDown(PointerEventData eventData)
    {
        // Left mouse click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Play Next Script
            StoryController.Instance.PlayNextScript();
        }
    }
}