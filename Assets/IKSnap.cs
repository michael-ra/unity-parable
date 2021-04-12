using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;

public class IKSnap : MonoBehaviour
{

    public bool useIK;
    public bool overwriteUseIKHand;
    public bool leftHandIK;
    public bool rightHandIK;


    public bool leftFootIK;
    public bool rightFootIK;

    public Vector3 leftHandPos;
    public Vector3 rightHandPos;

    public Vector3 leftFootPos;
    public Vector3 rightFootPos;

    public Vector3 leftFootOffset;
    public Vector3 rightFootOffset;

    public Vector3 leftHandFixDown;
    public Vector3 rightHandFixDown;

    public Quaternion leftHandRot;
    public Quaternion rightHandRot;

    private RaycastHit centerRaycastHit;

    public float speedRotate = 1.0f;

    public bool isRotated;

    public Collider coll;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        overwriteUseIKHand = false;
        anim = GetComponent<Animator>();
    }


    //UNUSED -> maybe in future? export into function
    public RaycastHit checkHandsObject()
    {
        RaycastHit CheckRHit;
        if (Physics.Raycast(
            transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) +
            Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            -transform.up + Vector3.Scale(transform.right, new Vector3(0.25f, 0.0f, 0.25f)) +
            Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out CheckRHit,
            1f))
        {
            return CheckRHit;
        }

        RaycastHit CheckLHit;
        if (Physics.Raycast(
            transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) +
            Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            -transform.up + Vector3.Scale(-transform.right, new Vector3(0.25f, 0.0f, 0.25f)) +
            Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out CheckLHit,
            1f))
        {
            return CheckLHit;
        }

        Debug.Log("Error in line 76: Please fix this with a check! I don't think this should happen!");
        return new RaycastHit();
    }

    public bool boolHandsObject()
    {
        RaycastHit CheckRHit;
        if (Physics.Raycast(
            transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) +
            Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            -transform.up + Vector3.Scale(transform.right, new Vector3(0.25f, 0.0f, 0.25f)) +
            Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out CheckRHit,
            1f))
        {
            return true;
        }

        RaycastHit CheckLHit;
        if (Physics.Raycast(
            transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) +
            Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            -transform.up + Vector3.Scale(-transform.right, new Vector3(0.25f, 0.0f, 0.25f)) +
            Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out CheckLHit,
            1f))
        {
            return true;
        }

        return false;
    }

    void FixedUpdate()
    {
        if (!Physics.Raycast(
            transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) +
            Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            transform.forward, 1f))
        {
           
            RaycastHit LHit;
            RaycastHit RHit;

            RaycastHit LFHit;
            RaycastHit RFHit;

        
            Physics.Raycast(
                transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 1.4f, 0.5f)) +
                Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
                transform.forward, out centerRaycastHit, 1f);

            //get Foot Hit for Position Offset Hand -> Is Hand muss foot TODO IMPORTANT
            RaycastHit CheckRHit;
            Physics.Raycast(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)), -transform.up + Vector3.Scale(transform.right, new Vector3(0.25f, 0.0f, 0.25f)) + Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out CheckRHit,
                1f);
            RaycastHit CheckLHit;
            Physics.Raycast(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)), -transform.up + Vector3.Scale(-transform.right, new Vector3(0.25f, 0.0f, 0.25f)) + Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out CheckLHit,
                1f);

            if (isRotated == false)
            {
                if (overwriteUseIKHand || useIK)
                {
                    Vector3 target;
                    RaycastHit towardsHit = checkHandsObject();
                    target = towardsHit.collider.ClosestPoint(transform.position);
                    isRotated = true;
                    transform.LookAt(target);
                    //Vector3 targetDirection = target.position - transform.position;
                    //targetDirection.z = target.position.z;
                    //targetDirection.y = target.position.y;
                    //transform.rotation.y= Quaternion.Lerp();
                }
            }



            //Left
            //check hits, save in LHit if hit
            if (Physics.Raycast(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)), -transform.up + Vector3.Scale(-transform.right, new Vector3(0.25f, 0.0f, 0.25f)) + Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out LHit,
                1f))
            {

                //Maybe use bounds?
                //->not working?
                //coll = LHit.collider;
                //Vector3 closestPoint = coll.ClosestPointOnBounds(LHit.point);

                leftHandIK = true;
                leftHandPos = LHit.collider.ClosestPointOnBounds(LHit.point);
                leftHandPos = leftHandPos - leftHandFixDown;
                //use hitting ray if either hits -> place hands on foot level -> At collider
                //if (CheckLHit.collider)
                //{
                //    leftHandPos[2] = CheckLHit.point[2];
                //}
                //else
                //{
                //    leftHandPos[2] = CheckRHit.point[2];
                //}
                Vector3 around = LHit.normal;
                leftHandRot = Quaternion.FromToRotation(Vector3.forward, around);


            }
            else
            {
                leftHandIK = false;
            }

            //Same for right
            if (Physics.Raycast(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)), -transform.up + Vector3.Scale(transform.right, new Vector3(0.25f, 0.0f, 0.25f)) + Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), out RHit,
                1f))
            {

                rightHandIK = true;
                rightHandPos = RHit.collider.ClosestPointOnBounds(RHit.point);
                rightHandPos = rightHandPos - rightHandFixDown;
                //if (CheckLHit.collider)
                //{
                //    rightHandPos[2] = CheckLHit.point[2];
                //}
                //else
                //{
                //    rightHandPos[2] = CheckRHit.point[2];
                //}

                rightHandRot = Quaternion.FromToRotation(Vector3.forward, RHit.normal);


            }
            else
            {
                rightHandIK = false;
            }

            if (leftHandIK) //check if Hands can grab -> only then move foot
            {
                //LeftFoot
                if (Physics.Raycast(transform.position + Vector3.Scale(-transform.right, new Vector3(0.8f, 0.8f, 0.8f)), transform.forward, out LFHit,
                    1f))
                {
                    leftFootIK = true;
                    leftFootPos = LFHit.point - leftFootOffset;
                }
                else
                {
                    leftFootIK = false;
                }
            }
            else
            {
                leftFootIK = false;
            }

            if (rightHandIK)
            {
                //RightFoot
                if (Physics.Raycast(transform.position + Vector3.Scale(transform.right, new Vector3(0.8f, 0.8f, 0.8f)), transform.forward, out LFHit, 1f))
                {
                    rightFootIK = true;
                    rightFootPos = LFHit.point - rightFootOffset;
                }
                else
                {
                    rightFootIK = false;
                }
            }
            else
            {
                rightFootIK = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //left - then right foot
        Debug.DrawRay(transform.position + Vector3.Scale(-transform.right, new Vector3(0.8f, 0.8f, 0.8f)), transform.forward, Color.green);
        Debug.DrawRay(transform.position + Vector3.Scale(transform.right, new Vector3(0.8f, 0.8f, 0.8f)), transform.forward, Color.green);

        Debug.DrawRay(transform.position + Vector3.Scale(transform.up, new Vector3(0.3f, 0.3f, 0.3f)), -transform.up, Color.blue);
        Debug.DrawRay(transform.position + Vector3.Scale(transform.up, new Vector3(0.3f, 0.3f, 0.3f)), -transform.up, Color.blue);

        //left hand
        Debug.DrawRay(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            -transform.up + Vector3.Scale(-transform.right, new Vector3(0.25f, 0.0f, 0.25f)) + Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), Color.red);
        //right hand
        Debug.DrawRay(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            -transform.up + Vector3.Scale(transform.right, new Vector3(0.25f, 0.0f, 0.25f)) + Vector3.Scale(transform.forward, new Vector3(0.15f, 0f, 0.15f)), Color.red);


        Debug.DrawRay(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 2.0f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            transform.forward, Color.red);

        Debug.DrawRay(transform.position + Vector3.Scale(transform.up, new Vector3(0.0f, 1.4f, 0.5f)) + Vector3.Scale(transform.forward, new Vector3(0.45f, 0f, 0.45f)),
            transform.forward, Color.white);
    }

    void OnAnimatorIK()
    {
        if (overwriteUseIKHand)
        {
            setHandsPos();
        }

        if (useIK)
        {
            setHandsPos();

            if (leftFootIK)
            {
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos);
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            }


            if (rightFootIK)
            {
                anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos);
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            }
        }
    }

    void setHandsPos()
    {
        if (leftHandIK)
        {
            //TODO: Possibly smoothen with Lerp or anything else
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRot);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        }
        if (rightHandIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandRot);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        }
    }
}
