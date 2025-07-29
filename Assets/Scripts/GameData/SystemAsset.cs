using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemAsset", menuName = "GameData/SystemAsset")]
public class SystemAsset : ScriptableObject
{
    public List<SystemEntry> SystemAssets;

    public void OnValidate()
    {
        Debug.Assert(SystemAssets.Count == (int)SystemType.SystemTypeCount, "SystemAsset count mismatch with SystemType enum.");

        for (int i = 0; i < SystemAssets.Count; i++)
        {
            Debug.Assert(i == (int)SystemAssets[i].SystemType, $"SystemAsset[{i}] type mismatch with SystemType enum.");
            Debug.Assert(null != SystemAssets[i].SystemPrefab, $"SystemAsset[{i}] prefab is null.");
        }
    }
}

[System.Serializable]
public class SystemEntry
{
    public SystemType SystemType;
    public GameObject SystemPrefab;
}

