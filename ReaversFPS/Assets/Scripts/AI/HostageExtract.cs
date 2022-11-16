using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageExtract : MonoBehaviour
{

    [SerializeField] SphereCollider coll;

    public void OnTriggerStay(Collider other)
    {
      
        
        Collider[] hitColliders = Physics.OverlapSphere(coll.transform.position, coll.radius);
        
        for (int i = 0; i < hitColliders.Length; i++)
        {    
            Debug.Log(hitColliders[i].name);     
            
            if (hitColliders[i].GetComponent<IDamage>() != null && !hitColliders[i].transform.CompareTag("Player"))
            {
                
                
                if (other.CompareTag("Hostage"))
                {
                    Debug.Log("hello");
                }
            }
           
        }
    }
}
