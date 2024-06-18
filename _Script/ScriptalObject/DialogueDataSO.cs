using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.AddressableAssets.HostingServices;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(fileName ="NewDialogue",menuName ="DataSO/DialogueSO")]
public class DialogueDataSO : ScriptableObject
{
    
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();
    public Dictionary<string, DialoguePiece> findPieceByID = new Dictionary<string, DialoguePiece>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        Refactor();
        findPieceByID.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!piece.id.Equals("") && !findPieceByID.ContainsKey(piece.id))
            {
                findPieceByID.Add(piece.id, piece);
            }
        }
        for (int i = 0; i < dialoguePieces.Count; i++)
        {
            dialoguePieces[i].index = i;
        }
    }
#endif
    #region refactor
    [SerializeField] private string originalID;
    [SerializeField] private string newID;
    [SerializeField] private bool startToRefactor;
    public void Refactor()//called in OnValidate()
    {
        if(startToRefactor)
        {//Traverse all the id that needed to check
            foreach (var piece in dialoguePieces)
            {
                if (piece.id.Equals(originalID)) piece.id = newID;

                foreach(var option in piece.options)
                {
                    if(option.targetID.Equals(originalID)) option.targetID = newID;
                }
            }
        }
        startToRefactor = false;
    }
    #endregion
}
