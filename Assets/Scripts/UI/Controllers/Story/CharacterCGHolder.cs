using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using AD.Story;

public class CharacterCGHolder : MonoBehaviour
{
    /****** Public Members ******/

    public void SetSprite(string characterID, Sprite sprite)
    {
        CharacterCGView view = GetCharacterView(characterID);
        Debug.Assert(null != view, $"CharacterCGView for ID '{characterID}' not found.");

        view.SetSprite(sprite);
    }

    public Tween DisplayCG(string characterID, float duration, Vector2 targetPosition, StoryCharacterCG.AnimationType animationType)
    {
        CharacterCGView view = GetCharacterView(characterID);
        Debug.Assert(null != view, $"CharacterCGView for ID '{characterID}' not found.");

        switch (animationType)
        {
            case StoryCharacterCG.AnimationType.Appear:
                view.SetPosition(targetPosition);
                view.SetActive(true);
                return Fade(view, 0f, 1f, duration);

            case StoryCharacterCG.AnimationType.Disappear:
                return Fade(view, 1f, 0f, duration)
                    .OnComplete(() => ReleaseCharacterView(characterID));

            case StoryCharacterCG.AnimationType.Move:
                return Move(view, targetPosition, duration);

            default:
                return DOTween.Sequence(); // return dummy tween not to await null
        }
    }


    /****** Private Members ******/

    [SerializeField] private List<CharacterCGView> _characterViews = new List<CharacterCGView>();
    private Dictionary<string, CharacterCGView> _activeCharacters = new Dictionary<string, CharacterCGView>();

    private void Awake()
    {
        foreach (var view in _characterViews)
        {
            view.Initialize();
            view.gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        Debug.Assert(0 < _characterViews.Count, "Character view list cannot be empty.");

        foreach (var view in _characterViews)
        {
            Debug.Assert(null != view, "CharacterCG View cannot be null.");
        }
    }

    private CharacterCGView GetCharacterView(string characterID)
    {
        if (_activeCharacters.TryGetValue(characterID, out CharacterCGView view))
        {
            return view;
        }

        // Find an inactive view from the pool
        CharacterCGView inactiveView = _characterViews.FirstOrDefault(v => false == v.gameObject.activeInHierarchy);
        Debug.Assert(null != inactiveView, $"No available character GameObject found for '{characterID}'.");

        // Assign the inactive view to the characterID and activate it
        _activeCharacters[characterID] = inactiveView;
        return inactiveView;
    }

    private void ReleaseCharacterView(string characterID)
    {
        if (_activeCharacters.TryGetValue(characterID, out CharacterCGView view))
        {
            view.gameObject.SetActive(false);
            _activeCharacters.Remove(characterID);
        }
    }

    private Tween Fade(CharacterCGView view, float startAlpha, float endAlpha, float duration)
    {
        view.CharacterImage.color = new Color(view.CharacterImage.color.r, view.CharacterImage.color.g, view.CharacterImage.color.b, startAlpha);
        return view.CharacterImage.DOFade(endAlpha, duration);
    }

    private Tween Move(CharacterCGView view, Vector3 targetPosition, float duration)
    { 
        return view.RectTransform.DOAnchorPos(targetPosition, duration).SetEase(Ease.OutSine);
    }
}
