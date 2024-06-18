using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="DataSO/WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    public WeaponType weaponType = WeaponType.OneHand;
    public float extraAttackRangeMultiplier = 0;
    public float extraSkillRangeMultiplier = 0;
    public int extraAttackForce=0;
    public float extraAttackForceMultiplier = 0;
    public float extraCoolDownMultiplier=0;
    public float extraFightBack=0;
    public float extraCriticalMultiplier = 0;
    public float extraCriticalChance = 0;
}
