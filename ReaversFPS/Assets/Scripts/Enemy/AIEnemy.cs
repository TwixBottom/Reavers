using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AwarenessSystem))]
public class AIEnemy : MonoBehaviour, IDamage
{
    [SerializeField] TextMeshProUGUI FeedbackDisplay;

    [SerializeField] float _VisionConeAngle = 60f;
    [SerializeField] float _VisionConeRange = 30f;
    [SerializeField] Color _VisionConeColour = new Color(1f, 0f, 0f, 0.25f);

    [SerializeField] float _HearingRange = 20f;
    [SerializeField] Color _HearingRangeColour = new Color(1f, 1f, 0f, 0.25f);

    [SerializeField] float _ProximityDetectionRange = 5.0f;
    [SerializeField] Color _ProximityRangeColor = new Color(1f, 1f, 1f, 0.25f);

    [SerializeField] bool _OnOff;


    [Header("Components")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Collider coll;
    [SerializeField] AudioSource aud;
    [SerializeField] Shader seeThroughWalls;
    [SerializeField] Shader originalShader;
    [SerializeField] Renderer render;


    [Header("EnemyStats")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int animLerpSpeed;

    [Header("Gun Stats")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] public float shootRate;
    [SerializeField] GameObject weaponDrop;

    [Header("Audio")]
    [SerializeField] AudioClip[] ShootAudio;
    [Range(0, 1)][SerializeField] float ShootVol;
    [SerializeField] AudioClip[] HitAudio;
    [Range(0, 1)][SerializeField] float HitVol;

    public Vector3 EyeLocation => transform.position;
    public Vector3 EyeDirection => transform.forward;

    public float VisionConeAngle => _VisionConeAngle;
    public float VisionConeRange => _VisionConeRange;
    public Color VisionConeColour => _VisionConeColour;

    public float HearingRange => _HearingRange;
    public Color HearingRangeColour => _HearingRangeColour;

    public float ProximityDetectionRange => _ProximityDetectionRange;
    public Color ProximityDetectionColor => _ProximityRangeColor;
    public float CosVisionConeAngle { get; private set; } = 0.0f;

    public Vector3 playerDirection;

    AwarenessSystem Awareness;

    public bool OnOff => _OnOff;
    public bool isShooting;
    public bool playerInRange;
    public bool isDead;

    public bool canShoot;
    public bool canThrowGrenade;
    public int grenadeCounter = 1;

    //bool isMoving = false;
    bool isHit = false;

    
    public bool tookDamage = false;
    bool chase;
    bool shoot;

    void Awake()
    {
        CosVisionConeAngle = Mathf.Cos(VisionConeAngle * Mathf.Deg2Rad);
        Awareness = GetComponent<AwarenessSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemiesToKill++;
        gameManager.instance.updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead != true)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

            if (chase)
            {
                playerDirection = (gameManager.instance.player.transform.position - transform.position).normalized;

                if (playerInRange == true && shoot == true)
                {
                    facePlayer();

                    if (isShooting == false)
                    {
                        StartCoroutine(shootPlayer());
                    }
                }
            }
            if (gameManager.instance.m_scene.name == "MainScene" && gameManager.instance.enemiesToKill <= 2)
            {
                render.material.shader = seeThroughWalls;
            }
        }
        else
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            render.material.shader = originalShader;
        }

        

        //if (!agent.isStopped && isMoving == false && isDead != true)
        //{
        //    StartCoroutine(WalkingAudio());
        //}
    }

    //IEnumerator WalkingAudio()
    //{
    //    isMoving = true;
    //    audio.PlayOneShot(WalkAudio[Random.Range(0, WalkAudio.Length)], WalkVol);
    //    yield return new WaitForSeconds(0.5f);
    //    isMoving = false;
    //}

    public void facePlayer()
    {
        playerDirection.y = 0;
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(flashDamage());

        gameManager.instance.points += 50;

        if (isHit != true)
        {
            StartCoroutine(HitSoundEffect());

        }

        if (HP <= 0)
        {
            isDead = true;

            gameManager.instance.points += 300;

            anim.SetBool("isDead", true);

            coll.enabled = false;

            gameManager.instance.updateEnemyNumbers();

            StartCoroutine(SpawnDrop());
        }
    }

    IEnumerator HitSoundEffect()
    {
        isHit = true;
        aud.PlayOneShot(HitAudio[Random.Range(0, HitAudio.Length)], HitVol);
        yield return new WaitForSeconds(0.4f);
        isHit = false;
    }


    IEnumerator SpawnDrop()
    {
        yield return new WaitForSeconds(1.5f);
        Instantiate(weaponDrop, gameObject.transform.position, gameObject.transform.rotation);
        StartCoroutine(DespawnEnemy());

    }

    IEnumerator DespawnEnemy()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = Color.white;
    }

    public IEnumerator shootPlayer()
    {
        isShooting = true;

        anim.SetTrigger("Shoot");

        aud.PlayOneShot(ShootAudio[Random.Range(0, ShootAudio.Length)], ShootVol);

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    public void ReportCanSee(DetectableTarget target)
    {
        Awareness.ReportCanSee(target);
    }

    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        Awareness.ReportCanHear(source, location, category, intensity);
    }

    public void ReportInProximity(DetectableTarget target)
    {      
        Awareness.ReportInProximity(target);
    }

    public void OnSuspicious()
    {
        //Debug.Log("I Hear You");

        canThrowGrenade = false;

        chase = true;

        shoot = false;
    }

    public void OnDetected(GameObject target)
    {
        //Debug.Log("I See You " + target.gameObject.name);

        canThrowGrenade = false;

        chase = true;

        shoot = false;
    }

    public void OnFullyDetected(GameObject target)
    {
        Debug.Log("Shoot");

        canThrowGrenade = true;

        chase = true;

        shoot = true;


    }

    public void OnLostDetection(GameObject target)
    {
        Debug.Log("Where Are You " + target.gameObject.name);

        canThrowGrenade = true;

        chase = false;

        shoot = false;
    }

    public void OnLostSuspicion()
    {
        //Debug.Log("Where Did You Go");

        canThrowGrenade = false;

        chase = false;

        shoot = false;
    }

    public void OnFullyLost()
    {
        //Debug.Log("Must Be Nothing");

        canThrowGrenade = false;

        chase = false;

        shoot = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AIEnemy))]
public class AIEnemyEditor : Editor
{
    public void OnSceneGUI()
    {
        AIEnemy ai = target as AIEnemy;
        if (ai.OnOff)
        {
            // draw the detection range
            Handles.color = ai.ProximityDetectionColor;
            Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.ProximityDetectionRange);


            // draw the hearing range
            Handles.color = ai.HearingRangeColour;
            Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.HearingRange);


            // work out the start point of the vision cone
            Vector3 startPoint = Mathf.Cos(-ai.VisionConeAngle * Mathf.Deg2Rad) * ai.transform.forward +
                                 Mathf.Sin(-ai.VisionConeAngle * Mathf.Deg2Rad) * ai.transform.right;

            // draw the vision cone
            Handles.color = ai.VisionConeColour;
            Handles.DrawSolidArc(ai.transform.position, Vector3.up, startPoint, ai.VisionConeAngle * 2f, ai.VisionConeRange);
        }
    }
}
#endif // UNITY_EDITOR
