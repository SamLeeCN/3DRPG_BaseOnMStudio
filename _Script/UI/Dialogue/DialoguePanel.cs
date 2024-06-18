using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class DialoguePanel : Singleton<DialoguePanel> 
{
    public TextMeshProUGUI dialogueTextUI;
    public Image dialogueImageUI;
    public Button nextBtn;
    public GameObject optionPanel;

    public DialogueDataSO currentDialogue;
    public int currentIndex;

    public GameObject optionPrefab;

    protected override void Awake()
    {
        base.Awake();
        nextBtn.onClick.AddListener(ContinueDialogue);
    }

    public void Open(DialogueController dialogueController)
    {
        EventManager.Instance.RaiseDialogueStartEvent(dialogueController);

        SetDialogue(dialogueController.currentDialogueData);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void SetDialogue(DialogueDataSO dialogueData)
    {
        currentDialogue = dialogueData;
        currentIndex = -1;
        //continue dialogue will add 1 to the index
        ContinueDialogue();
    }
    public void GoToDialoguePiece(int index)
    {
        currentIndex = index;
        DialoguePiece piece = currentDialogue.dialoguePieces[index];
        if (piece.sprite != null)
        {
            dialogueImageUI.sprite = piece.sprite;
            dialogueImageUI.enabled = true;
        }
        else dialogueImageUI.enabled = false;

        dialogueTextUI.text = "";
        //dialogueTextUI.DOText(piece.text, UIManager.Instance.dialogueTextShowDuration);
        dialogueTextUI.text = piece.text;
        if (piece.options.Count ==0&&currentDialogue.dialoguePieces.Count>0) 
        {
            
            nextBtn.gameObject.SetActive(true);
            optionPanel.SetActive(false);
        }
        else
        {
            nextBtn.gameObject.SetActive(false);
            optionPanel.SetActive(true);
            CreateOptions(piece);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    public void ContinueDialogue()
    {
        if (currentIndex < currentDialogue.dialoguePieces.Count-1)
        {
            GoToDialoguePiece(currentIndex+1);
        }
        else
        {
            UIManager.Instance.CloseDialoguePanel();
        }
    }
    public void CreateOptions(DialoguePiece piece)
    {
        if (piece.options.Count > 0)
        {
            for(int i=0;i< optionPanel.transform.childCount;i++)
            {
                Destroy(optionPanel.transform.GetChild(i).gameObject);
            }
        }
        for(int i = 0; i < piece.options.Count; i++)
        {
            GameObject option = Instantiate(optionPrefab, optionPanel.transform);
            option.GetComponent<OptionUI>().UpdateOption(piece,piece.options[i]);
        }
    }
}
