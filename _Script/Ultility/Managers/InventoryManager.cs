using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class InventoryManager : Singleton<InventoryManager>,IParentPanel
{
    public Inventory currentInventory;

    public InventoryContainerUI bagContainer;
    public InventoryContainerUI actionContainer;
    public InventoryContainerUI equipmentContainer;
    [Header("UI")]
    public GameObject bagPanel;
    public GameObject equipmentPanel;
    public TextMeshProUGUI maxHealthValueTxt;
    public TextMeshProUGUI attackValueTxt;
    public TextMeshProUGUI defenceValueTxt;
    public ItemTip itemTip;

    [Header("Drag")]
    public Canvas itemDragCanvas;
    public ItemDragData currentItemDrag;
    [Header("Buttons")]
    public Button bagCloseBtn;
    public Button equipmentCloseBtn;
    [Header("SonPanelState")]
    public bool isEquipmentPanelOpen;
    public bool isBagPanelOpen;
    protected override void Awake()
    {
        base.Awake();
        bagCloseBtn.onClick.AddListener(CloseBagPanel);
        equipmentCloseBtn.onClick.AddListener(CloseEquipmentPanel);
    }
    void Update()
    {
        PlayerActionBarControl();
        UpdateStatistic();
    }
    public void SetInventory(Inventory inventory)
    {
        currentInventory = inventory;
        bagContainer.SetInventoryData(inventory.bagData);
        actionContainer.SetInventoryData(inventory.actionBarData);
        equipmentContainer.SetInventoryData(inventory.equipmentData);
        
    }
    public void Open()
    {
        gameObject.SetActive(true);
        ShowAllSonPanel();
    } 
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void ShowAllSonPanel()
    {
        bagPanel.SetActive(true);
        equipmentPanel.SetActive(true);
        UpdateSonPanelState();
        UIManager.Instance.UpdateSonPanelState();
    }
    public void CloseBagPanel()
    {
        bagPanel.SetActive(false);
        isBagPanelOpen = false;
        if (!isBagPanelOpen && !isEquipmentPanelOpen)
        {
            UIManager.Instance.ToggleInventoryPanel();
            UIManager.Instance.UpdateSonPanelState();
        }
        UpdateSonPanelState();
        
    }
    public void CloseEquipmentPanel()
    {
        equipmentPanel.SetActive(false);
        isEquipmentPanelOpen=false;
        if (!isBagPanelOpen && !isEquipmentPanelOpen)
        {
            UIManager.Instance.ToggleInventoryPanel();
            UIManager.Instance.UpdateSonPanelState();
        }
        UpdateSonPanelState();
    }
    public void UpdateUI()
    {
        bagContainer.UpdateUI();
        actionContainer.UpdateUI();
        equipmentContainer.UpdateUI();
        
    }
    public void UpdateSonPanelState()
    {
        isBagPanelOpen = bagPanel.activeInHierarchy;
        isEquipmentPanelOpen = equipmentPanel.activeInHierarchy;
    }
    public bool CheckInBagSlot(Vector3 pos)
    {
        for (int i = 0; i < bagContainer.slots.Length; i++)
        {
            RectTransform rect = bagContainer.slots[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, pos))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInActionBarSlot(Vector3 pos)
    {
        for (int i = 0; i < actionContainer.slots.Length; i++)
        {
            RectTransform rect = actionContainer.slots[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, pos))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInEquipmentSlot(Vector3 pos)
    {
        for (int i = 0; i < equipmentContainer.slots.Length; i++)
        {
            RectTransform rect = equipmentContainer.slots[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, pos))
            {
                return true;
            }
        }
        return false;
    }
    public void SwapItemInUI(SlotUI originalSlot, SlotUI targetSlot)
    {
        InventoryDataSO originalInventory = originalSlot.GetInventoryData();
        int originalIndex = originalSlot.GetSlotIndex();
        InventoryDataSO targerInventory=targetSlot.GetInventoryData();
        int targetIndex=targetSlot.GetSlotIndex();
        SwapItem(originalInventory,originalIndex,targerInventory,targetIndex);
    }
    public void SwapItem(InventoryDataSO originalInventory,int originalIndex,InventoryDataSO targetInventory,int targetIndex)
    {
        InventoryItem originalItem = originalInventory.items[originalIndex];
        InventoryItem targetItem= targetInventory.items[targetIndex];
        if (targetItem.itemData!=null&&originalItem.itemData.id == targetItem.itemData.id && targetItem.itemData.isStackable)
        {   //Same stackable item
            targetInventory.items[targetIndex].amount += originalItem.amount;
            originalInventory.items[originalIndex].RemoveItem();
        }
        else
        {   //Swap
            originalInventory.items[originalIndex]= targetItem;
            targetInventory.items[targetIndex]= originalItem;
        }
    }
    private void PlayerActionBarControl()
    {
        Inventory playerInventory = GameManager.Instance.playerInventory;
        InventoryDataSO playerActionBar = playerInventory.actionBarData;
        if (InputManager.Instance.ActionBar1Input)
        {
            playerInventory.UseItem(playerActionBar, 0);
            actionContainer.UpdateUI();
        }
        if (InputManager.Instance.ActionBar2Input)
        {
            playerInventory.UseItem(playerActionBar, 1);
            actionContainer.UpdateUI();
        }
        if (InputManager.Instance.ActionBar3Input)
        {
            playerInventory.UseItem(playerActionBar, 2);
            actionContainer.UpdateUI();
        }
        if (InputManager.Instance.ActionBar4Input)
        {
            playerInventory.UseItem(playerActionBar, 3);
            actionContainer.UpdateUI();
        }
        if (InputManager.Instance.ActionBar5Input)
        {
            playerInventory.UseItem(playerActionBar, 4);
            actionContainer.UpdateUI();
        }
        if (InputManager.Instance.ActionBar6Input)
        {
            playerInventory.UseItem(playerActionBar, 5);
            actionContainer.UpdateUI();
        }
    }
    public void UpdateStatistic()
    {
        if (!UIManager.Instance.isInventoryOpen||!isEquipmentPanelOpen) return;
        maxHealthValueTxt.text = currentInventory.character.characterData.currentMaxHealth.ToString();
        int minAttackValue = currentInventory.character.regularAttackData.minAttackForce;
        int maxAttackValue = currentInventory.character.regularAttackData.maxAttackForce;
        attackValueTxt.text=minAttackValue.ToString()+" - "+maxAttackValue.ToString();
        defenceValueTxt.text=currentInventory.character.characterData.currentDefence.ToString();
    }

    public string GenerateWeaponDescription(WeaponDataSO weapon)
    {
        float extraAttackRangeMultiplier = weapon.extraAttackRangeMultiplier;
        float extraSkillRangeMultiplier = weapon.extraSkillRangeMultiplier;
        int extraAttackForce = weapon.extraAttackForce;
        float extraAttackForceMultiplier = weapon.extraAttackForceMultiplier;
        float extraCoolDownMultiplier = weapon.extraCoolDownMultiplier;
        float extraFightBack = weapon.extraFightBack;
        float extraCriticalMultiplier = weapon.extraCriticalMultiplier;
        float extraCriticalChance = weapon.extraCriticalChance;
        string description;
        description = "extraAttackRangeMultiplier:" + extraAttackRangeMultiplier + Environment.NewLine +
            "extraSkillRangeMultiplier:" + extraSkillRangeMultiplier + Environment.NewLine +
            "extraAttackForce:" + extraAttackForce + Environment.NewLine +
            "extraAttackForceMultiplier:" + extraAttackForceMultiplier + Environment.NewLine +
            "extraCoolDownMultiplier:" + extraCoolDownMultiplier + Environment.NewLine +
            "extraFightBack:" + extraFightBack + Environment.NewLine +
            "extraCriticalMultiplier:" + extraCriticalMultiplier + Environment.NewLine +
            "extraCriticalChance:" + extraCriticalChance;
        return description;
    }
    public string GenerateArmorDescription(ArmorDataSO armor)
    {
        int extraDefence = armor.extraDefence;
        string description;
        description = "extraDefence:" + extraDefence;
        return description;
    }

    

    public void InitializeAllSonPanels()
    {
        //Nothing Need to be done
    }
}

