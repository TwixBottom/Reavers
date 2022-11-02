using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [SerializeField] int sensHorz;
    [SerializeField] int sensVert;

    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool inverty;

    float xRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // gets input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHorz;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;

        if (inverty)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        // clamp the camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        // rotate the camera on the x-axis;
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate the player
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
