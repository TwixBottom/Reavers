using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerAction : BaseAction
{
    List<System.Type> supportedGoals = new List<System.Type>(new System.Type[] { typeof(FollowPlayerGoal) });

    FollowPlayerGoal followPlayer;

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

        followPlayer = (FollowPlayerGoal)LinkedGoal;
        
        agent.MoveTo(gameManager.instance.player.transform.position);
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();

        followPlayer = null;
    }

    public override void OnTick()
    {
        agent.MoveTo(gameManager.instance.player.transform.position);
    }
}
