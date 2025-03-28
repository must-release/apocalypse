using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using CharacterEums;
using WeaponEnums;
using UnityEngine;

public abstract class EnemyStateBase : MonoBehaviour
{
    protected Transform enemyTransform;
    protected EnemyController enemyController;
    protected SpriteRenderer enemySprite;
    protected Rigidbody2D enemyRigid;


    /*** Abstract Funtions ***/
    public abstract ENEMY_STATE GetState();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(ENEMY_STATE nextState);
    public abstract void DetectedPlayer();
    protected abstract void StartEnemyState();


    /*** Virtual Functions ***/
    public virtual void OnDamaged() 
    { 
        if( 0 < enemyController.HitPoint )
            enemyController.ChangeState(ENEMY_STATE.DAMAGED);
        else
            enemyController.ChangeState(ENEMY_STATE.DEAD);
    }


    /*** private Function ***/
    private void Start()
    {
        enemyTransform  =   transform.parent;
        enemyController =   enemyTransform.GetComponent<EnemyController>();
        enemySprite     =   enemyTransform.GetComponent<SpriteRenderer>();
        enemyRigid      =   enemyTransform.GetComponent<Rigidbody2D>();

        StartEnemyState();
    }
}