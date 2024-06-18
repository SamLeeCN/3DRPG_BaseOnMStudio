using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public interface IParentPanel 
{
    public void InitializeAllSonPanels();
    public void UpdateSonPanelState();
    public void Open();
    public void Close();
}
