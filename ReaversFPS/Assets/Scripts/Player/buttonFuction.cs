using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFuction : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.unPause();
        gameManager.instance.pauseMenu.SetActive(false);
        gameManager.instance.isPaused = false;
    }
    public void Restart()
    {
        gameManager.instance.unPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Respawn()
    {
        gameManager.instance.unPause();
        gameManager.instance.playerScript.Respawn();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
