using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="DataSO/UseableItemDataSO")]
public class UseableItemDataSO : ScriptableObject
{
    public int healthRecover;
    public float healthPercentageRecover;
    public int gainExp;

    [Header("buff")]
    public int healthRecoverBuffLevel;
    public int healthRecoverBuffDuration;
}
