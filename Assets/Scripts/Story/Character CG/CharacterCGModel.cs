using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class CharacterCGModel
{
    /****** Public Members ******/

    public CharacterExpressionAsset CharacterExpressionAsset { get; private set; }

    public async UniTask AsyncLoadCharacterExpressionAsset()
    {
        if (null == CharacterExpressionAsset)
        {
            var handle = Addressables.LoadAssetAsync<CharacterExpressionAsset>(AssetPath.CharacterExpressionAsset);
            await handle.ToUniTask();

            Debug.Assert(handle.Status == AsyncOperationStatus.Succeeded, "Failed to load CharacterExpressionAsset.");

            CharacterExpressionAsset = handle.Result;
        }
    }

    public Sprite GetExpressionSprite(string characterID, StoryCharacterCG.FacialExpressionType expression)
    {
        Debug.Assert(null != CharacterExpressionAsset, "CharacterExpressionAsset is not loaded.");

        string key = $"{characterID}_{expression}";
        if (_expressionSprites.TryGetValue(key, out Sprite sprite))
        {
            return sprite;
        }

        List<CharacterExpressionEntry> targetExpressions = null;
        if (string.Equals(characterID, "나"))
        {
            targetExpressions = CharacterExpressionAsset.HeroExpressions;
        }
        else if (string.Equals(characterID, "소녀"))
        {
            targetExpressions = CharacterExpressionAsset.HeroineExpressions;
        } // *** must refactor ***

        Debug.Assert(null != targetExpressions, $"Expression sprite for '{expression}' not found for character '{characterID}'.");

        foreach (var entry in targetExpressions)
        {
            if (entry.ExpressionType == expression)
            {
                _expressionSprites[key] = entry.Sprite;
                return entry.Sprite;
            }
        }
        
        return null;
    }


    /****** Private Members ******/

    private Dictionary<string, Sprite> _expressionSprites = new Dictionary<string, Sprite>();
}