using UnityEngine;
using System.Collections;

public class EmptyUIState : MonoBehaviour, IUIContoller
{
    /****** Single tone instance ******/
    public static EmptyUIState Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /****** Methods ******/

    // Enter Empty UI state
    public void StartUI()
    {
    }

    // Exit Empty UI state
    public void EndUI()
    {
    }

    public void Cancel()
    {
    }

    public UIController.STATE GetState()
    {
        return UIController.STATE.EMPTY;
    }

    public void UpdateUI() { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Move(float move) { return; }
    public void Stop() { return; }
}

