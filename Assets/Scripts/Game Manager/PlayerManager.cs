using UnityEngine;
using StageEnums;
using CharacterEums;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // Current data of the player
    public STAGE Stage { get; private set; }
    public int Map { get; private set; }
    public CHARACTER Character { get; private set; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetPlayerData(STAGE stage, int map, CHARACTER character)
    {
        Stage = stage;
        Map = map;
        Character = character;
    }

    public void GetPlayerData(out STAGE stage, out int map, out CHARACTER character)
    {
        stage = Stage;
        map = Map;
        character = Character;
    }

    // Get current stage & map info where player is playing.
    public void GetCurrentStageMapInfo(out string stage, out int map)
    {
        stage = Stage.ToString();
        map = Map;
    }
}
