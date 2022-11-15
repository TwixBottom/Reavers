using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeFunctions : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] Rigidbody rb;
    [SerializeField] int timeToExplode;

    [SerializeField] float throwForce;
    [SerializeField] float throwUpwardForce;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        Vector3 forceToAdd = gameManager.instance.cam.transform.forward * throwForce + transform.up * throwUpwardForce;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
        yield return new WaitForSeconds(timeToExplode);
        Instantiate(explosion, gameObject.transform.position, explosion.transform.rotation);
        Destroy(gameObject);

    }
}
