using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class OptionUI : MonoBehaviour
{
    public TextMeshProUGUI optionTxt;
    private Button button;
    private DialoguePiece currentPiece;
    private DialogueOption currentOption;
    public string targetID;
    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece,DialogueOption option)
    {
        currentPiece = piece;
        optionTxt.text = option.text;
        targetID=option.targetID;
        currentOption= option;
    }

    private void OnOptionClicked()
    {
        if(currentPiece.quest!=null)
        {
            if (currentOption.isTakingQuest)
            {
                TaskManager.Instance.AddTask(currentPiece.quest);
            }
        }
        if (targetID .Equals(""))
        {
            UIManager.Instance.CloseDialoguePanel();
            return;
        }
        else
        {
            DialoguePanel.Instance.GoToDialoguePiece(DialoguePanel.Instance.currentDialogue.findPieceByID[targetID].index);
        }
    }
}
