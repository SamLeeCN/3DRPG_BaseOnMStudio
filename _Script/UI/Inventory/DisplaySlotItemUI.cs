using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class DisplaySlotItemUI : MonoBehaviour
{
    public InventoryItem itemToDisplay;
    public Image icon = null;
    public TextMeshProUGUI amountText = null;
    public void UpdateItemUI()
    {
        SetUpItem(itemToDisplay);
    }
    public void SetUpItem(InventoryItem item)
    {
        itemToDisplay = item;
        if (item.amount == 0)
        {
            item.itemData = null;
        }
        if (item.itemData != null)
        {
            icon.sprite = item.itemData.itemIcon;
            amountText.text = item.amount.ToString();
            icon.gameObject.SetActive(true);
            if (item.itemData.isStackable) amountText.gameObject.SetActive(true);
            else amountText.gameObject.SetActive(false);
        }
        else
        {
            icon.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
    }
}
