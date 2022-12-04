using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyGrenadeFunctions : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] Rigidbody rb;
    [SerializeField] float timeInAir;
    [SerializeField] int timeToExplode;

    [SerializeField] AudioSource grenadeAud;
    [SerializeField] AudioClip grenadeExplosionAud;
    [Range(0, 1)][SerializeField] float HitVol;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Vector3 direction = (gameManager.instance.player.transform.position - gameObject.transform.position);

        rb.velocity = (direction / 2) - (Physics.gravity * (2 / 2));

        yield return new WaitForSeconds(timeToExplode);
        Instantiate(explosion, gameObject.transform.position, explosion.transform.rotation);
        Instantiate(explosionEffect, gameObject.transform.position, explosionEffect.transform.rotation);
        grenadeAud.PlayOneShot(grenadeExplosionAud, HitVol);
        Destroy(gameObject);

    }
}
