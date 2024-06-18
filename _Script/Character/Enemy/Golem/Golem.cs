using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Golem : Enemy
{
    public float kickForce;
    public float kickDizzyTime;
    public Transform handTransform;
    public GameObject rockPrefab;
    public AttackDataSO rockAttackData;

    public void KickOff()
    {
        if (ExtensionMethod.SectorJudge(transform, currentTarget, character.regularAttackData.lineCos, character.regularAttackData.skillRange))
        {
            Vector3 fightBackDir = new Vector3(currentTarget.position.x - transform.position.x, 0, currentTarget.position.z - transform.position.z).normalized;
            Character targetCharacter = currentTarget.GetComponent<Character>();
            targetCharacter.FoughtBack(fightBackDir, kickForce);
            targetCharacter.AddBuffDizzy(kickDizzyTime);
        }
    }
    public void ThrowRock()
    {
        if (currentTarget)
        {
            GameObject rock=Instantiate(rockPrefab, handTransform.position,Quaternion.identity);
            rock.GetComponent<Projectile>().Initialize(transform,targetPositonBeforeAttack, rockAttackData);
        }
    }
}
