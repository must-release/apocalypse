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

    public Coroutine PoolWeapons( CharacterBase owner, 
                                  WEAPON_TYPE weaponType, 
                                  Queue<WeaponBase> weapons, 
                                  int poolNum,
                                  bool attachToOwner = false
    ) 
    {
        return StartCoroutine(AsyncPoolWeapons(owner, weaponType, weapons, poolNum, attachToOwner));
    }

    IEnumerator AsyncPoolWeapons( CharacterBase owner, 
                                  WEAPON_TYPE weaponType, 
                                  Queue<WeaponBase> weapons, 
                                  int poolNum,
                                  bool attachToOwner
    )
    {
        string weaponPath = "WEAPON_" + weaponType.ToString();
        AsyncOperationHandle<GameObject> loadingWeapon = Addressables.InstantiateAsync(weaponPath);
        yield return loadingWeapon;
        if (loadingWeapon.Status == AsyncOperationStatus.Succeeded)
        {
            // Copy Weapon 
            GameObject loadedWeapon = loadingWeapon.Result;
            CreateAndEnqueueWeapon(loadedWeapon, owner, weapons, poolNum, attachToOwner);
        }
        else
        {
            Debug.LogError("Failed to load the weapon: " + weaponPath);
        }
    }

void CreateAndEnqueueWeapon( GameObject loadedWeapon, 
                             CharacterBase owner, 
                             Queue<WeaponBase> weapons, 
                             int count, 
                             bool attachToOwner
)
{
    for (int i = count; 0 < i; i--)
    {
        GameObject weaponInstance = (i == 1) ? loadedWeapon : Instantiate(loadedWeapon);
        WeaponBase weapon = weaponInstance.GetComponent<WeaponBase>();
        weapon.SetOwner(owner);
        weapons.Enqueue(weapon);
        weaponInstance.SetActive(false);

        if ( attachToOwner )
            weaponInstance.transform.SetParent(owner.transform, false);
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
