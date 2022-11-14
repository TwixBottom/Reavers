using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIHostage : MonoBehaviour, PlayerDamage
{

    [Header("Components")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Collider coll;


    [Header("Hostage Stats")]
    [SerializeField] int HP;
    [SerializeField] int animLerpSpeed;

    public bool playerInRange;
    public bool isDead = false;
    public bool isFollowing = false;

    bool rescued = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead != true)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

            if (playerInRange == true)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    isFollowing = !isFollowing;
                }
            }

           
        }
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            isDead = true;

            anim.SetBool("isDead", true);

            coll.enabled = false;

        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = Color.white;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rescued != true)
            {
                gameManager.instance.hostagePrompt.SetActive(true);
                rescued = true;
            }

            playerInRange = true;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.hostagePrompt.SetActive(false);

            playerInRange = false;
        }
    }
}
