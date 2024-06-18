using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TeleportEntrance : MonoBehaviour,IInteractable
{//Attach on the point to start teleport
    public TeleportType teleportType=TeleportType.Trans;
    public bool isEnterable = true;
    public bool isSameSceneTeleport = true;
    public GameSceneSO sceneToGo;
    [Header("TransTeleport")]
    [Range(0, 20)] public int teleportDestinationIdToGo;
    [Header("PosTeleport")]
    public Vector3 posToGo;
    public void TriggerAction()
    {
        if (isEnterable)
        {
            GameManager.Instance.playerControler.agent.ResetPath();
            GameManager.Instance.playerControler.currentTarget = null;
            SceneLoadManager.Instance.TeleportFromEntrance(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerControler.currentInteractable = GetComponent<IInteractable>();
            GameManager.Instance.playerControler.isInInteractArea = true;
            KeyPrompt.Instance.AddKeyPrompt(InputManager.Instance.interactAction);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerControler.isInInteractArea = false;
            KeyPrompt.Instance.DeleteKeyPrompt(InputManager.Instance.interactAction);
        }
    }

}
