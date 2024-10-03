using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NormalInfectee : EnemyController
{

    private Rigidbody2D enemyRigid;
    private float movingSpeed;
    private int movingdirection;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        enemyRigid = GetComponent<Rigidbody2D>();
        movingSpeed = 3f;
        movingdirection = 1;
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if(DetectedPlayer)
        {
            int direction = DetectedPlayer.transform.position.x > transform.position.x ? 1 : -1;
            enemyRigid.velocity = Vector2.right* direction * movingSpeed;

            if(direction !=  movingdirection)
            {
                transform.localScale = new Vector3(-transform.localScale.x, 
                    transform.localScale.y, transform.localScale.z);
                movingdirection = direction;
            }
        }
    }

    public override void ControlCharacter(ControlInfo controlInfo) { return; }
    public override void OnAir() { return; }
    public override void OnGround() { return ;}
    public override void OnDamaged() { return; }
}
