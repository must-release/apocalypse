using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class CharacterCGView : MonoBehaviour
{
    /****** Public Members ******/

    public void SetSprite(Sprite sprite)
    {
        Debug.Assert(null != sprite, "Sprite to set is null.");

        _characterImage.sprite = sprite;
        _characterImage.SetNativeSize();
    }

    public Tween AsyncFade(float startAlpha, float endAlpha, float duration)
    {
        _characterImage.color = new Color(_characterImage.color.r, _characterImage.color.g, _characterImage.color.b, startAlpha);
        return _characterImage.DOFade(endAlpha, duration);
    }

    public Tween AsyncMove(Vector3 targetPosition, float duration)
    {
        return _rectTransform.DOAnchorPos(targetPosition, duration);
    }

    public void SetPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }


    /****** Private Members ******/

    private Image _characterImage;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _characterImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }
}