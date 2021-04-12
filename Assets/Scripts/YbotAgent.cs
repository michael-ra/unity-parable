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
    public EnvironmentController envController;
    public Team team;
    [HideInInspector]
    public Collider agentCollider;
    [HideInInspector]
    public Rigidbody rBody;
    private int lives;
    Vector3 initialPosition;

    // Start is called before the first frame update
    public void Start()
    {
        // Access to environment parameters defined in training
        // configuration
        agentCollider = this.GetComponentInChildren<Collider>();
        rBody = this.GetComponentInChildren<Rigidbody>();
        lives = 100;
        initialPosition = this.transform.position;

    }

    void Update()
    {
        if (lives <= 0)
        {
            this.rBody.velocity = Vector3.zero;
            this.rBody.angularVelocity = Vector3.zero;
            this.transform.position = initialPosition;
            lives = 100;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("chaser"))
        {
            lives-= 10;
            Debug.Log(lives);
        }
    }
    void OnTriggerStay(Collider other)
    {
        // Runner wins when the goal is reached
        // but is not allowed to touch lava
        if (team == Team.Runner)
        {
            if (other.gameObject.CompareTag("goal"))
            {
                Debug.Log("GOAL");
            }
            if (other.gameObject.CompareTag("lava"))
            {
                lives--;
                Debug.Log(lives);
            }
        }
    }
}   