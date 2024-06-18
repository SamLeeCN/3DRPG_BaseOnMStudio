using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(fileName ="New Quest",menuName ="DataSO/QuestDataSO")]
public class QuestDataSO : ScriptableObject
{
    public string questID;
    [SerializeField] private bool GenerateID; 
    public string questName;
    [TextArea]
    public string description;

    public List<QuestRequire> requires=new List<QuestRequire>();
    public List<InventoryItem> rewards=new List<InventoryItem>();

    public bool isStarted;
    public bool isCompleted;
    public bool isFinished;

    private void OnValidate()
    {
        if (GenerateID)
        {
            if (questID == string.Empty)
            {
                questID = System.Guid.NewGuid().ToString();
            }
            GenerateID = false;
        }
        foreach (QuestRequire require in requires)
        {
            require.GenerateID();
        }
    }
    public void UpdateCompletement()
    {
        foreach (QuestRequire require in requires)
        {
            if (!require.IsRequireCompleted)
            {
                isCompleted = false;
                return;
            }
        }
        isCompleted = true;
    }
}
[System.Serializable]
public class QuestRequire
{
    public QuestRequireType questRequireType;
    [SerializeField]private CharacterEnum characterEnum=CharacterEnum.Null;
    [SerializeField]private ItemEnum itemEnum=ItemEnum.Null;
    [Header("DefeatCharacter/CollectItem")]
    public int id;
    public int requireAmount;
    public int currentAmount;
    
    public void GenerateID()
    {
        if(questRequireType==QuestRequireType.DefeatCharacter&& characterEnum != CharacterEnum.Null)
        {
            id=GameManager.CharacterEnumToID(characterEnum);
        }else if(questRequireType==QuestRequireType.CollectItem&&itemEnum!=ItemEnum.Null)
        {
            id = GameManager.ItemEnumToID(itemEnum);
        }
    }
    public bool IsRequireCompleted
    {
        get
        {
            switch (questRequireType)
            {
                case QuestRequireType.DefeatCharacter:

                    break; 
                case QuestRequireType.CollectItem:
                    TaskManager.Instance.UpdateCollectItemProgress(id, GameManager.Instance.playerInventory.GetItemAmount(id));
                    break;
            }
            if(currentAmount>=requireAmount) return true;
            else return false;

        }
    }
}
