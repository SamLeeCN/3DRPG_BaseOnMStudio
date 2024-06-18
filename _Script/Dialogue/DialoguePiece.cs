using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DialoguePiece
{
    public string id;
    public int index;
    public string characterName;
    public Sprite characterSprite;
    [TextArea]
    public string text;
    public Sprite sprite;
    public List<DialogueOption> options=new List<DialogueOption>();
    public QuestDataSO quest;

    [HideInInspector] public bool isExpanding;
}
