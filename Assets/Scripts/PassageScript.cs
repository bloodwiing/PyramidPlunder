using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageScript : MonoBehaviour
{
    private GameObject other = null;
    private GameObject self = null;
    private int otherIndex;
    private int selfIndex;
    [HideInInspector]
    public bool connected = false;
    public PassageScript otherSide = null;
    public Vector2 spawnLoc;

    public void Prepare(int sIndex, int oIndex, PassageScript p)
    {
        otherIndex = oIndex;
        selfIndex = sIndex;
        otherSide = p;

        otherSide.otherIndex = sIndex;
        otherSide.selfIndex = oIndex;
        otherSide.otherSide = this;

        connected = true;
        otherSide.connected = true;
    }

    public void Link(List<GameObject> map)
    {
        other = map[otherIndex];
        self = map[selfIndex];
    }

    public void Reload()
    {
        spawnLoc = transform.GetChild(0).position;  // EMPTY: Spawn
    }

    public void Teleport(Transform player)
    {
        if (otherSide)
        { 
            player.position = otherSide.spawnLoc;
            other.SetActive(true);
            self.SetActive(false);
        }
    }
}
