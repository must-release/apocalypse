using UIEnums;


public class LoadUIController : SaveLoadUIBase, IUIController<SubUI>
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

    public SubUI GetUIType() { return SubUI.Load; }


    /****** Protected Members ******/

    protected override void Start()
    {
        base.Start();

        ConfirmText.text    =   _LoadConfirmText;
        LabelText.text      =   _LoadLabelText;
        SaveOrLoad          =   GetUIType();

        SetConfirmButtonAction(LoadSavedData);
    }


    /****** Private Members ******/

    private const string _LoadConfirmText   =   "Load Data?";
    private const string _LoadLabelText     =   "LOAD";

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