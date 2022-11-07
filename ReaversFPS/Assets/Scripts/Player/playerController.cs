using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerController : MonoBehaviour
{
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [SerializeField] float HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpMax;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    [SerializeField] List<gunStats> gunStatList = new List<gunStats>();

    float startHP;
    Vector3 move;
    private Vector3 playerVelocity;
    int jumpTimes;
    bool isSprinting;
    bool isShooting;
    float playerStartSpeed;
    int selectedGun;

    // Start is called before the first frame update
    void Start()
    {
        startHP = HP;
        playerStartSpeed = playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerSprint();
        StartCoroutine(ShootWeapon());
        gunSelect();
    }

    // moves the player
    void PlayerMovement()
    {
        if(controller.isGrounded && playerVelocity.y < 0)
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
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
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
        if (!isShooting && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                if (hit.collider.GetComponent<PlayerDamage>() != null)
                {
                    hit.collider.GetComponent<PlayerDamage>().TakeDamage(shootDamage);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }
    public void respawn()
    {
        controller.enabled = false;
        HP = startHP;
        transform.position = gameManager.instance.spawnPosition.transform.position;
        controller.enabled = true;
    }

    public void gunPickup(gunStats gunStat)
    {
        shootRate = gunStat.fireRate;
        shootDist = gunStat.shootDistance;
        shootDamage = gunStat.damage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat.model.GetComponent<MeshRenderer>().sharedMaterial;

        if(!gunStatList.Contains(gunStat))
            gunStatList.Add(gunStat);
    }

    void gunSelect()
    {
        if (gunStatList.Count > 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunStatList.Count - 1)
            {
                selectedGun++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
            }
            gunPickup(gunStatList[selectedGun]);
        }
    }
}
