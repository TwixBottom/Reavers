using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AwarenessSystem))]
public class AIEnemy : MonoBehaviour, PlayerDamage
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


    [Header("EnemyStats")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;

    [Header("Gun Stats")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;

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

    Vector3 playerDirection;

    AwarenessSystem Awareness;

    public bool OnOff => _OnOff;

    bool chase = false;
    bool isShooting;
    bool playerInRange;

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
        if (chase)
        {
            playerDirection = (gameManager.instance.player.transform.position - transform.position).normalized;

            agent.SetDestination(gameManager.instance.player.transform.position);

            if (playerInRange == true)
            {
                facePlayer();

                if (isShooting == false)
                {
                    StartCoroutine(shootPlayer());
                }
            }
        }
    }

    void facePlayer()
    {
        playerDirection.y = 0;
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            Destroy(gameObject);
            gameManager.instance.updateEnemyNumbers();
        }
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

    IEnumerator shootPlayer()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    public void ReportCanSee(DetectableTarget target)
    {
        Debug.Log("CanSee00");
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
        Debug.Log("I Hear You");

        chase = true;
    }

    public void OnDetected(GameObject target)
    {
        Debug.Log("I See You " + target.gameObject.name);

        chase = true;
    }

    public void OnFullyDetected(GameObject target)
    {
        Debug.Log("Shoot");

        chase = true;
    }

    public void OnLostDetection(GameObject target)
    {
        Debug.Log("Where Are You " + target.gameObject.name);

        chase = false;
    }

    public void OnLostSuspicion()
    {
        Debug.Log("Where Did You Go");

        chase = false;
    }

    public void OnFullyLost()
    {
        Debug.Log("Must Be Nothing");

        chase = false;
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
