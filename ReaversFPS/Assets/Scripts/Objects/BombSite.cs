using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSite : MonoBehaviour
{

    bool playerInRange;
    bool defused;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.bombsToDefuse++;
    }

    // Update is called once per frame
    void Update()
    {
        if (defused != true && playerInRange == true && gameManager.instance.playerScript.interact == true)
        {
            defused = true;
            gameManager.instance.points += 500;
            gameManager.instance.updateBombNumbers();
            Debug.Log("Bomb Defused");
        }
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && defused == false)
        {
            gameManager.instance.playerScript.interactable = true;

            gameManager.instance.InteractBar.SetActive(true);

            playerInRange = true;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && defused == false)
        {
            gameManager.instance.playerScript.interactable = false;

            gameManager.instance.InteractBar.SetActive(false);

            playerInRange = false;
        }
    }
}
