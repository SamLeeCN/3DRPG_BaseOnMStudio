using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class UIManager : Singleton<UIManager>
{
    [Header("States")]
    public bool isSettingPanelOpen;
    public bool isInventoryOpen;
    public bool isDialoguePanelOpen;
    public bool isTaskPanelOpen;

    [Header("Reference")]
    public GameObject settingPanel;
    public GameObject playerStatus;
    public GameObject actionBarGameObject;
    public GameObject inventoryPanel;
    public GameObject dialoguePanel;
    public DialoguePanel dialoguePanelMono;
    public GameObject taskPanel;

    [Header("PlayerSettings")]
    public float dialogueTextShowDuration;

    
    protected override void Awake()
    {
        base.Awake();
        InitializeAllSonPanels();
    }
    void Start()
    {
        dialoguePanelMono = dialoguePanel.GetComponent<DialoguePanel>();
    }
    private void OnEnable()
    {
        EventManager.Instance.toggleSettingPanelEvent.OnEventRaised += OnToggleSettingPanelEvent;
    }
    private void OnDisable()
    {
        EventManager.Instance.toggleSettingPanelEvent.OnEventRaised -= OnToggleSettingPanelEvent;
    }
    void Update()
    {
        Control();
        
    } 
    public void InitializeAllSonPanels()
    {
        settingPanel.SetActive(true);
        SettingPanel.Instance.InitializeAllSonPanels();
        settingPanel.SetActive(false);
        inventoryPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        dialoguePanel.SetActive(true);
        dialoguePanel.SetActive(false);
        taskPanel.SetActive(true);
        taskPanel.SetActive(false);
    }
    public void UpdateCharacterHealthUI(FillBar healthBar,int currentHealth,int maxHealth)
    {
        if(healthBar) healthBar.UpdateFillUI(currentHealth, maxHealth);
    }
    public void Control()
    {
        if (InputManager.Instance.SettingPanelToggleInput)
        {
            EventManager.Instance.toggleSettingPanelEvent.RaiseEvent();
        }
        if(InputManager.Instance.InventoryToggleInput)
        {
            ToggleInventoryPanel();
        }
        if (InputManager.Instance.TaskPanelToogleInput)
        {
            ToggleTaskPanel();
        }
        //TODO:other controls
    }
    #region Setting Panel
    private void OnToggleSettingPanelEvent()
    {
        if (settingPanel.activeInHierarchy)
        {
            CloseSettingPanel();
        }
        else
        {
            OpenSettingPanel();
        }
    }
    public void OpenSettingPanel()
    {
        SettingPanel.Instance.Open();
        UpdateSonPanelState();
    }
    public void CloseSettingPanel()
    {
        SettingPanel.Instance.Close();
        UpdateSonPanelState();
    }
    #endregion
    #region Inventory Panel
    public void ToggleInventoryPanel()
    {
        if (inventoryPanel.activeInHierarchy)
        {
            CloseInventoryPanel();

        }
        else
        {
            OpenInventoryPanel();
        }
    }
    public void OpenInventoryPanel()
    {
        InventoryManager.Instance.SetInventory(GameManager.Instance.playerInventory);
        InventoryManager.Instance.Open();
        UpdateSonPanelState() ;
    }
    public void CloseInventoryPanel()
    {
        InventoryManager.Instance.Close();
        UpdateSonPanelState();
    }
    #endregion
    #region Task Panel
    public void ToggleTaskPanel()
    {
        if (taskPanel.activeInHierarchy)
        {
            CloseTaskPanel();
        }
        else
        {
            OpenTaskPanel();
        }
    }
    public void OpenTaskPanel()
    {
        TaskPanel.Instance.Open();
        UpdateSonPanelState();
    }
    public void CloseTaskPanel()
    {
        TaskPanel.Instance.Close();
        UpdateSonPanelState();
    }
    #endregion
    #region DialoguePanel
    public void OpenDialoguePanel(DialogueController dialogueController)
    {
        DialoguePanel.Instance.Open(dialogueController);
        UpdateSonPanelState();
    }
    public void CloseDialoguePanel()
    {
        DialoguePanel.Instance.Close();
        UpdateSonPanelState();
    }
    #endregion
    public void UpdateSonPanelState()
    {
        isSettingPanelOpen = settingPanel.activeInHierarchy;
        isInventoryOpen= inventoryPanel.activeInHierarchy;
        isDialoguePanelOpen = dialoguePanel.activeInHierarchy;
        isDialoguePanelOpen = dialoguePanel.activeInHierarchy;
        isTaskPanelOpen = taskPanel.activeInHierarchy;
    }
    
    public void SetPlayerStatusEnablity(bool enability)
    {
        playerStatus.SetActive(enability);
        actionBarGameObject.SetActive(enability);
    }
    public void SetWorldUIEnablity(bool enability)
    {
        CameraManager.Instance.SetWorldUICameraEnability(enability);
    }


    #region layout setting
    public void SaveLayoutOverride(DataDefination UI_ID)
    {
        RectTransform UIRectTransform = UI_ID.GetComponent<RectTransform>();
        PlayerPrefs.SetFloat("UILayoutSet" + UI_ID + "AnchoredPosX",UIRectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat("UILayoutSet" + UI_ID + "AnchoredPosY", UIRectTransform.anchoredPosition.y);
        PlayerPrefs.SetFloat("UILayoutSet" + UI_ID + "scaleX", UIRectTransform.localScale.x);
        PlayerPrefs.SetFloat("UILayoutSet" + UI_ID + "scaleY", UIRectTransform.localScale.y);
        PlayerPrefs.Save();//with this line, even if game crushed, it will save! 
    }
    public bool LoadLayoutOverride(DataDefination UI_ID)
    {
        RectTransform UIRectTransform = UI_ID.GetComponent<RectTransform>();
        if (PlayerPrefs.HasKey("UILayoutSet" + UI_ID + "AnchoredPosX"))
        {
            float anchorPosX = PlayerPrefs.GetFloat("UILayoutSet" + UI_ID + "AnchoredPosX", UIRectTransform.anchoredPosition.x);
            float anchorPosY= PlayerPrefs.GetFloat("UILayoutSet" + UI_ID + "AnchoredPosY", UIRectTransform.anchoredPosition.y);
            float scaleX= PlayerPrefs.GetFloat("UILayoutSet" + UI_ID + "scaleX", UIRectTransform.localScale.x);
            float scaleY= PlayerPrefs.GetFloat("UILayoutSet" + UI_ID + "scaleY", UIRectTransform.localScale.y);
            UIRectTransform.anchoredPosition = new Vector3(anchorPosX, anchorPosY,0);
            UIRectTransform.localScale = new Vector3(scaleX, scaleY, 1);
            return true;
        }
        return false;
    }
    public void DeleteLayoutOverride(DataDefination UI_ID)
    {
        if (PlayerPrefs.HasKey("UILayoutSet" + UI_ID + "AnchoredPosX"))
        {
            PlayerPrefs.DeleteKey("UILayoutSet" + UI_ID + "AnchoredPosX");
            PlayerPrefs.DeleteKey("UILayoutSet" + UI_ID + "AnchoredPosY");
            PlayerPrefs.DeleteKey("UILayoutSet" + UI_ID + "scaleX");
            PlayerPrefs.DeleteKey("UILayoutSet" + UI_ID + "scaleY");
        }
    }



    #endregion 
}
