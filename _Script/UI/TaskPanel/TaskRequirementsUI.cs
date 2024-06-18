using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TaskRequirementsUI : MonoBehaviour
{
    public TextMeshProUGUI requireNameTxt;
    public TextMeshProUGUI progressTxt;
    private void Awake()
    {
        requireNameTxt = GetComponent<TextMeshProUGUI>();
        progressTxt= transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void SetUpUI(string requireName,int currentAmount, int requireAmount, bool isTaskFinished)
    {
        requireNameTxt.text = requireName;
        if (isTaskFinished )
        {
            progressTxt.text = requireAmount.ToString() + "/" + requireAmount.ToString();
        }
        else
        {
            progressTxt.text = Mathf.Min(currentAmount, requireAmount).ToString() + "/" + requireAmount.ToString();
        }
    }
}
