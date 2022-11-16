using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gunStats;

    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.gunPickup(gunStats);
            Destroy(gameObject);
        }
    }
}
