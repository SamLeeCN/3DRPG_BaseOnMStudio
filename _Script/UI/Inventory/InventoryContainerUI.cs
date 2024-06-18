using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class InventoryContainerUI : MonoBehaviour
{
    public InventoryDataSO currentInventoryData;
    public SlotUI[] slots;
    
    private void Start()
    {
        
    }
    public void UpdateUI()
    {
        if (currentInventoryData == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].currentSlotItem.currentInventoryData = currentInventoryData;
            slots[i].currentSlotItem.index = i;
            slots[i].UpdateItem();
        }
    }
    public void SetInventoryData(InventoryDataSO inventoryToShow)
    {
        currentInventoryData = inventoryToShow;
        UpdateUI();
    }
}
