using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIEnums;

public class DataSlot
{
    public UserData slotData;
    public Button slotButton;
    public int slotNumber;

    private Image screenShot;
    private TextMeshProUGUI saveTimeText;
    private TextMeshProUGUI playTimeText;
    private TextMeshProUGUI slotText;

    public DataSlot(Transform slot)
    {
        screenShot = slot.Find("Screenshot Image").GetComponent<Image>();
        saveTimeText = slot.Find("Data Info Box").Find("Save Time Text").GetComponent<TextMeshProUGUI>();
        playTimeText = slot.Find("Data Info Box").Find("Play Time Text").GetComponent<TextMeshProUGUI>();
        slotText = slot.Find("Slot Text").GetComponent<TextMeshProUGUI>();
        slotButton = slot.GetComponent<Button>();
    }

    public void SetSlotInfo(UserData data, int idx)
    {
        if (data != null) // When there is data
        {
            slotData = data;
            if (data.SlotImage != null)
            {
                screenShot.sprite = Sprite.Create(data.SlotImage,
                    new Rect(0.0f, 0.0f, data.SlotImage.width, data.SlotImage.height), new Vector2(0.5f, 0.5f), 100.0f);

            }
            saveTimeText.text = "Save Time: " + data.SaveTime;
            playTimeText.text = "Play TIme: " + data.PlayTime;
        }
        else // When data is null
        {
            slotData = null;
            screenShot.sprite = null;
            saveTimeText.text = "Save Time: - ";
            playTimeText.text = "Play TIme: - ";
        }

        slotNumber = idx;
        if (idx == 0)
        {
            slotText.text = "AUTO";

            // Can not save at auto save slot
            if (UIModel.Instance.CurrentSubUI == SubUI.Save)
            {
                slotButton.interactable = false;
            }
            else
            {
                slotButton.interactable = true;
            }
        }
        else
        {
            slotButton.interactable = true;
            slotText.text = "SLOT " + idx;
        }
    }
}