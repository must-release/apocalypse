using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using WeaponEnums;
using System.Collections.Generic;
using System.Collections;

public class WeaponFactory : MonoBehaviour
{
    public static WeaponFactory Instance;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public Coroutine PoolWeapons(CharacterBase owner, WEAPON_TYPE weaponType, Queue<WeaponBase> weapons, int poolNum)
    {
        return StartCoroutine(AsyncPoolWeapons(owner, weaponType, weapons, poolNum));
    }
    IEnumerator AsyncPoolWeapons(CharacterBase owner, WEAPON_TYPE weaponType, Queue<WeaponBase> weapons, int poolNum)
    {
        string weaponPath = "WEAPON_" + weaponType.ToString();
        AsyncOperationHandle<GameObject> loadingWeapon = Addressables.InstantiateAsync(weaponPath);
        yield return loadingWeapon;
        if (loadingWeapon.Status == AsyncOperationStatus.Succeeded)
        {
            // Load Weapon in inactive state
            GameObject loadedWeapon = loadingWeapon.Result;
            loadedWeapon.SetActive(false);

            // Copy weapons
            WeaponBase firstWeapon = loadedWeapon.GetComponent<WeaponBase>();
            firstWeapon.SetOwner(owner);
            weapons.Enqueue(firstWeapon);
            for (int i = 0; i < poolNum - 1; i++)
            {
                GameObject weaponCopy = Instantiate(loadedWeapon);
                WeaponBase weapon = weaponCopy.GetComponent<WeaponBase>();
                weapon.SetOwner(owner);
                weapons.Enqueue(weapon);
            }
        }
        else
        {
            Debug.LogError("Failed to load the weapon: " + weaponPath);
        }
    }

    public Coroutine PoolAimingDots(WEAPON_TYPE weapon, List<GameObject> aimingDots, int poolNum)
    {
        return StartCoroutine(AsyncPoolAimingDots(weapon,aimingDots,poolNum));
    }
    IEnumerator AsyncPoolAimingDots(WEAPON_TYPE weapon, List<GameObject> aimingDots, int poolNum)
    {
        string aimingDotsPath = "AIMING_DOT_" + weapon.ToString();
        AsyncOperationHandle<GameObject> loadingDot = Addressables.InstantiateAsync(aimingDotsPath);
        yield return loadingDot;
        if (loadingDot.Status == AsyncOperationStatus.Succeeded)
        {
            // Load dots in inactive state
            GameObject loadedDot = loadingDot.Result;
            loadedDot.SetActive(false);

            // Copy dots
            aimingDots.Add(loadedDot);
            for (int i = 0; i < poolNum - 1; i++)
            {
                GameObject dotCopy = Instantiate(loadedDot);
                aimingDots.Add(dotCopy);
                aimingDots[i].GetComponent<AimingDot>().NextDot = aimingDots[i+1].GetComponent<AimingDot>();
            }
        }
        else
        {
            Debug.LogError("Failed to load the weapon: " + aimingDotsPath);
        }
    }
}
