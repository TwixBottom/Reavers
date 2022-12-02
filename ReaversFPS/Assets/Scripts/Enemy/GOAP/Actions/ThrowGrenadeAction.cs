using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeAction : BaseAction
{

   [SerializeField] GameObject grenade;

    List<System.Type> supportedGoals = new List<System.Type>(new System.Type[] {typeof(ThrowGrenadeGoal)});

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

        LinkedAI.grenadeCounter--;
        Instantiate(grenade, gameObject.transform.position, grenade.transform.rotation);
        Debug.Log("Throw Grenade");
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();
    }
}
