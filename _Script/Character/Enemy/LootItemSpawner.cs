using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class LootItemSpawner : MonoBehaviour
{
    public LootItem[] lootItems;

    public void SpawnLootItems()
    {
        for(int i = 0; i < lootItems.Length; i++)
        {
            float currentChangeValue = Random.value;
            if (currentChangeValue < lootItems[i].dropChance)
            {
                lootItems[i].Spawn(transform.position + Vector3.up * 2);
            }
        }
    }
}

