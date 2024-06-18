using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class DebugObject : MonoBehaviour
{
    RectTransform rectTransform;
    Vector3[] corners = new Vector3[4];
    Vector3 cumulativeScale;
    Vector2 offset;
    void Start()
    {

    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        cumulativeScale=ExtensionMethod.GetCumulativeScale(rectTransform);
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector2 mousePos;
            /*RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition, Camera.main, out mousePos);*/
            Debug.Log("mousePos" + Input.mousePosition/cumulativeScale.x);
            Debug.Log("anchorPos" + rectTransform.anchoredPosition);
            rectTransform.GetWorldCorners(corners);
            Debug.Log("corner1" + corners[1]/cumulativeScale.x);
            Debug.Log("vector" + ((Vector3)rectTransform.anchoredPosition - corners[1]/cumulativeScale.x));


            int cornerIndex = 1;
            
            rectTransform.anchoredPosition =
            ((Vector2)Input.mousePosition+(rectTransform.anchoredPosition*cumulativeScale-(Vector2)corners[cornerIndex])+offset)/cumulativeScale;
                
        }
        
    } 
    public void AddOffset(Vector2 originalOffset)
    {
        float offsetX=originalOffset.x/cumulativeScale.x;
        float offsetY=originalOffset.y/cumulativeScale.y;
        Vector2 anchoredPosOffset=new Vector2 (offsetX, offsetY);
    }
}
