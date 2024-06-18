using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(fileName ="New Attack Data",menuName ="DataSO/AttackDataSO")]
public class AttackDataSO : ScriptableObject
{
    public int attackType;
    public WeaponDataSO currentWeapon;
    public CharacterDataSO currentCharacter;
    public bool isDefenceable = true;
    [Header("Base Attack Info")]
    [SerializeField] private float baseAttackRange;
    [SerializeField] private float baseSkillRange;
    [SerializeField] private float baseCoolDown;
    [SerializeField] private int baseMinAttackForce;
    [SerializeField] private int baseMaxAttackForce;
    [SerializeField] private float baseCriticalMultiplier;
    [SerializeField] private float baseCriticalChance;
    [SerializeField] private float baseFightBack;
    [Header("Current Attack Info")]
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minAttackForce;
    public int maxAttackForce;
    public float criticalMultiplier;
    public float criticalChance;
    public float fightBack;
    [Header("Level Bonus")]
    public float levelSystemExtraAttackForce;
    [Header("Sector Range")]
    public float lineCos;
    public void ApplyWeaponData(WeaponDataSO weaponToEquip)
    {
        if (currentWeapon !=null) UnApplyWeaponData();
        currentWeapon = weaponToEquip;
        UpdateData();
    }
    public void UnApplyWeaponData()
    {
        currentWeapon = null;
        UpdateData();
    }
    public void ApplyCharacterData(CharacterDataSO characterData)
    {
        currentCharacter = characterData;
        UpdateData();
    }
    public void UpdateData()
    {
        attackRange = CalculateAttackRange();
        skillRange = CalculateSkillRange();
        coolDown = CalculateCoolDown();
        minAttackForce = CalculateMinAttackForce();
        maxAttackForce = CalculateMaxAttackForce();
        criticalMultiplier = CalculateCriticalMultiplier();
        criticalChance = CalculateCriticalChance();
        fightBack = CalculateFightBack();
    }
    private float CalculateAttackRange()
    {
        float finalAttackRangeMultiplier=1;
        if (currentWeapon != null) finalAttackRangeMultiplier += currentWeapon.extraAttackRangeMultiplier;
        float result = baseAttackRange * finalAttackRangeMultiplier;
        if(result < 0) result = 0;
        return result;
    }
    private float CalculateSkillRange()
    {
        float finalSkillRangeMultiplier=1;
        if (currentWeapon != null) finalSkillRangeMultiplier += currentWeapon.extraSkillRangeMultiplier;
        float result= baseSkillRange*finalSkillRangeMultiplier;
        if (result < 0) result = 0;
        return result;
    }
    private float CalculateCoolDown()
    {
        float finalCoolDownMultiplier=1;
        if (currentWeapon != null) finalCoolDownMultiplier += currentWeapon.extraCoolDownMultiplier;
        float result = baseCoolDown * finalCoolDownMultiplier;
        if (result < 0) result = 0;
        return result;
    }
    private int CalculateMinAttackForce()
    {
        int finalExtraAttackForce=0;
        if(currentWeapon!=null) finalExtraAttackForce += currentWeapon.extraAttackForce;
        if (currentCharacter != null) finalExtraAttackForce += currentCharacter.currentAttackForce;
        float finalAttackForceMultiplier=1;
        if (currentWeapon != null) finalAttackForceMultiplier += currentWeapon.extraAttackForceMultiplier;

        int result= (int)((baseMinAttackForce + finalExtraAttackForce) * finalAttackForceMultiplier);
        if (result < 0) result = 0;
        return result;
    }
    private int CalculateMaxAttackForce()
    {
        int finalExtraAttackForce = 0;
        if (currentWeapon != null) finalExtraAttackForce += currentWeapon.extraAttackForce;
        if (currentCharacter != null) finalExtraAttackForce += currentCharacter.currentAttackForce;
        float finalAttackForceMultiplier = 1;
        if (currentWeapon != null) finalAttackForceMultiplier += currentWeapon.extraAttackForceMultiplier;

        int result= (int)((baseMaxAttackForce + finalExtraAttackForce) * finalAttackForceMultiplier);
        if (result < 0) result = 0;
        return result;
    }
    private float CalculateCriticalMultiplier()
    {
        float finalExtraCriticalMultiplier = 0;
        if(currentWeapon != null)finalExtraCriticalMultiplier += currentWeapon.extraCriticalMultiplier;
        float result= baseCriticalMultiplier+finalExtraCriticalMultiplier;
        if (result < 0) result = 0;
        return result;
    }
    private float CalculateCriticalChance()
    {
        float finalExtraCriticalChance = 0;
        if(currentWeapon != null)finalExtraCriticalChance += currentWeapon.extraCriticalChance;
        float result = baseCriticalChance + finalExtraCriticalChance;
        if (result < 0) result = 0;
        return result;
    }
    private float CalculateFightBack()
    {
        float finalExtraFightBack = 0;
        if(currentWeapon!=null)finalExtraFightBack += currentWeapon.extraFightBack;
        float result = baseFightBack + finalExtraFightBack;
        if (result < 0) result = 0;
        return result;
    }
}