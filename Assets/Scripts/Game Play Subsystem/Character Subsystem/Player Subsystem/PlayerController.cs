using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class PlayerController : CharacterBase, SceneObejct
{
    public float MovingSpeed { get; private set; } = 15f;
    public float JumpingSpeed { get; private set; } = 30f;
    public float Gravity { get; private set; } = 10f;

    public IPlayer CurrentPlayer { get; private set; }
    public PLAYER CurrentCharacter { get; private set; }
    public ControlInfo CurrentControlInfo {get; private set; }
    public IPlayerLowerState LowerState { get; private set; }
    public IPlayerUpperState UpperState { get; private set; }

    private Dictionary<PLAYER, IPlayer> playerDictionary;
    private Dictionary<PLAYER_LOWER_STATE, IPlayerLowerState> lowerStateDictionary;
    private Dictionary<PLAYER_UPPER_STATE, IPlayerUpperState> upperStateDictionary;

    private Rigidbody2D playerRigid;

    protected override void AwakeCharacter()
    {
        base.AwakeCharacter();

        playerRigid = GetComponent<Rigidbody2D>();
        playerRigid.gravityScale = Gravity;
        CharacterHeight = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y;
        lowerStateDictionary = new Dictionary<PLAYER_LOWER_STATE, IPlayerLowerState>();
        upperStateDictionary = new Dictionary<PLAYER_UPPER_STATE, IPlayerUpperState>();

        // Get player character object
        playerDictionary = new Dictionary<PLAYER, IPlayer>();
        playerDictionary[PLAYER.HERO] = transform.Find("Hero").GetComponent<IPlayer>();
        playerDictionary[PLAYER.HEROINE] = transform.Find("Heroine").GetComponent<IPlayer>();
        CurrentPlayer = playerDictionary[PLAYER.HERO];
        CurrentCharacter = PLAYER.HERO;
    }

    public void Update()
    {
        // Update player's state
        LowerState?.UpdateState();
        UpperState?.UpdateState();
    }

    public bool IsLoaded()
    {
        return playerDictionary[PLAYER.HERO].IsLoaded; // && playerDictionary[CHARACTER.HEROINE].IsLoaded;
    }

    // Set player according to the info
    public void SetPlayer(PLAYER character)
    {
        // Initially change character
        ChangeCharacter(character);

        // Set initial state
        LowerState = lowerStateDictionary[PLAYER_LOWER_STATE.IDLE];
        UpperState = upperStateDictionary[PLAYER_UPPER_STATE.IDLE];
    }

    // Control player according to the control info
    public override void ControlCharacter(ControlInfo controlInfo)
    {
        // Set control info of current frame
        CurrentControlInfo = controlInfo; 

        // First, control interactable obejcts. Next, control lower body. Lastly, control upper body.
        ControlInteractionObjects(controlInfo);
        ControlLowerBody(controlInfo);
        ControlUpperBody(controlInfo);
    }

    // Control player's lower body
    private void ControlLowerBody(ControlInfo controlInfo)
    {
        // Change player state according to the input control info
        if (controlInfo.move != 0) LowerState.Move(controlInfo.move);
        else if (controlInfo.stop) LowerState.Stop();
        if (controlInfo.jump) LowerState.Jump();
        if (controlInfo.tag) LowerState.Tag();
        LowerState.Aim(controlInfo.aim != Vector3.zero);
        LowerState.UpDown(controlInfo.upDown);

        // Change player state according to the object control info
        LowerState.Climb(controlInfo.climb);
        LowerState.Push(controlInfo.push);
    }

    // Control player's upper body
    private void ControlUpperBody(ControlInfo controlInfo)
    {   
        if (LowerState.DisableUpperBody())
        {
            UpperState.Disable();
        }
        else
        {
            UpperState.Enable();
            if (controlInfo.move != 0) UpperState.Move();
            else if (controlInfo.stop) UpperState.Stop();
            if (controlInfo.jump) UpperState.Jump();
            UpperState.Aim(controlInfo.aim);
            if (controlInfo.upDown > 0) UpperState.LookUp(true);
            else UpperState.LookUp(false);
            if (controlInfo.attack) UpperState.Attack();
        }
    }

    // Called once when player is on air
    public override void OnAir()
    { 
        LowerState.OnAir();
        UpperState.OnAir();
    }

    // Called once when player is on ground
    public override void OnGround() 
    { 
        LowerState.OnGround();
        UpperState.OnGround();
    }

    public override void OnDamaged() 
    { 
        LowerState.Damaged(); 
        UpperState.Disable();
    }

    // Change player character
    public void ChangeCharacter(PLAYER character)
    {
        CurrentPlayer.ShowCharacter(false);
        CurrentPlayer = playerDictionary[character];
        CurrentCharacter = character;
        CurrentPlayer.ShowCharacter(true);
    }

    // Change player's lower body state
    public void ChangeLowerState(PLAYER_LOWER_STATE state)
    {
        Debug.Log("Lower : " + LowerState.GetState().ToString() + " -> " + state.ToString());
        LowerState.EndState();
        LowerState = lowerStateDictionary[state];
        LowerState.StartState();
    }

    // Change player's upper body state
    public void ChangeUpperState(PLAYER_UPPER_STATE state)
    {
        Debug.Log("Uppper : " + UpperState.GetState().ToString() + " -> " + state.ToString());
        UpperState.EndState(state);
        UpperState = upperStateDictionary[state];
        UpperState.StartState();
    }

    public void AddLowerState(PLAYER_LOWER_STATE stateKey, IPlayerLowerState state)
    {
        if (!lowerStateDictionary.ContainsKey(stateKey)) lowerStateDictionary[stateKey] = state;
    }

    public void AddUpperState(PLAYER_UPPER_STATE stateKey, IPlayerUpperState state)
    {
        if (!upperStateDictionary.ContainsKey(stateKey)) upperStateDictionary[stateKey] = state;
    }
}