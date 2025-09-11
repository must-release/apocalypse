using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class FallingObject : MonoBehaviour
    {
        /****** Private Members ******/

        [Header("Damage Setting")]
        [SerializeField] private int _damageValue;

        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            int otherLayer = other.gameObject.layer;

            if (otherLayer == LayerMask.NameToLayer(Layer.Projectile))
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }
            else if (otherLayer == LayerMask.NameToLayer(Layer.Character) && false == other.CompareTag("Player"))
            {
                DamageInfo damageInfo = new DamageInfo(gameObject, _damageValue, false);
                other.gameObject.GetComponent<ICharacter>()?.ApplyDamage(damageInfo);
            }
            else if (otherLayer == LayerMask.NameToLayer(Layer.Ground) && _rb.linearVelocityY < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}