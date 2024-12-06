using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { WEAPON, ACCESSORY, PACK, HELMET, MASK, PISTOL, VEST };

[System.Serializable]
public class Weapon
{
    public string itemName;
    public float itemWeight;
    public ItemType itemType;

}

[RequireComponent(typeof(BoxCollider), typeof(HighlightableObject))]
public class LoadItem : MonoBehaviour
{
    public Weapon weapon;
    private void Start()
    {
        weapon.itemName = transform.name;
    }

    public Weapon GetWeapon() { return weapon; }

}
