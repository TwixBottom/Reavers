using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerController : MonoBehaviour
{
    [Header("----- Components ------")]
    [SerializeField] CharacterController controller;

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
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
   
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
    
    public float startHP; 
    public int reseveGunAmmo;


    // Start is called before the first frame update
    void Start()
    { 
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
        StartCoroutine(ShootWeapon());
        StartCoroutine(RelodeWeapon());
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

        if (HP <= 0)
        {
            gameManager.instance.playerDeadMenu.SetActive(true);
            gameManager.instance.Pause();
        }
        
        StartCoroutine(gameManager.instance.playerDamageFlash());
    }

    IEnumerator ShootWeapon()
    {
        if (gunAmmo > 0)
        {
            if (!isShooting && Input.GetButton("Shoot"))
            {
                isShooting = true;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                {
                    if (hit.collider.GetComponent<PlayerDamage>() != null && hit.collider.tag == "Enemy")
                    {
                        hit.collider.GetComponent<PlayerDamage>().TakeDamage(shootDamage);
                    }
                }

                gunAmmo--;

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
            }
            else
            {
                Debug.Log("ELSE");
                gunAmmo = startAmmo;
                reseveGunAmmo -= startAmmo;
            }

            gameManager.instance.updateUI();
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

    
}
