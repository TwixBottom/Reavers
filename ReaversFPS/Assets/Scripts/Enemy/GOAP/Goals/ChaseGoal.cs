using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGoal : BaseGoal
{
    [SerializeField] int chasePriority = 60;

    [SerializeField] float minAwarenessToChase = 1.5f;
    [SerializeField] float awarenessToStopChase = 1;

    DetectableTarget currentTarget;

    [Header("DO NOT TOUCH")]
    public int currentPriority = 0;

    public Vector3 moveTarget => currentTarget != null ? currentTarget.transform.position : transform.position;

    public override void OnTickGoal()
    {
        // No Targets
        if (sensors.activeTargets == null || sensors.activeTargets.Count == 0)
        {
            return;
        }


        if (currentTarget != null)
        {
            // Checks if the current target is still sensed
            foreach (var candidate in sensors.activeTargets.Values)
            {
                if (candidate.Detectable == currentTarget)
                {
                    currentPriority = candidate.Awarness < awarenessToStopChase ? 0 : chasePriority;
                    return;
                }
            }

            // Clear current target
            currentTarget = null;
        }

        // Get a new target
        foreach (var candidate in sensors.activeTargets.Values)
        {

            // Found a target to chase
            if (candidate.Awarness >= minAwarenessToChase)
            {
                currentTarget = candidate.Detectable;
                currentPriority = chasePriority;
                return;
            }
        }
    }

    public override void OnGoalDeactivated()
    {
        base.OnGoalDeactivated();

        currentTarget = null;
    }

    public override int CalculatePriority()
    {
        return currentPriority;
    }

    public override bool CanRun()
    { 
        // no targets
        if (sensors.activeTargets == null || sensors.activeTargets.Count == 0)
        {
            return false;
        }          

        // check if we have anything we are aware of
        foreach (var candidate in sensors.activeTargets.Values)
        {
            if (candidate.Awarness >= minAwarenessToChase)
            {
                return true;
            } 
        }

        return false;
    }
}
