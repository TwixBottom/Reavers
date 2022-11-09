using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamGoal : BaseGoal
{
    [SerializeField] int roamPriority = 30;

    [SerializeField] float priorityBuildRate = 1.0f;
    [SerializeField] float priorityDecayRate = 0.1f;

    [Header("DO NOT TOUCH")]
    public float currentPriority = 0.0f;

    public override void OnTickGoal()
    {
        if (agent.IsMoving)
        {
            currentPriority -= priorityDecayRate * Time.deltaTime;
        }
        else
        {
            currentPriority += priorityBuildRate * Time.deltaTime;
        }
    }

    public override void OnGoalActivated(BaseAction linkedAction)
    {
        base.OnGoalActivated(linkedAction);

        currentPriority = roamPriority;
    }

    public override int CalculatePriority()
    {
        return Mathf.FloorToInt(currentPriority);
    }

    public override bool CanRun()
    {
        return true;
    }
}
