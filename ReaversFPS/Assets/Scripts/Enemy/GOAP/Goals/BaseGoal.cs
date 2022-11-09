using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoal
{
    int CalculatePriority();
    
    bool CanRun();
    
    void OnTickGoal();
    
    void OnGoalActivated(BaseAction linkedAction);
   
    void OnGoalDeactivated();
}

public class BaseGoal : MonoBehaviour, IGoal
{
    protected EnemyNavigation agent;
    protected AwarenessSystem sensors;

    protected BaseAction LinkedAction;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<EnemyNavigation>();
        sensors = GetComponent<AwarenessSystem>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        OnTickGoal();
    }

    public virtual int CalculatePriority()
    {
        return -1;
    }

    public virtual bool CanRun()
    {
        return false;
    }

    public virtual void OnTickGoal() 
    { 

    }

    public virtual void OnGoalActivated(BaseAction linkedAction)
    {
        LinkedAction = linkedAction;
    }

    public virtual void OnGoalDeactivated()
    {
        LinkedAction = null;
    }
}
