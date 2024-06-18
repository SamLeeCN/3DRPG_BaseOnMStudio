using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class DataDefination : MonoBehaviour
{
    public PersistentType persistentType;
    public string ID;
    private void OnValidate()
    {
        if (persistentType == PersistentType.ReadWrite)
        {
            if(ID==string.Empty)
            {
                ID=System.Guid.NewGuid().ToString();
            }
        }
        else
        {
            ID = string.Empty;
        }
    }
}
