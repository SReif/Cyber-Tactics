using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the Grid View Camera.
//This script allows the player to pan the camera using keyboard or mouse controls.
public class CameraPan : MonoBehaviour
{
    public float panSpeed = 0.5f, zBounds = 3f, xBounds = 2f;
    private float zMin, zMax, xMin, xMax;
    //private float zTranslate, xTranslate;

    private Vector3 camOrigin, camPos;
    private Vector3 mousePosOrigin, mousePos, mousePosDiff;

    private bool mPan = false, kbPan = false;
    //public bool canPanLeft, canPanRight, canPanUp, canPanDown;

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
        if (Input.GetKey(KeyCode.W) && !mPan) //Up
        {
            camPos = transform.localPosition;
            camPos += new Vector3(panSpeed, 0f, panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            kbPan = true;
        }

        if (Input.GetKey(KeyCode.S) && !mPan) //Down
        {
            camPos = transform.localPosition;
            camPos += new Vector3(-panSpeed, 0f, -panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            kbPan = true;
        }

        if (Input.GetKey(KeyCode.A) && !mPan) //Left
        {
            camPos = transform.localPosition;
            camPos += new Vector3(-panSpeed, 0f, panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            kbPan = true;
        }

        if (Input.GetKey(KeyCode.D) && !mPan) //Right
        {
            camPos = transform.localPosition;
            camPos += new Vector3(panSpeed, 0f, -panSpeed);
            transform.localPosition = new Vector3(Mathf.Clamp(camPos.x, xMin, xMax), camPos.y, Mathf.Clamp(camPos.z, zMin, zMax));
            kbPan = true;
        }

        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            kbPan = false;
        }

        //Mouse Input
        if (Input.GetKeyDown(KeyCode.Mouse1) && !kbPan)
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
        }

        //Resetting camera position to its original position
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse2))
        {
            transform.localPosition = camOrigin;
        }
    }

    //Old Code: Don't delete yet
    /*
     *  //Conditions for stopping camera from panning out of bounds
        if((transform.localPosition.z >= camOrigin.z + zBounds) && (transform.localPosition.x <= camOrigin.x - xBounds))
        {
            canPanLeft = false;
        }

        else
        {
            canPanLeft = true;
        }

        if((transform.localPosition.z <= camOrigin.z - zBounds) && (transform.localPosition.x >= camOrigin.x + xBounds))
        {
            canPanRight = false;
        }

        else
        {
            canPanRight = true;
        }

        if ((transform.localPosition.x >= camOrigin.x + xBounds) && (transform.localPosition.z >= camOrigin.z + zBounds))
        {
            canPanUp = false;
        }

        else
        {
            canPanUp = true;
        }

        if ((transform.localPosition.x <= camOrigin.x - xBounds && transform.localPosition.z <= camOrigin.z - zBounds))
        {
            canPanDown = false;
        }

        else
        {
            canPanDown = true;
        }

        //Controls for panning with mouse and conditions to not pan with keyboard
        if (Input.GetKeyDown(KeyCode.Mouse2) && !kbPan)
        {
            mousePosOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            mPan = true;
        }

        if(Input.GetKey(KeyCode.Mouse2) && mPan)
        {
            mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            zTranslate = ((mousePosOrigin.x - mousePos.x) * panSpeed/2f);
            xTranslate = ((mousePosOrigin.y - mousePos.y) * panSpeed / 2f);
            Debug.Log("Z Translate = " + zTranslate);
            Debug.Log("X Translate = " + xTranslate);

            if (canPanLeft && zTranslate >= 0f && xTranslate <= 0f)
            {
                transform.Translate(xTranslate, 0f, zTranslate, Space.World);
            }

            if(canPanRight && zTranslate <= 0f && xTranslate >= 0f)
            {
                transform.Translate(xTranslate, 0f, zTranslate, Space.World);
            }

            if(canPanUp && xTranslate >= 0f && zTranslate >= 0f)
            {
                transform.Translate(xTranslate, 0f, zTranslate, Space.World);
            }

            if (canPanDown && xTranslate <= 0f && zTranslate <= 0f)
            {
                transform.Translate(xTranslate, 0f, zTranslate, Space.World);
            }
        }

        if(Input.GetKeyUp(KeyCode.Mouse2) && !kbPan)
        {
            mPan = false;
        }

        //Controls for panning with keyboard and conditions to not pan with mouse
        if((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && !mPan)
        {
            kbPan = true;
        }

        if(Input.GetKey(KeyCode.A) && kbPan && canPanLeft)
        {
            transform.Translate(new Vector3(-panSpeed/5f, 0, panSpeed/5f), Space.World);
        }

        if(Input.GetKey(KeyCode.D) && kbPan && canPanRight)
        {
            transform.Translate(new Vector3(panSpeed/5f, 0, -panSpeed/5f), Space.World);
        }

        if(Input.GetKey(KeyCode.W) && kbPan && canPanUp)
        {
            transform.Translate(new Vector3(panSpeed/5f, 0, panSpeed/5f), Space.World);
        }

        if(Input.GetKey(KeyCode.S) && kbPan && canPanDown)
        {
            transform.Translate(new Vector3(-panSpeed/5f, 0, -panSpeed/5f), Space.World);
        }

        if((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) && !mPan)
        {
            kbPan = false;
        }
     */
}
