using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEnums;
using System;

/* Part of UIController which manages Preference UI logic */

public class KeySettingsUIController : MonoBehaviour, IUIContoller
{
    /****** Private fields ******/
    private string keySettingsUIName = "Key Settings UI";
    private Transform keySettingsUI;
    private Transform keySettingsScroll;
    private Transform content;
    private List<Button> contentButtons = new List<Button>();

    /****** Single tone instance ******/
    public static KeySettingsUIController Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Find Key Settings UI object
            keySettingsUI = transform.Find(keySettingsUIName);
            if (keySettingsUI == null)
            {
                Debug.LogError("Key Settings UI Initialization Error");
                return;
            }
            Debug.Log("Key Settings UI found.");

            // Find Key Settings Scroll object
            keySettingsScroll = keySettingsUI.Find("KeySettingScroll");
            if (keySettingsScroll == null)
            {
                Debug.LogError("Key Settings Scroll Initialization Error");
                return;
            }
            Debug.Log("Key Settings Scroll found.");

            // Find Content through Viewport
            Transform viewport = keySettingsScroll.Find("Viewport");
            if (viewport == null)
            {
                Debug.LogError("Viewport Initialization Error");
                return;
            }
            Debug.Log("Viewport found.");

            content = viewport.Find("Content");
            if (content == null)
            {
                Debug.LogError("Content Initialization Error");
                return;
            }
            Debug.Log("Content found.");

            // Find all buttons under Content by navigating to Key Image
            foreach (Transform child in content)
            {
                Button button = child.Find("Key Image")?.GetComponent<Button>();
                if (button != null)
                {
                    contentButtons.Add(button);
                }
            }
            Debug.Log("Content buttons found: " + contentButtons.Count);

            // Add event listeners to buttons by order
            if (contentButtons.Count > 0) contentButtons[0].onClick.AddListener(OnJumpButton);
            if (contentButtons.Count > 1) contentButtons[1].onClick.AddListener(OnDashButton);
            if (contentButtons.Count > 2) contentButtons[2].onClick.AddListener(OnAttackButton);
            if (contentButtons.Count > 3) contentButtons[3].onClick.AddListener(OnWeaponChangeButton);

            // Find and set up Reset and Confirm buttons in Key Settings UI
            Transform buttonsParent = keySettingsUI.Find("Buttons");
            if (buttonsParent != null)
            {
                Debug.Log("Buttons parent found.");

                foreach (Transform child in buttonsParent)
                {
                    Debug.Log("Child button found: " + child.name);
                }

                Button resetButton = buttonsParent.Find("Reset Button")?.GetComponent<Button>();
                if (resetButton != null)
                {
                    resetButton.onClick.AddListener(OnResetButton);
                    Debug.Log("Reset Button found and listener added.");
                }
                else
                {
                    Debug.LogError("Reset Button not found.");
                }

                Button confirmButton = buttonsParent.Find("Confirm Button")?.GetComponent<Button>();
                if (confirmButton != null)
                {
                    confirmButton.onClick.AddListener(OnConfirmButton);
                    Debug.Log("Confirm Button found and listener added.");
                }
                else
                {
                    Debug.LogError("Confirm Button not found.");
                }
            }
            else
            {
                Debug.LogError("Buttons parent not found.");
            }
        }
    }

    /****** Methods ******/

    // Enter Key Settings UI state
    public void StartUI()
    {
        // Activate key settings UI object
        keySettingsUI.gameObject.SetActive(true);
    }


    // Update Key Settings UI
    public void UpdateUI()
    {

    }

    // Exit Key Settings UI state
    public void EndUI()
    {
        // Inactivate key settings UI object
        keySettingsUI.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        UIController.Instance.TurnSubUIOff(SUBUI.KEYSETTINGS);
    }

    private void OnJumpButton()
    {
        Debug.Log("Jump Button Clicked");
    }

    private void OnDashButton()
    {
        Debug.Log("Dash Button Clicked");
    }

    private void OnAttackButton()
    {
        Debug.Log("Attack Button Clicked");
    }

    private void OnWeaponChangeButton()
    {
        Debug.Log("Weapon Change Button Clicked");
    }

    private void OnConfirmButton()
    {
        Debug.Log("Confirm Button Clicked");
    }

    private void OnResetButton()
    {
        Debug.Log("Reset Button Clicked");
    }
}
