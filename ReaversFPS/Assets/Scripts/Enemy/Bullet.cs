using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] float timer;


    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - rb.position) * speed;
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.name != "Player")
        {
            Destroy(gameObject);
        }

        else if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.TakeDamage(damage);
            Destroy(gameObject);

        }
    }

}
