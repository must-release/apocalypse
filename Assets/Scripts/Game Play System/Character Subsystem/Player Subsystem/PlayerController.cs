using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacter
{
    public float MovingSpeed { get; private set; } = 8f;
    public float JumpingSpeed { get; private set; } = 15f;
    public float Gravity { get; private set; } = 4f;

    public IPlayer CurrentPlayer { get; private set; }
    public CHARACTER CurrentCharacter { get; private set; }
    public IPlayerLowerState LowerState { get; private set; }
    public IPlayerUpperState UpperState { get; private set; }

    private Dictionary<CHARACTER, IPlayer> playerDictionary;
    private Dictionary<CHARACTER_LOWER_STATE, IPlayerLowerState> lowerStateDictionary;
    private Dictionary<CHARACTER_UPPER_STATE, IPlayerUpperState> upperStateDictionary;

    private Rigidbody2D playerRigid;
    private GameObject standingGround;

    private void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerRigid.gravityScale = Gravity;
        lowerStateDictionary = new Dictionary<CHARACTER_LOWER_STATE, IPlayerLowerState>();
        upperStateDictionary = new Dictionary<CHARACTER_UPPER_STATE, IPlayerUpperState>();

        // Get player character object
        playerDictionary = new Dictionary<CHARACTER, IPlayer>();
        playerDictionary[CHARACTER.HERO] = transform.Find("Hero").GetComponent<IPlayer>();
        playerDictionary[CHARACTER.HEROINE] = transform.Find("Heroine").GetComponent<IPlayer>();
        CurrentPlayer = playerDictionary[CHARACTER.HERO];
        CurrentCharacter = CHARACTER.HERO;
    }

    public void Update()
    {
        LowerState?.UpdateState();
        UpperState?.UpdateState();
    }

    // Set player according to the info
    public void SetPlayer(CHARACTER character)
    {
        // Initially change character
        ChangeCharacter(character);

        // Set initial state
        LowerState = lowerStateDictionary[CHARACTER_LOWER_STATE.IDLE];


    }

    // Control player according to the control info
    public void ControlCharacter(ControlInfo controlInfo)
    {
        if(controlInfo.move != 0)
        {
            LowerState.Move(controlInfo.move);
        }
        else if(controlInfo.stop)
        {
            LowerState.Stop();
        }

        if (controlInfo.jump)
        {
            LowerState.Jump();
        }

        if (controlInfo.tag)
        {
            LowerState.Tag();
        }
    }

    // Change player character
    public void ChangeCharacter(CHARACTER character)
    {
        CurrentPlayer.ShowCharacter(false);
        CurrentPlayer = playerDictionary[character];
        CurrentCharacter = character;
        CurrentPlayer.ShowCharacter(true);
    }

    // Change player's lower body state
    public void ChangeLowerState(CHARACTER_LOWER_STATE state)
    {
        LowerState.EndState();
        LowerState = lowerStateDictionary[state];
        LowerState.StartState();
        Debug.Log(LowerState.GetState().ToString());
    }

    // Change player's upper body state
    public void ChangeUpperState(CHARACTER_UPPER_STATE state)
    {
        UpperState.EndState();
        UpperState = upperStateDictionary[state];
        UpperState.StartState();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            if(collision.gameObject == standingGround)
            {
                LowerState.OnAir();
                standingGround = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            LowerState.OnGround();
            standingGround = collision.gameObject;
        }
    }

    public void AddLowerState(CHARACTER_LOWER_STATE stateKey, IPlayerLowerState state)
    {
        if (!lowerStateDictionary.ContainsKey(stateKey))
        {
            lowerStateDictionary[stateKey] = state;
        }
    }

    public void AddUpperState(CHARACTER_UPPER_STATE stateKey, IPlayerUpperState state)
    {
        if (!upperStateDictionary.ContainsKey(stateKey))
        {
            upperStateDictionary[stateKey] = state;
        }
    }
}

public interface IPlayer
{
    public void ShowCharacter(bool value);
}

public interface IPlayerLowerState
{
    public CHARACTER_LOWER_STATE GetState();
    public void StartState();
    public void UpdateState();
    public void EndState();

    public void Move(int move);
    public void Stop();
    public void UpDown(int upDown);
    public void Jump();
    public void Aim();
    public void OnAir();
    public void OnGround();
    public void Tag();
    public void Damaged();
}

public interface IPlayerUpperState
{
    public CHARACTER_UPPER_STATE GetState();
    public void StartState();
    public void UpdateState();
    public void EndState();
    public void Move(int move);
    public void Stop();
    public void Disable();
    public void Enable();
    public void Jump();
    public void Aim();
    public void OnAir();
    public void OnGround();
    public void Tag();
    public void Damaged();
}