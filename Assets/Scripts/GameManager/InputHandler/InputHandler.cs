using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public string moveAxisName = "Horizontal";
    public string attackButtonName = "Fire1";

    public float Move { get; private set; }
    public bool Attack { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        Move = Input.GetAxis(moveAxisName);
        Attack = Input.GetButtonDown(attackButtonName);
    }
}
