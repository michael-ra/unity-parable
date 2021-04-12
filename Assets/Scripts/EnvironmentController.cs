using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;
using Unity.MLAgents;



public class EnvironmentController : MonoBehaviour
{
    
    public Boolean multiAgentRL = false;
    public Boolean resetObstaclesOnEpisodeBegin = true;
    public List<ParkourAgent> AgentList = new List<ParkourAgent>();
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;
    public List<GameObject> agentSpawnRegions = new List<GameObject>();
    private List<Bounds> agentSpawnRegionBounds = new List<Bounds>();
    public GameObject pillarRoot;
    private List<GameObject> pillarList = new List<GameObject>();
    public Transform initialObstaclePosition;
    public GameObject platformRoot;
    private List<Bounds> pillarColliderBoundsList = new List<Bounds>();
    public GameObject pillarSpawnRegion;
    private Bounds pillarSpawnRegionBounds;

    public GameObject platformSpawnRegion;
    private Bounds platformSpawnRegionBounds;
    public int numPillars = 4;
    public int numPlatforms = 4;
    public int pillarOffset = 8;
    public float agentTargetDistance = 10;
    private SimpleMultiAgentGroup chaserAgentGroup;
    private SimpleMultiAgentGroup runnerAgentGroup;
    private int resetTimer;
    private List<GameObject> platformList = new List<GameObject>();
    private List<Bounds> platformColliderBoundsList = new List<Bounds>();
    private EnvironmentParameters resetParams;

