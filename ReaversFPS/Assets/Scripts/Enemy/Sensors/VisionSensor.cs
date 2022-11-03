using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIEnemy))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] LayerMask detectionMask = ~0;

    AIEnemy LinkedAI;
    // Start is called before the first frame update
    void Start()
    {
        LinkedAI = GetComponent<AIEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checks for each available candidate
        for (int i = 0; i < gameManager.instance.allTargets.Count; ++i)
        {
            DetectableTarget candidateTarget = gameManager.instance.allTargets[i];

            // Checks if we are the candidate
            if (candidateTarget.gameObject == gameObject)
            {
                continue;
            }

            // Checking Distance

            Vector3 distanceCheck = candidateTarget.transform.position - LinkedAI.EyeLocation;

            if (distanceCheck.sqrMagnitude > (LinkedAI.VisionConeRange * LinkedAI.VisionConeRange))
            {
                continue;
            }

            // Check in Vision Cone

            distanceCheck.Normalize();

            if (Vector3.Dot(distanceCheck, LinkedAI.EyeDirection) < LinkedAI.CosVisionConeAngle)
            {
                continue;
            }

            // Raycast

            RaycastHit hit;

            if (Physics.Raycast(LinkedAI.EyeLocation, distanceCheck, out hit, LinkedAI.VisionConeRange, detectionMask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponentInParent<DetectableTarget>() == candidateTarget)
                {
                    LinkedAI.ReportCanSee(candidateTarget);
                }
            }
        }
    }
}
