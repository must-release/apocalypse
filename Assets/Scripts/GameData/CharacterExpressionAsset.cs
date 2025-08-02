using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterExpressionAsset", menuName = "GameData/CharacterExpressionAsset")]
public class CharacterExpressionAsset : ScriptableObject
{
    /****** Public Members ******/

    public List<CharacterExpressionEntry> rounExpressions;
    public List<CharacterExpressionEntry> minaExpressions;


    /****** Protected Members ******/

    protected void OnValidate()
    {
        // Assertions for CharacterExpressions

        /* Debug.Assert(CharacterExpressions.Count == (int)ExpressionType.ExpressionTypeCount, "CharacterExpressions count mismatch with ExpressionType enum."); */

        foreach (var entry in rounExpressions)
        {
            Debug.Assert(null != entry.Sprite, $"Character Roun's {entry.ExpressionType} expression sprite is not assigned.");
        }

        foreach (var entry in minaExpressions)
        {
            Debug.Assert(null != entry.Sprite, $"Character Mina's {entry.ExpressionType} expression sprite is not assigned.");
        }
    }
}

[System.Serializable]
public class CharacterExpressionEntry
{
    public StoryCharacterStanding.ExpressionType ExpressionType;
    public Sprite Sprite;
}