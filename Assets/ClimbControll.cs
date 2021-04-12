using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbControll : MonoBehaviour
{
    private Animator anim;
    public bool isHanging;
    public IKSnap SnapInstance;
    public Movement MovementInstance;

    // Start is called before the first frame update
    void Start()
    {
        SnapInstance = GetComponent<IKSnap>();
        MovementInstance = GetComponent<Movement>();
        isHanging = false;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SnapInstance.useIK)
        {
            anim.SetBool("isHang", SnapInstance.useIK);
        }

        RaycastHit LHit;
        RaycastHit BHit;
        RaycastHit RHit;

        //if in front is something to grab on with right hight
        if (SnapInstance.boolHandsObject())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //depending on if on ground
                if (!Physics.Raycast(transform.position + new Vector3(0f, +0.3f, 0.0f), -transform.up, out BHit,
                    1f))
                {
                    SnapInstance.useIK = true;
                    SnapInstance.overwriteUseIKHand = false;
                }
                else
                {
                    SnapInstance.useIK = false;
                    SnapInstance.overwriteUseIKHand = true;
                }
            }
        }


        //priority 1 else will climb without hang first
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isHanging && MovementInstance.canMove)
            {
                isHanging = false;
                SnapInstance.useIK = false;
                SnapInstance.overwriteUseIKHand = true;
                //transform forward TODO
                MovementInstance.canMove = false;
                anim.Play("Climbing");
            }
        }

        //priority 2 else will climb without hang first
        //if climbing / using IK
        if (SnapInstance.useIK || SnapInstance.overwriteUseIKHand)
        {
            isHanging = true;
            //no climbing and unsnapping
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Climbing"))
            {
                //reset kinematics
                if (Input.GetKeyDown(KeyCode.S))
                {
                    isHanging = false;
                    giveControlBackAndUnhang();
                }
            }
        }
    }

    public void giveControlBackAndUnhang()
    {
        Debug.Log("giveControlBackAndUnhang");
        MovementInstance.canMove = true;
        SnapInstance.useIK = false;
        SnapInstance.overwriteUseIKHand = false;
        SnapInstance.isRotated = false;
        isHanging = false;
        MovementInstance.canJump = true;
    }


}
