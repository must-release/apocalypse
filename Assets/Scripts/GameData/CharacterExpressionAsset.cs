using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterExpressionAsset", menuName = "GameData/CharacterExpressionAsset")]
public class CharacterExpressionAsset : ScriptableObject
{
    /****** Public Members ******/

    public List<HeroExpressionEntry> HeroExpressions;
    public List<HeroineExpressionEntry> HeroineExpressions;


    /****** Protected Members ******/

    protected void OnValidate()
    {
        // Assertions for HeroExpressions
        Debug.Assert(HeroExpressions.Count == (int)HeroExpressionType.HeroExpressionTypeCount, "HeroExpressions count mismatch with HeroExpressionType enum.");
        foreach (var entry in HeroExpressions)
        {
            Debug.Assert(null != entry.Sprite, $"Hero {entry.ExpressionType} expression sprite is not assigned.");
        }

        // Assertions for HeroineExpressions
        Debug.Assert(HeroineExpressions.Count == (int)HeroineExpressionType.HeroineExpressionTypeCount, "HeroineExpressions count mismatch with HeroineExpressionType enum.");
        foreach (var entry in HeroineExpressions)
        {
            Debug.Assert(null != entry.Sprite, $"Heroine {entry.ExpressionType} expression sprite is not assigned.");
        }
    }
}

[System.Serializable]
public class HeroExpressionEntry
{
    public HeroExpressionType ExpressionType;
    public Sprite Sprite;
}

[System.Serializable]
public class HeroineExpressionEntry
{
    public HeroineExpressionType ExpressionType;
    public Sprite Sprite;
}