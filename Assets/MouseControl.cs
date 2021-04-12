using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{

    public IKSnap SnapControl;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;



    // Use this for initialization
    void Start()
    {
        SnapControl = GetComponent<IKSnap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SnapControl.useIK || SnapControl.overwriteUseIKHand)
        {
            return;
        }
        yaw += speedH * Input.GetAxis("Mouse X");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

    }
}