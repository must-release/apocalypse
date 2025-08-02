using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class CharacterCGController : MonoBehaviour
{
    public static CharacterCGController Instance;

    private CharacterCGModel _model;
    private Dictionary<string, CharacterCGView> _characterViews = new Dictionary<string, CharacterCGView>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _model = new CharacterCGModel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        await _model.LoadCharacterExpressionAsset();
    }

    public void RegisterCharacter(string characterID, CharacterCGView view)
    {
        if (!_characterViews.ContainsKey(characterID))
        {
            _characterViews.Add(characterID, view);
        }
    }

    public void HandleCharacterStanding(StoryCharacterStanding standingInfo, Action onComplete)
    {
        HandleCharacterStandingAsync(standingInfo, onComplete).Forget();
    }

    private async UniTaskVoid HandleCharacterStandingAsync(StoryCharacterStanding standingInfo, Action onComplete)
    {
        float duration = 1f / standingInfo.AnimationSpeed;
        Vector2 targetPosition = GetTargetPosition(standingInfo.TargetPosition);

        CharacterCGView characterView = GetCharacterView(standingInfo.Name);
        if (characterView == null)
        {
            Debug.LogError($"No available character GameObject found for '{standingInfo.Name}'.");
            onComplete?.Invoke();
            return;
        }

        Sprite expressionSprite = _model.GetExpressionSprite(standingInfo.Name, standingInfo.Expression);
        if (expressionSprite != null)
        {
            characterView.SetSprite(expressionSprite);
        }

        switch (standingInfo.Animation)
        {
            case StoryCharacterStanding.AnimationType.Appear:
                characterView.SetPosition(targetPosition);
                characterView.SetActive(true);
                await characterView.Fade(0f, 1f, duration);
                break;

            case StoryCharacterStanding.AnimationType.Disappear:
                await characterView.Fade(1f, 0f, duration);
                characterView.SetActive(false);
                break;

            case StoryCharacterStanding.AnimationType.Move:
                await characterView.Move(targetPosition, duration);
                break;
        }

        onComplete?.Invoke();
    }

    private CharacterCGView GetCharacterView(string characterID)
    {
        if (_characterViews.TryGetValue(characterID, out CharacterCGView view) && view.gameObject.activeInHierarchy)
        {
            return view;
        }

        var inactiveView = _characterViews.Values.FirstOrDefault(v => !v.gameObject.activeInHierarchy);
        if (inactiveView != null)
        {
            inactiveView.name = characterID;
            _characterViews.Remove(inactiveView.name);
            _characterViews[characterID] = inactiveView;
            return inactiveView;
        }
        return null;
    }

    private Vector2 GetTargetPosition(StoryCharacterStanding.TargetPositionType targetPositionType)
    {
        switch (targetPositionType)
        {
            case StoryCharacterStanding.TargetPositionType.Left:
                return new Vector2(-500, -219);
            case StoryCharacterStanding.TargetPositionType.Right:
                return new Vector2(500, -219);
            case StoryCharacterStanding.TargetPositionType.Center:
            default:
                return new Vector2(0, -219);
        }
    }
}