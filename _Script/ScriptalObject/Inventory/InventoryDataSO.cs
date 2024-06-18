using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="DataSO/InventoryDataSO")]
public class InventoryDataSO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public InventoryItem itemNewlyAdded = null;
    public int indexNewlyAdded;
    
    public bool AddItem(ItemDataSO newItem,int amount)
    {
        if (newItem == null) return true;
        if (newItem.isStackable)
        {
            foreach (InventoryItem item in items)
            {
                if(item.itemData!=null&&item.itemData.id == newItem.id)
                {
                    item.amount += amount;
                    return true;
                }
            }
        }
        for(int i=0;i<items.Count; i++)
        {
            if (items[i].itemData==null)
            {
                items[i].itemData = newItem;
                items[i].amount = amount;
                itemNewlyAdded = items[i];
                indexNewlyAdded = i;
                return true;
            }
        }
        return false;
    }
    
    public bool AddItemInstantiate(ItemDataSO itemToInstantiate,int amount)
    {
        ItemDataSO newItem=Instantiate(itemToInstantiate);
        if(AddItem(newItem, amount))return true;
        else return false;
    }
    
    public int FindItemIndex(ItemDataSO item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemData != null && items[i].itemData.id==item.id)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetItemAmount(int itemIdToCount)
    {
        int amount = 0;
        foreach (InventoryItem itemInInventory in items)
        {
            if (itemInInventory.itemData != null && itemInInventory.itemData.id == itemIdToCount)
            {
                amount += itemInInventory.amount;
            }
        }
        return amount;
    }
    public void DecreaseItemByIndex(int itemIndex,int reduceAmount)
    {
        items[itemIndex].DecreaseItem(reduceAmount);
    }

}
[System.Serializable]
public class InventoryItem
{
    public ItemDataSO itemData;
    public int amount;
    public void RemoveItem()
    {
        itemData = null;
        amount = 0;
    }

    public void DecreaseItem(int reduceAmount)
    {
        amount -=reduceAmount;
        if(amount<=0) RemoveItem();
    } 
}
