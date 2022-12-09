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

    public void LoadHostageMapEasy()
    {
        SceneManager.LoadScene("HostageEasy");
    }

    public void LoadHostageMapMedium()
    {
        SceneManager.LoadScene("HostageMedium");

    }

    public void LoadHostageMapHard()
    {

        SceneManager.LoadScene("HostageHard");

    }

    public void LoadDefuseEasy()
    {

        SceneManager.LoadScene("DefuseEasy");

    }

    public void LoadDefuseMedium()
    {

        SceneManager.LoadScene("DefuseMedium");

    }

    public void LoadDefuseHard()
    {

        SceneManager.LoadScene("DefuseHard");

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
