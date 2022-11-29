using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageExtract : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Hostage");
        }
    }
}
