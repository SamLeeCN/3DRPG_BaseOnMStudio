using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.ProBuilder;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class PlayerControler : MonoBehaviour
{
    
    public NavMeshAgent agent;
    public Animator animator;
    public Character character;
    public GameObject currentTarget;
    public GameObject projectileToHit;
    public GameObject InteractableToGo;
    public Inventory inventory;
    public IInteractable currentInteractable;
    public bool isInInteractArea;
    

    public float attackCoolDownTimer;
    IEnumerator moveToTargetAndAttackEnumerator;
    IEnumerator moveToProjectileAndHitEnumerator;


    private void OnEnable()
    {
        EventManager.Instance.mouseClickGroundEvent.OnEventRaised += OnMouseClickGroundEvent;
        EventManager.Instance.mouseClickEnemyEvent.OnEventRaised += OnMouseClickEnemyEvent;
        EventManager.Instance.mouseClickProjectileEvent.OnEventRaised += OnMouseClickProjectileEvent;
        EventManager.Instance.mouseClickInteractableEvent.OnEventRaised += OnMouseClickInteractableEvent;
        EventManager.Instance.gameOverEvent.OnEventRaised += OnGameOverEvent;
    }
    private void OnDisable()
    {
        EventManager.Instance.mouseClickGroundEvent.OnEventRaised -= OnMouseClickGroundEvent;
        EventManager.Instance.mouseClickEnemyEvent.OnEventRaised -= OnMouseClickEnemyEvent;
        EventManager.Instance.mouseClickProjectileEvent.OnEventRaised -= OnMouseClickProjectileEvent;
        EventManager.Instance.mouseClickInteractableEvent.OnEventRaised -= OnMouseClickInteractableEvent;
        EventManager.Instance.gameOverEvent.OnEventRaised -= OnGameOverEvent;
    }

    

    private void OnGameOverEvent()
    {
        agent.enabled=false;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator=GetComponent<Animator>();
        character = GetComponent<Character>();
        inventory = GetComponent<Inventory>();
        GameManager.Instance.RegisterPlayerCharacter(character);
        moveToTargetAndAttackEnumerator = MoveToTargetAndAtatckEnumerator();
        moveToProjectileAndHitEnumerator = MoveToProjectileAndHitEnumerator();
    }
    void Start()
    {
        GameManager.Instance.RegisterPlayerController(this);
    }

    void Update()
    {
        JudgeInteractInput();
    }
    private void FixedUpdate()
    {
        attackCoolDownTimer-= Time.deltaTime;
    }
    #region basic movement
    private void OnMouseClickGroundEvent(Vector3 mousePos)
    {
        if(character.isDead) return;
        ExitPreviousNavigation();
        agent.destination = mousePos;
    }
    private void OnMouseClickEnemyEvent(GameObject target)
    {
        if(character.isDead) return;
        if (target)
        {
            ExitPreviousNavigation();
            currentTarget=target;
            StartCoroutine(moveToTargetAndAttackEnumerator);
        }
    }
    private void OnMouseClickProjectileEvent(GameObject projectile)
    {
        if (character.isDead) return;
        character.isAttacking = false;
        projectileToHit = projectile;
        ExitPreviousNavigation();
        moveToProjectileAndHitEnumerator = MoveToProjectileAndHitEnumerator();
        StartCoroutine(moveToProjectileAndHitEnumerator);
    }
    private void OnMouseClickInteractableEvent(GameObject interactableGameObject)
    {
        if (character.isDead) return;
        character.isAttacking = false;
        InteractableToGo=interactableGameObject;
        ExitPreviousNavigation();
        agent.destination=interactableGameObject.transform.position;
    }
    private void ExitPreviousNavigation()
    {
        character.isAttacking = false;
        StopCoroutine(moveToTargetAndAttackEnumerator);
        StopCoroutine(moveToProjectileAndHitEnumerator);
    }
    IEnumerator MoveToTargetAndAtatckEnumerator()
    {
        while (true)
        {
            if (!currentTarget)
            {
                yield return null;
                continue;
            }
            //move
            character.isAttacking = false;
            transform.DOLookAt(currentTarget.transform.position,0.3f);
            while (currentTarget&& ExtensionMethod.PlaneDistance(currentTarget.transform.position, transform.position) > character.regularAttackData.attackRange)
            {
                agent.destination = currentTarget.transform.position;
                yield return null;
            }
            //attack
            character.isAttacking = true;
            while (currentTarget&& ExtensionMethod.PlaneDistance(currentTarget.transform.position, transform.position) < character.regularAttackData.attackRange)
            {
                if (attackCoolDownTimer < 0)
                {
                    transform.DOLookAt(currentTarget.transform.position,0.3f);
                    animator.SetTrigger("Attack");
                    //reset cool down time
                    attackCoolDownTimer = character.regularAttackData.coolDown;
                }
                yield return null;
            }
            yield return null;
        }
    }
    IEnumerator MoveToProjectileAndHitEnumerator()
    {
        while(true)
        {
            if (!projectileToHit
                || projectileToHit.GetComponent<Rigidbody>().velocity.magnitude > projectileToHit.GetComponent<Projectile>().speedToHarm)
            {
                projectileToHit = null;
                yield return null;
                continue;
            }
            //move
            character.isAttacking = false;
            transform.DOLookAt(projectileToHit.transform.position, 0.3f);
            while (projectileToHit && ExtensionMethod.PlaneDistance(projectileToHit.transform.position, transform.position) > character.regularAttackData.attackRange)
            {
                agent.destination = projectileToHit.transform.position;
                yield return null;
            }
            //hit
            character.isAttacking = true;
            while (projectileToHit && ExtensionMethod.PlaneDistance(projectileToHit.transform.position, transform.position) < character.regularAttackData.attackRange)
            {
                if (attackCoolDownTimer < 0)
                {
                    transform.DOLookAt(projectileToHit.transform.position, 0.3f);
                    animator.SetTrigger("Attack");
                    projectileToHit.GetComponent<Projectile>().hitBacker = transform;
                    //reset cool down time
                    attackCoolDownTimer = character.regularAttackData.coolDown;
                }
                yield return null;
            }
            yield return null;
        }
    }
    #endregion
    
    #region Animation Event
    public void RegularAttackHit()
    {//single damage
        if (!currentTarget) return;
        float lineCos = character.regularAttackData.lineCos;
        float rangeDistance = character.regularAttackData.attackRange;
        if (!ExtensionMethod.SectorJudge(transform, currentTarget.transform, lineCos, rangeDistance)) return;
        Character targetCharacter = currentTarget.GetComponent<Character>();
        targetCharacter.TakeDamage(character, targetCharacter, character.regularAttackData);
    }
    public void ProjectileHit()
    {
        if (!projectileToHit) return;
        float lineCos = character.regularAttackData.lineCos;
        float rangeDistance = character.regularAttackData.attackRange;
        if (!ExtensionMethod.SectorJudge(transform, projectileToHit.transform, lineCos, rangeDistance)) return;
        Projectile projectileScrpt = projectileToHit.GetComponent<Projectile>();
        projectileScrpt.hitBacker = transform;
        if (projectileScrpt.isTrail)
        {
            projectileScrpt.TrailToAttackerStart();
        }
        else
        {
            projectileScrpt.FlyToAttacker();
        }
    }

    #endregion
    #region interact
    private void JudgeInteractInput()
    {
        if (InputManager.Instance.InteractInput && isInInteractArea && !UIManager.Instance.isSettingPanelOpen)
        {
            currentInteractable.TriggerAction();
        }
    }
    
    #endregion

}
