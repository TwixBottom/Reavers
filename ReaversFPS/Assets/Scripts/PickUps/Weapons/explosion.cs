using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [SerializeField] int forceAmount;
    [SerializeField] SphereCollider coll;
    [SerializeField] int damage;

    [SerializeField] AudioSource grenadeAud;
    [SerializeField] AudioClip[] grenadeExplosionAud;
    [Range(0, 1)][SerializeField] float HitVol;

    bool damaged;
    // Start is called before the first frame update
    void Start()
    {
        ExplosionDMG();
        grenadeAud.PlayOneShot(grenadeExplosionAud[Random.Range(0, grenadeExplosionAud.Length)], HitVol);
        Destroy(gameObject, 0.1f);
    }

    void ExplosionDMG()
    {
        Collider[] hitColliders = Physics.OverlapSphere(coll.transform.position, coll.radius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Debug.Log(hitColliders[i]);
            if (hitColliders[i].GetComponent<IDamage>() != null && !hitColliders[i].transform.CompareTag("Player"))
            {
                hitColliders[i].GetComponent<IDamage>().TakeDamage(damage);
            }
            if (hitColliders[i].transform.CompareTag("Player"))
            {
                if (damaged == false)
                {

                    damaged = true;
                    gameManager.instance.playerScript.pushBack = ((hitColliders[i].transform.position - transform.position).normalized) * forceAmount;
                    gameManager.instance.playerScript.TakeDamage(damage);

                }

            }
        }
    }
}
