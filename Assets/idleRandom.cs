using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idleRandom : StateMachineBehaviour
{
    public int totalClips = 4;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("idleNrRnd", Random.Range(0, totalClips));
    }
}