using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamAction : BaseAction
{
    [SerializeField] float searchrange = 10.0f;

    bool wait;

    List<System.Type> supportedGoals = new List<System.Type>(new System.Type[] { typeof(RoamGoal) });

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
       
        if (wait == false)
        {
            Vector3 location = agent.PickLocationInRange(searchrange);
            agent.MoveTo(location);
            StartCoroutine(waitForNextMove());
        }
       
    }

    public override void OnDeactivated()
    {
        agent.CancelCurrentCommand();
    }

    public override void OnTick()
    {
        // Arrived at destination
        if (agent.AtDestination)
        {
            OnActivated(LinkedGoal);
        }
    }

    IEnumerator waitForNextMove()
    {
        wait = true;
        yield return new WaitForSeconds(5.0f);
        wait = false;
    }
}
