using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SetUILayoutPanel : Singleton<SetUILayoutPanel>
{
    public List<ResetableUILayout> layouts=new List<ResetableUILayout>();
    
    public Transform UILayoutDraggingParent;

    public ResetableUILayout selectedLayout;
    public Button saveBtn;
    public Button resetBtn;
    public Button closeBtn;
    public Slider scaleSlider;
    public float sliderScale = 5;
    protected override void Awake()
    {
        base.Awake();
        saveBtn.onClick.AddListener(Save);
        resetBtn.onClick.AddListener(ResetAll);
        closeBtn.onClick.AddListener(Close);
        scaleSlider.onValueChanged.AddListener(OnScaleSliderChange);
    }
    public void Open()
    {
        transform.gameObject.SetActive(true);

        for(int i = 0;i< layouts.Count; i++) 
        {
            layouts[i].StartSetting();
        }
    }
    public void Close()
    {

        for (int i = 0; i < layouts.Count; i++)
        {
            layouts[i].StopSetting();
        }

        transform.gameObject.SetActive(false);
    }
    public void Save()
    {
        for (int i = 0; i < layouts.Count; i++)
        {
            layouts[i].SaveLayout();
        }
    }
    public void ResetAll()
    {
        for (int i = 0; i < layouts.Count; i++)
        {
            layouts[i].ResetLayout();
        }
    }

    public void OnScaleSliderChange(float sliderValue)
    {
        selectedLayout.GoBackToOriginalParent();
        selectedLayout.rectTransform.localScale = 
            new Vector3(sliderValue * sliderScale, sliderValue * sliderScale, 1);
        selectedLayout.GoToLayoutSettingParent();
    }
}

