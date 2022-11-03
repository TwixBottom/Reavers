using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIEnemy))]
public class HearingSensor : MonoBehaviour
{

    AIEnemy LinkedAI;
    // Start is called before the first frame update
    void Start()
    {
        LinkedAI = GetComponent<AIEnemy>();
        gameManager.instance.HearingRegister(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (gameManager.instance != null)
        {
            gameManager.instance.HearingDeregister(this);
        }
    }


    public void OnHeardSound(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        // Outside of hearing range
        if (Vector3.Distance(location, LinkedAI.EyeLocation) > LinkedAI.HearingRange)
        {
            return;
        }

        LinkedAI.ReportCanHear(source, location, category, intensity);
    }
}
