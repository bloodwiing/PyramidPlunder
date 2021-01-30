using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModuleObject : MonoBehaviour
{
    private string passageTag = "Passage";
    [HideInInspector]
    public List<PassageScript>[] passages = new List<PassageScript>[4];
    [HideInInspector]
    public int totalPassages;
    [HideInInspector]
    public PassageScript lastPassage;

    public void SolvePassages()
    {
        for (int x = 0; x < 4; x++)
            passages[x] = new List<PassageScript>();

        var children = transform.Cast<Transform>().Where(child => child.gameObject.CompareTag(passageTag)).ToArray();
        foreach (var child in children)
        {
            passages[Mathf.RoundToInt(child.eulerAngles.z / 90)].Add(child.GetComponent<PassageScript>());
            lastPassage = child.GetComponent<PassageScript>();
        }
        totalPassages = children.Length;
    }

    public void Reload(ModuleObject data)
    {
        passages = data.passages;
        totalPassages = data.totalPassages;
        lastPassage = data.lastPassage;
    }    

    //void Start()
    //{
    //    for (int x = 0; x < 4; x++)
    //        for (int y = 0; y < passages[x].Count; y++)
    //            Debug.Log("X: " + x + " | Y: " + y + " | P: " + passages[x][y].ToString());
    //}
}
