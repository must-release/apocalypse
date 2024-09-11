 using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody2D playerRigid;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.IDLE, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.IDLE; }

    public void StartState()
    {

    }
    public void UpdateState()
    {

    }
    public void EndState()
    {

    }
    public void Move()
    {

    }
    public void Stop()
    {

    }
    public void Disable()
    {

    }
    public void Enable()
    {

    }
    public void Jump()
    {

    }
    public void Aim()
    {

    }
    public void Attack()
    {

    }
    public void OnAir()
    {

    }
    public void OnGround()
    {

    }
    public void Tag()
    {

    }
    public void Damaged()
    {
        
    }
    public void LookUp(bool loopUp)
    {

    }
    public void Aim(Vector3 aim)
    {

    }
}
