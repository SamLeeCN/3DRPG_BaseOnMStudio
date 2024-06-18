using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BaseDeadState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy=enemy;
        currentEnemy.agent.enabled=false;
        currentEnemy.coll.enabled=false;
        Object.Destroy(currentEnemy.gameObject,2f);
    }
    public override void LogicUpdate()
    {
        
    }
    public override void PhysicsUpdate()
    {
        
    }
    public override void OnExit()
    {
        
    }
}
