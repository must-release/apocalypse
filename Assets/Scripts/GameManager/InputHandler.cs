using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public string moveAxisName = "Horizontal";
    public string attackButtonName = "Fire1";
    public string submitButtonName = "Submit";
    public string cancelButtonName = "Cancel";

    public float Move { get; private set; }
    public bool Attack { get; private set; }
    public bool Submit { get; private set; }
    public bool Cancel { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        Move = Input.GetAxis(moveAxisName);
        Attack = Input.GetButtonDown(attackButtonName);
        Submit = Input.GetButtonDown(submitButtonName);
        Cancel = Input.GetButtonDown(cancelButtonName);
    }
}
