using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public interface ISaveable
{
    DataDefination GetID();
    void RegisterSaveable() => DataManager.Instance.RegisterSaveable(this);
    void UnregisterSaveable()=>DataManager.Instance.UnregisterSaveable(this);
    void SetFileName();
    void SaveData();
    void LoadData();

    void DeleteData();
}
