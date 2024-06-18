using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;
using static UnityEditor.Progress;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Inventory : MonoBehaviour,ISaveable
{
    public Character character;
    public InventoryDataSO templateInventoryData;
    public int bagSlotNumber;
    public int actionSlotNumber;
    public int EquipmentSlotNumber;
    public InventoryDataSO bagData;
    public InventoryDataSO actionBarData;
    public InventoryDataSO equipmentData;

    [Header("EquipmentShowing")]
    public Transform weaponEqBodyTrans;
    public Transform armorEqBodyTrans;
    public GameObject currentWeaponEq;
    public GameObject currentArmorEq;


    [Header("State")]
    public bool isEquipingWeapon;
    public bool isEquipingArmor;

    public bool isPlayer;
    private void Awake()
    {
        SetFileName();
        InitializeInventory();
        character=GetComponent<Character>();
        
    }
    private void Start()
    {
        UpdateWeapon();
        UpdateArmor();
        isPlayer = GameManager.Instance.playerInventory == this;
    }
    private void OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnregisterSaveable();
    }
    private void InitializeInventory()
    {
        bagData = Instantiate(templateInventoryData);
        actionBarData = Instantiate(templateInventoryData);
        equipmentData = Instantiate(templateInventoryData);
        for (int i = 0; i < bagSlotNumber; i++)
            bagData.items.Add(new InventoryItem());
        for (int i = 0; i < actionSlotNumber; i++)
            actionBarData.items.Add(new InventoryItem()); 
        for (int i = 0; i < EquipmentSlotNumber; i++)
            equipmentData.items.Add(new InventoryItem());
    }
    public bool GainItem(ItemDataSO itemData,int amount)
    {
        int actionBarIndex = actionBarData.FindItemIndex(itemData);
        if (actionBarIndex!=-1&&itemData.isStackable&&itemData.itemType==ItemType.Useable)
        {
            actionBarData.items[actionBarIndex].amount += amount;
            InventoryManager.Instance.actionContainer.UpdateUI();
        }
        else if (!bagData.AddItem(itemData, amount)) return false;

        if (InventoryManager.Instance.bagContainer.currentInventoryData == bagData)
            InventoryManager.Instance.bagContainer.UpdateUI();
        
        if(isPlayer) TaskManager.Instance.AddCollectItemProgress(itemData.id, amount);

        //Auto equipment
        if (itemData.itemType == ItemType.Weapon && !isEquipingWeapon) 
            EquipWeapon(bagData,bagData.indexNewlyAdded);
        if (itemData.itemType == ItemType.Armor && !isEquipingArmor) 
            EquipArmor(bagData, bagData.indexNewlyAdded);
        InventoryManager.Instance.UpdateUI();
        return true;
    }
    public void DropItemEach(InventoryDataSO inventoryData, int index)
    {
        InventoryItem itemToDrop = inventoryData.items[index];

        if (!itemToDrop.itemData) return;

        if (isPlayer) TaskManager.Instance.DecreaseCollectItemProgress(itemToDrop.itemData.id, itemToDrop.amount);

        if (itemToDrop.amount != 0)
        {
            DropItemEach(itemToDrop);
        }
    }
    public void DropItemEach(InventoryItem itemToDrop)
    {
        GameObject itemDropObj =
                Instantiate(itemToDrop.itemData.itemPickUpPrefab,
                transform.position + GameManager.Instance.itemThrowHeight * Vector3.up
                , Quaternion.identity);
        itemDropObj.GetComponent<Rigidbody>().velocity =
            Vector3.zero + transform.forward.normalized * GameManager.Instance.itemThrowSpeed;
        ItemPickUp itemPickUpMono = itemDropObj.GetComponent<ItemPickUp>();
        itemPickUpMono.SetAmount(1);
        itemToDrop.amount--;
        if (itemToDrop.amount == 0) itemToDrop.RemoveItem();
    }
    public void DropItemWhole(InventoryDataSO inventoryData, int index)
    {
        InventoryItem itemToDrop = inventoryData.items[index];

        if (!itemToDrop.itemData) return;

        if (isPlayer) TaskManager.Instance.DecreaseCollectItemProgress(itemToDrop.itemData.id, itemToDrop.amount);

        if (itemToDrop.amount != 0)
        {
            DropItemWhole(itemToDrop);
        }
    }
    public void DropItemWhole(InventoryItem itemToDrop)
    {
        GameObject itemDropObj =
                Instantiate(itemToDrop.itemData.itemPickUpPrefab,
                transform.position + GameManager.Instance.itemThrowHeight * Vector3.up
                , Quaternion.identity);
        itemDropObj.GetComponent<Rigidbody>().velocity =
            Vector3.zero + transform.forward.normalized * GameManager.Instance.itemThrowSpeed;
        ItemPickUp itemPickUpMono = itemDropObj.GetComponent<ItemPickUp>();
        itemPickUpMono.SetAmount(itemToDrop.amount);
        itemToDrop.RemoveItem();
    }


    #region Equipment
    public void EquipWeapon(InventoryDataSO inventoryData,int index)
    {
        if (!inventoryData.items[index].itemData|| !inventoryData.items[index].itemData.weaponData) return;
        if (inventoryData.items[index].itemData.weaponData.weaponType==WeaponType.TwoHand) 
        {
            if (!UnEquipArmor()) return;
            else InventoryManager.Instance.SwapItem(inventoryData, index, equipmentData, 0);
        }
        else
        {
            InventoryManager.Instance.SwapItem(inventoryData, index, equipmentData, 0);
        }
        if (currentWeaponEq != null)
        {
            Destroy(currentWeaponEq.gameObject);
            currentWeaponEq = Instantiate(equipmentData.items[0].itemData.weaponEqPrefab, weaponEqBodyTrans);
        }
        UpdateWeapon();
    }
    public bool UnEquipWeapon()
    {
        if (bagData.AddItem(equipmentData.items[0].itemData, equipmentData.items[0].amount)){
            equipmentData.items[0].RemoveItem();
            UpdateWeapon();
            return true;
        }
        else return false;
    }
    public void EquipArmor(InventoryDataSO inventoryData,int index)
    {
        if (equipmentData.items[0].itemData && equipmentData.items[0].itemData.weaponData.weaponType == WeaponType.TwoHand) return;
        InventoryManager.Instance.SwapItem(inventoryData, index, equipmentData, 1);
        if (currentArmorEq != null)
        {
            Destroy(currentArmorEq.gameObject);
            currentArmorEq = Instantiate(equipmentData.items[1].itemData.armorEqPrefab, armorEqBodyTrans);
        }
        UpdateArmor();
    }
    public bool UnEquipArmor()
    {
        if (bagData.AddItem(equipmentData.items[1].itemData, equipmentData.items[1].amount))
        {
            equipmentData.items[1].RemoveItem();
            UpdateArmor();
            return true;
        }
        else return false;
    }
    public void UseItem(InventoryDataSO inventoryData, int index)
    {
        InventoryItem itemToUse = inventoryData.items[index];
        if (!itemToUse.itemData) return;
        ApplyUseableItem(itemToUse.itemData.useableData);
        if (isPlayer) TaskManager.Instance.DecreaseCollectItemProgress(itemToUse.itemData.id, 1);
        if (itemToUse.itemData.isConsumable&&itemToUse.amount!=0) itemToUse.amount--;


        
    }
    private void ApplyUseableItem(UseableItemDataSO useableItem)
    {
        //Health Recovery
        character.ApplyHealthRecovery(useableItem.healthRecover);
        character.ApplyHealthPercentageRecovery(useableItem.healthPercentageRecover);

    }
    public void UpdateWeapon()
    {
        if (equipmentData.items[0].itemData != null)
        {
            isEquipingWeapon = true;
            character.regularAttackData.ApplyWeaponData(equipmentData.items[0].itemData.weaponData);
            if (currentWeaponEq == null) currentWeaponEq = Instantiate(equipmentData.items[0].itemData.weaponEqPrefab, weaponEqBodyTrans);
        }
        else
        {
            
            isEquipingWeapon = false;
            character.regularAttackData.UnApplyWeaponData();
            if (currentWeaponEq != null)Destroy(currentWeaponEq.gameObject);
        }
        UpdateAnimator();
    }
    public void UpdateArmor()
    {
        if (equipmentData.items[1].itemData != null)
        {
            isEquipingArmor = true;
            character.characterData.ApplyArmorData(equipmentData.items[1].itemData.armorData);
            if (currentArmorEq == null) currentArmorEq = Instantiate(equipmentData.items[1].itemData.armorEqPrefab, armorEqBodyTrans);
        }
        else
        {
            isEquipingArmor = false;
            character.characterData.UnApplyArmorData();
            if(currentArmorEq!= null) Destroy(currentArmorEq.gameObject);
        }
        UpdateAnimator();
    }
    private void UpdateAnimator()
    {
        if (isEquipingWeapon || isEquipingArmor)
        {
            if(isEquipingWeapon&& equipmentData.items[0].itemData.weaponData.weaponType == WeaponType.TwoHand)
            {
                character.animator.runtimeAnimatorController = character.twoHandAnimationController;
            }
            else
            {
                character.animator.runtimeAnimatorController = character.oneHandAnimationController;
            }
        }
        else
        {
            character.animator.runtimeAnimatorController = character.unarmedAnimationController;
        }
    }
    public int GetItemAmount(int itemId)
    {
        int amount = 0;
        amount += bagData.GetItemAmount(itemId);
        amount += actionBarData.GetItemAmount(itemId);
        amount += equipmentData.GetItemAmount(itemId);
        return amount;
    }
    public void DecreaseItem(int itemId,int amountToDecrease)
    {
        if(amountToDecrease<0) amountToDecrease=-amountToDecrease;

        if (isPlayer) TaskManager.Instance.DecreaseCollectItemProgress(itemId, amountToDecrease);

        amountToDecrease =DecreaseItemInInventoryData(bagData,itemId,amountToDecrease);
        if (amountToDecrease <= 0) return;

        amountToDecrease = DecreaseItemInInventoryData(actionBarData, itemId, amountToDecrease);
        if (amountToDecrease <= 0) return;
        amountToDecrease = DecreaseItemInInventoryData(equipmentData, itemId, amountToDecrease);
    }
    private int DecreaseItemInInventoryData(InventoryDataSO inventoryData, int itemId, int amountToDecrease)
    {
        for (int i = 0; i < inventoryData.items.Count; i++)
        {
            InventoryItem item = inventoryData.items[i];
            if (item.itemData != null && item.itemData.id == itemId)
            {
                if (item.amount >= amountToDecrease)
                {
                    item.DecreaseItem(amountToDecrease);
                    amountToDecrease = 0;
                    return amountToDecrease;
                }
                else
                {
                    amountToDecrease -= item.amount;
                    item.DecreaseItem(item.amount);
                }
            }
        }
        return amountToDecrease;
    }


    public DataDefination GetID()
    {
        return GetComponent<DataDefination>();
    }
    string bagSOKeyName;
    string actionBarSOKeyName;
    string equipmentSOKeyName;
    public void SetFileName()
    {
        bagSOKeyName = GetID().ID + "bagSO";
        actionBarSOKeyName = GetID().ID + "actionBarSO";
        equipmentSOKeyName = GetID().ID + "equipmentSO";
    }

    public void SaveData()
    {
        //Save SO
        DataManager.Instance.SaveSO(bagData, bagSOKeyName);
        DataManager.Instance.SaveSO(actionBarData, actionBarSOKeyName);
        DataManager.Instance.SaveSO(equipmentData, equipmentSOKeyName);
    }

    public void LoadData()
    {
        //Load SO
        if (!DataManager.Instance.LoadSO(bagData, bagSOKeyName))
            bagData = Instantiate(templateInventoryData);
        if (!DataManager.Instance.LoadSO(actionBarData, actionBarSOKeyName))
            actionBarData = Instantiate(templateInventoryData);
        if (!DataManager.Instance.LoadSO(equipmentData, equipmentSOKeyName))
            equipmentData = Instantiate(templateInventoryData);
    }
    public void DeleteData()
    {
        //Delete SO
        DataManager.Instance.DeleteSO(bagSOKeyName);
        DataManager.Instance.DeleteSO(actionBarSOKeyName);
        DataManager.Instance.DeleteSO(equipmentSOKeyName);
    }
    #endregion
}
