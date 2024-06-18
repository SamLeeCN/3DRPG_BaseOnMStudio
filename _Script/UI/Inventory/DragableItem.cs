using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[RequireComponent(typeof(SlotItemUI))]
public class DragableItem : MonoBehaviour,IBeginDragHandler,IDragHandler, IEndDragHandler
{
    public SlotItemUI currentSlotItem;
    public SlotUI currentSlot;
    public SlotUI targetSlot;
    public bool isDragging;
    private void Awake()
    {
        currentSlotItem = GetComponent<SlotItemUI>();
        currentSlot=GetComponentInParent<SlotUI>();
        isDragging = false;
    }
    private void Update()
    {
        JudgeSingleThrow();
    }
    private void JudgeSingleThrow()
    {
        if (isDragging&&InputManager.Instance.DropItemInput)
        {
            InventoryManager.Instance.currentInventory.DropItemEach(currentSlot.GetInventoryItem());
            if (currentSlot.GetInventoryItem().itemData == null)
            {
                transform.SetParent(InventoryManager.Instance.currentItemDrag.originalParent);
                transform.localPosition = new Vector3(0, 0, 0);
                isDragging = false;
            }
            currentSlot.UpdateItem();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentItemDrag = new ItemDragData(currentSlot, transform.parent);
        transform.SetParent(InventoryManager.Instance.itemDragCanvas.transform,true);
        //true means world position stays
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position= eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckInBagSlot(eventData.position) ||
                InventoryManager.Instance.CheckInEquipmentSlot(eventData.position) ||
                InventoryManager.Instance.CheckInActionBarSlot(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotUI>())
                {
                    targetSlot = eventData.pointerEnter.gameObject.GetComponent<SlotUI>();
                }
                else
                {
                    targetSlot = eventData.pointerEnter.gameObject.GetComponentInParent<SlotUI>();
                }
                if (targetSlot)
                {
                    switch (targetSlot.slotType)
                    {
                        case SlotType.Bag:
                            {
                                if (currentSlot.slotType == SlotType.Bag)
                                {
                                    InventoryManager.Instance.SwapItemInUI(currentSlot, targetSlot);
                                }
                                else if (currentSlot.slotType == SlotType.Weapon)
                                {
                                    if (targetSlot.GetItemData() == null) InventoryManager.Instance.SwapItemInUI(currentSlot, targetSlot);
                                    else if (targetSlot.GetItemData().itemType == ItemType.Weapon)
                                        InventoryManager.Instance.currentInventory.EquipWeapon(targetSlot.GetInventoryData(), targetSlot.GetSlotIndex());
                                }
                                else if (currentSlot.slotType == SlotType.Armor)
                                {
                                    if (targetSlot.GetItemData() == null) InventoryManager.Instance.SwapItemInUI(currentSlot, targetSlot);
                                    else if (targetSlot.GetItemData().itemType == ItemType.Armor)
                                        InventoryManager.Instance.currentInventory.EquipArmor(targetSlot.GetInventoryData(), targetSlot.GetSlotIndex());
                                }
                                else if (currentSlot.slotType == SlotType.Action)
                                {
                                    if (targetSlot.GetItemData() == null || targetSlot.GetItemData().itemType == ItemType.Useable)
                                    {
                                        InventoryManager.Instance.SwapItemInUI(currentSlot, targetSlot);
                                    }
                                }
                                InventoryManager.Instance.UpdateUI();
                            }
                            break;
                        case SlotType.Action:
                            if (currentSlot.GetItemData().itemType == ItemType.Useable)
                            {
                                InventoryManager.Instance.SwapItemInUI(currentSlot, targetSlot);
                                InventoryManager.Instance.UpdateUI();
                            }
                            break;
                        case SlotType.Weapon:
                            if (currentSlot.GetItemData().itemType == ItemType.Weapon)
                            {
                                InventoryManager.Instance.currentInventory.EquipWeapon(currentSlot.GetInventoryData(), currentSlot.GetSlotIndex());
                                InventoryManager.Instance.UpdateUI();
                            }
                            break;
                        case SlotType.Armor:
                            if (currentSlot.GetItemData().itemType == ItemType.Armor)
                            {
                                InventoryManager.Instance.currentInventory.EquipArmor(currentSlot.GetInventoryData(), currentSlot.GetSlotIndex());
                                InventoryManager.Instance.UpdateUI();
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            if (currentSlot.GetInventoryItem().itemData!=null)
            {
                InventoryManager.Instance.currentInventory.DropItemWhole(currentSlot.GetInventoryItem());
                currentSlot.UpdateItem();
            }
        }
        transform.SetParent(InventoryManager.Instance.currentItemDrag.originalParent);
        transform.localPosition = new Vector3(0, 0, 0);
        isDragging = false;
    }
    
}
public class ItemDragData
{
    public SlotUI originalSlot;
    public RectTransform originalParent;
    public ItemDragData(SlotUI originalSlot, Transform originalParent)
    {
        this.originalSlot = originalSlot;
        this.originalParent = (RectTransform)originalParent;
    }
}
