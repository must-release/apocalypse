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

    public Coroutine PoolWeapons(WEAPON_TYPE weapon, Queue<WeaponBase> weapons, int poolNum)
    {
        return StartCoroutine(AsyncPoolWeapons(weapon,weapons,poolNum));
    }
    IEnumerator AsyncPoolWeapons(WEAPON_TYPE weapon, Queue<WeaponBase> weapons, int poolNum)
    {
        string weaponPath = "WEAPON_" + weapon.ToString();
        AsyncOperationHandle<GameObject> loadingWeapon = Addressables.InstantiateAsync(weaponPath);
        yield return loadingWeapon;
        if (loadingWeapon.Status == AsyncOperationStatus.Succeeded)
        {
            // Load Weapon in inactive state
            GameObject loadedWeapon = loadingWeapon.Result;
            loadedWeapon.SetActive(false);

            // Copy weapons
            weapons.Enqueue(loadedWeapon.GetComponent<WeaponBase>());
            for (int i = 0; i < poolNum - 1; i++)
            {
                GameObject weaponCopy = Instantiate(loadedWeapon);
                weapons.Enqueue(weaponCopy.GetComponent<WeaponBase>());
            }
        }
        else
        {
            Debug.LogError("Failed to load the weapon: " + weaponPath);
        }
    }
}
