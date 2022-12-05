using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeFunctions : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] Rigidbody rb;
    [SerializeField] int timeToExplode;

    [SerializeField] float throwForce;
    [SerializeField] float throwUpwardForce;

    [SerializeField] AudioSource grenadeAud;
    [SerializeField] AudioClip[] grenadeExplosionAud;
    [Range(0, 1)][SerializeField] float HitVol;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Vector3 forceToAdd = gameManager.instance.cam.transform.forward * throwForce + transform.up * throwUpwardForce;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
        yield return new WaitForSeconds(timeToExplode);
        
        Instantiate(explosion, gameObject.transform.position, explosion.transform.rotation);
        Instantiate(explosionEffect, gameObject.transform.position, explosionEffect.transform.rotation);
        //grenadeAud.PlayOneShot(grenadeExplosionAud[Random.Range(0, grenadeExplosionAud.Length)], HitVol);
        Destroy(gameObject);
    }

}
