using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class MouseManager : Singleton<MouseManager>
{
    public bool isRayInteractionDisabled;
    public Texture2D normalCursor, leadCursor, attackCursor;
    public Vector2 cursorOffset;
    RaycastHit[] hitInfos;
    RaycastHit targetHitInfo;
    Ray ray;
    bool isAnythingHit;
    protected override void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        CastRayAndFindTarget();//must be the first to do
        SetCursorTexture();
        MouseControl();
        JudgeAnyPanelOpen();
    }
    private void CastRayAndFindTarget()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hitInfos = Physics.RaycastAll(ray);
        hitInfos = hitInfos.OrderBy(hit => hit.distance).ToArray();//Sort with distance
        isAnythingHit = false;
        for (int i = 0; i < hitInfos.Length; i++)
        {
            targetHitInfo = hitInfos[i];
            isAnythingHit = true;
            if (targetHitInfo.collider.tag == "Ground")
            {
                break;//if can block ray cast, break
            }
            else if (targetHitInfo.collider.tag == "Enemy")
            {
                break;
            }
            else if (targetHitInfo.collider.tag == "Projectile" && targetHitInfo.transform.GetComponent<Projectile>().isHitBackable)
            {
                break;
            }
            else if (targetHitInfo.collider.tag == "Interactable")
            {
                break;
            }else if(targetHitInfo.collider.tag == "NPC")
            {
                break;
            }
            isAnythingHit = false;
        }
    }
    void SetCursorTexture()
    {
        if (isRayInteractionDisabled||!isAnythingHit || IsInteractingWithUI())
        {
            Cursor.SetCursor(normalCursor, cursorOffset, CursorMode.Auto);
            return;
        }
        switch (targetHitInfo.collider.tag)
        {
            case "Ground":
                Cursor.SetCursor(leadCursor, cursorOffset, CursorMode.Auto);
                break;
            case "Enemy":
                Cursor.SetCursor(attackCursor, cursorOffset, CursorMode.Auto);
                break;
            case "Projectile":
                if (targetHitInfo.transform.GetComponent<Projectile>().isHitBackable)
                {
                    Cursor.SetCursor(attackCursor, cursorOffset, CursorMode.Auto);
                }
                break;
            case "Interactable":
                Cursor.SetCursor(attackCursor, cursorOffset, CursorMode.Auto);
                break;
            case "NPC":
                Cursor.SetCursor(leadCursor, cursorOffset, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(normalCursor, cursorOffset, CursorMode.Auto);
                break;
        }
    }
    void MouseControl()
    {
        if (isRayInteractionDisabled||!isAnythingHit||IsInteractingWithUI())
        {
            return;
        }
        if (!Input.GetMouseButtonDown(0)) return;
        switch (targetHitInfo.collider.tag)
        {
            case "Ground":
                EventManager.Instance.mouseClickGroundEvent.RaiseEvent(targetHitInfo.point);
                break;
            case "Enemy":
                EventManager.Instance.mouseClickEnemyEvent.RaiseEvent(targetHitInfo.collider.transform.gameObject);
                break;
            case "Projectile":
                if (targetHitInfo.transform.GetComponent<Projectile>().isHitBackable)
                {
                    EventManager.Instance.mouseClickProjectileEvent.RaiseEvent(targetHitInfo.collider.transform.gameObject);
                }
                break;
            case "Interactable":
                EventManager.Instance.mouseClickInteractableEvent.RaiseEvent(targetHitInfo.collider.transform.gameObject);
                break;
            case "NPC":
                EventManager.Instance.mouseClickInteractableEvent.RaiseEvent(targetHitInfo.collider.transform.gameObject);
                break;
        }
    }
    void JudgeAnyPanelOpen()
    {
        if (UIManager.Instance.isSettingPanelOpen){
            isRayInteractionDisabled = true;
        }
        else
        {
            isRayInteractionDisabled = false;
        }
    }
    public bool IsInteractingWithUI()
    {
        if(EventSystem.current != null&& EventSystem.current.IsPointerOverGameObject()) return true;
        else return false;
    }
    void Start()
    {

    }

    
}
