using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class ItemTip : MonoBehaviour
{
    public TextMeshProUGUI itemNameTxt;
    public TextMeshProUGUI itemDescriptionTxt;
    public RectTransform rectTransform;

    Vector3[] corners = new Vector3[4];
    Vector2 mousePos;

    public float cornerOffset = 20;
    private Vector3 cumulativeScale;
    // Corner offset depend on how large the cursor is

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        UpdatePosition(); 

    }
    void Update()
    {
        UpdatePosition();
    }
    public void SetUpUI(ItemDataSO item) { 
        itemNameTxt.text = item.itemName;
        string itemOriginalDescription = item.description;
        string itemFinalDescription;
        switch(item.itemType)
        {
            case ItemType.Weapon:
                itemFinalDescription = itemOriginalDescription + Environment.NewLine+ 
                    InventoryManager.Instance.GenerateWeaponDescription(item.weaponData);
                break;
            case ItemType.Armor:
                itemFinalDescription = itemOriginalDescription+ Environment.NewLine+
                    InventoryManager.Instance.GenerateArmorDescription(item.armorData);
                break;
            case ItemType.Useable: 
                itemFinalDescription = itemOriginalDescription;
                break;
            default:
                itemFinalDescription = itemOriginalDescription;
                break;
        }
        itemDescriptionTxt.text = itemFinalDescription;
    }
    
    private void UpdatePosition()
    {
        cumulativeScale = ExtensionMethod.GetCumulativeScale(rectTransform);
        mousePos =Input.mousePosition;
        //Get 4 corners(from left buttom, clockwise) of the gameObject
        rectTransform.GetWorldCorners(corners);
        
        float width = (corners[2].x - corners[1].x) ;
        float height = (corners[1].y - corners[0].y) ;

        bool isLeftToTheScreen = mousePos.x < width;
        bool isRightToTheScreen = Screen.width - mousePos.x < width;
        bool isTopToTheScreen = Screen.height - mousePos.y < height;
        bool isButtomToTheScreen = mousePos.y < height;
        Vector2 anchorPosBeforeScale;
        if (isLeftToTheScreen && isTopToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);
        else if (isTopToTheScreen && !isRightToTheScreen && !isLeftToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);
        else if (isRightToTheScreen && isTopToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(2);
        else if (isLeftToTheScreen && !isTopToTheScreen && !isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);
        else if (isRightToTheScreen && !isTopToTheScreen && !isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(2);
        else if (isLeftToTheScreen && isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(0);
        else if (isButtomToTheScreen && !isLeftToTheScreen && !isRightToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(0);
        else if (isRightToTheScreen && isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(3);
        else anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);

        rectTransform.anchoredPosition = anchorPosBeforeScale/cumulativeScale;
    }
    private Vector2 CalculateAnchorPosBeforeScale(int cornerIndex)
    {
        Vector2 anchorOffsetFromCorner=rectTransform.anchoredPosition*cumulativeScale-GetCornerAfterOffset(cornerIndex);
        return mousePos + anchorOffsetFromCorner;
    }
    private Vector2 GetCornerAfterOffset(int cornerIndex)
    {
        float x = corners[cornerIndex].x;
        float y = corners[cornerIndex].y;
        //depends on how your cursor looks
        //for most cursors only case 1 is needed
        switch (cornerIndex)
        {
            case 0:
                
                break;
            case 1:
                x += -cornerOffset;
                y += cornerOffset;
                break;
            case 2:
                
                break;
            case 3:
                
                break;
        }
        return new Vector2(x, y);
    }
    
}
