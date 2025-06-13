using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HazardObject : MonoBehaviour, IMapObject
{
    /****** Private Members ******/

    [Header("Damage Info Setting")]
    [SerializeField] private int _damageValue;
    [SerializeField] private bool _isContinuousHit;

    private DamageInfo _damageInfo;

    private void Awake()
    {
        _damageInfo = new DamageInfo(gameObject, _damageValue, _isContinuousHit);
        DamageArea damageArea = gameObject.AddComponent<DamageArea>();
        damageArea.SetDamageArea(_damageInfo, true);
    }
}