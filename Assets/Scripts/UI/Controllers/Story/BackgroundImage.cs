using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BackgroundImage : MonoBehaviour
{
    /****** Public Members ******/

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public Tween Zoom(float scale, float duration)
    {
        return _image.transform.DOScale(scale, duration);
    }

    public void SetScale(float scale)
    {
        _scaleVector.x = scale;
        _scaleVector.y = scale;
        _image.transform.localScale = _scaleVector;
    }


    /****** Private Members ******/

    private Image _image;
    private Vector3 _scaleVector;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _scaleVector = Vector3.one;
    }
}