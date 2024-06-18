using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BaseChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.isChaseState = true;
    }
    public override void LogicUpdate()
    {
        currentEnemy.MoveAndAttack();
        if (!currentEnemy.FindPlayer())
        {
            if(currentEnemy.isGuard)
            {
                currentEnemy.SwitchState(EnemyStates.Guard);
            }
            else
            {
                currentEnemy.SwitchState(EnemyStates.Patrol);
            }
        }
    }
    public override void PhysicsUpdate()
    {
        
    }
    public override void OnExit()
    {
        currentEnemy.isChaseState=false;
        currentEnemy.isChasing = false;
    }

}
