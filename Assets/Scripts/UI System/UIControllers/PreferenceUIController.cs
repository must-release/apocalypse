using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;
using System;

/* Part of UIController which manages Preference UI logic */

public class PreferenceUIController : MonoBehaviour, IUIContoller
{

    /****** Private fields ******/
    private string preferenceUIName = "Preference UI";
    private string buttonsName = "Buttons";
    private Transform preferenceUI;
    private List<Button> buttonList = new List<Button>();

    /****** Single tone instance ******/
    public static PreferenceUIController Instance;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;


            // Find Preference UI object
            preferenceUI = transform.Find(preferenceUIName);
            if (preferenceUI == null)
            {
                Debug.Log("Preference UI Initialization Error");
                return;
            }

            Transform buttons = preferenceUI.Find(buttonsName);
            for (int i = 0; i < buttons.childCount; i++)
            {
                // Add ith button to the list
                buttonList.Add(buttons.GetChild(i).GetComponent<Button>());
            }


            // Add event listener to buttons
            buttonList[0].onClick.AddListener(OnWindowScreenButton);
            buttonList[1].onClick.AddListener(OnFullScreenButton);
            buttonList[2].onClick.AddListener(OnMovetoKeySettingsButton);
            buttonList[3].onClick.AddListener(OnResetButton);
            buttonList[4].onClick.AddListener(ConfirmButton);


            buttonList[0].Select();
        }
    }

    public void Start()
    {
        
    }


    /****** Methods ******/

    private void Update()
    {
        Return();
    }

    // �������� �ӽ�
    private void Return()
    {
        if (UIModel.Instance.CurrentSubUI == SUBUI.PREFERENCE && Input.GetKeyDown(KeyCode.Backspace))
        {
            UIController.Instance.TurnSubUIOff(SUBUI.PREFERENCE);
        }
        else if
            (UIModel.Instance.CurrentSubUI == SUBUI.KEYSETTINGS && Input.GetKeyDown(KeyCode.Backspace))
        {
            UIController.Instance.TurnSubUIOff(SUBUI.KEYSETTINGS);
        }

    }

    // Enter Preference UI state
    public void StartUI()
    {
        // Activate preference UI object
        preferenceUI.gameObject.SetActive(true);


    }

    // Exit Preference UI state
    public void EndUI()
    {
        // Inactivate preference UI object
        preferenceUI.gameObject.SetActive(false);
    }


    // Ȯ�� , �ʱ�ȭ ui�� ��� ������ΰ�
    private void OnWindowScreenButton()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Debug.Log("Screen mode changed to: Windowed");
    }
    private void OnFullScreenButton()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Debug.Log("Screen mode changed to: Fullscreen");
    }
    private void ConfirmButton()
    {
        Debug.Log("�����Ͻðڽ��ϱ�?");
    }
    private void OnResetButton()
    {
        Debug.Log("������ �ʱ�ȭ �Ͻðڽ��ϱ�?");

        //�ӽ�
        buttonList[0].Select();
    }
    private void OnMovetoKeySettingsButton()
    {
        UIController.Instance.TurnSubUIOn(SUBUI.KEYSETTINGS);
    }
    public void Cancel()
    {
        
    }
}
