using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class YbotAgent : MonoBehaviour
{
    public enum Team
    {
        Chaser,
        Runner
    }
    public Transform target;
    [SerializeField]
    private EnvironmentController envController;
    [SerializeField]
    private UIManager _uiManager;
    public Team team;
    [HideInInspector]
    public Collider agentCollider;
    [HideInInspector]
    public Rigidbody rBody;
    private int lives;
    Vector3 initialPosition;
    Movement playerMovement;
    Animator anim;

    // Start is called before the first frame update
    public void Start()
    {
        // Access to environment parameters defined in training
        // configuration
        agentCollider = this.GetComponentInChildren<Collider>();
        rBody = this.GetComponentInChildren<Rigidbody>();
        lives = 100;
        initialPosition = this.transform.position;
        playerMovement = GetComponent<Movement>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        if (lives <= 0)
        {
            this.rBody.velocity = Vector3.zero;
            this.rBody.angularVelocity = Vector3.zero;
            this.transform.position = initialPosition;
            lives = 100;
            _uiManager.UpdateHP(lives);

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("chaser"))
        {
            lives-= 10;
            _uiManager.UpdateHP(lives);
            Debug.Log(lives);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (team == Team.Runner)
        {
            if (other.gameObject.CompareTag("goal"))
            {
                anim.Play("Macarena Dance");
                playerMovement.canMove = false;
            }
        }
            
    }
    void OnTriggerStay(Collider other)
    {
        // Runner wins when the goal is reached
        // but is not allowed to touch lava
        if (team == Team.Runner)
        {
            if (other.gameObject.CompareTag("lava"))
            {
                lives--;
                _uiManager.UpdateHP(lives);
                Debug.Log(lives);
            }
        }
    }
}   