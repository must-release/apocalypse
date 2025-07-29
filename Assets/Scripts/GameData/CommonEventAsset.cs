using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "CommonEventAsset", menuName = "GameData/CommonEventAsset")]
public class CommonEventAsset : ScriptableObject
{
    public List<CommonEventEntry> CommonEventAssets;

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
        Debug.Assert(CommonEventAssets.Count == (int)CommonEventType.CommonEventTypeCount, "Common event count mismatch with Common event enum.");
    }
}

[System.Serializable]
public class CommonEventEntry
{
    public CommonEventType commonEventType;
    public GameEventInfo CommonEvent;
}