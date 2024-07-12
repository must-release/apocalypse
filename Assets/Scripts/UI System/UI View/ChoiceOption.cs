using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

class ChoiceOption
{
    public Transform optionObject;
    public Button optionButton;
    public TextMeshProUGUI optionText;

    public ChoiceOption(Transform optionObject)
    {
        this.optionObject = optionObject;
        optionButton = optionObject.GetComponent<Button>();
        optionText = optionObject.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Set option info
    public void SetOption(string text)
    {
        optionText.text = text;
        optionObject.gameObject.SetActive(true);
    }

    // Inactive button object
    public void SetOptionInactive()
    {
        optionText.text = null;
        optionObject.gameObject.SetActive(false);
    }
}

