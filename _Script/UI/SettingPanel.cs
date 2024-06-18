using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SettingPanel : Singleton<SettingPanel>,IParentPanel
{
    public GameObject UILayoutSettingPanel;
    public bool isSettingUILayout=false;
    public void Open()
    {
        gameObject.SetActive(true);
        UIManager.Instance.playerStatus.SetActive(false);
        UIManager.Instance.actionBarGameObject.SetActive(false);
        UpdateSonPanelState();
    }
    public void InitializeAllSonPanels()
    {
        UILayoutSettingPanel.SetActive(true);
        UILayoutSettingPanel.SetActive(false);
    }
    public void Close()
    {   //close all son panel
        if(isSettingUILayout) SetUILayoutPanel.Instance.Close();
        UpdateSonPanelState();
        gameObject.SetActive(false);
        if (!SceneLoadManager.Instance.currentScene.isPlyerDisabled)
        {
            UIManager.Instance.playerStatus.SetActive(true);
            UIManager.Instance.actionBarGameObject.SetActive(true);
        }
    }
    public void OpenSetUILayoutPanel()
    {
        SetUILayoutPanel.Instance.Open();
        UpdateSonPanelState();
    }
    public void CloseSetUILayoutPanel()
    {
        SetUILayoutPanel.Instance.Close();
        UpdateSonPanelState();
    }
    public void UpdateSonPanelState()
    {
        isSettingUILayout = UILayoutSettingPanel.activeInHierarchy;
    }
}
