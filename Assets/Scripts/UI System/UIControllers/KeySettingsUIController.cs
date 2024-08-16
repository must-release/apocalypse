using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UIEnums;
using System;

public class KeySettingsUIController : MonoBehaviour, IUIController
{
    /****** Private fields ******/
    private string keySettingsUIName = "Key Settings UI";
    private Transform keySettingsUI;
    private Transform keySettingsScroll;
    private Transform content;
    private Transform currentButtonTextTransform;
    private List<Button> KeySettingButtons = new List<Button>();
    private bool isWaitingForKeyInput = false;
    private string currentKeySettingField;
    private Dictionary<string, KeyCode> originalKeySettings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> tempKeySettings = new Dictionary<string, KeyCode>();

    string upButtonPath = "Up Box/Up Button/UpButtonText";
    string rightButtonPath = "Right Box/Right Button/RightButtonText";
    string leftButtonPath = "Left Box/Left Button/LeftButtonText";
    string downButtonPath = "Down Box/Down Button/DownButtonText";
    string jumpButtonPath = "Jump Box/Jump Button/JumpButtonText";
    string attackButtonPath = "Attack Box/Attack Button/AttackButtonText";
    string aimButtonPath = "Aim Box/Aim Button/AimButtonText";
    string specialAttackButtonPath = "SpecialAttack Box/SpecialAttack Button/SpecialAttackButton Text";
    string tagButtonPath = "Tag Box/Tag Button/TagButtonText";
    string assistAttackButtonPath = "Assist Attack Box/Assist Attack Button/AssistAttackButtonText";
    string interactButtonPath = "Interact Box/Interact Button/InteractButton Text";
    string cancelButtonPath = "Cancel_Pause Box/Cancel Button/CancelButtonText";
    string confirmButtonPath = "Confirm Box/Confirm Button/ConfirmButton Text";

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
            //Debug.Log("Key Settings UI found.");

            keySettingsScroll = keySettingsUI.Find("KeySettingScroll");
            if (keySettingsScroll == null)
            {
                Debug.LogError("Key Settings Scroll Initialization Error");
                return;
            }
            //Debug.Log("Key Settings Scroll found.");

            Transform viewport = keySettingsScroll.Find("Viewport");
            if (viewport == null)
            {
                Debug.LogError("Viewport Initialization Error");
                return;
            }
            //Debug.Log("Viewport found.");

            content = viewport.Find("Content");
            if (content == null)
            {
                Debug.LogError("Content Initialization Error");
                return;
            }
            //Debug.Log("Content found.");

