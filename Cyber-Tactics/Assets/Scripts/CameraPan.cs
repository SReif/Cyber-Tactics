using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the Grid View Camera.
//This script allows the player to pan the camera using keyboard or mouse controls.
public class CameraPan : MonoBehaviour
{
    public float panSpeed = 0.5f, camBounds = 3f;
    private float zTranslate;

    private Vector3 camOrigin;
    private Vector3 mousePosOrigin, mousePos;

    private bool mPan = false, kbPan = false;
    private bool canPanLeft = true, canPanRight;

    // Start is called before the first frame update
    void Start()
    {
        camOrigin = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Conditions for stopping camera from panning out of bounds
        if(transform.localPosition.z >= camOrigin.z + camBounds)
        {
            canPanLeft = false;
        }

        else
        {
            canPanLeft = true;
        }

        if(transform.localPosition.z <= camOrigin.z - camBounds)
        {
            canPanRight = false;
        }

        else
        {
            canPanRight = true;
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

            if(canPanLeft && zTranslate >= 0f)
            {
                transform.Translate(0f, 0f, zTranslate, Space.World);
            }

            if(canPanRight && zTranslate <= 0f)
            {
                transform.Translate(0f, 0f, zTranslate, Space.World);
            }
            
        }

        if(Input.GetKeyUp(KeyCode.Mouse2) && !kbPan)
        {
            mPan = false;
        }

        //Controls for panning with keyboard and conditions to not pan with mouse
        if((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && !mPan)
        {
            kbPan = true;
        }

        if(Input.GetKey(KeyCode.A) && kbPan && canPanLeft)
        {
            transform.Translate(new Vector3(0, 0, panSpeed/5f), Space.World);
        }

        if(Input.GetKey(KeyCode.D) && kbPan && canPanRight)
        {
            transform.Translate(new Vector3(0, 0, -panSpeed/5f), Space.World);
        }

        if((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) && !mPan)
        {
            kbPan = false;
        }

        //Resetting camera position to its original position
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.localPosition = camOrigin;
        }
    }
}
