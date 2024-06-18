using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Billboard : MonoBehaviour
{
    public Camera cam;
    void Start()
    {
        cam=Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position+cam.transform.forward);
    }
}
