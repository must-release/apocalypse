using UnityEngine;

using EnemyStateController = IStateController<EnemyStateType>;

namespace AD.GamePlay
{
    public abstract class EnemyStateBase : MonoBehaviour
    {
        /****** Public Members ******/

        public void InitializeState(EnemyStateController stateController, EnemyPerception enemyPerception, IEnemyCharacter enemyCharacter)
        {
            Debug.Assert(null != stateController, $"Enemy state controller cannot be null in {StateType}");
            Debug.Assert(null != enemyPerception, $"Enemy perception cannot be null in {StateType}");

            StateController = stateController;
            Perception = enemyPerception;
            OwningCharacter  = enemyCharacter;

            IsInitialized = true;
        }

        public abstract EnemyStateType StateType { get; }
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit(EnemyStateType nextState) { }
        

        /****** Protected Members ******/

        protected EnemyStateController  StateController { get; private set; }
        protected EnemyPerception       Perception      { get; private set; }
        protected IEnemyCharacter       OwningCharacter { get; private set; }
        protected bool                  IsInitialized   { get; private set; }
    }
}