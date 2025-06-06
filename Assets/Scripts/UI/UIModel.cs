﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIModel : MonoBehaviour
{
    public static UIModel Instance { get; private set; }

    public BaseUI CurrentBaseUI { get; set; } // Base UI using right now
    public SubUI CurrentSubUI { get; private set; }  // SubUI using right now
    public List<string> ChoiceList { get; set; } // Choice option list
    public string SelectedChoice { get; set; } // Selected choice option

    private Stack<SubUI> savedSubUIs; // Sub UIs which are stacked

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savedSubUIs = new Stack<SubUI>();
        }
    }

    public void Start()
    {
    }

    // Push new sub UI in the stack
    public void PushNewSubUI(SubUI subUI)
    {
        CurrentSubUI = subUI;
        savedSubUIs.Push(subUI);
    }

    // Pop current sub UI in the stack
    public void PopCurrentSubUI()
    {
        if(savedSubUIs.Count == 0)
        {
            Debug.Log("error: no sub UI in the stack");
            return;
        }

        savedSubUIs.Pop();

        // Check previous sub UI
        if(savedSubUIs.Count > 0)
        {
            CurrentSubUI = savedSubUIs.Peek();
        }
        else
        {
            CurrentSubUI = SubUI.None;
        }
    }

}

