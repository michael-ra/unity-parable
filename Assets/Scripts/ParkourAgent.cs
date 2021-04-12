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

public class ParkourAgent : Agent
{
    public enum BrainType
    {
        Static,
        Random,
        User,

        NeuralNetwork,
    }
    public enum Team
    {
        Chaser,
        Runner
    }
    public Transform target;
    public EnvironmentController envController;
    public BrainType brainType;
    [HideInInspector]
    public Team team;
    [HideInInspector]
    public Collider agentCollider;
    [HideInInspector]
    public Rigidbody rBody;
    private BehaviorParameters behaviourParameters;
    private EnvironmentParameters resetParams;
    private float existential_reward;

    // Start is called before the first frame update
    public override void Initialize()
    {
        // Access to environment parameters defined in training
        // configuration
        resetParams = Academy.Instance.EnvironmentParameters;
        agentCollider = this.GetComponentInChildren<Collider>();
        rBody = this.GetComponentInChildren<Rigidbody>();
        behaviourParameters = this.GetComponent<BehaviorParameters>();
        
        // Assign team according to id
        if (behaviourParameters.TeamId == 0)
        {
            team = Team.Chaser;
            existential_reward = -1f / MaxStep;

        }
        else
        {
            team = Team.Runner;
            existential_reward = 1f / MaxStep;
        }
    }


    public override void OnEpisodeBegin()
    {
        // Get brain type of runner
        if (team == Team.Runner)
        {
            var param_id = resetParams.GetWithDefault("runner_brain_type", (float) brainType);
            if (param_id == 0.0f){brainType = BrainType.Static;}
            else if (param_id == 1.0f){brainType = BrainType.Random;}
            else if (param_id == 2.0f){brainType = BrainType.User;}
            else {brainType = BrainType.NeuralNetwork;}
        }
        if (brainType == BrainType.Random 
        || brainType == BrainType.Static 
        || brainType == BrainType.User)
        {
            behaviourParameters.BehaviorType = BehaviorType.HeuristicOnly;
        }
        else
        {
            behaviourParameters.BehaviorType = BehaviorType.Default;
        }
        
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Let agent observe own position, velocity and position of target
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rBody.velocity.x);
        sensor.AddObservation(this.rBody.velocity.z);
        sensor.AddObservation(target.localPosition);

        // Let agent observe to which team it belongs in MARL setting
        if (envController.multiAgentRL)
        {
            sensor.AddOneHotObservation((int) this.team, 2);
        }
    }

    public float forceScale = 10f;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move agent by adding forces to the rigid body
        Vector3 action = Vector3.zero;

        // Read outputs from neural network (or heuristic)
        action.x = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        action.z = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        this.rBody.AddForce(action * forceScale);
        
        // Incentivize fast chasing (team chaser) or long survival (team runner)
        AddReward(existential_reward);
        
    }

    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        // will be passed to OnActionReceived
        var continuousActionsOut = actionBuffers.ContinuousActions;
        
        // static brain, do nothing
        if (brainType == BrainType.Static){}
        // random brain, sample random action
        else if (brainType == BrainType.Random)
        {
            continuousActionsOut[0] = Random.Range(-4f, 4f);
            continuousActionsOut[1] = Random.Range(-4f, 4f);
        }
        // User inputs
        else
        {
            continuousActionsOut[0] = Input.GetAxis("Vertical");
            continuousActionsOut[1] = -Input.GetAxis("Horizontal");
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // chaser wins when runner reached and is not
        // allowed to touch the border
        if (team == Team.Chaser)
        {
            if (collision.gameObject.CompareTag("runner"))
            {
                AddReward(1.0f);
                envController.GoalReached(team);
            }
            if (collision.gameObject.CompareTag("border"))
            {
                AddReward(-1.0f);
                envController.ResetAgent(this);
                EndEpisode();
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        // Runner wins when the goal is reached
        // but is not allowed to touch lava
        if (other.gameObject.CompareTag("goal") &&
        (team == Team.Runner))
        {
            if (!envController.multiAgentRL){AddReward(1.0f);}
            envController.GoalReached(team);
        }
        if (other.gameObject.CompareTag("lava") &&
        (team == Team.Runner))
        {
            if (!envController.multiAgentRL){AddReward(-1.0f);}
            envController.GoalReached(Team.Chaser);
        }
    }
}   