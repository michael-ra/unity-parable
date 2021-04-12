using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour
{
    private Animator anim;
    public IKSnap SnapControl;
    public bool canMove;
    public bool canJump;
    public bool exhausted;
    public bool isDebug;
    public float duration;

    void Awake()
    {
        isDebug = false;
        canMove = true;
        canJump = true;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        SnapControl = GetComponent<IKSnap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            Move();
        }
    }

    void Move()
    {

        if (!SnapControl.boolHandsObject() && canJump && !exhausted )
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //TODO before calling jump exhaust duration = anim.GetStateByName("Jump").clip.length;
                anim.Play("Jumping Up");
                StartCoroutine("ExhaustHandler");

            }
        }

        //not snapped
        if (!SnapControl.useIK && !SnapControl.overwriteUseIKHand)
        {
            if (isDebug)
            {
                Debug.Log(SnapControl.useIK);
                Debug.Log(SnapControl.overwriteUseIKHand);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                anim.SetBool("Sprinting", true);
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                anim.SetBool("Sprinting", false);
            }

            anim.SetFloat("Forward", Input.GetAxisRaw("Vertical"));
        }
        else
        {
            anim.SetFloat("Forward", 0f);
            anim.SetBool("Sprinting", false);
        }
    }

    public void disableJump()
    {
        canJump = false;
    }
    public void enableJump()
    {
        canJump = true;
    }
    IEnumerator ExhaustHandler()
    {
        exhausted = true;
        yield return new WaitForSeconds(1.4f);
        exhausted = false;
    }


}
