using UnityEngine;
using System.Collections;

/*
 * EventProducer which creates the input event stream
 */

public class InputEventProducer : MonoBehaviour, PreferenceObserver
{
    public static InputEventProducer Instance { get; private set; }

    private string cancelButtonName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void PreferenceUpdated()
    {

    }
}

