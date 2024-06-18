using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Particle : MonoBehaviour
{
    float duration = 10;
    void Update()
    {
        duration-=Time.deltaTime;
        if(duration < 0)
        {
            Destroy(gameObject);
        }
    } 
}
