using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum EHeardSoundCategory
{
    EFootstep,
    EJump
}

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;
    public GameObject spawnPosition;

    [Header("----- UI -----")]
    public GameObject pauseMenu;
    public GameObject playerDamageScreen;
    public GameObject playerDeadMenu;
    public GameObject winMenu; 
    public GameObject newWave;
    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI waveNumber;
    public TextMeshProUGUI currentAmmo;
    public TextMeshProUGUI ammoRemaining;

    public int ammoCount;
    public int enemiesToKill;
    public int currentWaveNumber = 1;
    public bool isPaused;
   
    [Header("----- Enemy Stuff -----")]
    public List<GameObject> enemy;
    public List<GameObject> spawnLocations;

    float targetTime = 5.0f;
    float orgTime;

   
    public List<DetectableTarget> allTargets { get; private set; } = new List<DetectableTarget>(); // Vision
   
    public List<HearingSensor> allSensors { get; private set; } = new List<HearingSensor>(); // Hearing

    // Start is called before the first frame update
    void Awake()
    { 
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        spawnPosition = GameObject.FindGameObjectWithTag("Player Spawn Position");
        ammoCount = playerScript.gunAmmo;

        orgTime = targetTime;
        
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy Spawn Rooms").Length; i++)
        {
            spawnLocations.Add(GameObject.FindGameObjectsWithTag("Enemy Spawn Rooms")[i]);
        } 
        
        updateUI();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !playerDeadMenu.activeSelf && !winMenu.activeSelf)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);

            if (isPaused)
                Pause();
            else
                unPause();
        }
    }
    public void Pause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void unPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public IEnumerator playerDamageFlash()
    {
        playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageScreen.SetActive(false);
    }
    public void youWin()
    {
        winMenu.SetActive(true);
        Pause();
    }
    public void updateEnemyNumbers()
    {
        enemiesToKill--;
        updateUI();

        if (enemiesToKill <= 0 )
        {
            updateWaveNumber();
            if (currentWaveNumber < 5)
            {
                StartCoroutine(spawnEnemies());
            }
           
        }

        if (currentWaveNumber == 5)
        {
            youWin();
        } 
    }

    public void updateWaveNumber()
    {
        currentWaveNumber++;
        updateUI();
    }
    public void updateUI()
    {
        enemiesLeft.text = enemiesToKill.ToString("F0");
        waveNumber.text = currentWaveNumber.ToString("F0");
        currentAmmo.text =   playerScript.gunAmmo.ToString("F0");
        ammoRemaining.text = playerScript.reseveGunAmmo.ToString("F0");
    }
    public void AmmoCount()
    {
        ammoCount--;
        updateUI();
    }
    IEnumerator spawnEnemies()
    {
        newWave.SetActive(true);
        yield return new WaitForSeconds(5.0f); 
        newWave.SetActive(false);

        for (int i = 0; i < spawnLocations.Count; i++)
        {
            int randomInt = Random.Range(0, 3); 
            Instantiate(enemy[randomInt], spawnLocations[i].transform.position, spawnLocations[i].transform.rotation);
        }
       
    }

    // Vision Registers
    public void VisionRegister(DetectableTarget target)
    {
        allTargets.Add(target);
    }

    public void VisionDeregister(DetectableTarget target)
    {
        allTargets.Remove(target);
    }

    // Hearing Registers
    public void HearingRegister(HearingSensor sensor)
    {
        allSensors.Add(sensor);
    }

    public void HearingDeregister(HearingSensor sensor)
    {
        allSensors.Remove(sensor);
    }

    public void OnSoundEmitted(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        // Notify all of the sensores
        foreach (var sensor in allSensors)
        {
            sensor.OnHeardSound(source, location, category, intensity);
        }
    }
}
