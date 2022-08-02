using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IInteractable
{
    private NavMeshAgent agent;
    private PlayerInteract playerInteractScript;
    
    public GameObject[] waypoints;

    private int waypointInt;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(waypoints[waypointInt].transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                waypointInt++;
                if (waypointInt > waypoints.Length - 1)
                {
                    waypointInt = 0;
                }
            }
        }
    }

    public void Interact(PlayerInteract playerInteract)
    {
        playerInteractScript = playerInteract;
        agent.isStopped = true;
        playerInteract.isHolding = true;
    }

    public void EndInteract()
    {        
        agent.isStopped = false;
        playerInteractScript.isHolding = false;
    }
}
