using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeGoal : BaseGoal
{
    [SerializeField] int priority = 50;

    public override int CalculatePriority()
    {
        return priority;
    }

    public override bool CanRun()
    {
        if (LinkedAI.grenadeCounter > 0 && LinkedAI.canThrowGrenade == true)
        {
            Debug.Log("Can Throw Grenade");

            return true;
        }

        return false;
        
    }
}
