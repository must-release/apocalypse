using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ChoicePanel : MonoBehaviour
{
    /****** Public Members ******/

    public event Action<string> OnChoiceSelected;

    public void ShowChoices(string[] options)
    {
        Debug.Assert(null != OnChoiceSelected, "OnChoiceSelected event is not subscribed in ChoicePanel.");

        gameObject.SetActive(true);

        SetChoiceOptions(options);
    }

    public void HideChoices()
    {
        gameObject.SetActive(false);
    }

    /****** Private Members ******/

    [SerializeField] private List<Button> _choiceButtons = new();
    private List<TextMeshProUGUI> _choiceTexts = new();


    private void Awake()
    {
        CacheComponents();
    }

    private void OnValidate()
    {
        Debug.Assert(0 < _choiceButtons.Count, "Choice buttons list cannot be empty.");

        foreach (var button in _choiceButtons)
        {
            Debug.Assert(null != button, "Choice button cannot be null.");
            Debug.Assert(null != button.GetComponentInChildren<TextMeshProUGUI>(), "Choice button must have a TextMeshProUGUI component as a child.");
        }
    }

    private void SetChoiceOptions(string[] options)
    {
        Debug.Assert(options.Length <= _choiceButtons.Count, "More options than buttons available.");

        for (int i = 0; i < options.Length; ++i)
        {
            _choiceButtons[i].gameObject.SetActive(true);
            _choiceTexts[i].text = options[i];
            _choiceButtons[i].onClick.RemoveAllListeners();
            string optionText = options[i];
            _choiceButtons[i].onClick.AddListener(() => OnChoiceSelected.Invoke(optionText));
        }
    }

    private void CacheComponents()
    {
        _choiceTexts.Clear();

        foreach (var button in _choiceButtons)
        {
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            _choiceTexts.Add(text);
        }
    }
}