using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeFunctions : MonoBehaviour
{

    [SerializeField] Vector3 rangeVec;

    [SerializeField] GameObject explosion;
    [SerializeField] Rigidbody rb;
    [SerializeField] int time;

    IEnumerator Start()
    {
        rb.velocity = (rangeVec - rb.position) / time - (Physics.gravity * (time / 2));

        yield return new WaitForSeconds(time + 1);
        Instantiate(explosion, transform.position, explosion.transform.rotation);

        Destroy(gameObject);
    }
}
