
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class CharacterCGModel
{
    public CharacterExpressionAsset CharacterExpressionAsset { get; private set; }
    private Dictionary<string, Sprite> _expressionSprites = new Dictionary<string, Sprite>();

    public async UniTask LoadCharacterExpressionAsset()
    {
        if (CharacterExpressionAsset == null)
        {
            var handle = Addressables.LoadAssetAsync<CharacterExpressionAsset>(AssetPath.CharacterExpressionAsset);
            await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CharacterExpressionAsset = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load CharacterExpressionAsset.");
            }
        }
    }

    public Sprite GetExpressionSprite(string characterID, StoryCharacterStanding.ExpressionType expression)
    {
        string key = $"{characterID}_{expression}";
        if (_expressionSprites.TryGetValue(key, out Sprite sprite))
        {
            return sprite;
        }

        if (CharacterExpressionAsset == null)
        {
            Debug.LogError("CharacterExpressionAsset is not loaded.");
            return null;
        }

        List<CharacterExpressionEntry> targetExpressions = null;
        if (characterID == "나")
        {
            targetExpressions = CharacterExpressionAsset.rounExpressions;
        }
        else if (characterID == "소녀")
        {
            targetExpressions = CharacterExpressionAsset.minaExpressions;
        }

        if (targetExpressions != null)
        {
            foreach (var entry in targetExpressions)
            {
                if (entry.ExpressionType == expression)
                {
                    _expressionSprites[key] = entry.Sprite;
                    return entry.Sprite;
                }
            }
        }

        Debug.LogWarning($"Expression sprite for '{expression}' not found for character '{characterID}'.");
        return null;
    }
}
