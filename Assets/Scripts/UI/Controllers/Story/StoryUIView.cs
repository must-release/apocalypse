using UnityEngine;

public class StoryUIView : MonoBehaviour, IUIView<BaseUI>
{
    /****** Public Members ******/

    public BaseUI UIType => BaseUI.Story;
    public DialogueBox DialogueBox => _dialogueBox;
    public ChoicePanel ChoicePanel => _choicePanel;
    public CharacterCGHolder CharacterHolder => _characterHolder;

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

    }


    /****** Private Members ******/

    [SerializeField] private DialogueBox _dialogueBox;
    [SerializeField] private ChoicePanel _choicePanel;
    [SerializeField] private CharacterCGHolder _characterHolder;

    private void OnValidate()
    {
        Debug.Assert(null != _dialogueBox, "DialogueBox is not assigned in StoryUIView.");
        Debug.Assert(null != _choicePanel, "ChoicePanel is not assigned in StoryUIView.");
        Debug.Assert(null != _characterHolder, "CharacterCGHolder is not assigned in StoryUIView.");
    }

}