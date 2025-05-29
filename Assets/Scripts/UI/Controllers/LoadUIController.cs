using System.Collections.Generic;

public class LoadUIController : SaveLoadUIBase, IUIController<SubUI>
{
    /****** Public Methods ******/
    public SubUI UIType => SubUI.Load;

    public void EnterUI()
    {
        gameObject.SetActive(true);

        UpdateDataSlots();
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


    /****** Protected Members ******/

    protected override void Start()
    {
        base.Start();

        ConfirmText.text    =   _LoadConfirmText;
        LabelText.text      =   _LoadLabelText;
        SaveOrLoad          =   UIType;

        SetConfirmButtonAction(LoadSavedData);
    }


    /****** Private Members ******/

    private const string _LoadConfirmText   =   "Load Data?";
    private const string _LoadLabelText     =   "LOAD";

    private void LoadSavedData()
    {
        int slotNum = SelectedSlot.slotNumber;

        TryClosingConfirmPanel();

        UIController.Instance.TurnEverySubUIOff();

        var loadEvent = GameEventFactory.CreateSequentialEvent(new List<IGameEvent>
        {
            GameEventFactory.CreateDataLoadEvent(slotNum, false, false),
            GameEventFactory.CreateSceneLoadEvent(SceneEnums.SceneName.StageScene),
            GameEventFactory.CreateSceneActivateEvent()
        });

        GameEventManager.Instance.Submit(loadEvent);
    }
}