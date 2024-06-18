using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class InputManager : Singleton<InputManager>
{
    public InputController inputController;
    #region Game Play
    public Vector2 MoveInput { get; private set; }
    public bool InteractInput { get; private set; }
    //Jump
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool ActionBar1Input { get; private set; }
    public bool ActionBar2Input { get; private set; }
    public bool ActionBar3Input { get; private set; }
    public bool ActionBar4Input { get; private set; }
    public bool ActionBar5Input { get; private set; }
    public bool ActionBar6Input { get; private set; }
    public bool DropItemInput { get; private set; }
    #endregion
    #region UI
    public bool SettingPanelToggleInput { get; private set; }
    public bool ConfirmInput { get; private set; }
    public bool InventoryToggleInput { get; private set; }

    public bool TaskPanelToogleInput { get; private set; }
    #endregion

    #region Game Play
    public InputAction moveAction;
    public InputAction interactAction;
    public InputAction jumpAction;
    public InputAction actionBar1Action;
    public InputAction actionBar2Action;
    public InputAction actionBar3Action;
    public InputAction actionBar4Action;
    public InputAction actionBar5Action;
    public InputAction actionBar6Action;
    public InputAction dropItemAction;
    #endregion
    #region UI
    public InputAction confirmAction;
    public InputAction settingPanelToggleAction;
    public InputAction inventoryToggleAction;
    public InputAction taskPanelToggleAction;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        if(inputController == null) inputController = new InputController();        
        SetUpInputAction();
    }
    private void OnEnable()
    {
        inputController.Enable();
    }
    private void OnDisable()
    {
        inputController.Disable();
    }
    private void Update()
    {
        UpdateInputs();
    }
    public void SetUpInputAction()
    {
        moveAction = inputController.GamePlay.Move;
        interactAction = inputController.GamePlay.Interact;
        jumpAction = inputController.GamePlay.Jump;
        
        actionBar1Action = inputController.GamePlay.Action1;
        actionBar2Action = inputController.GamePlay.Action2;
        actionBar3Action = inputController.GamePlay.Action3;
        actionBar4Action = inputController.GamePlay.Action4;
        actionBar5Action = inputController.GamePlay.Action5;
        actionBar6Action = inputController.GamePlay.Action6;
        dropItemAction = inputController.GamePlay.DropItem;

        confirmAction = inputController.UI.Confirm;
        settingPanelToggleAction = inputController.UI.SettingPanelToggle;
        inventoryToggleAction = inputController.UI.InventoryToggle;
        taskPanelToggleAction = inputController.UI.TaskPanelToggle;
        
    }
    private void UpdateInputs()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        InteractInput=interactAction.WasPressedThisFrame();
        ActionBar1Input = actionBar1Action.WasPressedThisFrame();
        ActionBar2Input = actionBar2Action.WasPressedThisFrame();
        ActionBar3Input = actionBar2Action.WasPressedThisFrame();
        ActionBar4Input = actionBar2Action.WasPressedThisFrame();
        ActionBar5Input = actionBar2Action.WasPressedThisFrame();
        ActionBar6Input = actionBar2Action.WasPressedThisFrame();
        DropItemInput = dropItemAction.WasPressedThisFrame();

        //Jump
        JumpPressed = jumpAction.WasPressedThisFrame();
        JumpHeld =jumpAction.WasPerformedThisFrame();
        JumpReleased = jumpAction.WasReleasedThisFrame();

        ConfirmInput = confirmAction.WasPressedThisFrame();
        InventoryToggleInput = inventoryToggleAction.WasPressedThisFrame();
        SettingPanelToggleInput = settingPanelToggleAction.WasPressedThisFrame();
        TaskPanelToogleInput = taskPanelToggleAction.WasPressedThisFrame();
    }
    #region Rebind
    public string GetBindingString(InputAction action, int bindingIndex)
    {
        return action.GetBindingDisplayString(bindingIndex);
    }
    public void RebindInput(InputAction actionToRebind,int bindingIndex, TextMeshProUGUI bindingButtonText,bool excludeMouse)
    {
        if(actionToRebind==null||actionToRebind.bindings.Count<=bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }
        if (actionToRebind.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex=bindingIndex+1;
            /*coz for composited bindings,
            bindingIndex is actually the parent of the composited bindings group,
            and bindingIndex+1 is the index of the first member of the group*/
            if (firstPartIndex < actionToRebind.bindings.Count && actionToRebind.bindings[firstPartIndex].isComposite)
            {
                DoRebind(actionToRebind, firstPartIndex, bindingButtonText,true,excludeMouse);
            }
        }
        else
        {
            DoRebind(actionToRebind,bindingIndex, bindingButtonText,false,excludeMouse);
        }
    }
    private void DoRebind(InputAction actionToRebind,int bindingIndex,TextMeshProUGUI bindingButtonText,bool isCompositePart,bool excludeMouse)
    {//has to be private, or it will be confusing
        if(actionToRebind==null||bindingIndex<0) return;
        bindingButtonText.text = $"Press a {actionToRebind.expectedControlType}";
        //disable the action to rebind when rebinding, and enable it after rebinding
        actionToRebind.Disable();
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();//to prevent memories leaking
            if(isCompositePart)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                {
                    DoRebind(actionToRebind, nextBindingIndex, bindingButtonText, true, excludeMouse);
                }
            }
            SaveBindingOverride(actionToRebind);
            EventManager.Instance.RaiseRebindCompleteEvent();
        });
        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();//to prevent memories leaking
            EventManager.Instance.RaiseRebindCancelEvent();
        });
        //Cancel through or exclude specific key/device
        rebind.WithCancelingThrough("<Keyboard>/escape");
        if (excludeMouse) rebind.WithControlsExcluding("Mouse");

        EventManager.Instance.RaiseRebindStartedEvent(actionToRebind, bindingIndex);
        rebind.Start();//actually starts the rebinding process
    }
    
    private void SaveBindingOverride(InputAction action)
    {
        for(int i=0; i<action.bindings.Count;i++)
        {
            PlayerPrefs.SetString("ActionBinding" + action.actionMap + action.name + i, action.bindings[i].overridePath);
            PlayerPrefs.Save();//with this line, even if game crushed, it will save! 
        }
    }
    public void LoadBindingOverride(InputAction action)
    {
        for(int i=0;i<action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ActionBinding" + action.actionMap + action.name + i)))
            {
                action.ApplyBindingOverride(i, PlayerPrefs.GetString("ActionBinding" + action.actionMap + action.name + i));
            }
        }
    }
    public void ResetBinding(InputAction action,int bindingIndex)
    {
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }
        if (action.bindings[bindingIndex].isComposite)
        {
            for(int i=bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }
        SaveBindingOverride(action);
    }
    public void ResetAllKeyboardBindings()
    {
        foreach (var actionMap in inputController.asset.actionMaps)
        {
            foreach (var action in actionMap)
            {
                for(int i=0;i< action.bindings.Count; i++)
                {
                    if (action.bindings[i].groups!=null &&action.bindings[i].groups.Equals("Keyboard")){
                        ResetBinding(action, i);
                        //Update UI by closing the setting panel and reopen it
                        UIManager.Instance.settingPanel.SetActive(false);
                        UIManager.Instance.settingPanel.SetActive(true);
                    }
                }
            }
        }
    }
    public InputAction ActionNameToAction(string actionName)
    {
        if (inputController == null) inputController = new InputController();
        return inputController.asset.FindAction(actionName);
    }
    public InputAction ActionNameToAction(string actionName,string mapName)
    {
        if (inputController == null) inputController = new InputController();
        return inputController.asset.FindActionMap(mapName).FindAction(actionName);
    }
    public string ActionToActionName(InputAction action)
    {
        return action.name;
    }
    #endregion
}