            foreach (Transform child in content)
            {
                Button button = child.GetComponentInChildren<Button>();
                if (button != null)
                {
                    KeySettingButtons.Add(button);
                    //Debug.Log("Button found: " + child.name);
                }
                else
                {
                    Debug.LogWarning("Button not found in: " + child.name);
                }
            }
            //Debug.Log("Total keysetting buttons found: " + KeySettingButtons.Count);

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
            AssignButtonListeners();
        }
    }

    public void Start()
    {
        // Load existing settings from SettingsManager
        LoadExistingKeySettings();
        
        // Add current UI conroller
        UIController.Instance.AddUIController(SUBUI.KEYSETTINGS, Instance);
    }

    private void AssignButtonListeners()
    {
        if (KeySettingButtons.Count > 0) KeySettingButtons[0].onClick.AddListener(OnUpButton);
        if (KeySettingButtons.Count > 1) KeySettingButtons[1].onClick.AddListener(OnRightButton);
        if (KeySettingButtons.Count > 2) KeySettingButtons[2].onClick.AddListener(OnLeftButton);
        if (KeySettingButtons.Count > 3) KeySettingButtons[3].onClick.AddListener(OnDownButton);
        if (KeySettingButtons.Count > 4) KeySettingButtons[4].onClick.AddListener(OnJumpButton);
        if (KeySettingButtons.Count > 5) KeySettingButtons[5].onClick.AddListener(OnAttackButton);
        if (KeySettingButtons.Count > 6) KeySettingButtons[6].onClick.AddListener(OnAimButton);
        if (KeySettingButtons.Count > 7) KeySettingButtons[7].onClick.AddListener(OnSpecialAttackButton);
        if (KeySettingButtons.Count > 8) KeySettingButtons[8].onClick.AddListener(OnTagButton);
        if (KeySettingButtons.Count > 9) KeySettingButtons[9].onClick.AddListener(OnAssistButton);
        if (KeySettingButtons.Count > 10) KeySettingButtons[10].onClick.AddListener(OnInteractionButton);
        if (KeySettingButtons.Count > 11) KeySettingButtons[11].onClick.AddListener(OnCancel_PauseButton);
        if (KeySettingButtons.Count > 12) KeySettingButtons[12].onClick.AddListener(OnConfirm_Button);
    }

    // Load existing key settings from SettingsManager
    private void LoadExistingKeySettings()
    {
        if (content == null)
        {
            Debug.LogError("Content is not initialized");
            return;
        }

        if (SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsManager.Instance is not initialized");
            return;
        }

        var keySettings = SettingsManager.Instance.KeySettingInfo;

        if (keySettings == null)
        {
            Debug.LogError("SettingsManager.Instance.KeySettingInfo is not initialized");
            return;
        }

        originalKeySettings["pauseButton"] = keySettings.pauseButton;
        originalKeySettings["confirmButton"] = keySettings.confirmButton;
        originalKeySettings["cancelButton"] = keySettings.cancelButton;
        originalKeySettings["upButton"] = keySettings.UpButton;
        originalKeySettings["rightButton"] = keySettings.RightButton;
        originalKeySettings["leftButton"] = keySettings.LeftButton;
        originalKeySettings["downButton"] = keySettings.DownButton;
        originalKeySettings["jumpButton"] = keySettings.JumpButton;
        originalKeySettings["attackButton"] = keySettings.AttackButton;
        originalKeySettings["aimButton"] = keySettings.AimButton;
        originalKeySettings["specialattackButton"] = keySettings.SpecialAttackButton;
        originalKeySettings["tagButton"] = keySettings.TagButton;
        originalKeySettings["assistattackButton"] = keySettings.AssistAttackButton;
        originalKeySettings["interactButton"] = keySettings.InteractionButton;

        // Initialize tempKeySettings with original values
        tempKeySettings = new Dictionary<string, KeyCode>(originalKeySettings);

        // Update UI text with current key settings
        UpdateButtonText(content.Find(upButtonPath), keySettings.UpButton);
        UpdateButtonText(content.Find(rightButtonPath), keySettings.RightButton);
        UpdateButtonText(content.Find(leftButtonPath), keySettings.LeftButton);
        UpdateButtonText(content.Find(downButtonPath), keySettings.DownButton);
        UpdateButtonText(content.Find(jumpButtonPath), keySettings.JumpButton);
        UpdateButtonText(content.Find(attackButtonPath), keySettings.AttackButton);
        UpdateButtonText(content.Find(aimButtonPath), keySettings.AimButton);
        UpdateButtonText(content.Find(specialAttackButtonPath), keySettings.SpecialAttackButton);
        UpdateButtonText(content.Find(tagButtonPath), keySettings.TagButton);
        UpdateButtonText(content.Find(assistAttackButtonPath), keySettings.AssistAttackButton);
        UpdateButtonText(content.Find(interactButtonPath), keySettings.InteractionButton);
        UpdateButtonText(content.Find(cancelButtonPath), keySettings.cancelButton);
        UpdateButtonText(content.Find(confirmButtonPath), keySettings.confirmButton);

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

    private void OnUpButton()
    {
        //Debug.Log("Up Button Clicked");
        SetCurrentButtonTextTransform(content.Find(upButtonPath), "upButton");
    }

    private void OnRightButton()
    {
        //Debug.Log("Right Button Clicked");
        SetCurrentButtonTextTransform(content.Find(rightButtonPath), "rightButton");
    }

    private void OnLeftButton()
    {
        //Debug.Log("Left Button Clicked");
        SetCurrentButtonTextTransform(content.Find(leftButtonPath), "leftButton");
    }

    private void OnDownButton()
    {
        //Debug.Log("Down Button Clicked");
        SetCurrentButtonTextTransform(content.Find(downButtonPath), "downButton");
    }
    private void OnJumpButton()
    {
        //Debug.Log("Jump Button Clicked");
        SetCurrentButtonTextTransform(content.Find(jumpButtonPath), "jumpButton");
    }

    private void OnAttackButton()
    {
        //Debug.Log("Attack Button Clicked");
        SetCurrentButtonTextTransform(content.Find(attackButtonPath), "attackButton");
    }

    private void OnAimButton()
    {
        //Debug.Log("Aim Button Clicked");
        SetCurrentButtonTextTransform(content.Find(aimButtonPath), "aimButton");
    }

    private void OnSpecialAttackButton()
    {
        //Debug.Log("SpecialAttack Button Clicked");
        SetCurrentButtonTextTransform(content.Find(specialAttackButtonPath), "specialattackButton");
    }

    private void OnTagButton()
    {
        //Debug.Log("Tag Button Clicked");
        SetCurrentButtonTextTransform(content.Find(tagButtonPath), "tagButton");
    }

    private void OnAssistButton()
    {
        //Debug.Log("Assist Attack Button Clicked");
        SetCurrentButtonTextTransform(content.Find(assistAttackButtonPath), "assistattackButton");
    }

    private void OnInteractionButton()
    {
        //Debug.Log("Interact Button Clicked");
        SetCurrentButtonTextTransform(content.Find(interactButtonPath), "interactButton");
    }

    private void OnCancel_PauseButton()
    {
        //Debug.Log("Cancel Button Clicked");
        SetCurrentButtonTextTransform(content.Find(cancelButtonPath), "cancelButton");
    }

    private void OnConfirm_Button()
    {
        //Debug.Log("Confirm Button Clicked");
        SetCurrentButtonTextTransform(content.Find(confirmButtonPath), "confirmButton");
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

        foreach (var entry in tempKeySettings)
        {
            switch (entry.Key)
            {
                case "pauseButton":
                    keySettings.pauseButton = entry.Value;
                    break;
                case "confirmButton":
                    keySettings.confirmButton = entry.Value;
                    break;
                case "cancelButton":
                    keySettings.cancelButton = entry.Value;
                    break;
                case "upButton":
                    keySettings.UpButton = entry.Value;
                    break;
                case "rightButton":
                    keySettings.RightButton = entry.Value;
                    break;
                case "leftButton":
                    keySettings.LeftButton = entry.Value;
                    break;
                case "downButton":
                    keySettings.DownButton = entry.Value;
                    break;
                case "jumpButton":
                    keySettings.JumpButton = entry.Value;
                    break;
                case "attackButton":
                    keySettings.AttackButton = entry.Value;
                    break;
                case "aimButton":
                    keySettings.AimButton = entry.Value;
                    break;
                case "specialattackButton":
                    keySettings.SpecialAttackButton = entry.Value;
                    break;
                case "tagButton":
                    keySettings.TagButton = entry.Value;
                    break;
                case "assistattackButton":
                    keySettings.AssistAttackButton = entry.Value;
                    break;
                case "interactButton":
                    keySettings.InteractionButton = entry.Value;
                    break;
            }
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

    private Transform GetButtonTextTransform(string keySettingField)
    {
        switch (keySettingField)
        {
            case "pauseButton":
                return content.Find(cancelButtonPath);
            case "confirmButton":
                return content.Find(confirmButtonPath);
            case "cancelButton":
                return content.Find(cancelButtonPath);
            case "upButton":
                return content.Find(upButtonPath);
            case "rightButton":
                return content.Find(rightButtonPath);
            case "leftButton":
                return content.Find(leftButtonPath);
            case "downButton":
                return content.Find(downButtonPath);
            case "jumpButton":
                return content.Find(jumpButtonPath);
            case "attackButton":
                return content.Find(attackButtonPath);
            case "aimButton":
                return content.Find(aimButtonPath);
            case "specialattackButton":
                return content.Find(specialAttackButtonPath);
            case "tagButton":
                return content.Find(tagButtonPath);
            case "assistattackButton":
                return content.Find(assistAttackButtonPath);
            case "interactButton":
                return content.Find(interactButtonPath);
            default:
                Debug.LogError($"Unknown key setting field: {keySettingField}");
                return null;
        }
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
