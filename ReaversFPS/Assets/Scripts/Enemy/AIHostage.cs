using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIHostage : MonoBehaviour, IDamage
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
    public bool saved = false;
    public bool startExtraction = false;
    public bool rescued = false;

    bool once = false;
    bool spawn = true;
    bool endSpawn;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.hostageToRescue++;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead != true)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

            if (playerInRange == true && rescued == false)
            {
                //gameManager.instance.InteractBar.SetActive(true);

                if (gameManager.instance.playerScript.interact == true) 
                { 
                    
                    rescued = true;
            
                    isFollowing = true;
                    
                    gameManager.instance.playerScript.interact = false;
                }
            }
            else if (rescued == true && playerInRange == false)
            {
                gameManager.instance.InteractBar.SetActive(false);
            }

            if (saved == true && once == false)
            {
                once = true;

                anim.SetBool("isRescude", true);

                gameManager.instance.points += 800;

                gameManager.instance.updateHostageNumbers();
            }
        }

        if (startExtraction == true && saved == false)
        {
            StartCoroutine(StartExtraction());
        }
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;

        gameManager.instance.points -= 50;

        if (HP <= 0)
        {
            gameManager.instance.InteractBar.SetActive(false);

            gameManager.instance.points -= 500;

            isDead = true;

            anim.SetBool("isDead", true);

            coll.enabled = false;
            
            if (saved != true)
            {
                gameManager.instance.updateHostageNumbers();
            }
        }
    }

    IEnumerator StartExtraction()
    {
        if (spawn == true)
        {
            StartCoroutine(SpawnEnemies());
        }
        
        yield return new WaitForSeconds(60);
        saved = true;
    }
    
    IEnumerator SpawnEnemies()
    {
        spawn = false;

        for (int i = 0; i < gameManager.instance.spawnLocations.Count; i++)
        {
            Instantiate(gameManager.instance.enemy[3], gameManager.instance.spawnLocations[i].transform.position, gameManager.instance.spawnLocations[i].transform.rotation);
        }
     
        yield return new WaitForSeconds(10);

        spawn = true;
    }

    //IEnumerator flashDamage()
    //{
    //    model.material.color = Color.red;
    //    yield return new WaitForSeconds(0.15f);
    //    model.material.color = Color.white;
    //}

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDead && rescued == false)
        {
            gameManager.instance.playerScript.interactable = true;

            gameManager.instance.InteractBar.SetActive(true);

            playerInRange = true;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isDead && rescued == false)
        {
            gameManager.instance.playerScript.interactable = false;

            gameManager.instance.InteractBar.SetActive(false);

            playerInRange = false;
        }
    }
}
