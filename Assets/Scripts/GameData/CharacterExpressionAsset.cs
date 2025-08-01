using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterExpressionAsset", menuName = "GameData/CharacterExpressionAsset")]
public class CharacterExpressionAsset : ScriptableObject
{
    /****** Public Members ******/

    public List<CharacterExpressionEntry> CharacterExpressions;


    /****** Protected Members ******/

    protected void OnValidate()
    {
        // Assertions for CharacterExpressions

        /* Debug.Assert(CharacterExpressions.Count == (int)ExpressionType.ExpressionTypeCount, "CharacterExpressions count mismatch with ExpressionType enum."); */

        foreach (var entry in CharacterExpressions)
        {
            Debug.Assert(null != entry.Sprite, $"Character {entry.ExpressionType} expression sprite is not assigned.");
        }
    }
}

[System.Serializable]
public class CharacterExpressionEntry
{
    public ExpressionType ExpressionType;
    public Sprite Sprite;
}