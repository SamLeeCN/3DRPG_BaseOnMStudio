using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;
using JetBrains.Annotations;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TaskManager : Singleton<TaskManager>
{
    public List<Task> tasks = new List<Task>();
    public void AddTask(QuestDataSO quest)
    {
        if(HasQuestID(quest.questID))
        {// take reward
            QuestDataSO task = GetTaskQuestByID(quest.questID);
            task.isFinished = true;
            foreach(InventoryItem reward in task.rewards)
            {
                if (reward.amount >= 0)
                {
                    GameManager.Instance.playerInventory.GainItem(reward.itemData, reward.amount);
                }
                else
                {
                    GameManager.Instance.playerInventory.DecreaseItem(reward.itemData.id, reward.amount);
                }
            }
            InventoryManager.Instance.UpdateUI();
        }
        else
        {// take task
            QuestDataSO newQuestData = Instantiate(quest);
            Task newTask = new Task(newQuestData);
            tasks.Add(newTask);
            newTask.IsStarted = true;
        }
    }
    public bool HasQuestID(string questID)
    {
        if(tasks.Any(q=> q.questData.questID==questID))return true;
        else return false;
    }
    public QuestDataSO GetTaskQuestByID(string questID)
    {
        if (HasQuestID(questID))
        {
            foreach(Task task in tasks)
            {
                if (task.questData.questID == questID)
                {
                    return task.questData;
                }
            }
        }
        return null;
    }
    public void UpdateCollectItemProgress(int itemID, int amount)
    {
        foreach (Task task in tasks)
        {
            if (task.IsFinished) continue;
            foreach (QuestRequire require in task.questData.requires)
            {
                if (require.questRequireType == QuestRequireType.CollectItem && itemID == require.id)
                {
                    require.currentAmount = amount;
                }
            }
        }
    }
    public void AddCollectItemProgress(int itemID,int amount)
    {
        foreach (Task task in tasks)
        {
            if (task.IsFinished) continue;
            foreach (QuestRequire require in task.questData.requires)
            {
                if (require.questRequireType == QuestRequireType.CollectItem && itemID == require.id)
                {
                    require.currentAmount += amount;
                }
            }
        }
    }
    public void DecreaseCollectItemProgress(int itemID,int amount)
    {
        foreach (Task task in tasks)
        {
            if (task.IsFinished) continue;
            foreach (QuestRequire require in task.questData.requires)
            {
                if (require.questRequireType == QuestRequireType.CollectItem && itemID == require.id)
                {
                    require.currentAmount -= amount;
                }
            }
        }
    }
    public void AddDefeatCharacterProgress(int characterID)
    {
        foreach (Task task in tasks)
        {
            if (task.IsFinished) continue;
            foreach (QuestRequire require in task.questData.requires)
            {
                if (require.questRequireType == QuestRequireType.DefeatCharacter&& characterID == require.id)
                {
                    require.currentAmount++;
                }
            }
        }
    }
    private string taskCountString = "taskCount";
    private string TaskSaveString(int index){ return "task" + index; }

    public void SaveTasks()
    {
        PlayerPrefs.SetInt(taskCountString, tasks.Count);
        for (int i=0;i<tasks.Count; i++)
        {
            DataManager.Instance.SaveSO(tasks[i].questData,TaskSaveString(i));
        }
    }
    public void LoadTasks()
    {
        int taskCount=PlayerPrefs.GetInt(taskCountString);
        for (int i=0;i<taskCount;i++)
        {
            QuestDataSO newQuestData=ScriptableObject.CreateInstance<QuestDataSO>();
            DataManager.Instance.LoadSO(newQuestData, TaskSaveString(i));
            tasks.Add(new Task(newQuestData));
        }
    }
    public void DeleteTasks()
    {
        int taskCount = PlayerPrefs.GetInt(taskCountString);
        PlayerPrefs.DeleteKey(taskCountString);
        for(int i = 0; i < taskCount; i++)
        {
            DataManager.Instance.DeleteSO(TaskSaveString(i));
        }
    }
}
[System.Serializable]
public class Task
{
    public QuestDataSO questData;
    public Task(QuestDataSO questData)
    {
        this.questData = questData;
    }
    public bool IsStarted  { get { return questData.isStarted; }set { questData.isStarted = value; } }
    public bool IsCompleted { get { return questData.isCompleted; } set { questData.isCompleted = value; } }
    public bool IsFinished { get {  return questData.isFinished; }set { questData.isFinished = value; } }
    
}
