using UnityEngine;
using UnityEngine.Assertions;

public class ControlInputBuffer : MonoBehaviour
{
    
    /****** Public Members ******/
    
    public bool HasBufferedJump     => 0f < _jumpBufferTimer;
    public bool HasBufferedAttack   => 0f < _attackBufferTimer;
    public bool IsAttackInCooldown  => 0f < _attackCooldownTimer;
    
    public void BufferJump()
    {
        _jumpBufferTimer = _config.JumpBufferDuration;
    }
    
    public void ConsumeJumpBuffer()
    {
        _jumpBufferTimer = 0f;
    }
    
    public void BufferAttack()
    {
        _attackBufferTimer = _config.AttackBufferDuration;
    }
    
    public void ConsumeAttackBuffer()
    {
        _attackBufferTimer = 0f;
    }
    
    
    public void StartAttackCooldown(float cooldownTime)
    {
        _attackCooldownTimer = cooldownTime;
    }


    /****** Private Members ******/
    
    [SerializeField] private ControlInputBufferConfig _config;
    
    private float _jumpBufferTimer;
    private float _attackBufferTimer;
    private float _attackCooldownTimer;

    private void OnValidate()
    {
        Debug.Assert(null != _config, "PlayerInputBufferConfig must be assigned");
    }
    
    private void Update()
    {
        if (_jumpBufferTimer > 0f)
            _jumpBufferTimer -= Time.deltaTime;
            
        if (_attackBufferTimer > 0f)
            _attackBufferTimer -= Time.deltaTime;
            
        if (_attackCooldownTimer > 0f)
            _attackCooldownTimer -= Time.deltaTime;
    }
}