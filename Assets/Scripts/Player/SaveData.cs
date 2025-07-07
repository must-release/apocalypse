using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;


[JsonObject(MemberSerialization.Fields)]
public class SaveData
{
    /****** Public Memebers *******/

    public ChapterType          CurrentStage        { get => _currentStage; set => _currentStage = value; }
    public PlayerAvatarType     StartCharacter      { get => _startCharacter; set => _startCharacter = value; }
    public List<GameEventDTO>   ActiveEventDTOList  { get => _activeEventDTOList; set => _activeEventDTOList = value; }

    public int      CurrentMap  { get => _currentMap; set => _currentMap = value; }
    public string   PlayTime    { get => _playTime; set => _playTime = value; }
    public string   SaveTime    { get => _saveTime; set => _saveTime = value; }
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

    public SaveData(ChapterType curStage, int curMap, List<GameEventDTO> dtoList, PlayerAvatarType lastChar, string playTime, string saveTime)
    {
        CurrentStage        = curStage;
        CurrentMap          = curMap;
        StartCharacter      = lastChar;
        PlayTime            = playTime;
        SaveTime            = saveTime;
        ActiveEventDTOList  = dtoList;
    }

    public void UpdatePlayerData(ChapterType stage, int map, PlayerAvatarType character)
    {
        CurrentStage    = stage;
        CurrentMap      = map;
        StartCharacter  = character;
    }


    /****** Private Members ******/

    private ChapterType         _currentStage       = ChapterType.ChapterTypeCount;
    private PlayerAvatarType    _startCharacter     = PlayerAvatarType.PlayerAvatarTypeCount;
    private List<GameEventDTO>  _activeEventDTOList;

    private int    _currentMap;
    private string _slotImage; // Base64 string for the image
    private string _playTime = "00:00:00";
    private string _saveTime = "00:00:00";
}