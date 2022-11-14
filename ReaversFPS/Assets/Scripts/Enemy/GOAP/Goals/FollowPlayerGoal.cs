using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerGoal : BaseGoal
{
    [SerializeField] int chasePriority = 60;

    [Header("DO NOT TOUCH")]
    public int currentPriority = 0;

    public override void OnGoalDeactivated()
    {
        base.OnGoalDeactivated();

        LinkedHostage.isFollowing = false;
    }

    public override int CalculatePriority()
    {
        return chasePriority;
    }

    public override bool CanRun()
    {

        if (LinkedHostage.isFollowing == true)
        {
            return true;
        }

        return false;
    }
}
