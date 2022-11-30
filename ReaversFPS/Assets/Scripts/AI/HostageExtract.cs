using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageExtract : MonoBehaviour
{
    AIHostage hostage;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Hostage"))
        { 
            hostage = other.GetComponent<AIHostage>();

            if (hostage.saved != true)
            {
                hostage.saved = true;

                Debug.Log("Hostage");
            }
        }
    }
}
