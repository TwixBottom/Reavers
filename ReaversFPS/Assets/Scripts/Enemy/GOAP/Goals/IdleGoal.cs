using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleGoal : BaseGoal
{
    [SerializeField] int priority = 10;

    public override int CalculatePriority()
    {
        if (LinkedAI.isDead == true)
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
