using UnityEngine;
using StageEnums;
using CharacterEums;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // Current data of the player
    public Stage Stage { get; private set; }
    public int Map { get; private set; }
    public PLAYER Character { get; private set; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetPlayerData(Stage stage, int map, PLAYER character)
    {
        Stage = stage;
        Map = map;
        Character = character;
    }

    public void GetPlayerData(out Stage stage, out int map, out PLAYER character)
    {
        stage = Stage;
        map = Map;
        character = Character;
    }

    // Get current stage & map info where player is playing.
    public void GetStageMapInfo(out string stage, out int map)
    {
        stage = Stage.ToString();
        map = Map;
    }
}
