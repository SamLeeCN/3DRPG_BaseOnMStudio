using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class ItemPickUp : MonoBehaviour,IInteractable
{
    public int amount = 1;
    public ItemDataSO itemData;
    
    private void Awake()
    {
        if(itemData == null) 
        {
            SetAmount(amount);
        }
    }
    public void SetAmount(int amount)
    {
        this.amount = amount;
    }
    public void TriggerAction()
    {
        if (GameManager.Instance.playerInventory.GainItem(itemData,amount))
        {
            Destroy(gameObject);
            GameManager.Instance.playerControler.isInInteractArea = false;
            KeyPrompt.Instance.DeleteKeyPrompt(InputManager.Instance.interactAction);
        }
        else
        {
            //TODO:prompt user that inventory is full
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerControler.currentInteractable = GetComponent<IInteractable>();
            GameManager.Instance.playerControler.isInInteractArea = true;
            KeyPrompt.Instance.AddKeyPrompt(InputManager.Instance.interactAction);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerControler.isInInteractArea = false;
            KeyPrompt.Instance.DeleteKeyPrompt(InputManager.Instance.interactAction);
        }
    }
}
