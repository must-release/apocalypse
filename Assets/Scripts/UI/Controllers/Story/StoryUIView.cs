using UnityEngine;

public class StoryUIView : MonoBehaviour, IUIView<BaseUI>
{
    /****** Public Members ******/

    public BaseUI UIType => BaseUI.Story;
    public DialogueBox DialogueBox => _dialogueBox;

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

    private void OnValidate()
    {
        Debug.Assert(null != _dialogueBox, "DialogueBox is not assigned in StoryUIView.");
    }

}