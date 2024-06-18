using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class CameraManager : Singleton<CameraManager>
{
    Camera cam;
    public Camera UICam;
    public CinemachineVirtualCameraBase playerCam;
    public CinemachineVirtualCameraBase playerUICam;
    
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
    }
    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }
    public void SetPlayerCameraEnablity(bool enability)
    {
        playerCam.enabled = enability;
    }
    public void SetWorldUICameraEnability(bool enability)
    {
        playerUICam.enabled = enability;
    }
    
    void Start()
    {

    }

    void Update()
    {

    } 
}
