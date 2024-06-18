using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class DialogueController : MonoBehaviour,IInteractable
{
    public List<DialogueDataSO> dialogues = new List<DialogueDataSO>();
    public DialogueDataSO currentDialogueData;
    bool isTalkable=true;


    private void Awake()
    {
        currentDialogueData = dialogues[0];
    }
    public void SetDialogue(DialogueDataSO dialogueData)
    {
        currentDialogueData=dialogueData;
    }
    public void TriggerAction()
    {
        if(isTalkable) UIManager.Instance.OpenDialoguePanel(this);
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
