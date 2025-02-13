using UnityEngine;
using UnityEngine.EventSystems;

public class StoryClickPanel : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        UIController.Instance.IsStoryPanelClicked = true;
    }
}
