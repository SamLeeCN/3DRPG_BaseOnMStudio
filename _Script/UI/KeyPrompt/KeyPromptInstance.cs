using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class KeyPromptInstance : MonoBehaviour
{
    [SerializeField] private InputAction action;
    [Header("UI Field")]
    [SerializeField] private Image keyImage;
    [SerializeField] private TextMeshProUGUI actionNameText;
    private void OnEnable()
    {
        EventManager.Instance.rebindCompleteEvent += UpdateUI;
        EventManager.Instance.rebindCancelEvent += UpdateUI;
    }
    private void OnDisable()
    {
        EventManager.Instance.rebindCompleteEvent -= UpdateUI;
        EventManager.Instance.rebindCancelEvent -= UpdateUI;
    }
    public void AssignAction(InputAction action)
    {
        this.action = InputManager.Instance.inputController.asset.FindAction(action.name);
        UpdateUI();
    }
    public void UpdateUI()
    {
        string groupName = KeyPrompt.Instance.isKeyboard ? "Keyboard" : "Gamepad";
        //TODO:Update UI
        string keyName = GetKeyBinding(groupName);
        if (string.IsNullOrEmpty(keyName)) return;
        keyImage.sprite = KeyPrompt.Instance.isWhiteStyle ?
            KeyPrompt.Instance.whiteKeys[keyName] : KeyPrompt.Instance.blackKeys[keyName];
        actionNameText.text = action.name;
        actionNameText.color= KeyPrompt.Instance.isWhiteStyle ?Color.white : Color.black;
    }
    private string GetKeyBinding(string groupName)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].groups != null && action.bindings[i].groups.Equals(groupName))
            {
                if (action.bindings[i].isComposite) return null;else return action.GetBindingDisplayString(i);
            }
        }
        return null;
    }
}
