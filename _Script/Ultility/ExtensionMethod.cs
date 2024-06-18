using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Polybrush;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public static class ExtensionMethod 
{
    public static float PlaneDistance(Vector3 pos1, Vector3 pos2)
    {
        return Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
    }
    public static bool SectorJudge(Transform self, Transform target,float cos,float rangeDistance,int type=1)
    {
        Vector3 targetDir = new Vector3(target.position.x - self.position.x, 0f, target.position.z - self.position.z).normalized;
        float dot = Vector3.Dot(self.forward, targetDir);
        if (type == 1)
        {
            return dot >= cos && PlaneDistance(self.position, target.position) < rangeDistance;
        }
        else
        {
            return dot >= Mathf.Sqrt(1 - Mathf.Pow(cos, 2)) && PlaneDistance(self.position, target.position) < rangeDistance;
        }
    }
    public static Vector3 GetCumulativeScale(Transform trans)
    {
        Vector3 cumulativeScale = Vector3.one; // Start with no scale
        Transform currentGameObject = trans.parent;//Start from parent

        // Traverse up the hierarchy and multiply the scale factors
        while (currentGameObject != null)
        {
            cumulativeScale = Vector3.Scale(cumulativeScale, currentGameObject.localScale);
            currentGameObject = currentGameObject.parent;
        }
        return cumulativeScale;
    }
}
