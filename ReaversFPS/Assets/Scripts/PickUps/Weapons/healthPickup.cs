using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickup : MonoBehaviour
{

    [SerializeField] float healthToRecover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float combHP = gameManager.instance.playerScript.HP + healthToRecover;
            float startHP = gameManager.instance.playerScript.startHP;

            if (gameManager.instance.playerScript.HP + healthToRecover > gameManager.instance.playerScript.startHP)
            {
                healthToRecover %= gameManager.instance.playerScript.startHP;
                if (combHP > startHP)
                {
                    healthToRecover = 0;
                }
                gameManager.instance.playerScript.TakeDamage(-healthToRecover); // += healthToRecover;
                Debug.Log("The Health Kit exceeds the current health");
            }
            else if (gameManager.instance.playerScript.HP + healthToRecover <= gameManager.instance.playerScript.startHP)
            {

                gameManager.instance.playerScript.TakeDamage(-healthToRecover); // += healthToRecover;
                Debug.Log("The Health Kit does not exceeds the current health");
                
            }
            //gameManager.instance.playerScript.HP += healthToRecover;
            Destroy(gameObject);
        }
    }
}
