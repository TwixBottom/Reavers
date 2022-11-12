using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAction : BaseAction
{
    List<System.Type> supportedGoals = new List<System.Type>(new System.Type[] { typeof(ChaseGoal) });

    ChaseGoal chaseGoal;

    float targetTime;
    float orgTime;

    public override List<System.Type> getSupporterGoals()
    {
        return supportedGoals;
    }

    public override float GetCost()
    {
        return 0.0f;
    }

    public override void OnActivated(BaseGoal linkedGoal)
    {
        base.OnActivated(linkedGoal);

        chaseGoal = (ChaseGoal)LinkedGoal;
        
        agent.MoveTo(chaseGoal.moveTarget);
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();

        chaseGoal = null;
    }

    public override void OnTick()
    {
        agent.MoveTo(chaseGoal.moveTarget);
    }
}
