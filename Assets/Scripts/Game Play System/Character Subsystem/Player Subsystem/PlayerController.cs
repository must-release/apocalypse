using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class PlayerController : CharacterBase
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

    private void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerRigid.gravityScale = Gravity;
        CharacterHeight = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y;
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
        // Update player's state
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
    public override void ControlCharacter(ControlInfo controlInfo)
    {
        // First, control interactable obejcts. Next, control lower body. Lastly, control upper body.
        ControlInteractionObjects(controlInfo);
        ControlLowerBody(controlInfo);
        //ControlUpperBody(controlInfo);
    }

    // Control player's lower body
    private void ControlLowerBody(ControlInfo controlInfo)
    {
        // Change player state according to the input control info
        if (controlInfo.move != 0) LowerState.Move(controlInfo.move);
        else if (controlInfo.stop) LowerState.Stop();
        if (controlInfo.jump) LowerState.Jump();
        if (controlInfo.tag) LowerState.Tag();
        if (controlInfo.aim != Vector3.zero) LowerState.Aim(true);
        else LowerState.Aim(false);
        LowerState.UpDown(controlInfo.upDown);

        // Change player state according to the object control info
        LowerState.Climb(controlInfo.climb);
        LowerState.Push(controlInfo.push);
    }

    // Control player's upper body
    private void ControlUpperBody(ControlInfo controlInfo)
    {
        CHARACTER_LOWER_STATE lowerState = LowerState.GetState();

        if (lowerState == CHARACTER_LOWER_STATE.PUSHING || lowerState == CHARACTER_LOWER_STATE.TAGGING ||
            lowerState == CHARACTER_LOWER_STATE.CLIMBING)
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

        Debug.Log(LowerState.GetState().ToString() + " -> " + state.ToString());
        LowerState.EndState();
        LowerState = lowerStateDictionary[state];
        LowerState.StartState();
    }

    // Change player's upper body state
    public void ChangeUpperState(CHARACTER_UPPER_STATE state)
    {
        UpperState.EndState();
        UpperState = upperStateDictionary[state];
        UpperState.StartState();
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

    /********** Detect Collision **********/
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(collision.gameObject == StandingGround)
            {
                StandingGround = null;
                StartCoroutine(OnAirDelay());
            }
        }
    }
    IEnumerator OnAirDelay()
    {
        yield return new WaitForSeconds(0.05f);
        if(StandingGround == null) LowerState.OnAir();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Ground")) && StandingOnGround(collision))
        {
            LowerState.OnGround();
            StandingGround = collision.gameObject;
        }
    } 

    private bool StandingOnGround(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        float angle = Vector2.Angle(normal, Vector2.up);
        if(angle > 45) return false;
        else return true;
    }
}