    // Start is called before the first frame update
    void Start()
    {
        // Init Academy
        resetParams = Academy.Instance.EnvironmentParameters;

        // Init agent group
        chaserAgentGroup = new SimpleMultiAgentGroup();
        runnerAgentGroup = new SimpleMultiAgentGroup();

        // Get bounds of spawn volumes and deactivate them
        foreach (var item in agentSpawnRegions)
        {
            agentSpawnRegionBounds.Add(item.GetComponent<Collider>().bounds);
            item.SetActive(false);
        }
        pillarSpawnRegionBounds = pillarSpawnRegion.GetComponent<Collider>().bounds;
        platformSpawnRegionBounds = platformSpawnRegion.GetComponent<Collider>().bounds;
        pillarSpawnRegion.SetActive(false);
        platformSpawnRegion.SetActive(false);

        if (multiAgentRL)
        {
            Debug.Log(@"Observation space size has to be increased
            by two in order to let the agent observe which team it belongs to.");
            // Assign agents to groups
            foreach (var item in AgentList)
            {
                if (item.team is ParkourAgent.Team.Runner)
                {
                    runnerAgentGroup.RegisterAgent(item);
                }
                else
                {
                    chaserAgentGroup.RegisterAgent(item);
                }
            }
        }
        // Initial setup of environment
        ConfigureEnv(true);
    }

    // Instantiate obstacles and reset environment
    // Can be called at runtime when the number of obstacles
    // changes.
    void SetupEnv()
    {
        // Activate root obstacles so that the
        // clones are visible.
        pillarRoot.SetActive(true);
        platformRoot.SetActive(true);
        
        // Remove existing clones.
        var curNumPillars = pillarList.Count;
        for (int i = (curNumPillars - 1);i >= 0;i--)
        {
            Destroy(pillarList[i]);
            pillarList.RemoveAt(i);
            pillarColliderBoundsList.RemoveAt(i);
        }

        var curNumPlatforms = platformList.Count;
        for (int i = (curNumPlatforms - 1);i >= 0;i--)
        {
            Destroy(platformList[i]);
            platformList.RemoveAt(i);
            platformColliderBoundsList.RemoveAt(i);
        }
        
        // Instantiate obstacles accordingly
        foreach (var item in Enumerable.Range(0, numPillars ))
        {
            var pillar = Instantiate(pillarRoot, transform);
            pillarColliderBoundsList.Add(pillar.GetComponent<Collider>().bounds);
            pillarList.Add(pillar);
        }

        foreach (var item in Enumerable.Range(0, numPlatforms))
        {
            var platform = Instantiate(platformRoot, transform);
            platform.GetComponent<Platform>().StoreLavaZPos();
            platformColliderBoundsList.Add(platform.GetComponent<Collider>().bounds);
            platformList.Add(platform);
        }

        // Deactivate root objects again
        pillarRoot.SetActive(false);
        platformRoot.SetActive(false);
    }

    // Reset agents and randomize position of obstacles
    void ResetScene(Boolean setup)
    {
        resetTimer = 0;

        // Reset agents
        agentTargetDistance = resetParams.GetWithDefault("agent_target_distance", 4f);
        foreach (var item in AgentList)
        {
            ResetAgent(item);
        }
        
        if (resetObstaclesOnEpisodeBegin || setup)
        {
            // Get list of offsets to sample a unique offset for
            // each obstacle from
            List<int> offsetList = new List<int>();
            for (int n = 0; n < (numPillars + numPlatforms); n++)
            {
                offsetList.Add(n);
            }

            // Randomize pillar positions
            foreach (var wb in pillarList.Zip(pillarColliderBoundsList, Tuple.Create))
            {
                RandomPositionObstacle("pillar", wb.Item1, wb.Item2, offsetList);

            }

            // Randomize platform positions
            foreach (var wb in platformList.Zip(platformColliderBoundsList, Tuple.Create))
            {
                RandomPositionObstacle("platform", wb.Item1, wb.Item2, offsetList);
                wb.Item1.GetComponent<Platform>().ResetLavaZPos();
            }
        }
            
    }

    private void RandomPositionObstacle(
        string obstacleType, 
        GameObject obstacle, 
        Bounds obstacleBounds, 
        List<int> offsetList)
    {
        Vector3 position;
        
        // Get position in spawn volume
        if (obstacleType == "platform")
        {
            position = GetRandomSpawnPosition(
                platformSpawnRegion,
                platformSpawnRegionBounds,
                obstacleBounds
            );
        }
        else if (obstacleType == "pillar")
        {
            position = GetRandomSpawnPosition(
                pillarSpawnRegion,
                pillarSpawnRegionBounds,
                obstacleBounds
            );
        }
        else{position = new Vector3();}
        
        // Set position according to the formula:
        // initialObstaclePosition + offset in x direction + position in spawn volume
        int index = Random.Range(0, offsetList.Count - 1);
        int offset_multiplier = offsetList[index];
        position += initialObstaclePosition.position;
        position.y = obstacle.transform.position.y;
        position.x += pillarOffset * offset_multiplier;
        obstacle.transform.position = position;
        
        // remove offset
        offsetList.RemoveAt(index);
    }

    // Reset given agent
    public void ResetAgent(ParkourAgent agent)
    {
        // Reset velocity
        agent.rBody.angularVelocity = Vector3.zero;
        agent.rBody.velocity = Vector3.zero;
        // Respawn chaser in spawn volume
        var index = Random.Range(0, agentSpawnRegions.Count);
        var curRegion = agentSpawnRegions[index];
        var curBounds = agentSpawnRegionBounds[index];
        if (agent.team is ParkourAgent.Team.Chaser)
        {
            agent.transform.position = curRegion.transform.position +
                GetRandomSpawnPosition(
                    curRegion,
                    curBounds,
                    agent.agentCollider.bounds
                );
        }
        // Respawn at runner at given distance from chaser
        else 
        {
            var dist = -agentTargetDistance + curRegion.transform.localPosition.x;
            var targetPosition = new Vector3(Random.Range(dist-2, dist+2), 0.5f, Random.Range(-4f, 4f));
            // Move the target to a new spot
            agent.transform.localPosition = targetPosition;

        }
    }

    // Checks if a full setup is necessary,
    // i.e. when the environment parameters change,
    // or if the environemt only has to be reset
    void ConfigureEnv(bool setup)
    {
        if (numPillars != (int) resetParams.GetWithDefault("num_pillars", numPillars) ||
            numPlatforms != (int) resetParams.GetWithDefault("num_platforms", numPlatforms) ||
            setup)
        {
            numPillars = (int) resetParams.GetWithDefault("num_pillars", numPillars);
            numPlatforms = (int) resetParams.GetWithDefault("num_platforms", numPlatforms);
            SetupEnv();
            // Reset scene to position clones and agents
            ResetScene(true);
        }
        else
        {
            ResetScene(false);
        }
    }
    
    // Reset Enviroment and add group reward if either the
    // chaser reached the runner or the runner reached the goal
    public void GoalReached(ParkourAgent.Team team)
    {
        if (multiAgentRL)
        {
            // Chaser win
            if (team == ParkourAgent.Team.Chaser)
            {
                chaserAgentGroup.AddGroupReward(1.0f - (resetTimer / MaxEnvironmentSteps));
                runnerAgentGroup.AddGroupReward(-1.0f);
            }
            // Runner win
            else
            {
                runnerAgentGroup.AddGroupReward(1.0f - (resetTimer / MaxEnvironmentSteps));
                chaserAgentGroup.AddGroupReward(-1.0f);
            }
            chaserAgentGroup.EndGroupEpisode();
            runnerAgentGroup.EndGroupEpisode();
        }
        ConfigureEnv(false);
        
    }

    // Return a random position inside a given spawn volume
    // The position is constrained according to collider bounds
    public Vector3 GetRandomSpawnPosition(GameObject region, Bounds regionBounds, Bounds objectBounds)
    {
        // Random x position in the spawn volume
        var positionX = Random.Range(
            -(regionBounds.extents.x - objectBounds.extents.x),
            regionBounds.extents.x - objectBounds.extents.x 
        );
        float positionZ;

        // Random z position in the spawn volume
        if (Random.value < 0.75)
        {
            positionZ = Random.Range(
                -(regionBounds.extents.z - objectBounds.extents.z), 
                regionBounds.extents.z - objectBounds.extents.z 
            );
        }
        // z position is fixed so that object is completely
        // moved to the left or right
        else if (Random.value < 0.5)
        {
            positionZ = (regionBounds.extents.z - objectBounds.extents.z);
        }
        else 
        {
            positionZ = -(regionBounds.extents.z - objectBounds.extents.z);
        }
            
        return new Vector3(positionX, 0f, positionZ);
    }

    // FixedUpdate is called once per physics simulation step
    void FixedUpdate()
    {
        // Reset environment after a fixed amound of steps.
        resetTimer++;
        if (resetTimer >= MaxEnvironmentSteps)
        {
            if (multiAgentRL)
            {
                chaserAgentGroup.GroupEpisodeInterrupted();
                runnerAgentGroup.GroupEpisodeInterrupted();
            }
            ConfigureEnv(false);
        }
    }
}
