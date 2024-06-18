using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class FillBar : MonoBehaviour
{
    public Transform barUITrans;
    public Image fill;
    public Image fillDelay;
    public float fillDelayRate=0.1f;
    public bool isVisible=true;
    public bool isAlwaysVisible=false;
    public float visibleDuration=5;
    public float visibleRemainTime;
    void Update()
    {
        FillUIDelay();
        VisibleTimer();
    }
    private void Awake()
    {
        if (!isVisible||!isAlwaysVisible)
        {
            barUITrans.gameObject.SetActive(false);
        }
    }
    public void UpdateFillUI(int currentHealth,int maxHealth)
    {
        fill.fillAmount=(float)currentHealth/maxHealth;
        SetVisible();
    }
    public void FillUIDelay()
    {
        if (!fillDelay) return;
        if (fillDelay.fillAmount > fill.fillAmount)
        {
            fillDelay.fillAmount -= Time.deltaTime * fillDelayRate;
        }
        else
        {
            fillDelay.fillAmount = fill.fillAmount;
        }
    }
    public void VisibleTimer()
    {
        if (isAlwaysVisible) return;
            if(visibleRemainTime > 0)
            {
                visibleRemainTime -= Time.deltaTime;
            }
            else
            {
                barUITrans.gameObject.SetActive(false);
            }
    }
    public void SetVisible()
    {
        if (isVisible)
        {
            barUITrans.gameObject.SetActive(true);
            visibleRemainTime = visibleDuration;
        }
    }
}
