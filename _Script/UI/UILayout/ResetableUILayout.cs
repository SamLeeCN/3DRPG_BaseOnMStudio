using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[RequireComponent(typeof(DataDefination))]
public class ResetableUILayout : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public DataDefination dataDefination;
    public bool isSettingLayout;
    public Vector3 presetAnchoredPos;
    public Vector3 presetScale;
    
    public RectTransform rectTransform;
    public Vector3 cumulativeScale = new Vector3(1, 1, 1);

    public Transform originalParent;
    public bool isActiveBefore=false;

    private void Awake()
    {
        dataDefination = GetComponent<DataDefination>();
        rectTransform = GetComponent<RectTransform>();
        cumulativeScale = GetCumulativeScale();
        if (!LoadLayout())
        {
            rectTransform.anchoredPosition = presetAnchoredPos;
            rectTransform.localScale = presetScale;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        presetAnchoredPos=GetComponent<RectTransform>().anchoredPosition;
        presetScale= GetComponent<RectTransform>().localScale;
    }
#endif
    public void OnDrag(PointerEventData eventData)
    {
        if (!isSettingLayout) { return; }
        cumulativeScale = GetCumulativeScale();
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSettingLayout) { return; }
        rectTransform.SetAsLastSibling();
        OnSelected();
    }
    private void OnSelected()
    {
        SetUILayoutPanel.Instance.selectedLayout = this;
        GoBackToOriginalParent();
        SetUILayoutPanel.Instance.scaleSlider.value = 
            rectTransform.localScale.x/SetUILayoutPanel.Instance.sliderScale;
        GoToLayoutSettingParent();
    }
    private Vector3 GetCumulativeScale()
    {
        Vector3 cumulativeScale = Vector3.one; // Start with no scale
        Transform currentGameObject = transform;//Start from this gameObject

        // Traverse up the hierarchy and multiply the scale factors
        while (currentGameObject != null)
        {
            cumulativeScale = Vector3.Scale(cumulativeScale, currentGameObject.localScale);
            currentGameObject = currentGameObject.parent;
        }
        return cumulativeScale;
    }
    public void SaveLayout()
    {
        GoBackToOriginalParent();
        UIManager.Instance.SaveLayoutOverride(dataDefination);
        GoToLayoutSettingParent();
    }
    public bool LoadLayout()
    {
        return UIManager.Instance.LoadLayoutOverride(dataDefination);
    }
    public void ResetLayout()
    {
        UIManager.Instance.DeleteLayoutOverride(dataDefination);
        GoBackToOriginalParent();
        rectTransform.anchoredPosition = presetAnchoredPos;
        rectTransform.localScale = presetScale;
        GoToLayoutSettingParent();
    }
    public void StartSetting()
    {
        isActiveBefore = gameObject.activeInHierarchy;
        gameObject.SetActive(true);
        originalParent = transform.parent;
        GoToLayoutSettingParent();
        isSettingLayout = true;
    }
    public void StopSetting()
    {
        GoBackToOriginalParent();
        gameObject.SetActive(isActiveBefore);
        isSettingLayout = false;
    }
    public void GoToLayoutSettingParent()
    {
        transform.SetParent(SetUILayoutPanel.Instance.UILayoutDraggingParent);   
    }
    public void GoBackToOriginalParent()
    {
        transform.SetParent(originalParent);
    }

    
}
