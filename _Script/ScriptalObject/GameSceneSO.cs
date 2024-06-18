using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="DataSO/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public AssetReference sceneReference;
    public SceneType sceneType=SceneType.Location;
    public bool isOverrideCamera=false;
    public bool isPlyerDisabled=false;
    public bool isWorldUIDisabled = false;
}
