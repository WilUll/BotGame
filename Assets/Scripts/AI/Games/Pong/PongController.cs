using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PongController : MonoBehaviour
{
    public PongPaddleMovement playerScript, opponentScript;
    public GameObject ballPrefab;
    private GameObject ballGO;
    public Transform spawnPos;
    
    // Start is called before the first frame update
    void Start()
    {
        NewBall();
    }

    private void NewBall()
    {
        ballGO = Instantiate(ballPrefab, spawnPos.position, quaternion.identity, transform);
        playerScript.Ball = ballGO;
        opponentScript.Ball = ballGO;
    }

    public void ResetBall()
    {
        Destroy(ballGO);
        NewBall();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
