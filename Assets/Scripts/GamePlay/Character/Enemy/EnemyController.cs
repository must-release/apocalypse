using System.Collections.Generic;
using UnityEngine;
using AD.GamePlay;

namespace AD.GamePlay
{
    public class EnemyAIController : MonoBehaviour, IStateController<EnemyStateType>
    {
        /****** Public Members ******/

        public void ChangeState(EnemyStateType enemyState)
        {
            Debug.Assert(enemyAIStates.ContainsKey(enemyState), $"Enemy state {enemyState} is not registered.");

            Logger.Write(LogCategory.GamePlay, $"Changing to state: {enemyState}.");

            _currentState?.OnExit(enemyState);
            _currentState = enemyAIStates[enemyState];
            _currentState.OnEnter();
        }


        /****** Private Members ******/

        [SerializeField] private Transform          _enemyAIStateContainer;
        [SerializeField] private EnemyPerception    _enemyPerception;
        [SerializeField] private Transform          _enemyCharacterTransform;

        private Dictionary<EnemyStateType, EnemyStateBase> enemyAIStates = new();

        private EnemyStateBase _currentState;
        private IEnemyCharacter _enemyCharacter;


        private void OnValidate()
        {
            Debug.Assert(null != _enemyAIStateContainer, $"Enemy state container is not assigned in {gameObject.name}.");
            Debug.Assert(null != _enemyPerception, $"Enemy perception is not assigned in {gameObject.name}.");
            Debug.Assert(null != _enemyCharacterTransform.GetComponent<IEnemyCharacter>(), $"Enemy Character is not assigned in {gameObject.name}");
        }

        private void Awake()
        {
            _enemyCharacter = _enemyCharacterTransform.GetComponent<IEnemyCharacter>();
            
            _enemyCharacter.OnCharacterDeath    += () => ChangeState(EnemyStateType.Dead);
            _enemyCharacter.OnCharacterDamaged  += () => ChangeState(EnemyStateType.Chasing);

            RegisterStates();
        }

        private void Start()
        {
            _enemyPerception.InitializePerception(_enemyCharacter);

            ChangeState(EnemyStateType.Patrolling);
        }

        private void Update()
        {
            _currentState.OnUpdate();
        }

        private void RegisterStates()
        {
            Debug.Assert(null != _enemyAIStateContainer.GetComponent<EnemyStateBase>(), $"EnemyStateContainer must have an EnemyAIStateBase component in {gameObject.name}.");

            foreach (var enemyState in _enemyAIStateContainer.GetComponents<EnemyStateBase>())
            {
                Debug.Assert(false == enemyAIStates.ContainsKey(enemyState.StateType));

                enemyState.InitializeState(this, _enemyPerception, _enemyCharacter);
                enemyAIStates.Add(enemyState.StateType, enemyState);
            }
        }
    }
}