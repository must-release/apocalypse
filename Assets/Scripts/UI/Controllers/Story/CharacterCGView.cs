using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CharacterCGView : MonoBehaviour
{
    /****** Public Members ******/

    public Image CharacterImage { get; private set; }
    public RectTransform RectTransform { get; private set; }

    public void SetSprite(Sprite sprite)
    {
        Debug.Assert(null != sprite, "Sprite to set is null.");

        CharacterImage.sprite = sprite;
    }

    public void SetPosition(Vector2 position)
    {
        RectTransform.anchoredPosition = position;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Initialize()
    {
        CharacterImage = GetComponent<Image>();
        RectTransform = GetComponent<RectTransform>();
        CharacterImage.preserveAspect = true;
    }
}
