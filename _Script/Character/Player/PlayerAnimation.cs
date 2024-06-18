using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class PlayerAnimation : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public Character character;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
    }

    void Update()
    {
        SetAnimation();
    } 
    public void SetAnimation()
    {
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetBool("IsDead", character.isDead);
        animator.SetBool("IsDizzy", character.isDizzy);
    }
}
