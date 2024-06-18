using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[DefaultExecutionOrder(-1000)]
public class EventManager : Singleton<EventManager>
{
    public Vector3EventSO mouseClickGroundEvent;
    public GameObjectEventSO mouseClickEnemyEvent;
    public GameObjectEventSO mouseClickProjectileEvent;
    public GameObjectEventSO mouseClickInteractableEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO toggleSettingPanelEvent;
    public event Action rebindCompleteEvent;
    public event Action rebindCancelEvent;
    public event Action<InputAction,int> rebindStartedEvent;
    public event Action<GameSceneSO,bool,bool> sceneLoadEvent;
    public event Action <bool>sceneLoadEndEvent;
    public event Action<float> screenFadeInEvent;
    public event Action<float> screenFadeOutEvent;
    public event Action dataSaveEvent;
    public event Action dataLoadEvent;
    public event Action dataDeleteEvent;
    public event Action newGameEvent;
    public event Action<DialogueController> dialogueStartEvent;
    protected override void Awake()
    {
        base.Awake();
    }
    public void RaiseRebindCompleteEvent()
    {
        rebindCompleteEvent?.Invoke();
    }
    public void RaiseRebindCancelEvent()
    {
        rebindCancelEvent?.Invoke();
    }
    public void RaiseRebindStartedEvent(InputAction actionToRebind,int bindingIndex)
    {
        rebindStartedEvent?.Invoke(actionToRebind, bindingIndex);
    }
    public void RaiseSceneLoadEvent(GameSceneSO sceneToLoad,bool doTeleport,bool doFadeScreen=false)
    {
        sceneLoadEvent?.Invoke(sceneToLoad,doTeleport,doFadeScreen);
    }
    public void RaiseSceneLoadEndEvent(bool doTeleport)
    {
        sceneLoadEndEvent?.Invoke(doTeleport);
    }
    public void RaiseScreenFadeInEvent(float duration)
    {
        screenFadeInEvent?.Invoke(duration);
    }
    public void RaiseScreenFadeOutEvent(float duration)
    {
        screenFadeOutEvent?.Invoke(duration);
    }
    public void RaiseDataSaveEvent()
    {
        dataSaveEvent?.Invoke();
    }
    public void RaiseDataLoadEvent()
    {
        dataLoadEvent?.Invoke();
    }
    public void RaiseDataDeleteEvent()
    {
        dataDeleteEvent?.Invoke();
    }
    public void RaiseNewGameEvent()
    {
        newGameEvent?.Invoke();
    }
    public void RaiseDialogueStartEvent(DialogueController dialogueController)
    {
        dialogueStartEvent?.Invoke(dialogueController);
    }
}
