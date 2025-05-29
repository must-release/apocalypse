using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilityManager : MonoBehaviour
{
    public static UtilityManager Instance;

    private Dictionary<Type, IUtilityTool> utilityDictionary;
    private List<IUtilityTool> activeUtilities;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            utilityDictionary = new Dictionary<Type, IUtilityTool>();
            activeUtilities = new List<IUtilityTool>();
        }
    }

    // Add utility tool
    public void AddUtilityTool(IUtilityTool newUtility)
    {
        utilityDictionary[newUtility.GetType()] = newUtility;
    }

    // Get utility tool
    public T GetUtilityTool<T>() where T : IUtilityTool
    {
        if (utilityDictionary.TryGetValue(typeof(T), out IUtilityTool utility))
        {
            if (!activeUtilities.Contains(utility))
            {
                activeUtilities.Add(utility);
            }
            return (T)utility;
        }
        return default;
    }

    // Give utility back
    public void GiveUtilityBack(IUtilityTool utility)
    {
        if (utility != null && activeUtilities.Contains(utility))
        {
            utility.ResetTool();
            activeUtilities.Remove(utility);
        }
    }

    // Reset every utility tool
    public void ResetUtilityTools ()
    {
        foreach (var utility in activeUtilities)
        {
            utility.ResetTool();
        }
        activeUtilities.Clear();
    }
}

public interface IUtilityTool
{
    public void ResetTool();
}