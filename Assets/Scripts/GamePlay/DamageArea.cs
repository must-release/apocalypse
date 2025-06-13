using NUnit.Framework;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    /****** Public Members ******/

    public void SetDamageArea(Collider2D characterCollider, DamageInfo damageInfo, bool isDamagingPlayer)
    {
        // Add new Collider
        Collider2D damageCollider = gameObject.AddComponent(characterCollider.GetType()) as Collider2D;

        // Copy collider values
        CopyCollider2DProperties(characterCollider, damageCollider);

        // Additional settings
        damageCollider.isTrigger    = true;
        gameObject.layer            = LayerMask.NameToLayer("Default");
        _damageInfo                 = damageInfo;
        _isDamagingPlayer           = isDamagingPlayer;
    }

    public void SetDamageArea(DamageInfo damageInfo, bool isDamagingPlayer)
    {
        Collider2D damageCollider = GetComponent<Collider2D>();
        Assert.IsTrue(damageCollider != null, "DamageArea must have a Collider2D component.");

        damageCollider.isTrigger    = true;
        gameObject.layer            = LayerMask.NameToLayer("Default");
        _damageInfo                 = damageInfo;
        _isDamagingPlayer           = isDamagingPlayer;
    }

    /****** Private Members ******/

    private DamageInfo  _damageInfo;
    private bool        _isDamagingPlayer;

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (false == _damageInfo.IsContinuousHit)
            return;

        if (other.TryGetComponent(out ICharacter character))
        {
            if(character.IsPlayer == _isDamagingPlayer)
                character.OnDamaged(_damageInfo);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_damageInfo.IsContinuousHit)
            return;

        if (other.TryGetComponent(out ICharacter character))
        {
            if (character.IsPlayer == _isDamagingPlayer)
                character.OnDamaged(_damageInfo);
        }
    }

    private void CopyCollider2DProperties(Collider2D source, Collider2D target)
    {
        if (source is CapsuleCollider2D sourceCapsule && target is CapsuleCollider2D targetCapsule)
        {
            targetCapsule.size = sourceCapsule.size;
            targetCapsule.offset = sourceCapsule.offset;
            targetCapsule.direction = sourceCapsule.direction;
        }
        else if (source is BoxCollider2D sourceBox && target is BoxCollider2D targetBox)
        {
            targetBox.size = sourceBox.size;
            targetBox.offset = sourceBox.offset;
        }
        else if (source is CircleCollider2D sourceCircle && target is CircleCollider2D targetCircle)
        {
            targetCircle.radius = sourceCircle.radius;
            targetCircle.offset = sourceCircle.offset;
        }
        else if (source is PolygonCollider2D sourcePolygon && target is PolygonCollider2D targetPolygon)
        {
            targetPolygon.points = sourcePolygon.points;
        }
        else
        {
            Debug.LogWarning($"Unsupported Collider2D type: {source.GetType()}");
        }
    }
}