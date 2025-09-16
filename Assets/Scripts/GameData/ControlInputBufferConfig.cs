using UnityEngine;

[CreateAssetMenu(fileName = "ControlInputBufferConfig", menuName = "ScriptableObjects/ControlInputBufferConfig")]
public class ControlInputBufferConfig : ScriptableObject
{
    
    /****** Public Members ******/
    
    public float JumpBufferDuration     => _jumpBufferDuration;
    public float AttackBufferDuration   => _attackBufferDuration;
    public float CoyoteJumpDuration     => _coyoteJumpDuration;


    /****** Private Members ******/
    
    [SerializeField] private float _jumpBufferDuration = 0.4f;
    [SerializeField] private float _attackBufferDuration = 0.3f;
    [SerializeField] private float _coyoteJumpDuration = 0.2f;
}