using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFollowPlayerAction : BaseAction
{
    List<System.Type> supportedGoals = new List<System.Type>(new System.Type[] { typeof(AlwaysFollowPlayerGoal) });

    AlwaysFollowPlayerGoal chaseGoal;

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

        chaseGoal = (AlwaysFollowPlayerGoal)LinkedGoal;
        
        agent.MoveTo(gameManager.instance.player.transform.position);
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();

        chaseGoal = null;
    }

    public override void OnTick()
    {
        agent.MoveTo(gameManager.instance.player.transform.position);
    }
}
