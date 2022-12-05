using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    [SerializeField] float NearestPointSearchRange = 5f;

    NavMeshAgent agent;
    bool DestinationSet = false;
    bool ReachedDestination = false;
    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;

    public bool IsMoving => agent.velocity.magnitude > float.Epsilon;

    public bool AtDestination => ReachedDestination;

    // Start is called before the first frame update
    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (agent.enabled == true)
        {
            // have a path and near the end point?
            if (!agent.pathPending && !agent.isOnOffMeshLink && DestinationSet && (agent.remainingDistance <= agent.stoppingDistance))
            {
                DestinationSet = false;
                ReachedDestination = true;
            }

            // are we on an offmesh link?
            if (agent.isOnOffMeshLink)
            {
                // have we started moving along the link
                if (OffMeshLinkStatus == EOffmeshLinkStatus.NotStarted)
                    StartCoroutine(FollowOffmeshLink());
            }
        }
    }

    IEnumerator FollowOffmeshLink()
    {
        // start the offmesh link - disable NavMesh agent control
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // move along the path
        Vector3 newPosition = transform.position;
        while (!Mathf.Approximately(Vector3.Distance(newPosition, agent.currentOffMeshLinkData.endPos), 0f))
        {
            newPosition = Vector3.MoveTowards(transform.position, agent.currentOffMeshLinkData.endPos, agent.speed * Time.deltaTime);
            transform.position = newPosition;

            yield return new WaitForEndOfFrame();
        }

        // flag the link as completed
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
        agent.CompleteOffMeshLink();

        // return control the agent
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = true;
    }

    public Vector3 PickLocationInRange(float range)
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;

        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchLocation, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
            return hitResult.position;

        return transform.position;
    }

    public void CancelCurrentCommand()
    {
        // clear the current path
        agent.ResetPath();

        DestinationSet = false;
        ReachedDestination = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    public virtual void MoveTo(Vector3 destination)
    {
        CancelCurrentCommand();

        SetDestination(destination);
    }

    public virtual void SetDestination(Vector3 destination)
    {
        // find nearest spot on navmesh and move there
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hitResult.position);
            DestinationSet = true;
            ReachedDestination = false;
        }
    }
}

public enum EFaction
{
    Player,
    Enemy
}

public class CharacterBase : MonoBehaviour
{
    [SerializeField] EFaction _Faction;

    public EFaction Faction => _Faction;
}


