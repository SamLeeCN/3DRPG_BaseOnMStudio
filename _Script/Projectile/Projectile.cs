using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Projectile : MonoBehaviour
{
    public enum ProjectileState
    {
        hitTarget, hitAttacker
    }
    public bool isTrail=false;
    public bool isHitBackable;
    public float upSpeed=3;
    public float horizontalSpeed;
    public float dizzyDuration=1;
    public float trailForce;
    public float hitBackForce=20;
    private Vector3 horizontalSpeedDirection;
    public Vector3 targetPositionOffset = new Vector3(0, 0, 0);
    public float existingTime=30;
    public float speedToHarm=1;
    [Header("Reference")]
    public Rigidbody rb;
    public AttackDataSO attackData;
    public Transform attacker;
    public Transform transTarget;
    public Vector3 posTarget;
    public Vector3 posTargetAfterOffset;
    public Transform hitTarget;
    public Transform hitBacker;
    [Header("State")]
    public ProjectileState currentState;
    public bool isTrailingToTarget;
    public bool isTrailingToAttacker;
    [Header("Prefab")]
    public GameObject breakParticlePrefab;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        currentState = ProjectileState.hitTarget;
        FlyToTarget();
    }
    void Update()
    {
        TrailToTarget();
        existingTime-= Time.deltaTime;
        if(existingTime < 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        EventManager.Instance.dataLoadEvent += OnDataLoadEvent;
    }
    private void OnDisable()
    {
        EventManager.Instance.dataLoadEvent -= OnDataLoadEvent;
    }

    private void OnDataLoadEvent()
    {
        Destroy(gameObject);
    }

    public void Initialize(Transform attacker,Transform transTarget,AttackDataSO attackData)
    {//called by attcker to assign attacker, target and attack data.
        this.attacker=attacker;
        this.transTarget=transTarget;
        this.attackData = attackData;
    }
    public void Initialize(Transform attacker, Vector3 posTarget, AttackDataSO attackData)
    {//called by attcker to assign attacker, target and attack data.
        this.attacker = attacker;
        this.posTarget=posTarget;
        this.attackData = attackData;
        posTargetAfterOffset =posTarget + targetPositionOffset;
    }
    public void FlyToTarget()
    {
        if (isTrail) return;
        horizontalSpeedDirection =
            new Vector3(posTargetAfterOffset.x-transform.position.x,0,posTargetAfterOffset.z-transform.position.z).normalized;
        horizontalSpeed = CalculateHorizontalSpeed();
        rb.velocity = new Vector3(horizontalSpeed*horizontalSpeedDirection.x, upSpeed, horizontalSpeed*horizontalSpeedDirection.z);
    }
    public float CalculateHorizontalSpeed()
    {
        
        float d = ExtensionMethod.PlaneDistance(posTargetAfterOffset, transform.position);
        float h = transform.position.y-posTargetAfterOffset.y;
        float g = -Physics.gravity.y;
        float vy = upSpeed;
        float delta = Mathf.Pow(vy, 2) + 2 * g * h;
        float t = (vy+Mathf.Sqrt(delta)) / g;
        float vx = d / t;
        return vx;
    }
    public void TrailToTarget()
    {
        if (!isTrail||!transTarget) return;
        horizontalSpeedDirection = (transTarget.transform.position - transform.position).normalized;
        rb.AddForce(horizontalSpeedDirection * trailForce, ForceMode.Force);
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch(currentState)
        {
            case ProjectileState.hitTarget:
                if (!attacker || collision.transform == attacker) return;//to make sure rock initialized
                if (rb.velocity.magnitude < speedToHarm) return;
                currentState = ProjectileState.hitAttacker;
                if (collision.transform.GetComponent<Character>())
                {
                    hitTarget = collision.transform;
                    Character hitTargetCharacter = hitTarget.GetComponent<Character>();
                    Character attackerChracter= attacker.GetComponent<Character>();
                    hitTargetCharacter.TakeDamage(attackerChracter, hitTargetCharacter, attackerChracter.regularAttackData);
                    FightBackAndDizzy();
                }else if (collision.transform.CompareTag("Ground"))
                {
                    currentState = ProjectileState.hitAttacker;
                }
                break;

            case ProjectileState.hitAttacker:
                if (collision.transform == attacker&&rb.velocity.magnitude>speedToHarm)
                {
                    Instantiate(breakParticlePrefab, transform.position, Quaternion.identity);
                    attacker.GetComponent<Character>().TakeDamage(hitBacker.GetComponent<Character>(), attacker.GetComponent<Character>(), attackData);
                    Destroy(gameObject);
                }
                break;
        }
    }
    public void FightBackAndDizzy()
    {
        Vector3 fightBackDir = new Vector3(hitTarget.position.x - attacker.position.x, 0, hitTarget.position.z - attacker.position.z).normalized;
        Character hitTargetCharacter = hitTarget.GetComponent<Character>();
        hitTargetCharacter.FoughtBack(fightBackDir, attackData.fightBack);
        hitTargetCharacter.AddBuffDizzy(dizzyDuration);
    }
    public void FlyToAttacker()
    {
        if (isTrail||!attacker) return;
        transTarget = attacker;
        horizontalSpeedDirection = attacker.position - transform.position; 
        horizontalSpeedDirection.Normalize();
        rb.AddForce(horizontalSpeedDirection * hitBackForce + Vector3.up, ForceMode.Impulse);
    }
    public void TrailToAttackerStart()
    {
        if (!isTrail||!attacker) return;
        isTrailingToAttacker = true;
    }
    public void TrailToAttacker()
    {
        if (!isTrail||!isTrailingToAttacker||!attacker) return;
        horizontalSpeedDirection = (attacker.transform.position - transform.position).normalized;
        rb.AddForce(horizontalSpeedDirection * trailForce, ForceMode.Force);
    }
}
