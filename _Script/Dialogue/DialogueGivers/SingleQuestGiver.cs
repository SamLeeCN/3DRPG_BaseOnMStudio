using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[RequireComponent(typeof(DialogueController))]
public class SingleQuestGiver : MonoBehaviour
{
    public DialogueController currentDialogueController;
    public QuestDataSO currentTaskQuestData;

    public DialogueDataSO startDialogue;
    public DialogueDataSO progressingDialogue;
    public DialogueDataSO completedDialogue;
    public DialogueDataSO finishedDialogue;
    
    public bool IsStarted 
    { get {return TaskManager.Instance.GetTaskQuestByID(currentTaskQuestData.questID); } }
    public bool IsCompleted
    { get { return TaskManager.Instance.GetTaskQuestByID(currentTaskQuestData.questID).isCompleted; } }
    public bool IsFinished
    { get { return TaskManager.Instance.GetTaskQuestByID(currentTaskQuestData.questID).isFinished; } }
    void OnEnable()
    {
        EventManager.Instance.dialogueStartEvent += OnDialogueStartEvent;
    }
    void OnDisable()
    {
        EventManager.Instance.dialogueStartEvent -= OnDialogueStartEvent;
    }
    void Awake()
    {
        currentDialogueController = GetComponent<DialogueController>();
    }
    void Start()
    {
        currentDialogueController.SetDialogue(startDialogue);
        
    }
    private void OnDialogueStartEvent(DialogueController dialogueControllerStarted)
    {
        if(dialogueControllerStarted!=currentDialogueController)
        {
            return;
        }
        if (!IsStarted)
        {
            currentDialogueController.SetDialogue(startDialogue);
            return;
        }
        TaskManager.Instance.GetTaskQuestByID(currentTaskQuestData.questID).UpdateCompletement();
        if (IsFinished)
        {
            currentDialogueController.SetDialogue(finishedDialogue);
        }
        else if(IsCompleted)
        {
            currentDialogueController.SetDialogue(completedDialogue);
        }
        else
        {
            currentDialogueController.SetDialogue(progressingDialogue);
        }
    }
}
