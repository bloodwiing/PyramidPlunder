using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageScript : MonoBehaviour
{
    public GameObject other;  // Debug
    private PassageScript otherSide;
    private Vector2 spawnLoc;

    // Start is called before the first frame update
    void Start()
    {
        spawnLoc = transform.GetChild(0).position;  // EMPTY: Spawn
        otherSide = other.GetComponent<PassageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Teleport(Transform player)
    {
        player.position = otherSide.spawnLoc;
    }
}
