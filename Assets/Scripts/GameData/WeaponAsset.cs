using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAsset", menuName = "GameData/WeaponAsset")]
public class WeaponAsset : ScriptableObject
{
    public List<WeaponEntry> WeaponAssets;

    private void OnValidate()
    {
        ValidateAsset();
    }

    private void OnEnable()
    {
        ValidateAsset();
    }

    private void ValidateAsset()
    {
        Assert.IsTrue(WeaponAssets.Count == (int)WeaponType.WeaponTypeCount, "Weapon count mismatch with WeaponType enum.");
        Assert.IsTrue(WeaponAssets.Count == WeaponAssets.Distinct().Count(), "There are duplicate items in weapon asset.");
    }
}

[System.Serializable]
public class WeaponEntry
{
    public WeaponType WeaponType;
    public GameObject WeaponPrefab;
    public int WeaponPoolCount;
    public GameObject AimingDotPrefab;
    public int AimingDotPoolCount;
}