using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(fileName ="New CharacterData",menuName ="DataSO/CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    [Header("Character Info")]
    public int characterID = 0;
    [SerializeField] private int baseMaxHealth;
    [SerializeField] private int baseDefence;
    [SerializeField] private int baseAttackForce;
    public int currentMaxHealth;
    public int currentHealth;
    public int currentDefence;
    public int currentAttackForce;

    [Header("Level System")]
    public int maxLevel=30;
    public int currentLevel = 1;
    public int baseExpRequireToLevelUp=50;
    public int currentExpRequireToLevelUp=50;
    public int currentExp=0;
    [Header("Armor")]
    public ArmorDataSO currentArmor;

    [SerializeField] private float expRequiredLevelBuff;
    [SerializeField] private float expRequiredLevelMultiplier { get { return 1 + (currentLevel - 1) * expRequiredLevelBuff; } }
    [SerializeField] private float maxHealthLevelBuff;
    [SerializeField] private float maxHealthLevelMultiplier{ get { return 1 + (currentLevel - 1) * maxHealthLevelBuff; } }
    [SerializeField] private float defenceLevelBuff;
    [SerializeField] private float defenceLevelMultiplier { get { return 1 + (currentLevel-1) * defenceLevelBuff; } }
    [SerializeField] private float attackForceLevelBuff;
    [SerializeField] private float attackForceLevelMultiplier { get { return 1 + (currentLevel - 1) * attackForceLevelBuff; } }
    public void GainExp(int exp)
    {
        currentExp += exp;
        if(currentExp>currentExpRequireToLevelUp)
        {
            LevelUp(currentExp - currentExpRequireToLevelUp);
        }
    }
    public void LevelUp(int remainExp)
    {
        currentExp = currentExpRequireToLevelUp;
        if (currentLevel == maxLevel) return;
        currentExp = 0;
        currentLevel++;
        UpdateData();
        GainExp(remainExp);
        //TODO:Update UI
    }
    #region Armor
    public void ApplyArmorData(ArmorDataSO armorToEquip)
    {
        if (currentArmor != null) UnApplyArmorData();
        currentArmor = armorToEquip;
        UpdateData();
    }
    public void UnApplyArmorData()
    {
        currentArmor = null;
        UpdateData();
    }
    private void UpdateData()
    {
        currentExpRequireToLevelUp = (int)(expRequiredLevelMultiplier * baseExpRequireToLevelUp);
        currentMaxHealth = (int)(maxHealthLevelMultiplier * baseMaxHealth);
        currentHealth = currentMaxHealth;
        currentDefence = CalculateDefence();
        currentAttackForce = (int)(attackForceLevelMultiplier * baseAttackForce);
    }
    private int CalculateDefence()
    {
        int finalExtraDefence=0;
        if (currentArmor != null) finalExtraDefence += currentArmor.extraDefence;
        return (int)(defenceLevelMultiplier * baseDefence)+finalExtraDefence;
    }

    #endregion
    [Header("Defeat Bonus")]
    public int defeatExp;
}
