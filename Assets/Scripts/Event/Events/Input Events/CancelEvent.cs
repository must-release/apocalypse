using System.Collections.Generic;

public class CancelEvent : InputEvent, KeySettingsObserver
{
    public void Start()
    {
        SettingsManager.Instance.AddObserver(this);
    }

    // Update key binding
    public void KeySettingsUpdated()
    {
        eventButton = SettingsManager.Instance.KeySettingInfo.cancelButton.buttonKeyCode;
    }

    // Check compatibiliry with event list and current UI
    public override bool CheckCompatibility(List<InputEvent> eventList, BaseUI baseUI, SubUI subUI)
    {
        bool isEventListEmpty = eventList.Count == 0;
        bool isInvalidSubUI = subUI == SubUI.None || subUI == SubUI.Saving;

        return isEventListEmpty && !isInvalidSubUI;
    }

    // Play cancel event
    public override void PlayEvent()
    {
        // Cancel current UI
        UIController.Instance.CancelCurrentUI();

        // Terminate cancel event
        InputEventManager.Instance.TerminateInputEvent(this);
    }
}
