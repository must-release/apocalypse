using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemAsset", menuName = "GameData/SystemAsset")]
public class SystemAsset : ScriptableObject
{
    public List<SystemEntry> SystemAssets;

    public void OnValidate()
    {
        Assert.IsTrue(SystemAssets.Count == (int)SystemType.SystemTypeCount, "SystemAsset count mismatch with SystemType enum.");

        for (int i = 0; i < SystemAssets.Count; i++)
        {
            Assert.IsTrue(i == (int)SystemAssets[i].SystemType, $"SystemAsset[{i}] type mismatch with SystemType enum.");
            Assert.IsTrue(null != SystemAssets[i].SystemPrefab, $"SystemAsset[{i}] prefab is null.");
        }
    }
}

[System.Serializable]
public class SystemEntry
{
    public SystemType SystemType;
    public GameObject SystemPrefab;
}

