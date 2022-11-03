using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    float startHP; 
    float playerStartSpeed;
    int jumpTimes;

    Vector3 move;
    private Vector3 playerVelocity;
    
    bool isSprinting;
    bool isShooting;

    protected float TimeUntilNextFootstep = -1f;
    protected Rigidbody CharacterRB;

    [Header("Events")]
    [SerializeField] UnityEvent OnPlayFootstepAudio;
    [SerializeField] UnityEvent OnPlayJumpAudio;
    [SerializeField] UnityEvent OnPlayDoubleJumpAudio;
    [SerializeField] UnityEvent OnPlayLandAudio;


    // Start is called before the first frame update
    void Start()
    {
        CharacterRB = GetComponent<Rigidbody>();
        isSprinting = false;
        startHP = HP;
        playerStartSpeed = playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerSprint();
        StartCoroutine(ShootWeapon());
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

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }
    public void Respawn()
    {
        controller.enabled = false;
        HP = startHP;
        transform.position = gameManager.instance.spawnPosition.transform.position;
        gameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }
}
