using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UIEnums;


public class SaveUIController : SaveLoadUIBase, IUIController<SubUI>
{
    /****** Public Methods ******/

    public void StartUI()
    {
        SetSaveUI();

        gameObject.SetActive(true);
    }


    public void UpdateUI()
    {
        UpdateDataSlots();
    }

    public void EndUI()
    {
        gameObject.SetActive(false);

        ResetUISettings();
    }

    public void Cancel()
    {
        if ( false == TryConfirmPanelClose() )
            UIController.Instance.TurnSubUIOff( GetUIType() );
    }

    public SubUI GetUIType() { return SubUI.Save; }

    /****** Private Members ******/

    private const string _SaveConfirmText   =   "Save Data?";
    private const string _SaveLabelText     =   "Save";

    private void SetSaveUI()
    {
        ConfirmText.text    =   _SaveConfirmText;
        LabelText.text      =   _SaveLabelText;
        SaveOrLoad          =   GetUIType();
        
        SetConfirmButtonAction(SaveAtSelectedSlot);
    }

    // Save current player data at the selected slot
    private void SaveAtSelectedSlot()
    {
        // Close confirm panel
        TryConfirmPanelClose();

        // Generate Save Game Event Stream. Save data at the selected slot
        GameEventProducer.Instance.GenerateSaveGameEventStream(SelectedSlot.slotNumber);
    }
}