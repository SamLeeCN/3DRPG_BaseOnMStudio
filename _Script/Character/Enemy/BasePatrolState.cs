using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BasePatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.GetNewPoint();
    }
    public override void LogicUpdate()
    {
        if(ExtensionMethod.PlaneDistance(currentEnemy.wayPoint,currentEnemy.transform.position)<=currentEnemy.agent.stoppingDistance)
        {
            currentEnemy.isWalking = false;
            if (currentEnemy.waitTimeTimer > 0)
            {
                currentEnemy.waitTimeTimer-=Time.deltaTime;
            }
            else
            {
                currentEnemy.GetNewPoint();
                currentEnemy.waitTimeTimer = currentEnemy.waitTime;
            }
        }
        else
        {
            currentEnemy.isWalking = true;
            currentEnemy.agent.destination= currentEnemy.wayPoint;
        }
        if (currentEnemy.FindPlayer()&&!GameManager.Instance.isPlayerDead)
        {
            currentEnemy.SwitchState(EnemyStates.Chase);
        }
    }
    public override void PhysicsUpdate()
    {
        
    }
    public override void OnExit()
    {
        
    }
}
