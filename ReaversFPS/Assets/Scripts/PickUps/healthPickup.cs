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
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float combHP = gameManager.instance.playerScript.HP + healthToRecover;
            float startHP = gameManager.instance.playerScript.startHP;

            if (gameManager.instance.playerScript.HP == startHP)
            {
                return;
            }

            if (combHP > startHP)
            {
                gameManager.instance.playerScript.HP = gameManager.instance.playerScript.startHP;
            }
            else
            {
                gameManager.instance.playerScript.HP += healthToRecover;
            }
            gameManager.instance.playerScript.updatePlayerHBar();
            Destroy(gameObject);
        }
    }
}
