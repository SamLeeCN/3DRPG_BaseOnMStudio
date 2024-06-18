using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TeleportDestination : MonoBehaviour
{//Attach on the point to arrive
 //Sometimes even in the same portal, the starting point are not the same as the arriving point
    [Range(0, 20)] public int id;
}
