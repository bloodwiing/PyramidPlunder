using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ModuleObject : MonoBehaviour
{
    private string passageTag = "Passage";
    public List<PassageScript>[] passages = new List<PassageScript>[4];
    public int totalPassages;
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

    public void ClonePassages()
    {
        for (int x = 0; x < 4; x++)
            passages[x] = new List<PassageScript>();

        var prevChildren = transform.Cast<Transform>().Where(child => child.gameObject.CompareTag(passageTag)).ToArray();
        foreach (var passage in prevChildren)
        {
            var newPass = Instantiate(passage.gameObject, transform);
            passages[Mathf.RoundToInt(passage.eulerAngles.z / 90)]
                .Add(newPass.GetComponent<PassageScript>());
            lastPassage = newPass.GetComponent<PassageScript>();
            Destroy(passage.gameObject);
        }
        totalPassages = prevChildren.Length;
    }

    public ModuleObject Reload(ModuleObject data = null)
    {
        if (data)
        {
            passages = Array.ConvertAll(data.passages, e => new List<PassageScript>(e));
            totalPassages = data.totalPassages;
            var choices = passages.SelectMany(x => x).ToArray();
            lastPassage = choices[UnityEngine.Random.Range(0, choices.Length)];
        }
        else
        {
            passages = new List<PassageScript>[4];
            totalPassages = 0;
            lastPassage = null;
        }
        return this;
    }
}
