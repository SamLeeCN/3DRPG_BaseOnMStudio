using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[System.Serializable]
public class LootItem
{
    public ItemDataSO ItemData;
    [Range(0f, 1f)]
    public float dropChance;
    public int fixedAmount;
    public bool isAmountVariable = false;
    public int minAmount;
    public int maxAmount;
    
    public void Spawn(Vector3 pos,Quaternion rotation)
    {
        int num=GetAmount();
        for(int i=0; i<num; i++)
        {
            Object.Instantiate(ItemData.itemPickUpPrefab, pos, rotation);
        }
    }
    public void Spawn(Vector3 pos)
    {
        int num = GetAmount();
        for (int i = 0; i < num; i++)
        {
            Object.Instantiate(ItemData.itemPickUpPrefab, pos, Quaternion.identity);
        }
    }
    private int GetAmount()
    {
        if (!isAmountVariable)
        {
            return fixedAmount;
        }
        else
        {
            return Random.Range(minAmount, maxAmount);
        }
    }

}
