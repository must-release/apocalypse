using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ConsoleInputHandler : MonoBehaviour, InputHandler
{
    public string moveAxisName = "Horizontal";
    public string attackButtonName = "Fire1";
    public string submitButtonName = "Submit";
    public string cancelButtonName = "Cancel";

    public float Move { get; set; }
    public bool Attack { get; set; }
    public bool Submit { get; set; }
    public bool Cancel { get; set; }

    public void Awake()
    {
        if (InputHandler.Instance == null && IUIState.isConsole)
        {
            InputHandler.Instance = this;
        }
    }

    private void Update()
    {
        Move = Input.GetAxis(moveAxisName);
        Attack = Input.GetButtonDown(attackButtonName);
        Submit = Input.GetButtonDown(submitButtonName);
        Cancel = Input.GetButtonDown(cancelButtonName);
    }
}

