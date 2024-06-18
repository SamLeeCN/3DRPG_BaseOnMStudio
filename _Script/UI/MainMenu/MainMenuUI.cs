using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class MainMenuUI : MonoBehaviour
{
    public Button newGameBtn;
    public Button continueBtn;
    public Button exitBtn;
    void Start()
    {

    }

    void Update()
    {

    } 
    public void NewGame()
    {
        GameManager.Instance.NewGame();
    }
    public void Continue()
    {
        GameManager.Instance.Continue();
    }
    public void Exit()
    {
        GameManager.Instance.ExitGame();
    }
}
