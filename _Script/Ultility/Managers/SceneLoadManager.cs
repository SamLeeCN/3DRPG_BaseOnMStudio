using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEditor.U2D.Animation;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine.AI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SceneLoadManager : Singleton<SceneLoadManager>,ISaveable
{//Not naming it SceneManager to avoid conflict with Unity pre-set class
    public GameSceneSO mainMenuScene;
    public GameSceneSO natureScene;
    public GameSceneSO mazeScene;

    public GameSceneSO currentScene;
    [SerializeField] private GameSceneSO sceneToLoad;

    [SerializeField] private TeleportType teleportType;
    [SerializeField] private Vector3 posToGo;
    [SerializeField] private int idToGo;
    [SerializeField] private bool isSameSceneTeleport=true;

    [SerializeField] private float screenFadeDuration=1;
    
    private string currentSceneSOKeyName;

    private void OnEnable()
    {
        EventManager.Instance.sceneLoadEvent += OnSceneLoadEvent;
        EventManager.Instance.sceneLoadEndEvent += OnSceneLoadEndEvent;
        EventManager.Instance.newGameEvent += OnNewGameEvent;
    }
    private void OnDisable()
    {
        EventManager.Instance.sceneLoadEvent -= OnSceneLoadEvent;
        EventManager.Instance.sceneLoadEndEvent -= OnSceneLoadEndEvent;
        EventManager.Instance.newGameEvent -= OnNewGameEvent;
    }

    private void OnNewGameEvent()
    {
        EventManager.Instance.RaiseSceneLoadEvent(natureScene,false,false);
    }

    protected override void Awake()
    {
        base.Awake();
        SetFileName();
        //FIXME:Load main menu after main menu is created
        
    }
    private void Start()
    {
        EventManager.Instance.RaiseSceneLoadEvent(mainMenuScene, false, true);

    }
    public void TeleportFromEntrance(TeleportEntrance entrance)
    {
        teleportType = entrance.teleportType;
        sceneToLoad = entrance.sceneToGo;
        isSameSceneTeleport = entrance.isSameSceneTeleport;
        switch (entrance.teleportType)
        {
            case TeleportType.Pos:
                posToGo = entrance.posToGo;
                TeleportPos();
                break;
            case TeleportType.Trans:
                idToGo = entrance.teleportDestinationIdToGo;
                TeleportTrans();
                break;
        }
    }
    private void TeleportTrans()
    {
        Transform player = GameManager.Instance.playerCharacter.transform;
        NavMeshAgent playerAgent= player.GetComponent<NavMeshAgent>(); ;

        if (isSameSceneTeleport)
        {
            TeleportDestination destination = GetTeleportDestination(idToGo);
            playerAgent.enabled = false;
            player.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
            playerAgent.enabled = true;
        }
        else
        {
            EventManager.Instance.RaiseSceneLoadEvent(sceneToLoad, true);
        }
    }
    private void TeleportPos()
    {
        Transform player = GameManager.Instance.playerCharacter.transform;
        if (isSameSceneTeleport)
        {
            player.SetPositionAndRotation(posToGo, player.rotation);
        }
        else
        {
            EventManager.Instance.RaiseSceneLoadEvent(sceneToLoad, true);
        }
    }
    private TeleportDestination GetTeleportDestination(int transportDestinationIdToGo)
    {
        TeleportDestination[] destinations;
        destinations=FindObjectsOfType<TeleportDestination>();
        for(int i=0;i<destinations.Length;i++)
        {
            if (destinations[i].id== transportDestinationIdToGo)
            {
                return destinations[i];
            }
        }
        return null;
    }
    private void OnSceneLoadEvent(GameSceneSO sceneToLoad,bool doTeleport,bool doFadeScreen)
    {
        StartCoroutine(SceneLoadEnumerator(sceneToLoad,doTeleport,doFadeScreen));
    }
    private IEnumerator SceneLoadEnumerator(GameSceneSO sceneToLoad,bool doTeleport, bool doFadeScreen)
    {
        if (doFadeScreen)
        {
            EventManager.Instance.RaiseScreenFadeInEvent(screenFadeDuration);
        }
        if(currentScene!=null)
        {
            ResetSceneSettings(currentScene);
            yield return new WaitForSeconds(screenFadeDuration/2);
            GameManager.Instance.playerControler.gameObject.SetActive(false);
            yield return currentScene.sceneReference.UnLoadScene();
        }
        if (currentScene == null||currentScene.sceneReference!=sceneToLoad.sceneReference) {

            var handle = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
            //true means activate the scene after it is loaded
            yield return handle;
            SceneManager.SetActiveScene(handle.Result.Scene);
            //if it's not active,active by this line
            currentScene = sceneToLoad;
            SetSceneSettings(currentScene);
        }
        EventManager.Instance.RaiseSceneLoadEndEvent(doTeleport);
    }
    private void ResetSceneSettings(GameSceneSO sceneToUnload)
    {
        if (sceneToUnload.isOverrideCamera) CameraManager.Instance.SetPlayerCameraEnablity(true);
        if (sceneToUnload.isPlyerDisabled)
        {
            GameManager.Instance.SetPlayerEnablity(true);
            UIManager.Instance.SetPlayerStatusEnablity(true);
        }
        if (sceneToUnload.isWorldUIDisabled) UIManager.Instance.SetWorldUIEnablity(true);
    }
    private void SetSceneSettings(GameSceneSO sceneLoaded)
    {
        if (sceneLoaded.isOverrideCamera) CameraManager.Instance.SetPlayerCameraEnablity(false);
        if (sceneLoaded.isPlyerDisabled)
        {
            GameManager.Instance.SetPlayerEnablity(false);
            UIManager.Instance.SetPlayerStatusEnablity(false);
        }
        if (sceneLoaded.isWorldUIDisabled) UIManager.Instance.SetWorldUIEnablity(false);
    }
    private void OnSceneLoadEndEvent(bool doTeleport)
    {
        if (currentScene.isPlyerDisabled) return;
        GameManager.Instance.SetPlayerEnablity(true);
        if (!doTeleport) return;
        Transform player = GameManager.Instance.playerCharacter.transform;
        if (teleportType == TeleportType.Trans)
        {
            TeleportDestination destination = GetTeleportDestination(idToGo);
            GameManager.Instance.playerControler.agent.enabled = false;
            //truning off the agent to avoid being send to some where no nav mesh surface and fail to transport
            player.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
            GameManager.Instance.playerControler.agent.enabled = true;
        }
        else
        {
            player.SetPositionAndRotation(posToGo, player.rotation);
        }
        EventManager.Instance.RaiseScreenFadeOutEvent(screenFadeDuration);
    }
    public void SetFileName()
    {
        currentSceneSOKeyName = "savedScene";
    }
    public DataDefination GetID()
    {
        return GetComponent<DataDefination>();
    }

    public void SaveData()
    {
        //Save SO
        DataManager.Instance.SaveSO(currentScene, currentSceneSOKeyName);
    }
    public void LoadData()
    {
        //Load SO
        if (GameManager.Instance.IsPlayerEnable())
        {
            GameManager.Instance.playerControler.agent.enabled = false;
            GameManager.Instance.SetPlayerEnablity(false);
        }
        GameSceneSO savedScene = new GameSceneSO();
        if(DataManager.Instance.LoadSO(savedScene, currentSceneSOKeyName))
        {
            EventManager.Instance.RaiseSceneLoadEvent(savedScene, false, true);
        }
        else
        {
            EventManager.Instance.RaiseSceneLoadEvent(natureScene, false, true);//Load First Scene(not menu!)
        }
    }
    public void DeleteData()
    {
        //DeleteSO
        PlayerPrefs.DeleteKey(currentSceneSOKeyName);
    }
}
