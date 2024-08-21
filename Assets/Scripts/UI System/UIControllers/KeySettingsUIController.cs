using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UIEnums;
using System;
using System.Reflection;

public class KeySettingsUIController : MonoBehaviour, IUIController
{
    /****** Private fields ******/
    private string keySettingsUIName = "Key Settings UI";
    private string keyBoxPath = " Key Box";
    private string keyButtonTextPath = " Key Box/Key Button/Key Button Text";
    private Transform keySettingsUI;
    private Transform content;
    private GameObject keyBox;
    private Transform currentButtonTextTransform;
    private bool isWaitingForKeyInput = false;
    private string currentKeySettingField;
    private Dictionary<string, KeyCode> originalKeySettings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> tempKeySettings;

    private string confirmPanelName = "Confirm Panel";
    private Transform confirmPanel;
    private TextMeshProUGUI confirmText;
    private TextMeshProUGUI labelText;
    private Button _confirmButton_;
    private Button _cancelButton_;

    /****** Singleton instance ******/
    public static KeySettingsUIController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            keySettingsUI = transform.Find(keySettingsUIName);
            if (keySettingsUI == null)
            {
                Debug.LogError("Key Settings UI Initialization Error");
                return;
            }

            content = keySettingsUI.Find("KeySettingScroll/Viewport/Content");
            if (content == null)
            {
                Debug.LogError("Content Initialization Error");
                return;
            }

            keyBox = content.Find("Key Box").gameObject;
            if (keyBox == null)
            {
                Debug.LogError("Key Box Initialization Error");
                return;
            }

