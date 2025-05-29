using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Reflection;

public class KeySettingsUIController : MonoBehaviour, IUIController<SubUI>
{
    /****** Public Members ******/

    public void EnterUI()
    {
        gameObject.SetActive(true);
    }

    public void UpdateUI()
    {

    }

    public void ExitUI()
    {
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        if (_confirmPanel.gameObject.activeInHierarchy)
        {
            _confirmPanel.gameObject.SetActive(false);
        }
        else
        {
            UIController.Instance.TurnSubUIOff(SubUI.KeySettings);
        }
    }

    public SubUI UIType => SubUI.KeySettings;


    /****** Private fields ******/

    private const string _KeyBoxPath = " Key Box";
    private const string _KeyButtonTextPath = " Key Box/Key Button/Key Button Text";
    private const string _ConfirmPanelName = "Confirm Panel";
    private Transform _content;
    private GameObject _keyBox;
    private Transform _currentButtonTextTransform;
    private bool _isWaitingForKeyInput = false;
    private string _currentKeySettingField;
    private Dictionary<string, KeyCode> _originalKeySettings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> _tempKeySettings;
    private Transform _confirmPanel;
    private TextMeshProUGUI _confirmText;
    private TextMeshProUGUI _labelText;
    private Button _confirmButton_;
    private Button _cancelButton_;

    private void Awake()
    {
        _content = transform.Find("KeySettingScroll/Viewport/Content");
        if ( null == _content)
        {
            Debug.LogError("Content Initialization Error");
            return;
        }

        _keyBox = _content.Find("Key Box").gameObject;
        if ( null == _keyBox )
        {
            Debug.LogError("Key Box Initialization Error");
            return;
        }

        Button resetButton = transform.Find("Buttons/Reset Button")?.GetComponent<Button>();
        if ( null != resetButton )
            resetButton.onClick.AddListener(OnResetButton);

        Button confirmButton = transform.Find("Buttons/Confirm Button")?.GetComponent<Button>();
        if ( null != confirmButton )
            confirmButton.onClick.AddListener(OnConfirmButton);

        InitailizeConfirmPanel();
    }

    private void Start()
    {
        // Load existing settings from SettingsManager
        LoadExistingKeySettings();
    }

    // Load existing key settings from SettingsManager
    private void LoadExistingKeySettings()
    {
        // Load key settings
        KeySettings keySettings = SettingsManager.Instance.KeySettingInfo;

        // Create and initialize Key Box objects according to the settings
        FieldInfo[] fields = keySettings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            GameButton gameButton = fields[i].GetValue(keySettings) as GameButton;

            // Instantiate a new keyBox if it's not the first iteration
            GameObject newKeyBox = i > 0 ? Instantiate(_keyBox, _content) : _keyBox;

            // Set the name of the new key box and key text
            newKeyBox.name = gameButton.buttonName + _KeyBoxPath;
            newKeyBox.transform.Find("Key Text").GetComponent<TextMeshProUGUI>().text = gameButton.buttonName;

            // Update the button text
            UpdateButtonText(GetButtonTextTransform(gameButton.buttonName), gameButton.buttonKeyCode);

            // Get the button component and assign the click event
            Button button = newKeyBox.transform.Find("Key Button").GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClick(gameButton));

            // Store the original key settings
            _originalKeySettings[gameButton.buttonName] = gameButton.buttonKeyCode;
        }

        // Initialize tempKeySettings with original values
        _tempKeySettings = new Dictionary<string, KeyCode>(_originalKeySettings);
    }

    private void OnButtonClick(GameButton gameButton)
    {
        SetCurrentButtonTextTransform(GetButtonTextTransform(gameButton.buttonName), gameButton.buttonName);
    }

    // Reset to original key settings
    private void OnResetButton()
    {
        Debug.Log("Reset Button Clicked");

        foreach (var entry in _originalKeySettings)
        {
            // Update tempKeySettings with the original values
            _tempKeySettings[entry.Key] = entry.Value;

            // Update the UI text to reflect the original key bindings
            UpdateButtonText(GetButtonTextTransform(entry.Key), entry.Value);
        }

        Debug.Log("All key settings have been reset to their original values.");
    }

    private void OnConfirmButton() // On KeySetting UI
    {
        //Debug.Log("Confirm Button Clicked");
        _confirmPanel.gameObject.SetActive(true);
    }

    // Update KeySetting
    private void Update()
    {
        if (_isWaitingForKeyInput)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // Update the text with the new key for the current button
                        UpdateButtonText(_currentButtonTextTransform, keyCode);

                        // Temporarily store the new key in tempKeySettings
                        UpdateKeySetting(_currentKeySettingField, keyCode);

                        _isWaitingForKeyInput = false;
                        break;
                    }
                }
            }
        }
    }

    private void SetCurrentButtonTextTransform(Transform buttonTextTransform, string keySettingField)
    {
        _currentButtonTextTransform = buttonTextTransform;
        if (_currentButtonTextTransform == null)
        {
            Debug.LogError($"Transform not found for path: {buttonTextTransform}");
            return;
        }
        _isWaitingForKeyInput = true;
        _currentKeySettingField = keySettingField;
    }

    private void _OnConfirmButton_() // On Confirm Panel
    {
        Debug.Log("_Confirm Button_ Clicked");

        var keySettings = SettingsManager.Instance.KeySettingInfo;

        FieldInfo[] fields = keySettings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            GameButton gameButton = fields[i].GetValue(keySettings) as GameButton;
            gameButton.buttonKeyCode = _tempKeySettings[gameButton.buttonName];
            fields[i].SetValue(keySettings, gameButton);
        }
        

        // Update the originalKeySettings to match the confirmed tempKeySettings
        _originalKeySettings = new Dictionary<string, KeyCode>(_tempKeySettings);

        SettingsManager.Instance.ChangeKeySettings(keySettings);
        _confirmPanel.gameObject.SetActive(false);
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
        foreach (var entry in _tempKeySettings)
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
            KeyCode temp = _tempKeySettings[keySettingField];
            _tempKeySettings[keySettingField] = newKey;
            _tempKeySettings[existingKeyField] = temp;

            // Update the UI text for both fields
            UpdateButtonText(GetButtonTextTransform(keySettingField), newKey);
            UpdateButtonText(GetButtonTextTransform(existingKeyField), temp);
        }
        else
        {
            // Update the current key setting if there's no conflict
            _tempKeySettings[keySettingField] = newKey;

            // Update the UI text for the current button
            UpdateButtonText(_currentButtonTextTransform, newKey);
        }

        // Debug.Log($"Key {newKey} assigned to {keySettingField}");
    }

    private Transform GetButtonTextTransform(string keyButtonName)
    {
        return _content.Find(keyButtonName + _KeyButtonTextPath);
    }

    private void InitailizeConfirmPanel()
    {
        _confirmPanel = transform.Find(_ConfirmPanelName);
        _confirmButton_ = _confirmPanel.GetChild(0).GetChild(1).GetComponent<Button>();
        _confirmButton_.onClick.AddListener(_OnConfirmButton_);
        _confirmText = _confirmPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _confirmText.text = "Confirm?";

        _cancelButton_ = _confirmPanel.GetChild(0).GetChild(2).GetComponent<Button>();
        _cancelButton_.onClick.AddListener(_OnCancelButton_);
    }

    private string GetKeyName(KeyCode keyCode) // Chanhe KeyCode to familiar name
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0:
                return "Left Click";
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
