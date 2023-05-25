/*using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentManager : Singleton<AgentManager>
{

    *//*[SerializeField]
    TagPlayer agentPrefab; // changed from Agent type*//*

    [SerializeField]
    int agentSpawnCount;

    //public List<TagPlayer> agents = new List<TagPlayer>(); // changed from Agent type

    private float cameraHalfWidth, cameraHalfHeight;


    // gameplay logic
    int itPlayerIndex = 0;


    protected AgentManager() { }

    // Start is called before the first frame update
    void Start()
    {
        // camera
        cameraHalfHeight = Camera.main.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;

        // agent spawning - EDIT
        for (int i = 0; i < agentSpawnCount; i++)
        {
            agents.Add(Instantiate<TagPlayer>(agentPrefab)); // changed from Agent type
        }

        agents[itPlayerIndex].currentState = TagState.Counting;

    }

    // Update is called once per frame
    void Update()
    {
        if (agents[itPlayerIndex].currentState == TagState.It)
        {

        }

        *//*Vector3 closestAgentPos = Vector3.positiveInfinity;

        for (int i = 0; i < agents.Count; i++)
        {
            if (i == itPlayerIndex) // dont seek yourself
            {
                continue;
            }

            float dist = Vector3.Distance(agents[itPlayerIndex].transform.position, agents[i].transform.position);

            if (dist < agents[itPlayerIndex].Radius * 2f)
            {
                agents[i].currentState = TagState.Counting;
                agents[itPlayerIndex].currentState = TagState.NotIt;
                itPlayerIndex = i;
            }
        }*//*
    }

    public Vector3 ItPLayerPosition()
    {
        return agents[itPlayerIndex].transform.position;
    }

    public Vector3 ClosestNotItPlayer()
    {
        float closestDistance = Mathf.Infinity;
        Vector3 closestAgentPos = Vector3.positiveInfinity;

        for (int i = 0; i < agents.Count; i++)
        {
            if (i == itPlayerIndex) // dont seek yourself
            {
                continue;
            }

            float dist = Vector3.Distance(agents[itPlayerIndex].transform.position, agents[i].transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestAgentPos = agents[i].transform.position;
            }
        }

        return closestAgentPos;
    }

}
*/