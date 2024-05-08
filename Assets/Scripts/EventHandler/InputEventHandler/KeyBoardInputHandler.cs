using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyBoardInputHandler : MonoBehaviour, InputHandler
{
    public string moveAxisName = "Horizontal";
    public string submitButtonName = "Submit";
    public string cancelButtonName = "Cancel";

    public float Move { get; set; }
    public bool Attack { get; set; }
    public bool Submit { get; set; }
    public bool Cancel { get; set; }

    public void Awake()
    {
        if(InputHandler.Instance == null && !IUIContollerState.isConsole)
        {
            InputHandler.Instance = this;
        }
    }

    private void Update()
    {
        Move = Input.GetAxis(moveAxisName);
        Submit = Input.GetButtonDown(submitButtonName);
        Cancel = Input.GetButtonDown(cancelButtonName);
    }
}
