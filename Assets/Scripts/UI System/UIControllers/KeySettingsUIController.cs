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
    private bool isWaitingForKeyInput = false; // Flag to check if waiting for key input

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
            Debug.Log("Key Settings UI found.");

            keySettingsScroll = keySettingsUI.Find("KeySettingScroll");
            if (keySettingsScroll == null)
            {
                Debug.LogError("Key Settings Scroll Initialization Error");
                return;
            }
            Debug.Log("Key Settings Scroll found.");

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

            foreach (Transform child in content)
            {
                Button button = child.GetComponentInChildren<Button>();
                if (button != null)
                {
                    KeySettingButtons.Add(button);
                    Debug.Log("Button found: " + child.name);
                }
                else
                {
                    Debug.LogWarning("Button not found in: " + child.name);
                }
            }
            Debug.Log("Total keysetting buttons found: " + KeySettingButtons.Count);

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
        if (KeySettingButtons.Count > 3) KeySettingButtons[3].onClick.AddListener(OnJumpButton);
        if (KeySettingButtons.Count > 4) KeySettingButtons[4].onClick.AddListener(OnAttackButton);
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

        UpdateButtonText(content.Find("Up Box/Up Button/UpButtonText"), keySettings.pauseButton.ToString());
        UpdateButtonText(content.Find("Right Box/Right Button/RightButtonText"), keySettings.confirmButton.ToString());
        UpdateButtonText(content.Find("Left Box/Left Button/LeftButtonText"), keySettings.cancelButton.ToString());

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
        Debug.Log("Cancel");
        UIController.Instance.TurnSubUIOff(SUBUI.KEYSETTINGS);
    }

    // �� ��ư�� ���� �Լ���
    private void OnUpButton()
    {
        Debug.Log("Up Button Clicked");
        SetCurrentButtonTextTransform(content.Find("Up Box/Up Button/UpButtonText"), "pauseButton");
    }

    private void OnRightButton()
    {
        Debug.Log("Right Button Clicked");
        SetCurrentButtonTextTransform(content.Find("Right Box/Right Button/RightButtonText"), "confirmButton");
    }

    private void OnLeftButton()
    {
        Debug.Log("Left Button Clicked");
        SetCurrentButtonTextTransform(content.Find("Left Box/Left Button/LeftButtonText"), "cancelButton");
    }

    private void OnJumpButton()
    {
        Debug.Log("Jump Button Clicked");
        SetCurrentButtonTextTransform(content.Find("Jump Box/Jump Button/JumpButtonText"), "jumpButton");
    }

    private void OnAttackButton()
    {
        Debug.Log("Attack Button Clicked");
        SetCurrentButtonTextTransform(content.Find("Attack Box/Attack Button/AttackButtonText"), "attackButton");
    }

    private void OnConfirmButton()
    {
        Debug.Log("Confirm Button Clicked");
        // ���� �ڵ� �״�� ����
    }

    private void OnResetButton()
    {
        Debug.Log("Reset Button Clicked");
        // ���� �ڵ� �״�� ����
    }

    // ���� �ؽ�Ʈ Ʈ�������� �����ϰ� Ű �Է� ��� ���·� ��ȯ�ϴ� �޼���
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

    private string currentKeySettingField;

    // Update �޼��忡�� Ű �Է� ���� �� �ؽ�Ʈ ������Ʈ
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
                        UpdateButtonText(currentButtonTextTransform, keyCode.ToString());
                        UpdateKeySetting(currentKeySettingField, keyCode);
                        isWaitingForKeyInput = false;
                        break;
                    }
                }
            }
        }
    }

    // ��θ� ���� TextMeshProUGUI ������Ʈ�� �ؽ�Ʈ�� �����ϴ� �޼���
    private void UpdateButtonText(Transform buttonTextTransform, string newText)
    {
        if (buttonTextTransform == null)
        {
            Debug.LogError("ButtonText Transform is null");
            return;
        }

        TextMeshProUGUI tmp = buttonTextTransform.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = newText; // Update the text
            Debug.Log($"TextMeshProUGUI component found and text updated to {newText}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found on ButtonText Transform");
        }
    }

    // KeySetting�� ������Ʈ�ϰ� SettingsManager�� �����ϴ� �޼���
    private void UpdateKeySetting(string keySettingField, KeyCode newKey)
    {
        var keySettings = SettingsManager.Instance.KeySettingInfo;

        switch (keySettingField)
        {
            case "pauseButton":
                keySettings.pauseButton = newKey;
                break;
            case "confirmButton":
                keySettings.confirmButton = newKey;
                break;
            case "cancelButton":
                keySettings.cancelButton = newKey;
                break;
                // �ʿ信 ���� �ٸ� Ű ������ �߰�
        }

        SettingsManager.Instance.ChangeKeySettings(keySettings);
    }
}
