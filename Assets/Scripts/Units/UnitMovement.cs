using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if(targeter.GetTarget() != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;
        }

        if (!agent.hasPath) { return; }
        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    #region server
    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();
        if(!NavMesh.SamplePosition(position,out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }
    #endregion

    
}
