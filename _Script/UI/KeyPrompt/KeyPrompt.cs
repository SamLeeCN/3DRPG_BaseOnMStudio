using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class KeyPrompt : Singleton<KeyPrompt> 
{
    [SerializeField] private Sprite[] blackKeySprites;
    [SerializeField] private Sprite[] whiteKeySprites;
    [SerializeField] public Dictionary<string, Sprite> blackKeys = new Dictionary<string, Sprite>();
    [SerializeField] public Dictionary<string, Sprite> whiteKeys = new Dictionary<string, Sprite>();
    [SerializeField] private GameObject instancePrefab;
    public Dictionary<string,GameObject> keyPromptInstances= new Dictionary<string,GameObject>();
    public bool isKeyboard;
    public bool isWhiteStyle;
    private void Start()
    {
        foreach (Sprite keySprite in blackKeySprites)
            blackKeys.Add(keySprite.name, keySprite);
        foreach (Sprite keySprite in whiteKeySprites)
            whiteKeys.Add(keySprite.name, keySprite);
        isKeyboard= true;
        AddKeyPrompt(InputManager.Instance.settingPanelToggleAction);
    }
    private void OnEnable()
    {
        InputSystem.onActionChange += OnInputActionChange;
    }
    private void OnDisable()
    {
        InputSystem.onActionChange -= OnInputActionChange;
    }
    private void OnInputActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            var device = ((InputAction)obj).activeControl.device;
            switch (device.device)
            {
                case Keyboard:
                    isKeyboard = true;
                    break;
                case Gamepad:
                    isKeyboard = false;
                    break;
            }
            UpdateAllUI();
        }
    }
    public void AddKeyPrompt(InputAction action)
    {
        if (keyPromptInstances.ContainsKey(action.name)) return;
        GameObject currentInstanceGameObject= Instantiate(instancePrefab, transform.position, Quaternion.identity,transform);
        KeyPromptInstance currentInstance= currentInstanceGameObject.GetComponent<KeyPromptInstance>();
        currentInstance.AssignAction(action);
        keyPromptInstances.Add(action.name, currentInstanceGameObject);
    }
    public void DeleteKeyPrompt(InputAction action)
    {
        if (!keyPromptInstances.ContainsKey(action.name)) return;
        Destroy(keyPromptInstances[action.name]);
        keyPromptInstances.Remove(action.name);
    }
    public void UpdateAllUI()
    {
        foreach (var pair in keyPromptInstances)
        {
            pair.Value.GetComponent<KeyPromptInstance>().UpdateUI();
        }
    }
}