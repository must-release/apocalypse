using UnityEngine;

public class ControlInfo
{
    public Vector2 move;
    public bool jump;
    public bool attack;
    public bool assistAttack;
    public Vector3 aim;
    public bool specialAttack;
    public bool tag;

    public void Reset()
    {
        move = Vector2.zero;
        jump = false;
        attack = false;
        assistAttack = false;
        aim = Vector3.zero;
        specialAttack = false;
        tag = false;
    }
}