            Button resetButton = keySettingsUI.Find("Buttons/Reset Button")?.GetComponent<Button>();
            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetButton);
            }

            Button confirmButton = keySettingsUI.Find("Buttons/Confirm Button")?.GetComponent<Button>();
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmButton);
            }

            InitailizeConfirmPanel();
        }
    }

    public void Start()
    {
        // Load existing settings from SettingsManager
        LoadExistingKeySettings();

        // Add current UI conroller
        UIController.Instance.AddUIController(SUBUI.KEYSETTINGS, Instance);
    }

    // Load existing key settings from SettingsManager
    private void LoadExistingKeySettings()
    {
        // Load key settings
        var keySettings = SettingsManager.Instance.KeySettingInfo;

        // Create and initialize Key Box objects according to the settings
        FieldInfo[] fields = keySettings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            GameButton gameButton = fields[i].GetValue(keySettings) as GameButton;

            // Instantiate a new keyBox if it's not the first iteration
            GameObject newKeyBox = i > 0 ? Instantiate(keyBox, content) : keyBox;

            // Set the name of the new key box and key text
            newKeyBox.name = gameButton.buttonName + keyBoxPath;
            newKeyBox.transform.Find("Key Text").GetComponent<TextMeshProUGUI>().text = gameButton.buttonName;

            // Update the button text
            UpdateButtonText(GetButtonTextTransform(gameButton.buttonName), gameButton.buttonKeyCode);

            // Get the button component and assign the click event
            Button button = newKeyBox.transform.Find("Key Button").GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClick(gameButton));

            // Store the original key settings
            originalKeySettings[gameButton.buttonName] = gameButton.buttonKeyCode;
        }

        // Initialize tempKeySettings with original values
        tempKeySettings = new Dictionary<string, KeyCode>(originalKeySettings);
    }

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
        if (confirmPanel.gameObject.activeInHierarchy) // check if confirm panel is on.
        {
            confirmPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Cancel");
            UIController.Instance.TurnSubUIOff(SUBUI.KEYSETTINGS);
        }
    }

    private void OnButtonClick(GameButton gameButton)
    {
        SetCurrentButtonTextTransform(GetButtonTextTransform(gameButton.buttonName), gameButton.buttonName);
    }


    // Reset to original key settings
    private void OnResetButton()
    {
        Debug.Log("Reset Button Clicked");

        foreach (var entry in originalKeySettings)
        {
            // Update tempKeySettings with the original values
            tempKeySettings[entry.Key] = entry.Value;

            // Update the UI text to reflect the original key bindings
            UpdateButtonText(GetButtonTextTransform(entry.Key), entry.Value);
        }

        Debug.Log("All key settings have been reset to their original values.");
    }
    private void OnConfirmButton() // On KeySetting UI
    {
        //Debug.Log("Confirm Button Clicked");
        confirmPanel.gameObject.SetActive(true);
    }




    // Update KeySetting
    private void Update()
    {
        if (isWaitingForKeyInput)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // Update the text with the new key for the current button
                        UpdateButtonText(currentButtonTextTransform, keyCode);

                        // Temporarily store the new key in tempKeySettings
                        UpdateKeySetting(currentKeySettingField, keyCode);

                        isWaitingForKeyInput = false;
                        break;
                    }
                }
            }
        }
    }
    private void SetCurrentButtonTextTransform(Transform buttonTextTransform, string keySettingField)
    {
        currentButtonTextTransform = buttonTextTransform;
        if (currentButtonTextTransform == null)
        {
            Debug.LogError($"Transform not found for path: {buttonTextTransform}");
            return;
        }
        isWaitingForKeyInput = true;
        currentKeySettingField = keySettingField;
    }

    private void _OnConfirmButton_() // On Confirm Panel
    {
        Debug.Log("_Confirm Button_ Clicked");

        var keySettings = SettingsManager.Instance.KeySettingInfo;

        FieldInfo[] fields = keySettings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            GameButton gameButton = fields[i].GetValue(keySettings) as GameButton;
            gameButton.buttonKeyCode = tempKeySettings[gameButton.buttonName];
            fields[i].SetValue(keySettings, gameButton);
        }
        

        // Update the originalKeySettings to match the confirmed tempKeySettings
        originalKeySettings = new Dictionary<string, KeyCode>(tempKeySettings);

        SettingsManager.Instance.ChangeKeySettings(keySettings);
        confirmPanel.gameObject.SetActive(false);
    }

    private void _OnCancelButton_() // On Confirm Panel
    {
        Cancel();
    }

    // Update Button TextMeshProUGUI component with new text
    private void UpdateButtonText(Transform buttonTextTransform, KeyCode keyCode)
    {
        if (buttonTextTransform == null)
        {
            Debug.LogError("ButtonText Transform is null");
            return;
        }

        TextMeshProUGUI tmp = buttonTextTransform.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            string keyName = GetKeyName(keyCode);
            tmp.text = keyName;
            //Debug.Log($"TextMeshProUGUI component found and text updated to {keyName}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found on ButtonText Transform");
        }
    }



    // Update the key setting
    private void UpdateKeySetting(string keySettingField, KeyCode newKey)
    {
        // Check if the new key is already assigned to another action
        string existingKeyField = null;
        foreach (var entry in tempKeySettings)
        {
            if (entry.Value == newKey && entry.Key != keySettingField)
            {
                existingKeyField = entry.Key;
                break;
            }
        }

        if (!string.IsNullOrEmpty(existingKeyField))
        {
            // Debug.Log($"Key {newKey} is already assigned to {existingKeyField}. Swapping keys.");

            // Swap the keys between the current field and the existing one
            KeyCode temp = tempKeySettings[keySettingField];
            tempKeySettings[keySettingField] = newKey;
            tempKeySettings[existingKeyField] = temp;

            // Update the UI text for both fields
            UpdateButtonText(GetButtonTextTransform(keySettingField), newKey);
            UpdateButtonText(GetButtonTextTransform(existingKeyField), temp);
        }
        else
        {
            // Update the current key setting if there's no conflict
            tempKeySettings[keySettingField] = newKey;

            // Update the UI text for the current button
            UpdateButtonText(currentButtonTextTransform, newKey);
        }

        // Debug.Log($"Key {newKey} assigned to {keySettingField}");
    }

    private Transform GetButtonTextTransform(string keyButtonName)
    {
        return content.Find(keyButtonName + keyButtonTextPath);
    }

    private void InitailizeConfirmPanel()
    {
        confirmPanel = keySettingsUI.Find(confirmPanelName);
        _confirmButton_ = confirmPanel.GetChild(0).GetChild(1).GetComponent<Button>();
        _confirmButton_.onClick.AddListener(_OnConfirmButton_);
        confirmText = confirmPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        confirmText.text = "Confirm?";

        _cancelButton_ = confirmPanel.GetChild(0).GetChild(2).GetComponent<Button>();
        _cancelButton_.onClick.AddListener(_OnCancelButton_);
    }

    private string GetKeyName(KeyCode keyCode) // Chanhe KeyCode to familiar name
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0:
                return "left Click";
            case KeyCode.Mouse1:
                return "Right Click";
            case KeyCode.Mouse2:
                return "Wheel";
            case KeyCode.Alpha0:
                return "0";
            case KeyCode.Alpha1:
                return "1";
            case KeyCode.Alpha2:
                return "2";
            case KeyCode.Alpha3:
                return "3";
            case KeyCode.Alpha4:
                return "4";
            case KeyCode.Alpha5:
                return "5";
            case KeyCode.Alpha6:
                return "6";
            case KeyCode.Alpha7:
                return "7";
            case KeyCode.Alpha8:
                return "8";
            case KeyCode.Alpha9:
                return "9";
            default:
                return keyCode.ToString();
        }
    }

}
