using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeFunctions : MonoBehaviour
{

    SphereCollider blastzone;
    [SerializeField] float grenadeDamage; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        other = blastzone;

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            gameManager.instance.playerScript.TakeDamage(grenadeDamage);
            //gameManager.instance.AIEnemy.TakeDamage(grenadeDamage);
        }
    }

    IEnumerator grenadeBlast()
    {
        //blastzone.enabled;
        yield return new WaitForSeconds(1);
    }
}
