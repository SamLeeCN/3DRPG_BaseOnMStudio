using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BaseGuardState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy=enemy;
        currentEnemy.wayPoint = currentEnemy.spawnPoint;
        currentEnemy.agent.SetDestination(currentEnemy.spawnPoint);
    }
    public override void LogicUpdate()
    {
        if (ExtensionMethod.PlaneDistance(currentEnemy.wayPoint, currentEnemy.transform.position) <= currentEnemy.agent.stoppingDistance)
        {
            currentEnemy.isWalking = false;
        }
        else
        {
            currentEnemy.isWalking = true;
        }
        if (currentEnemy.FindPlayer())
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
