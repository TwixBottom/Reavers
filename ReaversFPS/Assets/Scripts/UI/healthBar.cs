using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class healthBar : MonoBehaviour
{
    public GameObject player;
    public playerController playerHealth;
    public Image fillImage;
    private Slider slider;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        slider = GetComponent<Slider>();
        playerHealth = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float fillValue = playerHealth.HP / playerHealth.startHP;
        slider.value = fillValue;
    }
}
