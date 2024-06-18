using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
//*****************************************
//创建人： SamLee 
//功能说明：
//*****************************************

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Character))]
public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Character character;
    public Collider coll;
    protected BaseState currentState;
    protected BaseState guardState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;
    protected BaseState deadState;
    public bool isGuard;
    public float patrolRadius;
    public float sightRadius;
    public Transform currentTarget;
    public Vector3 targetPositonBeforeAttack;
    public float attackCoolDownTimer;
    public Animator animator;
    public bool isWalking;
    public bool isChaseState;
    public bool isChasing;
    public bool isAttackPerforming;
    public Vector3 spawnPoint;
    public Vector3 wayPoint;
    public float waitTime;
    public float waitTimeTimer;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        character=GetComponent<Character>();
        coll = GetComponent<Collider>();
        waitTimeTimer = waitTime;
    }
    public void OnEnable()
    {
        spawnPoint = transform.position;
        //FIXME:delete after creating specific scripts for each enemy
        guardState = new BaseGuardState();
        patrolState= new BasePatrolState();
        chaseState = new BaseChaseState();
        deadState = new BaseDeadState();
        //

        if (!isGuard)
        {
            currentState = patrolState;
        }
        else
        {
            currentState = guardState;
        }
        currentState.OnEnter(this);
        EventManager.Instance.gameOverEvent.OnEventRaised += OnGameOverEvent;
    }
    private void OnDisable()
    {
        EventManager.Instance.gameOverEvent.OnEventRaised -= OnGameOverEvent;
        currentState.OnExit();
        
    }

    

    void Update()
    {
        
        attackCoolDownTimer-=Time.deltaTime;
        currentState.LogicUpdate();
        SetAnimation();
        if(character.isDead)
        {
            SwitchState(EnemyStates.Dead);
        }
    }
    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();
    }
    public void SwitchState(EnemyStates state)
    {
        var newState = state switch
        {
            EnemyStates.Guard => guardState,
            EnemyStates.Patrol => patrolState,
            EnemyStates.Chase => chaseState,
            EnemyStates.Skill => skillState,
            EnemyStates.Dead => deadState,
            _=>null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }
    
    public bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                currentTarget = target.transform;
                return true;
            }
        }
        return false;
    }
    protected void SetAnimation()
    {
        animator.SetBool("IsChaseState", isChaseState);
        animator.SetBool("IsChasing", isChasing);
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsDead", character.isDead);
    }
    #region patrol state
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(spawnPoint, patrolRadius);
    }
    public void GetNewPoint()
    {
        Vector3 randomOffset = Random.insideUnitCircle * patrolRadius;
        Vector3 randomPoint = new Vector3(spawnPoint.x + randomOffset.x, spawnPoint.y, spawnPoint.z + randomOffset.z);
        //keep y still
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, 1))
        {//"1" means the first layer in Navigation, which is "Walkable"
            wayPoint = randomPoint;
        }
        else
        {
            GetNewPoint();
        }
    }
    #endregion
    #region chase state
    public void MoveAndAttack()
    {
        if(!isAttackPerforming) transform.DOLookAt(currentTarget.transform.position,0.3f);
        if (!TargetInAttackRange(0.3f) && !TargetInSkillRange(0.3f))
        {
            //move
            character.isAttacking = false;
            isChasing = true;
            agent.destination = currentTarget.transform.position;
        }
        if(TargetInAttackRange(0)||TargetInSkillRange(0)) 
        {//attack
            character.isAttacking=true;
            isChasing = false;
            agent.destination = transform.position;
            if (attackCoolDownTimer < 0)
            {
                character.isCritical = Random.value < character.regularAttackData.criticalChance;
                animator.SetBool("IsCritical", character.isCritical);
                Attack();
            }
        }
    }
    protected bool TargetInAttackRange(float offSet)
    {
        if (currentTarget)
        {
            return ExtensionMethod.PlaneDistance(currentTarget.position, transform.position)<character.regularAttackData.attackRange+offSet;
        }
        else
        {
            return false;
        }
    } 
    protected bool TargetInSkillRange(float offSet)
    {
        if (currentTarget)
        {
            return ExtensionMethod.PlaneDistance(currentTarget.position, transform.position) < character.regularAttackData.skillRange+offSet;
        }
        else
        {
            return false;
        }
    }
    public void Attack()
    {
        targetPositonBeforeAttack=currentTarget.position;
        transform.DOLookAt(currentTarget.position,0.3f);
        if (TargetInAttackRange(0))
        {
            animator.SetTrigger("Attack");
        }
        else if(TargetInSkillRange(0))
        {
            animator.SetTrigger("Skill");
        }
        //reset cool down time
        attackCoolDownTimer = character.regularAttackData.coolDown;
        isAttackPerforming = true;
    }
    #endregion
    
    #region Animation Event
    public void RegularAttackHit()
    {//single damage
        isAttackPerforming = false;
        if (!currentTarget) return;
        float lineCos=character.regularAttackData.lineCos;
        float rangeDistance=character.regularAttackData.attackRange;
        if(!ExtensionMethod.SectorJudge(transform,currentTarget,lineCos,rangeDistance))return;
        Character targetCharacter = currentTarget.GetComponent<Character>();
        targetCharacter.TakeDamage(character, targetCharacter, character.regularAttackData);
    }
    public void SkillHit()
    {//single damage
        isAttackPerforming = false;
        if (!currentTarget) return;
        float lineCos = character.regularAttackData.lineCos;
        float rangeDistance = character.regularAttackData.skillRange;
        if (!ExtensionMethod.SectorJudge(transform, currentTarget, lineCos, rangeDistance)) return;
        Character targetCharacter = currentTarget.GetComponent<Character>();
        targetCharacter.TakeDamage(character, targetCharacter, character.regularAttackData);
    }
    #endregion
    private void OnGameOverEvent()
    {
        agent.isStopped = true;
        currentTarget = null;
        isChasing = false;
        isChaseState = false;
        animator.SetBool("IsVictory", true);
        SwitchState(EnemyStates.Patrol);
    }
}
