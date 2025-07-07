using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerAvatarType   CurrentPlayerType   { get; private set; }
    public ChapterType  CurrentChapter      { get; private set; }
    public int          CurrentStage        { get; private set; }

    public void SetPlayerData(ChapterType chapter, int stage, PlayerAvatarType playerType)
    {
        CurrentPlayerType   = playerType;
        CurrentChapter      = chapter;
        CurrentStage        = stage;
    }

    public void GetPlayerData(out ChapterType chapter, out int stage, out PlayerAvatarType PlayerType)
    {
        PlayerType  = CurrentPlayerType;
        chapter     = CurrentChapter;
        stage       = CurrentStage;
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
