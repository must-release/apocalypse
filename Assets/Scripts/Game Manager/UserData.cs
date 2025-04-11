using System;
using UnityEngine;
using StageEnums;
using CharacterEums;
using System.Collections.Generic;


[Serializable]
public class UserData
{
    /****** Public Memebers *******/

    public Stage    CurrentStage { get => (Stage)_currentStage; set => _currentStage = (int)value; }
    public int      CurrentMap { get => _currentMap; set => _currentMap = value; }
    public PLAYER   LastCharacter { get => (PLAYER)_lastCharacter; set => _lastCharacter = (int)value; }
    public string   PlayTime { get => _playTime; set => _playTime = value; }
    public string   SaveTime { get => _saveTime; set => _saveTime = value; }
    public List<GameEventInfo> ActiveEventInfoList { get => _activeEventInfoList; set => _activeEventInfoList = value; }
    public Texture2D SlotImage
    {
        get
        {
            byte[] imageBytes = Convert.FromBase64String(_slotImage);

            // convert byte array to Texture2D
            Texture2D texture = new Texture2D(2, 2); // Initial size doesn't matter, LoadImage resizes it
            if (texture.LoadImage(imageBytes))
            {
                // Convert Texture2D to Sprite
                return texture;
            }
            else
            {
                return null;
            }
        }
        set
        {
            // Get PNG data
            byte[] imageBytes = value.EncodeToPNG();

            // Convert PNG data to Base64 string
            _slotImage = Convert.ToBase64String(imageBytes);
        }
    }

    public UserData(Stage curStage, int curMap, List<GameEventInfo> infoList, PLAYER lastChar, string playTime, string saveTime)
    {
        CurrentStage    = curStage;
        CurrentMap      = curMap;
        LastCharacter   = lastChar;
        PlayTime        = playTime;
        SaveTime        = saveTime;
        ActiveEventInfoList    = infoList;
    }

    // Update user data
    public void UpdatePlayerData(Stage stage, int map, PLAYER character)
    {
        CurrentStage    = stage;
        CurrentMap      = map;
        LastCharacter   = character;
    }


    /****** Private Members ******/

    [SerializeField] private int _currentStage = (int)Stage.StageCount;
    [SerializeField] private int _currentMap = 0;
    [SerializeField] private int _lastCharacter = (int)PLAYER.PLAYER_COUNT;
    [SerializeField] private string _playTime = "00:00:00";
    [SerializeField] private string _saveTime = "00:00:00";
    [SerializeField] private string _slotImage = null; // Base64 string for the image
    [SerializeField] private List<GameEventInfo> _activeEventInfoList = null;
}