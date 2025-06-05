using NUnit.Framework;
using UnityEngine;

public class WeaponPoolHandler : IPoolHandler<IWeapon>
{
    public WeaponPoolHandler(WeaponType weaponType)
    {
        _weaponType = weaponType;
    }

    public IWeapon Get()
    {
        return PoolManager.Instance.Get<WeaponType, IWeapon>(_weaponType);
    }

    public void Return(IWeapon weapon)
    {
        Assert.IsTrue(null != weapon, $"Trying to return null {_weaponType} weapon");

        PoolManager.Instance.Return(_weaponType, weapon);
    }


    /****** Private Members ******/

    WeaponType _weaponType;
}