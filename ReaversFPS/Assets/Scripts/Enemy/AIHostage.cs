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

    bool rescued = false;
    

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

                    //anim.SetBool("isRescude", true);
                    
                    //gameManager.instance.updateHostageNumbers();
                   
                    isFollowing = !isFollowing;
                    
                    Debug.Log(isFollowing);
                    
                    gameManager.instance.playerScript.interact = false;
                }
            }
            else if (rescued == true && playerInRange == false)
            {
                gameManager.instance.InteractBar.SetActive(false);
            }

           
        }
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;

        //StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.InteractBar.SetActive(false);

            isDead = true;

            anim.SetBool("isDead", true);

            coll.enabled = false;
            
            if (saved != true)
            {
                gameManager.instance.updateHostageNumbers();
            }
        }
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
                    
            gameManager.instance.InteractBar.SetActive(true);

            playerInRange = true;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isDead && rescued == false)
        {
            gameManager.instance.InteractBar.SetActive(false);

            playerInRange = false;
        }
    }
}
