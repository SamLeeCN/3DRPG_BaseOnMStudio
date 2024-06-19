using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using System;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;
using System.IO;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CustomEditor(typeof(DialogueDataSO))]    //really important
public class DialogueCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open In Editor"))
        {
            DialogueEditor.InitWindow((DialogueDataSO)target);
        }
        base.OnInspectorGUI();
    }
}
public class DialogueEditor : EditorWindow
{
    DialogueDataSO currentData;
    ReorderableList pieceList=null;
    Dictionary<int, ReorderableList> optionListDict = new Dictionary<int, ReorderableList>();
    Vector2 scrollPos = Vector2.zero;

    public object OnDrawPieceElement { get; private set; }

    [MenuItem("CustomEditors/DialogueEditor")]
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.autoRepaintOnSceneChange = true;
    }

    public static void InitWindow(DialogueDataSO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.currentData = data;
    }
    [OnOpenAsset]
    public static bool OpenAsset(int instanceID,int line)
    {
        DialogueDataSO data=EditorUtility.InstanceIDToObject(instanceID) as DialogueDataSO;
        if(data!=null) 
        { 
            DialogueEditor.InitWindow(data);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnSelectionChange()
    {
        DialogueDataSO newData = Selection.activeObject as DialogueDataSO;

        if (newData != null)
        {
            currentData = newData;
            pieceList = null;
        }
        else
        {
            currentData = null;
            pieceList = null;
        }
        Repaint();
    }
    private void OnGUI()
    {
        if(currentData != null)
        {
            optionListDict.Clear();
            for (int i = 0; i < currentData.dialoguePieces.Count; i++)
            {
                currentData.dialoguePieces[i].index = i;
            }

            EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);
            GUILayout.Space(10);

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (pieceList == null)
            {
                SetUpReorderbleList();
            }
            pieceList.DoLayoutList();

            GUILayout.EndScrollView();
        }
        else
        {
            if(GUILayout.Button("Create New Dialogue"))
            {
                string datePath = "Assets/_DataSO/Dialogue/";
                if(!Directory.Exists(datePath))
                {
                    Directory.CreateDirectory(datePath);
                }
                DialogueDataSO newData = ScriptableObject.CreateInstance<DialogueDataSO>();
                AssetDatabase.CreateAsset(newData, datePath+"NewDialogue.asset");
                currentData = newData;
            }
            GUILayout.Label("No data selected!", EditorStyles.boldLabel);
        }
    }
    private void OnDisable()
    {
        optionListDict.Clear();
    }
    private void SetUpReorderbleList()
    {
        pieceList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece), true, true, true, true);

        pieceList.drawHeaderCallback += OnDrawPieceListHeader;
        pieceList.drawElementCallback += OnDrawPieceListElement;
        pieceList.elementHeightCallback += OnHeightChanged;
    }
    private void OnDrawPieceListHeader(Rect rect)
    {
        GUI.Label(rect, "Dialogue Pieces");

    }
    private float OnHeightChanged(int index)
    {
        return GetPieceHeight(currentData.dialoguePieces[index]);
    }

    private float GetPieceHeight(DialoguePiece piece)
    {
        float height = EditorGUIUtility.singleLineHeight;
        if(piece.isExpanding)
        {
            height += EditorGUIUtility.singleLineHeight * 9;
            if (piece.options.Count >= 1)
            {
                height += EditorGUIUtility.singleLineHeight * piece.options.Count;
            }
        }
        return height;
    }

    private void OnDrawPieceListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorUtility.SetDirty(currentData);  //To make sure save in real time

        GUIStyle textStyle = new GUIStyle("TextField");
        if(index<currentData.dialoguePieces.Count)
        {
            DialoguePiece currentPiece= currentData.dialoguePieces[index];
            Rect tmpRect = rect;

            //fold button
            tmpRect.width = rect.width * 0.1f;
            tmpRect.height = EditorGUIUtility.singleLineHeight;
            currentPiece.isExpanding = EditorGUI.Foldout(tmpRect, currentPiece.isExpanding, currentPiece.id);

            if (!currentPiece.isExpanding) return;

            //id
            tmpRect.y += EditorGUIUtility.singleLineHeight;
            tmpRect.height = EditorGUIUtility.singleLineHeight;
            tmpRect.width = 20;
            EditorGUI.LabelField(tmpRect, "ID");

            tmpRect.x += tmpRect.width + 5;
            tmpRect.width = rect.width * 0.3f;
            currentPiece.id = EditorGUI.TextField(tmpRect, currentPiece.id);
            //quest
            tmpRect.x += tmpRect.width + 5;
            tmpRect.width = 40;
            EditorGUI.LabelField(tmpRect, "Quest");

            tmpRect.x += tmpRect.width;
            tmpRect.width = rect.width * 0.4f;
            currentPiece.quest = (QuestDataSO)EditorGUI.ObjectField(tmpRect, currentPiece.quest, typeof(QuestDataSO), false);
            //line change
            //sprite
            tmpRect.y += EditorGUIUtility.singleLineHeight + 5;
            tmpRect.x = rect.x;
            tmpRect.height = 60;
            tmpRect.width = tmpRect.height;
            currentPiece.sprite = (Sprite)EditorGUI.ObjectField(tmpRect, currentPiece.sprite, typeof(Sprite), false);
            //text
            tmpRect.x += tmpRect.width + 5;
            tmpRect.width = rect.width - tmpRect.x;
            textStyle.wordWrap = true;
            currentPiece.text = EditorGUI.TextField(tmpRect, currentPiece.text, textStyle);
            //line change
            //option
            tmpRect.y += tmpRect.height + 5;
            tmpRect.x = rect.x;
            tmpRect.width = rect.width;
            int optionListKey = currentPiece.index;

            if (!optionListDict.ContainsKey(optionListKey))
            {
                ReorderableList optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption), true, true, true, true);
                optionListDict[optionListKey] = optionList;
                optionList.drawHeaderCallback += OnDrawOptionHeader;
                optionList.drawElementCallback += (optionRect, optionIndex, optionActive, optionFocused) =>
                {
                    OnDrawOptionElement(currentPiece, optionRect, optionIndex, optionActive, optionFocused);
                };

            }
            optionListDict[optionListKey].DoList(tmpRect);   //Not DoLayoutList()


        }
    }

    private void OnDrawOptionHeader(Rect rect)
    {
        Rect tmpRect = rect;
        tmpRect.width = rect.width * 0.5f;
        tmpRect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(tmpRect,"Option Text");
        
        tmpRect.x += tmpRect.width + 5;
        tmpRect.width = rect.width * 0.2f;
        tmpRect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(tmpRect, "Target ID");

        tmpRect.x += tmpRect.width + 5;
        tmpRect.width = rect.width * 0.25f;
        tmpRect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(tmpRect, "Take Quest");

    }

    private void OnDrawOptionElement(DialoguePiece currentPiece, Rect optionRect, int optionIndex, bool optionActive, bool optionFocused)
    {
        DialogueOption currentOption = currentPiece.options[optionIndex];
        Rect tmpRect = optionRect;
        tmpRect.height = EditorGUIUtility.singleLineHeight;
        //option text
        tmpRect.width = optionRect.width * 0.5f;
        currentOption.text = EditorGUI.TextField(tmpRect, currentOption.text);
        //target id
        tmpRect.x += tmpRect.width + 5;
        tmpRect.width = optionRect.width * 0.2f;
        currentOption.targetID = EditorGUI.TextField(tmpRect, currentOption.targetID);
        //take quest toggle
        tmpRect.x += tmpRect.width + 5;
        tmpRect.width = optionRect.width * 0.1f;
        currentOption.isTakingQuest = EditorGUI.Toggle(tmpRect, currentOption.isTakingQuest);
        
    }

    
}
