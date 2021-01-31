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

    // Easing transition
    private bool moving = false;
    private float time;
    private float transitionDuration = .5f;

    private Vector3 origin;
    private Vector3 playerLoc;
    private Vector3 move;

    private Transform player;

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
            Vector3 oLoc = otherSide.spawnLoc;
            Vector3 sLoc = spawnLoc;

            other.transform.position = transform.position - otherSide.transform.position;

            if (Mathf.RoundToInt(otherSide.transform.eulerAngles.z / 90) % 2 == 0)
                player.position = oLoc + Scale(player.position - sLoc, new Vector3(-.5f, 1)) + other.transform.position;
            else
                player.position = oLoc + Scale(player.position - sLoc, new Vector3(1, -.5f)) + other.transform.position;

            origin = other.transform.position;
            move = -origin;
            playerLoc = player.position;
            time = 0;

            player.GetComponent<PlayerController>().UpdateFreeze(true);
            this.player = player;

            moving = true;

            self.layer = 7;
            other.layer = 0;

            other.SetActive(true);
        }
    }

    Vector3 Scale(Vector3 a, Vector3 b)
    {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;
        return a;
    }

    Vector3 EaseInOut(float t, float d, Vector3 s, Vector3 m)
    {
        if (t >= d) return s + m;
        t /= d / 2;
        if (t < 1)
            return m / 2 * t * t + s;
        t--;
        return -m / 2 * (t * (t - 2) - 1) + s;
    }

    float EaseInOutFloat(float t, float d, float s, float m)
    {
        t /= d / 2;
        if (t < 1)
            return m / 2 * t * t + s;
        t--;
        return -m / 2 * (t * (t - 2) - 1) + s;
    }

    void UpdateOpacity(GameObject parent, float opacity)
    {
        SpriteRenderer[] children = parent.GetComponentsInChildren<SpriteRenderer>();
        foreach (var child in children)
        {
            var color = child.color;
            color.a = opacity;
            child.color = color;
        }
    }

    void Update()
    {
        if (moving)
        {
            if (Vector3.Distance(other.transform.position, Vector3.zero) > 0.001)
            {
                time += Time.deltaTime;

                other.transform.position = EaseInOut(time, transitionDuration, origin, move);
                UpdateOpacity(other, EaseInOutFloat(time, transitionDuration, 0, 1));

                self.transform.position = EaseInOut(time, transitionDuration, Vector3.zero, move);
                UpdateOpacity(self, EaseInOutFloat(time, transitionDuration, 1, -1));

                player.position = EaseInOut(time, transitionDuration, playerLoc, move);
            }
            else
            {
                other.transform.position = Vector3.zero;
                player.position = playerLoc + move;

                self.SetActive(false);
                self.transform.position = Vector3.zero;

                player.GetComponent<PlayerController>().UpdateFreeze(false);
                moving = false;
            }
        }
    }
}
