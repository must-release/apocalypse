using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ProjectileAsset", menuName = "GameData/ProjectileAsset")]
public class ProjectileAsset : ScriptableObject
{
    public List<ProjectileEntry> ProjectileAssets;

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
        Assert.IsTrue(ProjectileAssets.Count == (int)ProjectileType.ProjectileTypeCount, "Projectile count mismatch with ProjectileType enum.");
        Assert.IsTrue(ProjectileAssets.Count == ProjectileAssets.Distinct().Count(), "There are duplicate items in projectile asset.");
    }
}

[System.Serializable]
public class ProjectileEntry
{
    public ProjectileType SelectedProjectileType;
    public AssetReferenceGameObject ProjectileReference;
    public int ProjectilePoolCount;
    public AssetReferenceGameObject AimingDotReference;
    public int AimingDotPoolCount;
}