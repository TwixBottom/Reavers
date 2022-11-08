using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerController : MonoBehaviour
{
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;

    [Header("----- Player Stats -----")]
    [SerializeField] public float HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpMax;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] public int gunAmmo;
    [SerializeField] public int magazineCount;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    [SerializeField] List<gunStats> gunStatList = new List<gunStats>();
    //FOV
    float lerpDuration = 0.2f;
    float endValue = 15;
    float valueToLerp;

    int startAmmo;
    float playerStartSpeed;
    int jumpTimes;

    Vector3 move;
    private Vector3 playerVelocity;

    bool isSprinting;
    bool isShooting;
    bool isReloding;

    [Header("Events")]
    [SerializeField] UnityEvent OnPlayFootstepAudio;
    [SerializeField] UnityEvent OnPlayJumpAudio;
    [SerializeField] UnityEvent OnPlayDoubleJumpAudio;
    [SerializeField] UnityEvent OnPlayLandAudio;
    [SerializeField] UnityEvent OnPlayShootAudio;

    public float startHP;
    public int reseveGunAmmo;
    int selectedGun;

    float fovOriginal;

    // Start is called before the first frame update
    void Start()
    {
        fovOriginal = playerCamera.fieldOfView;

        isSprinting = false;
        startHP = HP;
        startAmmo = gunAmmo;
        playerStartSpeed = playerSpeed;
        reseveGunAmmo = magazineCount * startAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerSprint();
        StartCoroutine(aimDownSights());
        StartCoroutine(ShootWeapon());
        StartCoroutine(RelodeWeapon());
        gunSelect();
    }

    // moves the player
    void PlayerMovement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpTimes = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * Time.deltaTime * playerSpeed);

        // changes the height position of the player
        if (Input.GetButtonDown("Jump") && jumpTimes < jumpMax)
        {
            jumpTimes++;

            if (isSprinting == false)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            else
            {
                playerVelocity.y += Mathf.Sqrt((jumpHeight * 2) * -3.0f * gravityValue);
            }
            OnPlayLandAudio?.Invoke();
            gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EJump, 2.0f);
        }

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
            OnPlayFootstepAudio?.Invoke();
            gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EFootstep, isSprinting ? 2f : 1f);
        }
    }


    public void TakeDamage(float dmg)
    {
        HP -= dmg;

        StartCoroutine(gameManager.instance.playerDamageFlash());

        if (HP <= 0)
        {
            gameManager.instance.playerDeadMenu.SetActive(true);
            gameManager.instance.Pause();
        }
    }


    IEnumerator ShootWeapon()
    {
        if (gunAmmo > 0)
        {
            if (gunStatList.Count > 0 && !isShooting && Input.GetButton("Shoot"))
            {
                isShooting = true;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                {
                    if (hit.collider.GetComponent<PlayerDamage>() != null && hit.collider.tag == "Enemy")
                    {
                        hit.collider.GetComponent<PlayerDamage>().TakeDamage(shootDamage);
                    }

                    //Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);

                    OnPlayShootAudio?.Invoke();
                    gameManager.instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EShoot, 2.0f);

                }

                gunAmmo--;
                saveWeaponAmmo();

                yield return new WaitForSeconds(shootRate);
                isShooting = false;
            }
        }
        else if (!isReloding && gunAmmo == 0 && reseveGunAmmo > 0)
        {

            isReloding = true;
            yield return new WaitForSeconds(2.0f);
            isReloding = false;

            if (reseveGunAmmo - startAmmo <= 0)
            {
                Debug.Log("IF");
                gunAmmo = reseveGunAmmo;
                reseveGunAmmo = 0;
                saveWeaponAmmo();
            }
            else
            {
                Debug.Log("ELSE");
                gunAmmo = startAmmo;
                reseveGunAmmo -= startAmmo;
                saveWeaponAmmo();
            }
        }

        gameManager.instance.updateUI();

    }


    IEnumerator RelodeWeapon()
    {
        if (!isReloding && Input.GetButtonDown("Reload") && reseveGunAmmo > 0 && gunAmmo != startAmmo)
        {
            int ammoLeft = startAmmo - gunAmmo;

            isReloding = true;
            yield return new WaitForSeconds(2.0f);
            isReloding = false;

            if (reseveGunAmmo - ammoLeft <= 0)
            {
                gunAmmo += ammoLeft;
                reseveGunAmmo = 0;
            }
            else
            {
                gunAmmo += ammoLeft;
                reseveGunAmmo -= ammoLeft;
            }

            gameManager.instance.updateUI();
        }
    }


    public void Respawn()
    {
        controller.enabled = false;
        HP = startHP;
        gunAmmo = startAmmo;
        reseveGunAmmo = startAmmo * magazineCount;
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
                reseveGunAmmo += (gunStat.magazineCount * gunStat.ammoCount) + gunStat.ammoCount;
            }
            else
            {
                gunStatList[list].ammoReserves += (gunStat.magazineCount * gunStat.ammoCount) + gunStat.ammoCount;
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
        
            if (gunStatList.Count != 0)
            {
                gunStat.ammoReserves = gunStat.magazineCount * gunStat.ammoCount;
                gunStat.currentAmmo = gunStat.ammoCount;
            }

            gunStatList.Add(gunStat);

            
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


    void gunSelect()
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
                StartCoroutine(LerpFOV(false));
            }
            else
            {
                StartCoroutine(LerpFOV(true));
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
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(endValue, fovOriginal, timeElapsed / lerpDuration);

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
}
