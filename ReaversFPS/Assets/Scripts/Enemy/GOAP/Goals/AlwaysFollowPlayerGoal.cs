using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFollowPlayerGoal : BaseGoal
{
    [SerializeField] int chasePriority = 60;

    [Header("DO NOT TOUCH")]
    public int currentPriority = 0;

    public Vector3 moveTarget;

    public override void OnTickGoal()
    {
        moveTarget = gameManager.instance.player.transform.position;
    }

    public override void OnGoalDeactivated()
    {
        base.OnGoalDeactivated();
    }

    public override int CalculatePriority()
    {
        return currentPriority;
    }

    public override bool CanRun()
    { 
        return true;
    }
}
