using UnityEngine;
using StageEnums;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerType   CurrentPlayerType   { get; private set; }
    public Stage        CurrentStage        { get; private set; }
    public int          CurrentMapNumber    { get; private set; }

    public void SetPlayerData(Stage stage, int map, PlayerType playerType)
    {
        CurrentPlayerType   = playerType;
        CurrentStage        = stage;
        CurrentMapNumber    = map;
    }

    public void GetPlayerData(out Stage stage, out int map, out PlayerType PlayerType)
    {
        PlayerType  = CurrentPlayerType;
        stage       = CurrentStage;
        map         = CurrentMapNumber;
    }

    public void GetStageMapInfo(out string stage, out int map)
    {
        stage = CurrentStage.ToString();
        map = CurrentMapNumber;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
