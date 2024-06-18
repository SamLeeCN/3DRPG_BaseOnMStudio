using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TaskPanel : Singleton<TaskPanel>
{
    [Header("TaskList")]
    public RectTransform taskListTrans;
    public GameObject taskNameBtnPrefab;

    [Header("TaskInfo")]
    public TextMeshProUGUI taskNameTxt;
    public TextMeshProUGUI taskDescriptionTxt;

    [Header("Requirements")]
    public RectTransform taskRequirementsTrans;
    public GameObject taskRequirementPrefab;

    [Header("Rewards")]
    public RectTransform rewardSlotContainerTrans;
    public SlotUI[] rewardSlots;
    public GameObject rewardSlotPrefab;

    [Header("Buttons")]
    public Button closeBtn;
    protected override void Awake()
    {
        base.Awake();
        closeBtn.onClick.AddListener(UIManager.Instance.CloseTaskPanel);
    }
    public void Open()
    {
        gameObject.SetActive(true);
        SetUpUI();
        UIManager.Instance.UpdateSonPanelState();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UpdateSonPanelState();
    }
    public void SetUpUI()
    {
        foreach (Transform item in taskListTrans)
        {
            Destroy(item.gameObject);
        }

        ClearTaskInfo();
        
        foreach(var task in TaskManager.Instance.tasks)
        {
            var newTaskNameBtn = Instantiate(taskNameBtnPrefab, taskListTrans);
            TaskNameBtn newTaskNameBtnMono = newTaskNameBtn.GetComponent<TaskNameBtn>();
            newTaskNameBtnMono.SetUpUI(task.questData);
        }
    }
    private void ClearTaskInfo()
    {
        foreach (Transform item in taskRequirementsTrans)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in rewardSlotContainerTrans)
        {
            Destroy(item.gameObject);
        }
        taskNameTxt.text = "";
        taskDescriptionTxt.text = "";
    }
    public void SetUpTaskInfo(QuestDataSO quest)
    {
        ClearTaskInfo();
        taskNameTxt.text = quest.questName;
        taskDescriptionTxt.text = quest.description;
        SetUpRequireList(quest);
        SetUpRewardSlots(quest);
    }
    public void SetUpRequireList(QuestDataSO quest)
    {
        foreach (QuestRequire requirement in quest.requires)
        {
            QuestRequireType questType = requirement.questRequireType;
            if (questType == QuestRequireType.CollectItem || questType == QuestRequireType.DefeatCharacter)
            {
                GameObject requirementUIObject = Instantiate(taskRequirementPrefab, taskRequirementsTrans);
                int requireId = requirement.id;
                string requireName;
                if (questType == QuestRequireType.CollectItem)
                {
                    TaskManager.Instance.UpdateCollectItemProgress(requireId,GameManager.Instance.playerInventory.GetItemAmount(requireId));
                    requireName = GameManager.ItemIdToEnum(requireId).ToString();
                }
                else
                {
                    requireName = GameManager.CharacterIdToEnum(requireId).ToString();
                }
                requirementUIObject.GetComponent<TaskRequirementsUI>().SetUpUI
                    (requireName, requirement.currentAmount, requirement.requireAmount,quest.isFinished);

            }
        }
    }
    public void SetUpRewardSlots(QuestDataSO quest)
    {
        foreach(InventoryItem reward in quest.rewards)
        {
            if (reward.amount <= 0) continue;
            GameObject rewardSlot = Instantiate(rewardSlotPrefab, rewardSlotContainerTrans);
            rewardSlot.GetComponent<SlotUI>().displaySlotItem.SetUpItem(reward);
        }
    }
    
}
