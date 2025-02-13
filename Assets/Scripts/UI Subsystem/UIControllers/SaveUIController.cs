using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UIEnums;


public class SaveUIController : SaveLoadUIBase, IUIController<SubUI>
{
    /****** Public Methods ******/

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

        ResetUISettings();
    }

    public void Cancel()
    {
        if ( false == TryConfirmPanelClose() )
            UIController.Instance.TurnSubUIOff( GetUIType() );
    }

    public SubUI GetUIType() { return SubUI.Save; }


    /****** Protected Members ******/

    protected override void Start()
    {
        base.Start();

        ConfirmText.text    =   _SaveConfirmText;
        LabelText.text      =   _SaveLabelText;
        SaveOrLoad          =   GetUIType();
        
        SetConfirmButtonAction(SaveAtSelectedSlot);
    }

    /****** Private Members ******/

    private const string _SaveConfirmText   =   "Save Data?";
    private const string _SaveLabelText     =   "Save";

    // Save current player data at the selected slot
    private void SaveAtSelectedSlot()
    {
        // Close confirm panel
        TryConfirmPanelClose();

        // Generate Save Game Event Stream. Save data at the selected slot
        GameEventProducer.Instance.GenerateSaveGameEventStream(SelectedSlot.slotNumber);
    }
}