using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class PlayerStatus : MonoBehaviour
{
    public FillBar playerHealthBar;
    public FillBar playerExpBar;
    public Text playerLevelTxt;
    public Character playerCharacter;
    private void Awake()
    {
        playerCharacter = GameManager.Instance.playerCharacter;
    }
    void Update()
    {
        UpdatePlayerHealthUI();
        UpdatePlayerExpUI();
        UpdatePlayerLevelUI();
    } 
    public void UpdatePlayerHealthUI()
    {
        playerHealthBar.UpdateFillUI(playerCharacter.characterData.currentHealth, playerCharacter.characterData.currentMaxHealth);
    }
    public void UpdatePlayerExpUI()
    {
        playerExpBar.UpdateFillUI(playerCharacter.characterData.currentExp, playerCharacter.characterData.currentExpRequireToLevelUp);
    }
    public void UpdatePlayerLevelUI()
    {
        playerLevelTxt.text = "Level " + playerCharacter.characterData.currentLevel.ToString("00");
    }
}
