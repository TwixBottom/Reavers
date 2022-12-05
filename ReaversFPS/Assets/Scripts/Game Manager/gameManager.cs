using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum EHeardSoundCategory
{
    EFootstep,
    EJump,
    EShoot,
    EHit
}

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;
    public Transform spawnPosition;
    public Transform cam;

    [Header("----- UI -----")]
    public GameObject healthBarLabel;
    public GameObject grenadeLabel;
    public GameObject ammoLabel;
    public GameObject reticle;
    public GameObject pauseMenu;
    public GameObject playerDamageScreen;
    public GameObject playerDeadMenu;
    public GameObject winMenu; 
    public GameObject newWave;
    public GameObject hostagePrompt;
    public GameObject waveLabel;
    public GameObject enemiesLabel;
    public GameObject HostageLabel;
    public GameObject DefuseLabel;
    public GameObject BombLabel;
    public GameObject InteractBar;
    public GameObject pointsLabel;
    public TextMeshProUGUI pointsEarned;
    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI grenadesLeft;
    public TextMeshProUGUI hostageLeft;
    public TextMeshProUGUI BombLeft;
    public TextMeshProUGUI waveNumber; 
    public TextMeshProUGUI currentAmmo;
    public TextMeshProUGUI ammoRemaining;
    public TextMeshProUGUI defuseTimer;
    public Image HPBar;
    public Image interactBarFill;

   

    public int ammoCount;
    public int enemiesToKill;
    public int currentWaveNumber = 1;
    public int hostageToRescue;
    public int bombsToDefuse;
    public bool isPaused;
   
    [Header("----- Enemy Stuff -----")]
    public List<GameObject> enemy;
    public List<GameObject> spawnLocations;

    [Header("----- Enemy Stuff -----")]
    public List<GameObject> objective;

    float targetTime = 5.0f;
    float orgTime;

    public bool defuse;
    public int points;

    public Scene m_scene;



    public List<DetectableTarget> allTargets { get; private set; } = new List<DetectableTarget>(); // Vision
   
    public List<HearingSensor> allSensors { get; private set; } = new List<HearingSensor>(); // Hearing

    // Start is called before the first frame update
    void Awake()
    {
        unPause();
        instance = this;
        m_scene = SceneManager.GetActiveScene();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        spawnPosition = GameObject.FindGameObjectWithTag("Player Spawn Position").transform;
        ammoCount = playerScript.gunAmmo;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        orgTime = targetTime;
        pointsLabel.SetActive(false);
        

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy Spawn Rooms").Length; i++)
        {
            spawnLocations.Add(GameObject.FindGameObjectsWithTag("Enemy Spawn Rooms")[i]);
        }

        if (m_scene.name == "MainScene")
        {
            enemiesLabel.SetActive(true);
            waveLabel.SetActive(true);
        }
        else if (m_scene.name == "HostageMap")
        {
            HostageLabel.SetActive(true);
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Hostage").Length; i++)
            {
                objective.Add(GameObject.FindGameObjectsWithTag("Hostage")[i]);
            }
        }
        else
        {
            BombLabel.SetActive(true);
           // DefuseLabel.SetActive(true);
        }

       

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Bomb").Length; i++)
        {
            objective.Add(GameObject.FindGameObjectsWithTag("Bomb")[i]);
        }


        if (spawnPosition == null)
        {
            spawnPosition = player.transform;
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
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void unPause()
    {      
        isPaused = false;
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
        Pause();
        newWave.SetActive(false);
        hostagePrompt.SetActive(false);
        points += 10000;
        pointsLabel.SetActive(true);
        winMenu.SetActive(true);
        
    }
    public void updateEnemyNumbers()
    {
        enemiesToKill--;
        updateUI();

        if (enemiesToKill <= 0 && m_scene.name == "MainScene")
        {
            updateWaveNumber();
            if (currentWaveNumber <= 5)
            {
                StartCoroutine(spawnEnemies());
            }
           
        }

        //if (currentWaveNumber > 5)
        //{
        //    youWin();
        //} 
    }

    public void updateHostageNumbers()
    {
        hostageToRescue--;
        updateUI();

        if (hostageToRescue <= 0)
        {
            youWin();
        }
    }

    public void updateBombNumbers()
    {
        bombsToDefuse--;
        updateUI();

        if (bombsToDefuse <= 0)
        {
            youWin();
        }
    }

    public void updateWaveNumber()
    {
        points += 1000;
        currentWaveNumber++;
        updateUI();
    }
    public void updateUI()
    {
        grenadesLeft.text = playerScript.totalThrows.ToString("F0");
        enemiesLeft.text = enemiesToKill.ToString("F0");
        pointsEarned.text = points.ToString("F0");
        hostageLeft.text = hostageToRescue.ToString("F0");
        BombLeft.text = bombsToDefuse.ToString("F0");
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
