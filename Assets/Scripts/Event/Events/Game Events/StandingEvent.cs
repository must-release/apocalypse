using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class StandingEvent : GameEventBase<StandingEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved => false;
    public override GameEventType EventType => GameEventType.Standing;

    public static bool IsBlockingAnimationActive { get; private set; } = false;
    public static StandingEvent ActiveStandingEvent { get; private set; } = null;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        // StandingEvent can always be activated immediately.
        // If there are concurrency issues, they should be handled within PlayEventCoroutine
        // or by making StandingEvent an exclusive event if only one can run at a time.
        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not set");

        base.PlayEvent();
        ActiveStandingEvent = this;
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        if (null != _eventCoroutine)
        {
            StopCoroutine( _eventCoroutine );
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<StandingEvent, StandingEventInfo>.Release(this);

        base.TerminateEvent();
    }
    

    /****** Private Methods ******/

    private Coroutine _eventCoroutine = null;
    private CharacterExpressionAsset _characterExpressionAsset;
    // private Coroutine _fadeCoroutine = null;
    // private Coroutine _moveCoroutine = null;
    

    private IEnumerator PlayEventCoroutine()
    {
        Debug.Assert(null != Info, "Event info is not set");

        // setting Name Text
        StoryController.Instance.nameText.text = Info.CharacterID;

        float duration = 1f / Info.AnimationSpeed;

        Vector3 targetPosition = GetTargetPosition(Info.TargetPosition);

        // Load CharacterExpressionAsset if not already loaded
        if (_characterExpressionAsset == null)
        {
            var handle = Addressables.LoadAssetAsync<CharacterExpressionAsset>(AssetPath.CharacterExpressionAsset); // Use AssetPath.CharacterExpressionAsset
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _characterExpressionAsset = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load CharacterExpressionAsset.");
                TerminateEvent();
                yield break;
            }
        }

        // Determine which expression list to use based on CharacterID
        List<CharacterExpressionEntry> targetExpressions = null;
        if (Info.CharacterID == "나")
        {
            targetExpressions = _characterExpressionAsset.rounExpressions;
        }
        else if (Info.CharacterID == "소녀")
        {
            targetExpressions = _characterExpressionAsset.minaExpressions;
        }
        else
        {
            Debug.LogError($"Unknown CharacterID: {Info.CharacterID}. Cannot find expression list.");
            TerminateEvent();
            yield break;
        }

        GameObject character = StoryController.Instance.characters
                                .FirstOrDefault(c => c.name == Info.CharacterID && c.activeInHierarchy);

        if (character == null)
        {
            // If character with ID not found, try to find an inactive one
            character = StoryController.Instance.characters.FirstOrDefault(c => !c.activeInHierarchy);
            if (character != null)
            {
                character.name = Info.CharacterID; // Assign ID to the reused character
                character.SetActive(true);
            }
        }

        if (character == null)
        {
            Debug.LogError($"No available character GameObject found for '{Info.CharacterID}'.");
            TerminateEvent();
            yield break;
        }

        Image characterImage = character.GetComponent<Image>();
        if (characterImage == null)
        {
            Debug.LogError($"Image component not found on character for '{Info.CharacterID}'.");
            TerminateEvent();
            yield break;
        }

        // Set Expression Sprite
        Sprite expressionSprite = targetExpressions
                                    .FirstOrDefault(e => e.ExpressionType == Info.Expression)?.Sprite;

        if (expressionSprite != null)
        {
            characterImage.sprite = expressionSprite;
            // Adjust RectTransform size to match the sprite's native size
            characterImage.SetNativeSize();
        }
        else
        {
            Debug.LogWarning($"Expression sprite for '{Info.Expression}' not found for character '{Info.CharacterID}'.");
        }


        RectTransform characterRectTransform = character.GetComponent<RectTransform>();
        if (characterRectTransform == null)
        {
            Debug.LogError($"RectTransform component not found on character for '{Info.CharacterID}'.");
            TerminateEvent();
            yield break;
        }

        if (Info.IsBlockingAnimation)
        {
            IsBlockingAnimationActive = true;
            // Wait for animation to finish (already handled by yield return in Fade/Move)
        }

        switch (Info.Animation)
        {
            case StoryCharacterStanding.AnimationType.Appear:
                characterRectTransform.anchoredPosition = targetPosition;

                yield return Fade(characterImage, 0f, 1f, duration);
                break;

            case StoryCharacterStanding.AnimationType.Disappear:

                yield return Fade(characterImage, 1f, 0f, duration);
                character.SetActive(false); // Deactivate after fade out
                break;

            case StoryCharacterStanding.AnimationType.Move:
                yield return Move(characterRectTransform, targetPosition, duration);
                break;
        }

        TerminateEvent();
        IsBlockingAnimationActive = false;
    }

    private IEnumerator Fade(Image renderer, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = renderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            renderer.color = color;
            yield return null;
        }

        color.a = endAlpha;
        renderer.color = color;
    }

    private IEnumerator Move(RectTransform rectTransform, Vector3 targetPosition, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    private Vector2 GetTargetPosition(StoryCharacterStanding.TargetPositionType targetPositionType)
    {
        switch (targetPositionType)
        {
            case StoryCharacterStanding.TargetPositionType.Left :
                return new Vector2(-500, -219); // Example position for RectTransform

            case StoryCharacterStanding.TargetPositionType.Right :
                return new Vector2(500, -219); // Example position for RectTransform
                
            case StoryCharacterStanding.TargetPositionType.Center :
            default:
                return new Vector2(0, -219); // Example position for RectTransform
        }
    }
}