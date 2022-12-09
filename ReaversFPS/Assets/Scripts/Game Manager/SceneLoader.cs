using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void LoadHostageMap()
    {
        //SceneManager.LoadScene("HostageMap");

        // Loads depending on difficulty selected
        if (gameObject.name == "Recruit")
        {
            SceneManager.LoadScene("HostageMap");
        }
        else if (gameObject.name == "Agent")
        {
            SceneManager.LoadScene("HostageMap");
        }
        else
        {
            SceneManager.LoadScene("HostageMap");
        }
    }

    public void LoadDefuseMap()
    {
        //SceneManager.LoadScene("DefuseMap");

        // Loads depending on difficulty selected
        if (gameObject.name == "Recruit")
        {
            SceneManager.LoadScene("DefuseMap");
        }
        else if (gameObject.name == "Agent")
        {
            SceneManager.LoadScene("DefuseMap");
        }
        else
        {
            SceneManager.LoadScene("DefuseMap");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
