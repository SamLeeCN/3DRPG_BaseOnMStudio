using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using System;
using Newtonsoft.Json;
using System.Xml.Serialization;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[DefaultExecutionOrder(-500)]
public class DataManager : Singleton<DataManager>
{
    public string saveFolder;
    private List<ISaveable> saveableList = new List<ISaveable>();

    public SaveProfile<SundrySaveData> sundrySaveProfile;
    const string sundryFileName="sundryData";

    public bool isSavedSceneLoading=false;
    protected override void Awake()
    {
        base.Awake();
        saveFolder = Application.persistentDataPath + "/GameData";
        sundrySaveProfile = new SaveProfile<SundrySaveData>(sundryFileName, new SundrySaveData());
    }
    private void Start()
    {
        LoadUserSetting();
    }
    private void OnEnable()
    {
        
        EventManager.Instance.sceneLoadEndEvent += OnSceneLoadEndEvent;
    }
    private void OnDisable()
    {
        
        EventManager.Instance.sceneLoadEndEvent -= OnSceneLoadEndEvent;
    }
    private void LoadUserSetting()
    {
        LoadKeyBindingOverride();
        //Load other settings
    }
    private void LoadKeyBindingOverride()
    {
        foreach (var actionMap in InputManager.Instance.inputController.asset.actionMaps)
        {
            foreach (var action in actionMap)
            {
                InputManager.Instance.LoadBindingOverride(action);
            }
        }
    }
    public void RegisterSaveable(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }
    public void UnregisterSaveable(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }
    public void Save<T>(SaveProfile<T> saveProfile) where T : SaveProfileData
    {
        if (File.Exists($"{saveFolder}/{saveProfile.profileName}"))
        {
            Delete(saveProfile.profileName);
        }
        var jsonString=JsonConvert.SerializeObject(saveProfile, Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        if(!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
        File.WriteAllText($"{saveFolder}/{saveProfile.profileName}", jsonString);
    }
    public SaveProfile<T> Load<T>(string profileName) where T : SaveProfileData
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            Debug.Log($"Save Profile{profileName} not found!");
            return null;
        }
        var fileContents = File.ReadAllText($"{saveFolder}/{profileName}");
        //decrypt
        return JsonConvert.DeserializeObject<SaveProfile<T>>(fileContents);
    }
    public void Delete(string profileName)
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            Debug.Log($"Save Profile{profileName} not found!");
            return;
        }
        File.Delete($"{saveFolder}/{profileName}");
    }
    public void SaveSO(UnityEngine.Object SO, string key)
    {
        var jsonString = JsonUtility.ToJson(SO);
        PlayerPrefs.SetString(key, jsonString);
        PlayerPrefs.Save();//with this line, even if game crushed, it will save! 
    }
    public bool LoadSO(UnityEngine.Object SO, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), SO);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void DeleteSO(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
    public void SaveAll()
    {
        //Save Scene
        SceneLoadManager.Instance.SaveData();
        //Save saveables(before save sundries)
        foreach (var saveable in saveableList)
        {
            saveable.SaveData();//also save sundries to variable
        }
        //Save sundries from variable to a file
        Save(sundrySaveProfile);
        EventManager.Instance.RaiseDataSaveEvent();
        //Save Tasks
        TaskManager.Instance.SaveTasks();
    }
    public void LoadAll()
    {
        if(GameManager.Instance.IsPlayerEnable()) GameManager.Instance.playerControler.agent.enabled = false;
        isSavedSceneLoading = true;
        //Load Scene first
        SceneLoadManager.Instance.LoadData();
        //Load all saveable and sundries in OnSceneLoadEvent
        EventManager.Instance.RaiseDataLoadEvent();
        //Load Tasks
        TaskManager.Instance.LoadTasks();
    } 
    private void OnSceneLoadEndEvent(bool arg)
    {
        if (!isSavedSceneLoading) return;
        GameManager.Instance.playerControler.gameObject.SetActive(true);
        //Load sundries from file to variable(before load saveables)
        Load<SundrySaveData>(sundryFileName);
        //Load saveables
        foreach (var saveable in saveableList)
        {
            saveable.LoadData();//also read sundries from the variable
        }
        GameManager.Instance.playerControler.agent.enabled = true;
        isSavedSceneLoading = false;
    }
    public void DeleteAll()
    {
        //Delet scene
        SceneLoadManager.Instance.DeleteData();
        //Delete saveables
        foreach (var saveable in saveableList)
        {
            saveable.DeleteData();
        }
        //Delete sundries file
        Delete(sundryFileName);
        EventManager.Instance.RaiseDataDeleteEvent();
        //Delete tasks
        TaskManager.Instance.DeleteTasks();

    }


    public void DebugSaveLoad()//FIXME
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveAll();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadAll();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteAll();
        }
    }
    private void Update()
    {
        DebugSaveLoad();//FIXME
    }
}
