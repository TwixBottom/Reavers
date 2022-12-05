using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageExtract : MonoBehaviour
{
    AIHostage hostage;

    public void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Hostage"))
        { 
            hostage = other.GetComponent<AIHostage>();

            if (hostage.startExtraction != true)
            {
                hostage.startExtraction = true;
                hostage.isFollowing = false;
                Debug.Log("Hostage");
            }
        }
    }
}
