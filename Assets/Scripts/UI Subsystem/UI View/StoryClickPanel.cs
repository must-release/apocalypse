using UnityEngine;
using UnityEngine.EventSystems;

public class StoryClickPanel : MonoBehaviour, IPointerClickHandler
{
    // 패널이 클릭되었을 때 호출되는 메소드
    public void OnPointerClick(PointerEventData eventData)
    {
        UIController.Instance.IsStoryPanelClicked = true;
    }
}
