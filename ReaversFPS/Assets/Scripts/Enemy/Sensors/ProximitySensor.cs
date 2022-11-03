using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIEnemy))]
public class ProximitySensor : MonoBehaviour
{
    AIEnemy LinkedAI;

    // Start is called before the first frame update
    void Start()
    {
        LinkedAI = GetComponent<AIEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < gameManager.instance.allTargets.Count; ++i)
        {
            DetectableTarget candidateTarget = gameManager.instance.allTargets[i];

            // Checks if we are the candidate
            if (candidateTarget.gameObject == gameObject)
            {
                continue;
            }

            if (Vector3.Distance(LinkedAI.EyeLocation, candidateTarget.transform.position) <= LinkedAI.ProximityDetectionRange)
            {
                LinkedAI.ReportInProximity(candidateTarget);
            }
        }
    }
}
