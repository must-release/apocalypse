using UnityEngine;
using UnityEngine.EventSystems;

public class StoryClickPanel : MonoBehaviour, IPointerClickHandler
{
    // �г��� Ŭ���Ǿ��� �� ȣ��Ǵ� �޼ҵ�
    public void OnPointerClick(PointerEventData eventData)
    {
        UIController.Instance.IsStoryPanelClicked = true;
    }
}
