using UnityEngine;
using System.Collections;

public class CutsceneUIController : MonoBehaviour, IUIContoller
{

    /****** Private fields ******/
    private string cutsceneUIName = "Cutscene UI";
    private Transform cutsceneUI;


    /****** Single tone instance ******/
    public static CutsceneUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find cutscene UI object
            cutsceneUI = transform.Find(cutsceneUIName);
            if (cutsceneUI == null)
            {
                Debug.Log("Cutscene UI Initialization Error");
                return;
            }
        }
    }

    /****** Methods ******/

    // Enter Cutscene UI state
    public void StartUI()
    {
        // Activate cutscene UI object
        cutsceneUI.gameObject.SetActive(true);
    }

    // Exit Cutscene UI state
    public void EndUI()
    {
        // Inactivate cutscene UI object
        cutsceneUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {
    }

    public void UpdateUI() { return; }
    public void Attack() { return; }
    public void Submit() { return; }
    public void Move(float move) { return; }
    public void Stop() { return; }
}

