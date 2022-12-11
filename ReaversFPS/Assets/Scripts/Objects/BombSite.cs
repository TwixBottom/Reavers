using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSite : MonoBehaviour
{

    bool playerInRange;
    bool defused;
    bool once;

    [SerializeField] List<GameObject> spawnPositions;


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

            gameManager.instance.playerScript.interact = false;

            Debug.Log("Bomb Defused");
        }

        if (defused == true && once == false)
        {
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                Instantiate(gameManager.instance.followPlayerEnemy, spawnPositions[i].transform.position, spawnPositions[i].transform.rotation);
            }



            once = true;
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
