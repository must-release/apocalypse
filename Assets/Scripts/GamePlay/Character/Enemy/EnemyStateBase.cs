using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public abstract class EnemyStateBase : MonoBehaviour
{
    protected Transform enemyTransform;
    protected EnemyController enemyController;
    protected SpriteRenderer enemySprite;
    protected Rigidbody2D enemyRigid;


    /*** Abstract Funtions ***/
    public abstract EnemyState GetState();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(EnemyState nextState);
    public abstract void DetectedPlayer();


    /*** Virtual Functions ***/
    public virtual void OnDamaged() 
    { 
        if( 0 < enemyController.CurrentHitPoint )
            enemyController.ChangeState(EnemyState.Damaged);
        else
            enemyController.ChangeState(EnemyState.Dead);
    }


    protected virtual void Awake()
    {
        enemyTransform  =   transform.parent;
        enemyController =   enemyTransform.GetComponent<EnemyController>();
        enemySprite     =   enemyTransform.GetComponent<SpriteRenderer>();
        enemyRigid      =   enemyTransform.GetComponent<Rigidbody2D>();
    }
}