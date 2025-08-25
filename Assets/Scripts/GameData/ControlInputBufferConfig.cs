using UnityEngine;

[CreateAssetMenu(fileName = "ControlInputBufferConfig", menuName = "ScriptableObjects/ControlInputBufferConfig")]
public class ControlInputBufferConfig : ScriptableObject
{
    
    /****** Public Members ******/
    
    public float JumpBufferDuration     => _jumpBufferDuration;
    public float AttackBufferDuration   => _attackBufferDuration;


    /****** Private Members ******/
    
    [SerializeField] private float _jumpBufferDuration = 0.4f;
    [SerializeField] private float _attackBufferDuration = 0.3f;
}