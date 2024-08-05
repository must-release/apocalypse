using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SingleToneManager: MonoBehaviour
{
    private static SingleToneManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Prevent multiple audio listener
    private void Start()
    {
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true;
    }
}

