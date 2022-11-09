using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPPlanner : MonoBehaviour
{
    BaseGoal[] Goals;
    BaseAction[] Actions;

    BaseGoal activeGoal;
    BaseAction activeAction;

    void Awake()
    {
        Goals = GetComponents<BaseGoal>();
        Actions = GetComponents<BaseAction>();
    }

    void Update()
    {
        BaseGoal bestGoal = null;
        BaseAction bestAction = null;

        // Find the highest priority goal

        foreach (var goal in Goals)
        {
            goal.OnTickGoal();

            if (!goal.CanRun())
            {
                continue;
            }

            if (!(bestGoal == null || goal.CalculatePriority() > bestGoal.CalculatePriority()))
            {
                continue;
            }

            BaseAction candidateAction = null;


            foreach (var action in Actions)
            {
                if (!action.getSupporterGoals().Contains(goal.GetType()))
                {
                    continue;
                }

                if (candidateAction == null || action.GetCost() < candidateAction.GetCost())
                {
                    candidateAction = action;
                }

            }

            if (candidateAction != null)
            {
                bestGoal = goal;
                bestAction = candidateAction;
            }
        }

        if (activeGoal == null)
        {
            activeGoal = bestGoal;
            activeAction = bestAction;

            if (activeGoal != null)
            {
                activeGoal.OnGoalActivated(activeAction);
            }
            if (activeAction != null)
            {
                activeAction.OnActivated(activeGoal);
            }

        }
        else if (activeGoal == bestGoal)
        {
            if (activeAction != bestAction)
            {
                activeAction.OnDeactivated();

                activeAction = bestAction;

                activeAction.OnActivated(activeGoal);
            }
        }
        else if (activeGoal != bestGoal)
        {
            activeGoal.OnGoalDeactivated();
            activeAction.OnDeactivated();

            activeGoal = bestGoal;
            activeAction = bestAction;

            if (activeGoal != null)
            {
                activeGoal.OnGoalActivated(activeAction);
            }
            if (activeAction != null)
            {
                activeAction.OnActivated(activeGoal);
            }
        }

        if (activeAction != null)
        {
            activeAction.OnTick();
        }

        Debug.Log(activeAction);
        Debug.Log(activeGoal);
    }
}
