using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModuleObject : MonoBehaviour
{
    private string passageTag = "Passage";
    private List<PassageScript>[] passages = new List<PassageScript>[4];

    void Start()
    {
        for (int x = 0; x < 4; x++)
            passages[x] = new List<PassageScript>();

        var children = transform.Cast<Transform>().Where(child => child.gameObject.tag == passageTag).ToArray();
        foreach (var child in children)
            passages[Mathf.RoundToInt(child.eulerAngles.z / 90)].Add(child.GetComponent<PassageScript>());

        /*
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < passages[x].Count; y++)
                Debug.Log("X: " + x + " | Y: " + y + " | P: " + passages[x][y].ToString());
        */
    }

    void Update()
    {
        
    }
}
