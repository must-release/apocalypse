using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AD.Story;

[RequireComponent(typeof(Image))]
public class BackgroundImage : MonoBehaviour
{
    /****** Public Members ******/

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public Tween PlayAnimation(StoryBackgroundCG.BackgroundAnimationType animationType, float duration, StoryBackgroundCG.BackgroundPositionType targetPositionType)
    {
        switch (animationType)
        {
            case StoryBackgroundCG.BackgroundAnimationType.Appear:
                return Appear(duration);
            case StoryBackgroundCG.BackgroundAnimationType.Move:
                {
                    Vector2 targetPosition = GetTargetPosition(targetPositionType);
                    return Move(targetPosition, duration);
                }
            case StoryBackgroundCG.BackgroundAnimationType.ZoomIn:
                {
                    Vector2 targetPosition = GetTargetPosition(targetPositionType);
                    return Zoom(scale: 1.3f, targetPosition, duration);
                }
            case StoryBackgroundCG.BackgroundAnimationType.ZoomOut:
                {
                    Vector2 targetPosition = GetTargetPosition(targetPositionType);
                    return Zoom(scale: 0.8f, targetPosition, duration);
                }
            case StoryBackgroundCG.BackgroundAnimationType.Shake:
                return Shake(strength: 10f, duration); // Default strength
            case StoryBackgroundCG.BackgroundAnimationType.Reset:
                return ResetBackground(duration);
            default:
                return DOTween.Sequence();
        }
    }


    /****** Private Members ******/

    private Image _image;

    private Vector2 _leftPosition = new Vector2(150, 0);
    private Vector2 _rightPosition = new Vector2(-150, 0);
    private Vector2 _centerPosition = new Vector2(0, 0);

    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private Color   _originalColor;

    private void Awake()
    {
        _image = GetComponent<Image>();
        
        _originalScale      = _image.transform.localScale;
        _originalPosition   = _image.transform.localPosition;
        _originalColor      = _image.color;
    }

    private Tween Zoom(float scale, Vector2 targetPosition, float duration = 0f)
    {
        var sequence = DOTween.Sequence();
        sequence.Join(_image.transform.DOScale(scale, duration));
        sequence.Join(_image.transform.DOLocalMove(targetPosition, duration));

        return sequence;
    }

    private Tween Appear(float duration)
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
        return _image.DOFade(1f, duration);
    }

    private Tween Move(Vector2 targetPosition, float duration)
    {
        return _image.transform.DOLocalMove(targetPosition, duration);
    }

    private Tween Shake(float strength, float duration)
    {
        return _image.transform.DOShakePosition(duration, strength);
    }

    private Tween ResetBackground(float duration)
    {
        var sequence = DOTween.Sequence();
        sequence.Join(_image.transform.DOLocalMove(_originalPosition, duration));
        sequence.Join(_image.transform.DOScale(_originalScale, duration));
        sequence.Join(_image.DOColor(_originalColor, duration));

        return sequence;
    }

    private Vector2 GetTargetPosition(StoryBackgroundCG.BackgroundPositionType targetPositionType)
    {
        switch (targetPositionType)
        {
            case StoryBackgroundCG.BackgroundPositionType.Left:
                return _leftPosition;
            case StoryBackgroundCG.BackgroundPositionType.Right:
                return _rightPosition;
            case StoryBackgroundCG.BackgroundPositionType.Center:
            default:
                return _centerPosition;
        }
    }
}