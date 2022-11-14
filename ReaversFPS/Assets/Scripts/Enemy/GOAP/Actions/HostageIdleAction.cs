using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageIdleAction : BaseAction
{
    List<System.Type> supportedGoals = new List<System.Type>(new System.Type[] {typeof(HostageIdleGoal)});

    public override List<System.Type> getSupporterGoals()
    {
        return supportedGoals;
    }

    public override float GetCost()
    {
        return 0.0f;
    }
}
