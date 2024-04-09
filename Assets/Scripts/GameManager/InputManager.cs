using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public bool SubmitLock { get; set; } = false; // Lock submit button
    public bool AttackLock { get; set; } = false; // Lock attack button

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void Update()
    {
        /* UI Input */
        if (InputHandler.Instance.Move != 0) { UIManager.Instance.Move(InputHandler.Instance.Move); }
        else { UIManager.Instance.Stop(); }

        if (InputHandler.Instance.Attack)
        {
            UIManager.Instance.Attack();
            InputHandler.Instance.Attack = false; // Reset Attack after handling
        }

        if (InputHandler.Instance.Submit)
        {
            // When there is a lock, unlock it and skip the submit action
            if (SubmitLock)
                SubmitLock = false;
            else
                UIManager.Instance.Submit();
        }

        if (InputHandler.Instance.Cancel) { UIManager.Instance.Cancel(); }


        /* Character Input */
        if (UIManager.Instance.CurrentState == UIManager.STATE.CONTROL)
        {

        }
    }
}

public interface InputHandler
{
    public static InputHandler Instance { get; protected set; }

    public float Move { get; set; }
    public bool Attack { get; set; }
    public bool Submit { get; set; }
    public bool Cancel { get; set; }
}