using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TrackedTarget
{
    public DetectableTarget Detectable;
    public Vector3 rawPosition;

    public float LastSensedTime = -1.0f;
    public float Awarness; // 0     = not aware
                           // 0-1   = rought idea
                           // 1-2   = likely target
                           // 2     = fully detected

    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareneess)
    {
        var oldAwarness = Awarness;

        if (target != null)
        {
            Detectable = target;
        }

        rawPosition = position;
        LastSensedTime = Time.time;

        Awarness = Mathf.Clamp(Mathf.Max(Awarness, minAwareneess) + awareness, 0f, 2f);

        if (oldAwarness < 2.0f && Awarness >= 2.0f)
        {
            return true;
        }
        if (oldAwarness < 1.0f && Awarness >= 1.0f)
        {
            return true;
        }
        if (oldAwarness <= 0.0f && Awarness >= 0)
        {
            return true;
        }

        return false;

    }

    public bool DecayAwareness(float decayTime, float amount)
    {
        if ((Time.time - LastSensedTime) < decayTime)
        {
            return false;
        }

        float oldAwarness = Awarness;

        Awarness -= amount;

        if (oldAwarness >= 2.0f && Awarness < 2.0f)
        {
            return true;
        }
        if (oldAwarness >= 1.0f && Awarness < 1.0f)
        {
            return true;
        }

        return Awarness <= 0.0f;
    }

}
[RequireComponent(typeof(AIEnemy))]
public class AwarenessSystem : MonoBehaviour
{
    [SerializeField] AnimationCurve visionSens;

    [SerializeField] float VisionMinAwareness = 1.0f;
    [SerializeField] float VisionAwarenessBuildRate = 0.0f;

    [SerializeField] float HearingMinAwareness = 0.0f;
    [SerializeField] float HearingAwarenessBuildRate = 0.5f;

    [SerializeField] float ProximityMinAwareness = 0.0f;
    [SerializeField] float ProximityAwarenessBuildRate = 1.0f;

    [SerializeField] float AwarenessDecayDelay = 0.1f;
    [SerializeField] float AwarenessDecayrate = 0.1f;

    Dictionary<GameObject, TrackedTarget> Targets = new Dictionary<GameObject, TrackedTarget>();

    AIEnemy LinkedAI;

    // Start is called before the first frame update
    void Start()
    {
        LinkedAI = GetComponent<AIEnemy>();
    }

    // Update is called once per frame
    void Update()
    {

        List<GameObject> toCleanUp = new List<GameObject>();

        foreach (var targetGO in Targets.Keys)
        {

            if (Targets[targetGO].DecayAwareness(AwarenessDecayDelay, AwarenessDecayrate * Time.deltaTime))
            {
                if (Targets[targetGO].Awarness <= 0)
                {
                    LinkedAI.OnFullyLost();
                    toCleanUp.Add(targetGO);
                }
                else
                {
                    if (Targets[targetGO].Awarness >= 1.0f)
                    {
                        LinkedAI.OnLostDetection(targetGO);
                    }
                    else
                    {
                        LinkedAI.OnLostSuspicion();
                    }
                }
            }
        }

        foreach (var target in toCleanUp)
        {
            Targets.Remove(target);
        }
    }
    void UpdateAwareness(GameObject targetGo, DetectableTarget target, Vector3 position, float awareness, float minAwareneess)
    {
        // Non in target
        if (!Targets.ContainsKey(targetGo))
        {
            Targets[targetGo] = new TrackedTarget();
        }

        // Update target awareness
        if (Targets[targetGo].UpdateAwareness(target, position, awareness, minAwareneess))
        {
            if (Targets[targetGo].Awarness >= 2)
            {
                LinkedAI.OnFullyDetected(targetGo);
            }
            else if (Targets[targetGo].Awarness >= 1.0f)
            {
                LinkedAI.OnDetected(targetGo);
            }
            else
            {
                LinkedAI.OnSuspicious();
            }
        }

    }

    public void ReportCanSee(DetectableTarget target)
    {
        // Determine where the player is in teh field of view
        Vector3 vectorToTarget = (target.transform.position - LinkedAI.EyeLocation).normalized;
        float dotProduct = Vector3.Dot(vectorToTarget, LinkedAI.EyeDirection);

        // Determine the Awareness contribution
        float awareness = visionSens.Evaluate(dotProduct) * VisionAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, VisionMinAwareness);
    }

    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        float awareness = intensity * HearingAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(source, null, location, awareness, HearingMinAwareness);
    }

    public void ReportInProximity(DetectableTarget target)
    {
        float awareness = ProximityAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, ProximityMinAwareness);

    }
}
