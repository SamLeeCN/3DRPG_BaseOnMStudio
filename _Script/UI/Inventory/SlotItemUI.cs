using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SlotItemUI : MonoBehaviour
{
    public InventoryDataSO currentInventoryData;
    public int index=-1;

    public Image icon=null;
    public TextMeshProUGUI amountText=null;
    public void UpdateItemUI()
    {
        SetUpItem(GetInventoryItem());
    }
    public void SetUpItem(InventoryItem item)
    {
        if(item.amount==0)
        {
            item.itemData = null;
        }
        if (item.itemData != null)
        {
            icon.sprite = item.itemData.itemIcon;
            amountText.text=item.amount.ToString();
            icon.gameObject.SetActive(true);
            if(item.itemData.isStackable) amountText.gameObject.SetActive(true);
            else amountText.gameObject.SetActive(false);
        }
        else
        {
            icon.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
    }
    public InventoryItem GetInventoryItem()
    {
        if(currentInventoryData == null)return null;
        else return currentInventoryData.items[index];
    }
    public ItemDataSO GetItemData()
    {
        if(currentInventoryData==null)return null;
        else if (currentInventoryData.items[index]==null) return null;
        else return currentInventoryData.items[index].itemData;
    }
}
