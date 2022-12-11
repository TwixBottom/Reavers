using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera weaponCamera;
    [SerializeField] AudioSource aud;
    [SerializeField] GameObject smgShootPos;
    [SerializeField] GameObject rifleShootPos;
    [SerializeField] GameObject sniperShootPos;

    [Header("----- DroneComponents ------")]
    [SerializeField] GameObject drone;
    [SerializeField] Camera droneCamera;
    [SerializeField] CharacterController droneController;
    [SerializeField] Shader seeThroughWalls;

    [Header("----- Player Stats -----")]
    [SerializeField] public float HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpMax;
    [SerializeField] gunStats startinWeapon;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] public int gunAmmo;
    [SerializeField] public int magazineCount;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject flashImage;
    [SerializeField] List<gunStats> gunStatList = new List<gunStats>();

    [Header("----- Projectile Stats -----")]
    [SerializeField] GameObject granade;
    [SerializeField] public int totalThrows;
    [SerializeField] float throwCooldown;

    [Header("----- Player Physics -----")]
    [SerializeField] float pushBackTime;

    [Header("----- Player Audio -----")]
    [SerializeField] AudioClip[] JumpAudio;
    [Range(0, 1)][SerializeField] float JumpVol;
    [SerializeField] AudioClip[] ShootAudio;
    [Range(0, 1)][SerializeField] float ShootVol;
    [SerializeField] AudioClip[] HurtAudio;
    [Range(0, 1)][SerializeField] float HurtVol;
    [SerializeField] AudioClip[] SprintAudio;
    [Range(0, 1)][SerializeField] float SprintVol;
    [SerializeField] AudioClip[] ReloadAudio;
    [Range(0, 1)][SerializeField] float ReloadVol;
    [SerializeField] AudioClip[] NoAmmoAudio;
    [Range(0, 1)][SerializeField] float NoAmmoVol;

    public bool inDrone = false;
    Vector3 droneVelocity;
    bool switching;

    //FOV
    float lerpDuration = 0.2f;
    float endValue = 15;
    float valueToLerp;

    int startAmmo;
    float playerStartSpeed;
    int jumpTimes;

    Vector3 move;
    public Vector3 pushBack;
    private Vector3 playerVelocity;

    bool isSprinting;
    bool isShooting;
    bool isReloding;
    bool isJumping;
    bool sprintSoundEffects = false;
    bool readyToThrow = true;
    public bool isAiming;

    public float startHP;
    public int reseveGunAmmo;
    int selectedGun;

    float fovOriginal;

    public Vector3 normalPosition;
    public Vector3 aimingPosition;

    public float aimSmooth = 10;

    public bool interactable;
    public bool interact;
    public bool randomizeRecoil;
    public Vector2 randRecoilConstraints;
    public Vector2 recoilPattern;

    // Start is called before the first frame update
    void Start()
    {
        normalPosition = gunModel.transform.localPosition;
        fovOriginal = playerCamera.fieldOfView;
        isSprinting = false;
        startHP = HP;
        startAmmo = gunAmmo;
        playerStartSpeed = playerSpeed;
        reseveGunAmmo = magazineCount * startAmmo; 
        gunPickup(startinWeapon);
        if (gameManager.instance.m_scene.name != "MainScene")
        {
            droneController = drone.GetComponent<CharacterController>();
            droneCamera.enabled = false;
            droneCamera.GetComponent<cameraControls>().enabled = false;
        }
       
       
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.m_scene.name != "MainScene")
        {
            if (Input.GetButtonDown("Toggle"))
            {
                StartCoroutine(ToggleDrone());
            }
                
        }



        if (interactable == true || inDrone == true)
        {
            Interact();
        }

        if (inDrone == true)
        {
            DroneMovement();
        }
        else
        {
            FindAim();
            pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackTime);
            PlayerMovement();
            PlayerSprint();
            StartCoroutine(aimDownSights());
            StartCoroutine(ShootWeapon());
            StartCoroutine(BladeSwipe());
            StartCoroutine(Throw());
            StartCoroutine(RelodeWeapon());
            gunSelect();
        }
    }

    // moves the player
    void PlayerMovement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpTimes = 0;
            playerVelocity.y = 0f;
            isJumping = false;
        }

        move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move((move + pushBack) * Time.deltaTime * playerSpeed);

        gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EFootstep, 1.0f);

        // changes the height position of the player
        if (Input.GetButtonDown("Jump") && jumpTimes < jumpMax)
        {
            isJumping = true;

            aud.PlayOneShot(JumpAudio[Random.Range(0, JumpAudio.Length)], JumpVol);

            jumpTimes++;

            if (isSprinting == false)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            else
            {
                playerVelocity.y += Mathf.Sqrt((jumpHeight * 2) * -3.0f * gravityValue);
            }

            gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EJump, 2.0f);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (gameManager.instance.m_scene.name != "MainScene")
        {
            droneVelocity.y += gravityValue * Time.deltaTime;
            droneController.Move(droneVelocity * Time.deltaTime);
        }
    }

    void DroneMovement()
    {
        if (droneController.isGrounded && droneVelocity.y < 0)
        {
            jumpTimes = 0;
            droneVelocity.y = 0f;
            isJumping = false;
        }

        move = drone.transform.right * Input.GetAxis("Horizontal") + drone.transform.forward * Input.GetAxis("Vertical");
        droneController.Move((move) * Time.deltaTime * (playerSpeed * 2));

        // changes the height position of the player
        if (Input.GetButtonDown("Jump") && jumpTimes < jumpMax)
        {
            isJumping = true;

            jumpTimes++;

            droneVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

            gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EJump, 0.5f);
        }

        droneVelocity.y += gravityValue * Time.deltaTime;
        droneController.Move(droneVelocity * Time.deltaTime);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }


    // makes the player sprint
    void PlayerSprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
            isSprinting = false;
        }

        if (isSprinting)
        {
            gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EFootstep, isSprinting ? 2f : 1f);

            if (sprintSoundEffects == false && isJumping != true)
            {
                StartCoroutine(SprintSoundEffects());
            }
        }
    }

    IEnumerator ToggleDrone()
    {
        //if (Input.GetButtonDown("Toggle"))
        //{
            if (switching != true)
            {
                switching = true;
                //Check if it's the player or the drone
                if (inDrone == false)
                {
                    gameManager.instance.healthBarLabel.SetActive(false);
                    gameManager.instance.grenadeLabel.SetActive(false);
                    gameManager.instance.ammoLabel.SetActive(false);
                    gameManager.instance.InteractBar.SetActive(true);
                    //Switch the camera to drone camera
                    playerCamera.enabled = false;
                    weaponCamera.enabled = false;
                    playerCamera.GetComponent<cameraControls>().enabled = false;
                    droneCamera.GetComponent<cameraControls>().enabled = true;
                    droneCamera.enabled = true;
                    inDrone = true;
                    yield return new WaitForSeconds(1);
                    switching = false;
                }
                else if (inDrone == true)
                {
                    gameManager.instance.healthBarLabel.SetActive(true);
                    gameManager.instance.grenadeLabel.SetActive(true);
                    gameManager.instance.ammoLabel.SetActive(true);
                    gameManager.instance.InteractBar.SetActive(false);
                    //Switch camera to player camera
                    playerCamera.enabled = true;
                    weaponCamera.enabled = true;
                    playerCamera.GetComponent<cameraControls>().enabled = true;
                    droneCamera.GetComponent<cameraControls>().enabled = false;
                    droneCamera.enabled = false;
                    inDrone = false;
                    yield return new WaitForSeconds(1);
                    switching = false;
                }
            }
        //}
    }

    IEnumerator SprintSoundEffects()
    {
        sprintSoundEffects = true;
        aud.PlayOneShot(SprintAudio[Random.Range(0, SprintAudio.Length)], SprintVol);
        yield return new WaitForSeconds(0.5f);
        sprintSoundEffects = false;
    }

    public void updatePlayerHBar()
    {
        gameManager.instance.HPBar.fillAmount = HP / startHP;
    }

    public void TakeDamage(float dmg)
    {
        HP -= dmg;

        StartCoroutine(gameManager.instance.playerDamageFlash());
        updatePlayerHBar();

        aud.PlayOneShot(HurtAudio[Random.Range(0, HurtAudio.Length)], HurtVol);

        if (HP <= 0)
        {

            gameManager.instance.playerDeadMenu.SetActive(true);
            gameManager.instance.pointsLabel.SetActive(true);
            gameManager.instance.Pause();
        }
    }

    IEnumerator BladeSwipe()
    {
        if (gunStatList[selectedGun].name == "Knife Stats")
        {
            if (gunStatList.Count > 0 && !isShooting && Input.GetButton("Shoot"))
            {
                isShooting = true;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                {
                    if (hit.collider.GetComponent<IDamage>() != null && hit.collider.tag == "Enemy" || hit.collider.tag == "Hostage")
                    {
                        hit.collider.GetComponent<IDamage>().TakeDamage(shootDamage);
                    }

                    //Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
                }

                yield return new WaitForSeconds(shootRate);
                isShooting = false;
            }
        }
    }

    IEnumerator Throw()
    {
        if (totalThrows > 0 && !gameManager.instance.isPaused)
        {
            if (readyToThrow == true && Input.GetButton("Throw"))
            {
                readyToThrow = false;

                Instantiate(granade, gameObject.transform.position, gameObject.transform.rotation);

                totalThrows--;

                yield return new WaitForSeconds(throwCooldown);

                readyToThrow = true;

            }
        }
    }

    IEnumerator ShootWeapon()
    {
        if (gunAmmo > 0 && gunStatList[selectedGun].name != "Knife Stats" && !gameManager.instance.isPaused)
        {
            if (gunStatList.Count > 0 && !isShooting && !isReloding && Input.GetButton("Shoot"))
            {
                isShooting = true;
                FindRecoil();
                if (!isAiming)
                {
                    StartCoroutine(MuzzleFlash());
                }              

                aud.PlayOneShot(ShootAudio[Random.Range(0, ShootAudio.Length)], ShootVol);

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                {
                    if (hit.collider.GetComponent<IDamage>() != null && hit.collider.tag == "Enemy" || hit.collider.tag == "Hostage")
                    {
                        hit.collider.GetComponent<IDamage>().TakeDamage(shootDamage);
                    }

                       Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
                    
               
                    gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EShoot, 2.0f);

                }
                gunAmmo--;
                saveWeaponAmmo();

                yield return new WaitForSeconds(shootRate);
                isShooting = false;
            }
        }
        else if (gunAmmo == 0 && gunStatList[selectedGun].name != "Knife Stats" && !gameManager.instance.isPaused && Input.GetButton("Shoot"))
        {
            if (!isShooting && !isReloding)
            {
                isShooting = true;

                aud.PlayOneShot(NoAmmoAudio[Random.Range(0, NoAmmoAudio.Length)], ShootVol);

                yield return new WaitForSeconds(shootRate);

                isShooting = false;
            }

        }

        gameManager.instance.updateUI();

    }

    void FindRecoil()
    {
        if (gunStatList[selectedGun].name == "Sniper Gun Stat")
            gunModel.transform.localPosition -= Vector3.forward * 0.8f;
        else if (gunStatList[selectedGun].name == "Assault Gun Stats")
            gunModel.transform.localPosition -= Vector3.forward * 0.5f;
        else if (gunStatList[selectedGun].name == "SMG Gun Stats")
            gunModel.transform.localPosition -= Vector3.forward * 0.3f;
    }

    void FindAim()
    {
        Vector3 targetPos = normalPosition;

        Vector3 desiredPos = Vector3.Lerp(gunModel.transform.localPosition, targetPos, Time.deltaTime * aimSmooth);

        gunModel.transform.localPosition = desiredPos;
    }

    IEnumerator MuzzleFlash()
    {
        if (gunStatList[selectedGun].name == "Sniper Gun Stat")
        {
            Instantiate(flashImage, sniperShootPos.transform.position, flashImage.transform.rotation, sniperShootPos.transform);
            yield return new WaitForSeconds(0.3f);
            //flashImage.SetActive(false);
        }
        else if (gunStatList[selectedGun].name == "Assault Gun Stats")
        {
            Instantiate(flashImage, rifleShootPos.transform.position, flashImage.transform.rotation, rifleShootPos.transform);
            yield return new WaitForSeconds(0.3f);
            //flashImage.SetActive(false);
        }
        else if (gunStatList[selectedGun].name == "SMG Gun Stats")
        {
            Instantiate(flashImage, smgShootPos.transform.position, flashImage.transform.rotation, smgShootPos.transform);
            yield return new WaitForSeconds(0.3f);
            //flashImage.SetActive(false);
        }
    }


    IEnumerator RelodeWeapon()
    {
        if (Input.GetButtonDown("Reload"))
        {

            if (!isReloding && gunAmmo == 0 && reseveGunAmmo > 0 && gunStatList[selectedGun].name != "Knife Stats")
            {
                aud.PlayOneShot(ReloadAudio[Random.Range(0, ReloadAudio.Length)], ReloadVol);

                isReloding = true;
                yield return new WaitForSeconds(2.0f);
                isReloding = false;

                if (reseveGunAmmo - startAmmo <= 0)
                {
                    gunAmmo = reseveGunAmmo;
                    reseveGunAmmo = 0;
                    saveWeaponAmmo();
                }
                else
                {
                    gunAmmo = startAmmo;
                    reseveGunAmmo -= startAmmo;
                    saveWeaponAmmo();
                }
            }
            else if (!isReloding && reseveGunAmmo > 0 && gunAmmo != startAmmo && gunStatList[selectedGun].name != "Knife Stats")
            {
                aud.PlayOneShot(ReloadAudio[Random.Range(0, ReloadAudio.Length)], ReloadVol);

                int ammoLeft = startAmmo - gunAmmo;

                isReloding = true;
                yield return new WaitForSeconds(2.0f);
                isReloding = false;

                if (reseveGunAmmo - ammoLeft <= 0)
                {
                    gunAmmo += reseveGunAmmo;
                    reseveGunAmmo = 0;
                }
                else
                {
                    gunAmmo += ammoLeft;
                    reseveGunAmmo -= ammoLeft;
                }
            }
        }

        gameManager.instance.updateUI();
    }


    public void Respawn()
    {
        controller.enabled = false;
        HP = startHP;
        gameManager.instance.pointsLabel.SetActive(false);
        updatePlayerHBar();
        transform.position = gameManager.instance.spawnPosition.transform.position;
        gameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }


    public void gunPickup(gunStats gunStat)
    {

        if (gunStatList.Contains(gunStat))
        {

            int list = gunStatList.IndexOf(gunStat);

            if (list == selectedGun)
            {
                if (gunStat.maxAmmo <= reseveGunAmmo + (gunStat.magazineCount * gunStat.ammoCount) + gunStat.ammoCount)
                {
                    Debug.Log("Equip Gun Max Ammo");

                    reseveGunAmmo = gunStat.maxAmmo;
                }
                else if (gunStat.maxAmmo > reseveGunAmmo)
                {
                    Debug.Log("Equip Gun Not Max Ammo");
                    reseveGunAmmo += (gunStat.magazineCount * gunStat.ammoCount) + gunStat.ammoCount;
                }

            }
            else
            {
                if (gunStat.maxAmmo <= gunStatList[list].ammoReserves + (gunStat.magazineCount * gunStat.ammoCount) + gunStat.ammoCount)
                {
                    Debug.Log("Unequiped Gun Max Ammo");

                    gunStatList[list].ammoReserves = gunStat.maxAmmo;
                }
                else if (gunStat.maxAmmo > gunStatList[list].ammoReserves)
                {
                    Debug.Log("Unequiped Gun Not Max Ammo");
                    gunStatList[list].ammoReserves += (gunStat.magazineCount * gunStat.ammoCount) + gunStat.ammoCount;
                }

            }
        }

        if (gunStatList.Count == 0)
        {

            shootRate = gunStat.fireRate;
            shootDist = gunStat.shootDistance;
            shootDamage = gunStat.damage;
            gunAmmo = gunStat.ammoCount;
            startAmmo = gunStat.ammoCount;
            magazineCount = gunStat.magazineCount;
            reseveGunAmmo = gunStat.magazineCount * gunStat.ammoCount;

            gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat.model.GetComponent<MeshRenderer>().sharedMaterial;
        }

        if (!gunStatList.Contains(gunStat))
        {

            gunStat.ammoReserves = gunStat.magazineCount * gunStat.ammoCount;
            gunStat.currentAmmo = gunStat.ammoCount;

            gunStatList.Add(gunStat);

            if (gunStatList.Count == 2)
            {
                selectedGun++;
                selectWeapon();
            }
        }
    }


    public void selectWeapon()
    {
        shootRate = gunStatList[selectedGun].fireRate;
        shootDist = gunStatList[selectedGun].shootDistance;
        shootDamage = gunStatList[selectedGun].damage;
        startAmmo = gunStatList[selectedGun].ammoCount;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStatList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStatList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;

        gunAmmo = gunStatList[selectedGun].currentAmmo;
        reseveGunAmmo = gunStatList[selectedGun].ammoReserves;
    }

    void quickSwap()
    {
        // Check nnumber
        // Check if its valid

        // selectedWeapon = number

        selectWeapon();
    }

    void gunSelect()
    {
        if (!isReloding)
        {
            if (gunStatList.Count > 1)
            {
                saveWeaponAmmo();

                if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunStatList.Count - 1)
                {
                    selectedGun++;
                    selectWeapon();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
                {
                    selectedGun--;
                    selectWeapon();
                }
            }
        }
    }


    void saveWeaponAmmo()
    {
        gunStatList[selectedGun].ammoReserves = reseveGunAmmo;
        gunStatList[selectedGun].currentAmmo = gunAmmo;

        gameManager.instance.updateUI();
    }

    IEnumerator aimDownSights()
    {
        //gunStatList.Count > 0 && 
        if (Input.GetButtonDown("Fire2")) //bool needed to not allow fov change when menu is open
        {

            if (playerCamera.fieldOfView != fovOriginal)
            {
                isAiming = false;
                StartCoroutine(LerpFOV(isAiming));
            }
            else
            {
                isAiming = true;
                StartCoroutine(LerpFOV(isAiming));
            }
            yield return new WaitForSeconds(1);
        }
    }


    IEnumerator LerpFOV(bool isAiming)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            if (isAiming)
            {
                playerCamera.fieldOfView = Mathf.Lerp(fovOriginal, endValue, timeElapsed / lerpDuration);
                //gameManager.instance.reticle.SetActive(false);
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(endValue, fovOriginal, timeElapsed / lerpDuration);
                //gameManager.instance.reticle.SetActive(true);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (isAiming)
        {
            playerCamera.fieldOfView = endValue;
        }
        else
        {
            playerCamera.fieldOfView = fovOriginal;
        }
    }

    void Interact()
    {
        if (Input.GetButton("Interact"))
        {
            gameManager.instance.interactBarFill.fillAmount += Time.deltaTime / 2;
        }
        else
        {
            gameManager.instance.interactBarFill.fillAmount -= Time.deltaTime / 2;
        }

        if (gameManager.instance.interactBarFill.fillAmount == 1)
        {
            interact = true;
            Debug.Log("complete");
            gameManager.instance.InteractBar.SetActive(false);
            gameManager.instance.interactBarFill.fillAmount = 0;

            if (inDrone == true)
            {
                StartCoroutine(interactOn());

                RaycastHit hit;
                if (Physics.Raycast(droneCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, 100000))
                {

                    if (hit.collider.tag == "Bomb")
                    {
                        Debug.Log("Hit Objective");

                        hit.collider.GetComponent<Renderer>().material.shader = seeThroughWalls;
                    }
                }
            }

            IEnumerator interactOn()
            {
                yield return new WaitForSeconds(1.0f);
                gameManager.instance.InteractBar.SetActive(true);
                gameManager.instance.interactBarFill.fillAmount = 0;
            }

        }
    }
}
