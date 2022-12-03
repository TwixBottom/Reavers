using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyGrenadeFunctions : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] Rigidbody rb;
    [SerializeField] float timeInAir;
    [SerializeField] int timeToExplode;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        Vector3 direction = (gameManager.instance.player.transform.position - gameObject.transform.position);

        rb.velocity = (direction / 2) - (Physics.gravity * (2 / 2));

        yield return new WaitForSeconds(timeToExplode);
        Instantiate(explosion, gameObject.transform.position, explosion.transform.rotation);
        Destroy(gameObject);

    }
}
