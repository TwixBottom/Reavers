using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class granadePickup : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.totalThrows++;
            Destroy(gameObject);
        }
    }
}
