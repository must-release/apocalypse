using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HazardObject : MonoBehaviour, IMapObject
{
    /****** Private Members ******/

    [Header("Damage Setting")]
    [SerializeField] private int _damageValue;
    [SerializeField] private bool _isContinuousHit;
    [SerializeField] private Collider2D _damageArea;

    private DamageInfo _damageInfo;

    private void Awake()
    {
        Assert.IsTrue(_damageArea != null, $"Damage Area Collider for {gameObject.name} is not assigned in the editor.");

        DamageArea damageArea = _damageArea.gameObject.AddComponent<DamageArea>();
        _damageInfo = new DamageInfo(gameObject, _damageValue, _isContinuousHit);
        damageArea.SetDamageArea(_damageArea, _damageInfo, true);
    }
}