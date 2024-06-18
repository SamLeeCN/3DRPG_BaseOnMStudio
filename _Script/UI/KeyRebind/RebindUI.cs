using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[DefaultExecutionOrder(1000)]
public class RebindUI : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputActionReference;//this is on the SO
    [SerializeField]
    private bool excludeMouse = true;
    [Range(0, 20)]
    [SerializeField]
    private int selectedBindingIndex;
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding currentInputBinding;
    private int currentBindingIndex;
    [SerializeField] private string actionName;
    [SerializeField] private string actionMap;
    [Header("UI Fields")]
    [SerializeField]
    private TextMeshProUGUI actionNameText;
    [SerializeField]
    private Button bindingButton;
    [SerializeField]
    private TextMeshProUGUI bindingButtonText;
    [SerializeField]
    private Button resetButton;
    private void OnEnable()
    {
        bindingButton.onClick.AddListener(() => Rebind());
        resetButton.onClick.AddListener(() => ResetBinding());
        if (inputActionReference != null)
        {
            GetBindingInfo();
            UpdateUI();
        }

        EventManager.Instance.rebindCompleteEvent += UpdateUI;
        EventManager.Instance.rebindCancelEvent += UpdateUI;
    }
    private void OnDisable()
    {
        EventManager.Instance.rebindCompleteEvent -= UpdateUI;
        EventManager.Instance.rebindCancelEvent -= UpdateUI;
    }
    private void OnValidate()
    {
        if(inputActionReference == null) return;
        GetBindingInfo();
        UpdateUI();
    }
    private void GetBindingInfo()
    {//ONLY USE IN EDITING! so grab information from SO
        if (inputActionReference.action != null) 
        {
            actionMap = inputActionReference.action.actionMap.name;
            actionName = inputActionReference.action.name; 
        }
        if (inputActionReference.action.bindings.Count > selectedBindingIndex)
        {
            currentInputBinding = inputActionReference.action.bindings[selectedBindingIndex];
            currentBindingIndex = selectedBindingIndex;
        }
    }
    private void UpdateUI()
    {
        if(actionNameText!= null) actionNameText.text = actionName;
        if (bindingButtonText != null)
        {
            if(Application.isPlaying)
            {//When playing, grabing information from input manager, not SO!!!
                InputAction action = InputManager.Instance.ActionNameToAction(actionName,actionMap);
                bindingButtonText.text = InputManager.Instance.GetBindingString(action, currentBindingIndex);
            }
            else//ONLY USE IN EDITING! so grab information from SO
            {//grab information from SO
                bindingButtonText.text=inputActionReference.action.GetBindingDisplayString(currentBindingIndex); 
            }
        }
    }
    private void Rebind()
    {
        InputAction action = InputManager.Instance.ActionNameToAction(actionName,actionMap);
        InputManager.Instance.RebindInput(action,currentBindingIndex, bindingButtonText,excludeMouse);
    }
    public void ResetBinding()
    {
        InputAction action = InputManager.Instance.ActionNameToAction(actionName, actionMap);
        InputManager.Instance.ResetBinding(action, currentBindingIndex);
        UpdateUI();
    }
}
