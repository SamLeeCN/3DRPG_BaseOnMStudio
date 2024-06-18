using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TaskNameBtn : MonoBehaviour
{
    public TextMeshProUGUI taskNameTxt;
    public QuestDataSO currentTaskData;
    public Button buttonComponent;
    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
        buttonComponent.onClick.AddListener(SetUpTaskInfo);
    }

    public void SetUpUI(QuestDataSO questData )
    {
        currentTaskData = questData;
        questData.UpdateCompletement();

        if (currentTaskData.isFinished)
        {
            taskNameTxt.text = currentTaskData.questName + " (Finished)";
        }
        else if(currentTaskData.isCompleted)
        {
            taskNameTxt.text = currentTaskData.questName + " (Completed)";
        }
        else
        {
            taskNameTxt.text = currentTaskData.questName;
        }
    }
    private void SetUpTaskInfo()
    {
        TaskPanel.Instance.SetUpTaskInfo(currentTaskData);
    }
}
