using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="DataSO/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public int id;
    [Header("Basic Information")]
    public ItemType itemType = ItemType.Useable;
    public string itemName= "";
    public Sprite itemIcon;
    [TextArea]
    public string description = "";
    
    public bool isStackable=true;
    [Header("Pick Up Prefab")]
    public GameObject itemPickUpPrefab;

    
    [Header("Useable")]
    public bool isConsumable=true;
    public UseableItemDataSO useableData;
    [Header("Weapon")]
    public WeaponDataSO weaponData;
    public GameObject weaponEqPrefab;
    [Header("Armor")]
    public ArmorDataSO armorData;
    public GameObject armorEqPrefab;
}

