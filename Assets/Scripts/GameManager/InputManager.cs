using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

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
        if (InputHandler.Instance.Move != 0) { UIManager.Instance.Move(InputHandler.Instance.Move);}
        else { UIManager.Instance.Stop(); }

        if (InputHandler.Instance.Attack) { UIManager.Instance.Attack(); }

        if (InputHandler.Instance.Submit) { UIManager.Instance.Submit(); }

        if (InputHandler.Instance.Cancel) { UIManager.Instance.Cancel(); }


        /* Character Input */
        if (UIManager.Instance.CurrentState == UIManager.STATE.CONTROL)
        {

        }
    }
}

