using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class WeaponFactory : MonoBehaviour
{
    public static WeaponFactory Instance { get; private set; } = null;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator AsyncPoolWeapons(GameObject owner, 
                                        WeaponType weaponType, 
                                        Queue<IWeapon> weapons, 
                                        int poolNum,
                                        bool attachToOwner = false
    )
    {
        AsyncOperationHandle<GameObject> loadingWeapon = Addressables.InstantiateAsync(WeaponAsset.GetWeaponPath(weaponType));
        yield return loadingWeapon;
        if (loadingWeapon.Status == AsyncOperationStatus.Succeeded)
        {
            // Copy Weapon 
            GameObject loadedWeapon = loadingWeapon.Result;
            CreateAndEnqueueWeapon(loadedWeapon, owner, weapons, poolNum, attachToOwner);
        }
        else
        {
            Debug.LogError("Failed to load the weapon: " + WeaponAsset.GetWeaponPath(weaponType));
        }
    }

    public IEnumerator AsyncPoolAimingDots(WeaponType weaponType, List<GameObject> aimingDots, int poolNum)
    {
        AsyncOperationHandle<GameObject> loadingDot = Addressables.InstantiateAsync(WeaponAsset.GetAimingDotPath(weaponType));
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
                aimingDots[i].GetComponent<AimingDot>().NextDot = aimingDots[i + 1].GetComponent<AimingDot>();
            }
        }
        else
        {
            Debug.LogError("Failed to load the weapon: " + WeaponAsset.GetAimingDotPath(weaponType));
        }
    }


    private void CreateAndEnqueueWeapon(GameObject loadedWeapon,
                                        GameObject owner,
                                        Queue<IWeapon> weapons,
                                        int count,
                                        bool attachToOwner
)
    {
        for (int i = count; 0 < i; i--)
        {
            GameObject weaponInstance = (i == 1) ? loadedWeapon : Instantiate(loadedWeapon);
            IWeapon weapon = weaponInstance.GetComponent<IWeapon>();
            weapon.SetOwner(owner);
            weapons.Enqueue(weapon);
            weaponInstance.SetActive(false);

            if (attachToOwner)
                weaponInstance.transform.SetParent(owner.transform, false);
        }
    }
}
