using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IInteractable
{
    private NavMeshAgent agent;
    private PlayerInteract playerInteractScript;

    private bool lookAtPlayer;
    private GameObject player;
    
    public GameObject[] waypoints;

    private int waypointInt;

    public GameObject head;

    private Coroutine smoothMove;

    [Header("Needs")]
    public float Food;
    public float Pats;
    public float Play;
    public float Sleep;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (agent.isActiveAndEnabled)
        // {
        //     agent.SetDestination(waypoints[waypointInt].transform.position);
        //     if (agent.remainingDistance <= agent.stoppingDistance)
        //     {
        //         waypointInt++;
        //         if (waypointInt > waypoints.Length - 1)
        //         {
        //             waypointInt = 0;
        //         }
        //     }
        // }

        if (lookAtPlayer)
        {
            LookAtPlayer();
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
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            lookAtPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lookAtPlayer = false;
        }
    }
    
    private void LookAtPlayer()
    {
        Vector3 lookAt = player.transform.position;
        lookAt.y = transform.position.y;
        
        Quaternion currentRot = head.transform.rotation;
        Quaternion newRot = Quaternion.LookRotation(lookAt - head.transform.position,
            head.transform.TransformDirection(Vector3.up));

        newRot.z = 0;
        
        head.transform.rotation = newRot;

    }

    IEnumerator LookAtSmoothly(Transform objectToMove, Vector3 worldPosition, float duration)
    {


        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }
    }
}
