using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SlotUI : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerMoveHandler,IPointerExitHandler
{
    
    public SlotType slotType=SlotType.Bag;
    public SlotItemUI currentSlotItem;
    public DisplaySlotItemUI displaySlotItem;

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.Bag:
                break;
            case SlotType.Weapon:
                GetCurrentInventory().UpdateWeapon();
                break;
            case SlotType.Armor:
                GetCurrentInventory().UpdateArmor();
                break;
            case SlotType.Action:
                break;
            case SlotType.Display:
                break;
        }
        currentSlotItem.UpdateItemUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            switch (slotType)
            {
                case SlotType.Bag:
                    if(GetItemData().itemType==ItemType.Weapon) GetCurrentInventory().EquipWeapon(GetInventoryData(),GetSlotIndex());
                    else if(GetItemData().itemType==ItemType.Armor)GetCurrentInventory().EquipArmor(GetInventoryData(),GetSlotIndex());
                    else if(GetItemData().itemType==ItemType.Useable) GetCurrentInventory().UseItem(GetInventoryData(),GetSlotIndex());
                    break;
                case SlotType.Weapon:
                    if(GetItemData()!=null) GetCurrentInventory().UnEquipWeapon();
                    break;
                case SlotType.Armor:
                    if (GetItemData() != null) GetCurrentInventory().UnEquipArmor();
                    break;
                case SlotType.Action:
                    GetCurrentInventory().UseItem(GetInventoryData(), GetSlotIndex());
                    break;
                case SlotType.Display:
                    break;
            }
            InventoryManager.Instance.UpdateUI();
        }
    }
    private void ShowItemTip(ItemDataSO item)
    {
        InventoryManager.Instance.itemTip.SetUpUI(item);
        InventoryManager.Instance.itemTip.gameObject.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotType == SlotType.Display)
        {
            ShowItemTip(displaySlotItem.itemToDisplay.itemData);
            return;
        }
        if (GetItemData() != null)
        {
            ShowItemTip(GetItemData());
        }

    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (slotType == SlotType.Display)
        {
            ShowItemTip(displaySlotItem.itemToDisplay.itemData);
            return;
        }
        if (GetItemData() != null)
        {
            ShowItemTip(GetItemData());
        }
        else
        {
            InventoryManager.Instance.itemTip.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.itemTip.gameObject.SetActive(false);
    }
    public Inventory GetCurrentInventory()
    {
        return InventoryManager.Instance.currentInventory;
    }
    public InventoryItem GetInventoryItem()
    {
        if (slotType == SlotType.Display) return displaySlotItem.itemToDisplay;
        else return currentSlotItem.GetInventoryItem();

    }
    public ItemDataSO GetItemData()
    {
        if (slotType == SlotType.Display) return displaySlotItem.itemToDisplay.itemData;
        else return currentSlotItem.GetItemData();

    }
    public InventoryDataSO GetInventoryData()
    {
        return currentSlotItem.currentInventoryData;
    }
    public int GetSlotIndex()
    {
        return currentSlotItem.index;
    }

    
    void OnDisable()
    {
        InventoryManager.Instance.itemTip.gameObject.SetActive(false);
    }

    
}
