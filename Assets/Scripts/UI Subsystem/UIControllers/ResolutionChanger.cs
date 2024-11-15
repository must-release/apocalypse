using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionChanger : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public TextMeshProUGUI resolutionText;
    private Resolution[] resolutions;

    //Set the dropdown options with the resolutions supported by the monitor
    void Start()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogError("Resolution Dropdown is not assigned.");
            return;
        }

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.value); });

        UpdateResolutionText();
    }

    void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogError("Resolutions are not available.");
            return;
        }

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed);
        Debug.Log($"Resolution changed to: {resolution.width} x {resolution.height}");
        UpdateResolutionText();
    }

    void UpdateResolutionText()
    {
        if (resolutionText != null)
        {
            resolutionText.text = $"Current Resolution: {Screen.width} x {Screen.height}";
        }
    }
}
