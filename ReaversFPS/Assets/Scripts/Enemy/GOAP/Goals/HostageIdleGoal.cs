using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageIdleGoal : BaseGoal
{
    [SerializeField] int priority = 10;

    public override int CalculatePriority()
    {

        if (LinkedHostage.isDead || LinkedHostage.saved)
        {
            priority = 100;
        }


        return priority;
    }

    public override bool CanRun()
    {
        return true;
    }
}
