using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [SerializeField] int forceAmount;
    [SerializeField] int dmg;
    [SerializeField] SphereCollider blastzone;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        if (other.CompareTag("Player"))
        {
            //dammage minus the position of the player
            gameManager.instance.playerScript.transform.localPosition = ((other.transform.position - transform.position).normalized) * forceAmount;
            gameManager.instance.playerScript.TakeDamage(dmg);
        }
    }
}
