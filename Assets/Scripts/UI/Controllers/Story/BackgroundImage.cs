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
                Vector2 targetPosition = GetTargetPosition(targetPositionType);
                return Move(targetPosition, duration);
            case StoryBackgroundCG.BackgroundAnimationType.ZoomIn:
                return ZoomIn(duration);
            case StoryBackgroundCG.BackgroundAnimationType.ZoomOut:
                return ZoomOut(duration);
            case StoryBackgroundCG.BackgroundAnimationType.Shake:
                return Shake(duration, 10f); // Default strength
            default:
                return DOTween.Sequence();
        }
    }


    /****** Private Members ******/

    private Image _image;
    private Vector3 _scaleVector;

    private Vector2 _leftPosition = new Vector2(-500, 0);
    private Vector2 _rightPosition = new Vector2(500, 0);
    private Vector2 _centerPosition = new Vector2(0, 0);

    private void Awake()
    {
        _image = GetComponent<Image>();
        _scaleVector = Vector3.one;
    }

    private Tween Zoom(float scale, float duration)
    {
        return _image.transform.DOScale(scale, duration);
    }

    private void SetScale(float scale)
    {
        _scaleVector.x = scale;
        _scaleVector.y = scale;
        _image.transform.localScale = _scaleVector;
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

    private Tween ZoomIn(float duration = 1.0f)
    {
        return Zoom(1.2f, duration);
    }

    private Tween ZoomOut(float duration = 1.0f)
    {
        return Zoom(0.8f, duration);
    }

    private Tween Shake(float duration, float strength)
    {
        return _image.transform.DOShakePosition(duration, strength);
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
