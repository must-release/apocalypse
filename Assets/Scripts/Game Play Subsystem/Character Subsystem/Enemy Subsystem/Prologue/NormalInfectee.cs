using UnityEngine;

public class NormalInfectee : EnemyController
{
    private const float PATROL_RANGE_MAX = 10;
    private const float PATROL_RANGE_MIN = 3;
    private float patrolLeftEnd, patrolRightEnd; // Each end side of the partrol range
    private float patrolAnchor; // Center of the patrol range

    


    // Called every frame when patrolling
    public override void Patrol()
    {
        transform.Translate(Vector3.left * Time.deltaTime);
    }
    public override void ControlCharacter(ControlInfo controlInfo) { return; }
    public override void OnAir() { return; }
    public override void OnGround() { return; }
    public override void OnDamaged() 
    {
        gameObject.SetActive(false);
    }
}