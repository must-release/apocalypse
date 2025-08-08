using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SaveUIController : SaveLoadUIBase, IUIView<SubUI>
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
        if (false == TryClosingConfirmPanel())
            UIController.Instance.TurnSubUIOff(UIType);
    }

    public SubUI UIType => SubUI.Save;


    /****** Protected Members ******/

    protected override void Start()
    {
        base.Start();

        ConfirmText.text    =   _SaveConfirmText;
        LabelText.text      =   _SaveLabelText;
        SaveOrLoad          =   UIType;
        
        SetConfirmButtonAction(SaveAtSelectedSlot);
    }

    /****** Private Members ******/
 
    private const string _SaveConfirmText   =   "Save Data?";
    private const string _SaveLabelText     =   "Save";

    private void SaveAtSelectedSlot()
    {
        TryClosingConfirmPanel();
        
        var dataSaveEvent = GameEventFactory.CreateDataSaveEvent(SelectedSlot.slotNumber);
        dataSaveEvent.OnTerminate += UpdateDataSlots;
        GameEventManager.Instance.Submit(dataSaveEvent);
    }
}