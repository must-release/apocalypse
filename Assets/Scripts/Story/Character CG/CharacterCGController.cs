using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class CharacterCGController : MonoBehaviour
{
    /****** Public Members ******/

    public static CharacterCGController Instance { get; private set; }

    public void RegisterCharacter(string characterID, CharacterCGView view)
    {
        if (false == _characterViews.ContainsKey(characterID))
        {
            _characterViews.Add(characterID, view);
        }
    }

    public void HandleCharacterStanding(StoryCharacterStanding standingInfo, Action onComplete)
    {
        AsyncHandleCharacterStanding(standingInfo, onComplete).Forget();
    }


    /****** Private Members ******/

    private CharacterCGModel _model;
    private Dictionary<string, CharacterCGView> _characterViews = new Dictionary<string, CharacterCGView>();
    private Vector2 _leftPosition = new Vector2(-500, -219);
    private Vector2 _rightPosition = new Vector2(500, -219);
    private Vector2 _centerPosition = new Vector2(0, -219);

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            _model = new CharacterCGModel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await _model.AsyncLoadCharacterExpressionAsset();
    }

    private async UniTaskVoid AsyncHandleCharacterStanding(StoryCharacterStanding standingInfo, Action onComplete)
    {
        float duration = 1f / standingInfo.AnimationSpeed;
        Vector2 targetPosition = GetTargetPosition(standingInfo.TargetPosition);

        CharacterCGView characterView = GetCharacterView(standingInfo.Name);

        Sprite expressionSprite = _model.GetExpressionSprite(standingInfo.Name, standingInfo.Expression);

        Debug.Assert(null != expressionSprite, "expressionSprite is null.");
        
        characterView.SetSprite(expressionSprite);

        switch (standingInfo.Animation)
        {
            case StoryCharacterStanding.AnimationType.Appear:
                characterView.SetPosition(targetPosition);
                characterView.SetActive(true);
                await characterView.AsyncFade(0f, 1f, duration);
                break;

            case StoryCharacterStanding.AnimationType.Disappear:
                await characterView.AsyncFade(1f, 0f, duration);
                characterView.SetActive(false);
                break;

            case StoryCharacterStanding.AnimationType.Move:
                await characterView.AsyncMove(targetPosition, duration);
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
        Debug.Assert(null == inactiveView, $"No available character GameObject found for '{characterID}'.");

        inactiveView.name = characterID;
        _characterViews.Remove(inactiveView.name);
        _characterViews[characterID] = inactiveView;
        return inactiveView;
    }

    private Vector2 GetTargetPosition(StoryCharacterStanding.TargetPositionType targetPositionType)
    {
        switch (targetPositionType)
        {
            case StoryCharacterStanding.TargetPositionType.Left:
                return _leftPosition;
            case StoryCharacterStanding.TargetPositionType.Right:
                return _rightPosition;
            case StoryCharacterStanding.TargetPositionType.Center:
            default:
                return _centerPosition;
        }
    }
}