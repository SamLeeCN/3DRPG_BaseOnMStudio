using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[DefaultExecutionOrder(-500)]
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }
    protected virtual void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }
    public static bool isInitialized
    {
        get { return instance != null; }
    }
    protected virtual void OnDestroy()
    {
        if (instance != null)
        {
            instance=null;
        }
    }
}
