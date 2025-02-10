using UIEnums;


public class LoadUIController : SaveLoadUIBase, IUIController<SubUI>
{
    /****** Public Methods ******/

    public void StartUI()
    {
        SetLoadUI();

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

    public SubUI GetUIType() { return SubUI.Load; }

    /****** Private Members ******/

    private const string _LoadConfirmText   =   "Load Data?";
    private const string _LoadLabelText     =   "LOAD";


    // Set SaveLoad UI objects
    private void SetLoadUI()
    {
        ConfirmText.text    =   _LoadConfirmText;
        LabelText.text      =   _LoadLabelText;
        SaveOrLoad          =   GetUIType();
        SetConfirmButtonAction(LoadSavedData);
    }

    // Load saved data of the selected slot
    private void LoadSavedData()
    {
        // Get slot number
        int slotNum = SelectedSlot.slotNumber;

        // Close confirm panel
        TryConfirmPanelClose();

        // Turn every sub UI off
        UIController.Instance.TurnEverySubUIOff();

        // Generate Load Game Event Stream. Load data of the selected slot
        GameEventProducer.Instance.GenerateLoadGameEventStream(slotNum);
    }
}