using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectableTarget : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.VisionRegister(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (gameManager.instance != null)
        {
            gameManager.instance.VisionDeregister(this);
        }
    }

}
