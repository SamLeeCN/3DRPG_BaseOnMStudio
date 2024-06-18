using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class DragPanel : MonoBehaviour,IDragHandler,IPointerDownHandler
{

    RectTransform rectTransform;
    public Vector3 cumulativeScale=new Vector3(1,1,1);
    

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cumulativeScale = ExtensionMethod.GetCumulativeScale(rectTransform);
    }
    public void OnDrag(PointerEventData eventData)
    {
        cumulativeScale = ExtensionMethod.GetCumulativeScale(rectTransform);
        rectTransform.anchoredPosition += eventData.delta /cumulativeScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }
}
