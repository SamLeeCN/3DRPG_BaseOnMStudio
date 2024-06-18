using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="Event/GameObjectEventSO")]
public class GameObjectEventSO : ScriptableObject
{
    public UnityAction<GameObject> OnEventRaised;
    public void RaiseEvent(GameObject gameObject)
    {
        OnEventRaised?.Invoke(gameObject);
    }
}

