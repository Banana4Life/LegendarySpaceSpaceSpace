﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public GameObject mineObject;
    public GameObject baseObject;

    public GameObject lastTarget;

    public bool restart;

    public bool active = false;

    // Update is called once per frame
    void Update()
    {    
        var agent = GetComponent<NavMeshAgent>();
        //var dist = Mathf.CeilToInt(lastTarget.GetComponent<Collider>().bounds.size.y / 2); 
        if (restart)
        {
            flipTarget(agent);
            restart = false;
        }
        else if (DidAgentReachDestination(agent)) //Arrived.
        {
            flipTarget(agent);
        }
    }
    
    public static bool DidAgentReachDestination(NavMeshAgent agent)
    {
        var distance = Vector3.Distance(agent.gameObject.transform.position, agent.destination);
        return distance <= agent.stoppingDistance;
    }

    private void flipTarget(NavMeshAgent agent)
    {
        lastTarget = mineObject == lastTarget ? baseObject : mineObject;
        agent.ResetPath();
        agent.SetDestination(lastTarget.transform.position);
    }

    private void OnCollisionEnter(Collision other)
    {
        var rocket = other.gameObject.GetComponent<RocketController>();
        if (rocket)
        {
            Destroy(gameObject);
        }
    }
}