using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapons", order = 1)]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;

    public Sprite weaponSprite;
    [TextArea(5, 5)]
    public string weaponDescription;

    public int bulletAmount;
    public int bulletDamage;
    public float fireRate;
    public float shotDuration;
    public float reloadTime;

}
