using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;


[JsonObject(MemberSerialization.Fields)]
public class UserData
{
    /****** Public Memebers *******/

    public ChapterType    CurrentStage { get => (ChapterType)_currentStage; set => _currentStage = (int)value; }
    public int      CurrentMap { get => _currentMap; set => _currentMap = value; }
    public PlayerType   LastCharacter { get => (PlayerType)_lastCharacter; set => _lastCharacter = (int)value; }
    public string   PlayTime { get => _playTime; set => _playTime = value; }
    public string   SaveTime { get => _saveTime; set => _saveTime = value; }
    public List<GameEventDTO> ActiveEventDTOList { get => _activeEventDTOList; set => _activeEventDTOList = value; }
    public Texture2D SlotImage
    {
        get
        {
            byte[] imageBytes = Convert.FromBase64String(_slotImage);

            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                return texture;
            }
            else
            {
                return null;
            }
        }
        set
        {
            byte[] imageBytes = value.EncodeToPNG();
            _slotImage = Convert.ToBase64String(imageBytes);
        }
    }

    public UserData(ChapterType curStage, int curMap, List<GameEventDTO> dtoList, PlayerType lastChar, string playTime, string saveTime)
    {
        CurrentStage        = curStage;
        CurrentMap          = curMap;
        LastCharacter       = lastChar;
        PlayTime            = playTime;
        SaveTime            = saveTime;
        ActiveEventDTOList  = dtoList;
    }

    public void UpdatePlayerData(ChapterType stage, int map, PlayerType character)
    {
        CurrentStage    = stage;
        CurrentMap      = map;
        LastCharacter   = character;
    }


    /****** Private Members ******/

    private int    _currentStage   = (int)ChapterType.ChapterTypeCount;
    private int    _currentMap     = 0;
    private int    _lastCharacter  = (int)PlayerType.PlayerCount;
    private string _playTime       = "00:00:00";
    private string _saveTime       = "00:00:00";
    private string _slotImage      = null; // Base64 string for the image

    private List<GameEventDTO> _activeEventDTOList = null;
}