using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "UIAsset", menuName = "GameData/UIAsset")]
public class UIAsset : ScriptableObject
{
    public List<BaseUIEntry> BaseUIAssets;
    public List<SubUIEntry> SubUIAssets;

    public void OnValidate()
    {
        Debug.Assert(BaseUIAssets.Count == (int)BaseUI.BaseUICount, "Base UI count mismatch with BaseUI enum.");
        Debug.Assert(SubUIAssets.Count == (int)SubUI.SubUICount - 1, "Sub UI count mismatch with SubUI enum."); // excluding 'none' of SubUI Enum
    }
}

[System.Serializable]
public class BaseUIEntry
{
    public BaseUI BaseUIType;
    public GameObject BaseUIPrefab;
}

[System.Serializable]
public class SubUIEntry
{
    public SubUI SubUIType;
    public GameObject SubUIPrefab;
}
