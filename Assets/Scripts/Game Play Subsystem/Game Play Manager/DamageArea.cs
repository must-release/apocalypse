using UnityEngine;

public class DamageArea : MonoBehaviour
{
    private DamageInfo damageInfo;
    private bool isDamagingPlayer;

    public void SetDamageArea(Collider2D characterCollider, DamageInfo damageInfo, bool IsDamagingPlayer)
    {
        // Add new Collider
        Collider2D damageCollider = (Collider2D)gameObject.AddComponent(characterCollider.GetType());

        // Copy collider values
        CopyCollider2DProperties(characterCollider, damageCollider);

        // Additional settings
        gameObject.layer = LayerMask.NameToLayer("Default");
        damageCollider.isTrigger = true;
        this.damageInfo = damageInfo;
        this.isDamagingPlayer = IsDamagingPlayer;
    }

    private void OnTriggerStay2D(Collider2D other) 
    {   
        // Damage entered character
        if (other.TryGetComponent(out CharacterBase character))
        {
            if(character.CompareTag("Player") == isDamagingPlayer)
                character.OnDamaged(damageInfo);
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