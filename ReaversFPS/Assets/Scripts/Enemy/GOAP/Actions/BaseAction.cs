using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAction : MonoBehaviour
{
    protected EnemyNavigation agent;
    protected AwarenessSystem sensors;
    protected BaseGoal LinkedGoal;

    protected AIEnemy LinkedAI;
  
    void Awake()
    {
        agent = GetComponent<EnemyNavigation>();
        sensors = GetComponent<AwarenessSystem>();
        LinkedAI = GetComponent<AIEnemy>();
    }

    public virtual List<System.Type> getSupporterGoals()
    {
        return null;
    }

    public virtual float GetCost()
    {
        return 0.0f;
    }

    public virtual void OnActivated(BaseGoal linkedGoal)
    {
        LinkedGoal = linkedGoal;
    }

    public virtual void OnDeactivated()
    {
        LinkedGoal = null;
    }

    public virtual void OnTick()
    {

    }
}
