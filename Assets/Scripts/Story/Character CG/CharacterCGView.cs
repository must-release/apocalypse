using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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

    public async UniTask AsyncFade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = _characterImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            _characterImage.color = color;
            await UniTask.Yield();
        }

        color.a = endAlpha;
        _characterImage.color = color;
    }

    public async UniTask AsyncMove(Vector3 targetPosition, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = _rectTransform.anchoredPosition;

        while (elapsed < duration)
        { 
            elapsed += Time.deltaTime;
            _rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            await UniTask.Yield();
        }

        _rectTransform.anchoredPosition = targetPosition;
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