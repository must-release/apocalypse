using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Collider2D))]
    public class HazardObject : MonoBehaviour
    {
        /****** Private Members ******/

        [Header("Damage Setting")]
        [SerializeField] private int _damageValue;
        [SerializeField] private bool _isContinuousHit;
        [SerializeField] private Collider2D _damageArea;

        private DamageInfo _damageInfo;

        private void Awake()
        {
            Debug.Assert(_damageArea != null, $"Damage Area Collider for {gameObject.name} is not assigned in the editor.");

            DamageArea damageArea = _damageArea.gameObject.AddComponent<DamageArea>();
            _damageInfo = new DamageInfo(gameObject, _damageValue, _isContinuousHit);
            damageArea.SetDamageArea(_damageInfo, true, LayerMask.NameToLayer(Layer.Obstacle));
        }
    }
}