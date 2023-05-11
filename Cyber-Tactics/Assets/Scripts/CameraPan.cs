using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the Grid View Camera.
//This script allows the player to pan the camera using keyboard or mouse controls.
public class CameraPan : MonoBehaviour
{
    public float panSpeed = 0.5f, zBounds = 3f, xBounds = 2f;
    private float zMin, zMax, xMin, xMax;

    private Vector3 camOrigin, camPos;
    private Vector3 mousePosOrigin, mousePos, mousePosDiff;

    //private bool mPan = false, kbPan = false;

    // Start is called before the first frame update
    void Start()
    {
        camOrigin = transform.localPosition;
        zMin = camOrigin.z - zBounds;
        zMax = camOrigin.z + zBounds;
        xMin = camOrigin.x - xBounds;
        xMax = camOrigin.x + xBounds;
    }

    // Update is called once per frame
    void Update()
    {
        //Keyboard Input
        //Pans camera towards the top of the screen
        if (Input.GetKey(KeyCode.W) /*&& !mPan*/) 
        {
            camPos = transform.localPosition;
            camPos += new Vector3(panSpeed, 0f, panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            //kbPan = true;
        }

        //Pans camera towards the bottom of the screen
        if (Input.GetKey(KeyCode.S) /*&& !mPan*/)
        {
            camPos = transform.localPosition;
            camPos += new Vector3(-panSpeed, 0f, -panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            //kbPan = true;
        }

        //Pans camera towards the left side of the screen
        if (Input.GetKey(KeyCode.A) /*&& !mPan*/)
        {
            camPos = transform.localPosition;
            camPos += new Vector3(-panSpeed, 0f, panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            //kbPan = true;
        }

        //Pans camera to the right side of the screen
        if (Input.GetKey(KeyCode.D) /*&& !mPan*/) 
        {
            camPos = transform.localPosition;
            camPos += new Vector3(panSpeed, 0f, -panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            //kbPan = true;
        }

        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            //kbPan = false;
        }

        //Mouse Input
        /*if (Input.GetKeyDown(KeyCode.Mouse1) && !kbPan)
        {
            mousePosOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mPan = true;
        }

        if (Input.GetKey(KeyCode.Mouse1) && mPan)
        {
            mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mousePosDiff = mousePosOrigin - mousePos;
            Debug.Log(mousePosDiff);

            if(mousePosDiff.x > 0f && mousePosDiff.y > 0f) //Up
            {
                camPos = transform.localPosition;
                camPos += new Vector3(mousePosDiff.x + panSpeed/2f, 0f, mousePosDiff.z + panSpeed/2f);
                transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            }

            if (mousePosDiff.x < 0f && mousePosDiff.y < 0f) //Down
            {
                camPos = transform.localPosition;
                camPos += new Vector3(mousePosDiff.x - panSpeed/2f, 0f, mousePosDiff.z - panSpeed/2f);
                transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            }

            if (mousePosDiff.x < 0f && mousePosDiff.y > 0f) //Left
            {
                camPos = transform.localPosition;
                camPos += new Vector3(mousePosDiff.x - panSpeed/2f, 0f, mousePosDiff.z + panSpeed/2f);
                transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            }

            if (mousePosDiff.x > 0f && mousePosDiff.y < 0f) //Right
            {
                camPos = transform.localPosition;
                camPos += new Vector3(mousePosDiff.x + panSpeed/2f, 0f, mousePosDiff.z - panSpeed/2f);
                transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            } 
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) && !kbPan)
        {
            mPan = false;
        }*/

        //Resetting camera position to its original position
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse2))
        {
            transform.localPosition = camOrigin;
        }
    }
}
