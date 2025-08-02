
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class CharacterCGView : MonoBehaviour
{
    private Image _characterImage;
    private Image CharacterImage
    {
        get
        {
            if (_characterImage == null)
            {
                _characterImage = GetComponent<Image>();
            }
            return _characterImage;
        }
    }

    private RectTransform _rectTransform;
    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (CharacterImage != null && sprite != null)
        {
            CharacterImage.sprite = sprite;
            CharacterImage.SetNativeSize();
        }
    }

    public async UniTask Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = CharacterImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            CharacterImage.color = color;
            await UniTask.Yield();
        }

        color.a = endAlpha;
        CharacterImage.color = color;
    }

    public async UniTask Move(Vector3 targetPosition, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = RectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            RectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            await UniTask.Yield();
        }

        RectTransform.anchoredPosition = targetPosition;
    }

    public void SetPosition(Vector2 position)
    {
        RectTransform.anchoredPosition = position;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
