using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class ModuleLoader : MonoBehaviour
{
    [Header("Map generation")]

    [Tooltip("Generates a new seed on play (DOESN'T SAVE WHEN STOPPED).")]
    public bool randomSeed;
    [Tooltip("A number, specifying how Random should randomize.")]
    public int seed;

    [Tooltip("The limit of how far the generator can traverse\n(max amount of Passages before hitting a dead end from the spawn).")]
    [Range(1, 20)]
    public int maxDepth;
    private int prevMaxDepth;

    [Tooltip("The minimum amount of modules.")]
    [Min(2)]
    public int minModules;

    [Header("Other options")]
    [Tooltip("A list of all modules, which can be used for map generation.\nPlease don't forget to add modules here.\nTIP: To add multiple modules, lock the Inspector.")]
    public GameObject[] possibleModules;

    private List<GameObject> map = new List<GameObject>();

    void Start()
    {
        prevMaxDepth = maxDepth;
        if (randomSeed) seed = Random.Range(int.MinValue, int.MaxValue);
        if (!Application.isPlaying) return;

        Random.InitState(seed);

        for (int i = 0; i < possibleModules.Length; i++)
        {
            possibleModules[i] = Instantiate(possibleModules[i]);
            possibleModules[i].SetActive(false);
            possibleModules[i].AddComponent<ModuleObject>().SolvePassages();
        }

        int startingIndex = Random.Range(0, possibleModules.Length);
        MapBranch(startingIndex, 1, minModules-1).SetActive(true);

        foreach (var module in map)
            foreach (var passage in module.transform.Cast<Transform>().Where(c => c.CompareTag("Passage")))
                passage.GetComponent<PassageScript>().Link(map);
    }

    int InvertPassage(int number)
    {
        return number >= 2 ? number - 2 : number + 2;
    }

    GameObject MapBranch(int moduleID, int depth, int reqMod, int skipR = -1)
    {
        var module = Instantiate(possibleModules[moduleID], transform);
        module.GetComponent<ModuleObject>().Reload().ClonePassages();

        int moduleIndex = map.Count;
        map.Add(module);

        module.SetActive(false);
        var moduleScript = module.GetComponent<ModuleObject>();

        int maxPossible = 3 + (maxDepth - depth) * 9;
        if (maxPossible < 0)
            maxPossible = 1;

        if (depth > maxDepth)
        {
            for (int r = 0; r < 4; r++)
                foreach (var passage in moduleScript.passages[r])
                    passage.Reload();
            return module;
        }

        // For some reason combining these two loops makes Unity completely freeze.

        for (int r = 0; r < 4; r++)
        {
            var passages = new List<PassageScript>(moduleScript.passages[r]);
            for (int i = 0; i < passages.Count; i++)
            {
                var passage = passages[i];
                passage.Reload();
                if (r == skipR)
                {
                    skipR = -1;
                    continue;
                }
                if (depth == maxDepth)
                {
                    GameObject[] possibilities = possibleModules.Where(
                        m => m.GetComponent<ModuleObject>().passages[InvertPassage(r)].Count > 0
                        && m.GetComponent<ModuleObject>().totalPassages == 1
                        ).ToArray();

                    int modIndex = map.Count;
                    var mod = MapBranch(System.Array.IndexOf(possibleModules, possibilities[0]), depth + 1, 0, InvertPassage(r));

                    passage.Prepare(moduleIndex, modIndex, map[modIndex].GetComponent<ModuleObject>().lastPassage);
                }
                else
                {
                    int margin = maxPossible - reqMod;
                    if (margin < 0)
                        margin = 0;
                    if (margin > reqMod)
                        margin = reqMod;

                    int rng = 0;
                    if (margin > 1 && passages.Count - 1 > i)
                        rng = Random.Range(-margin / 4, margin / 4);
                    int split = reqMod / (passages.Count - i) + rng;
                    reqMod -= split;
                    if (split < 0)
                        split = 0;

                    GameObject[] possibilities = possibleModules.Where(
                        m => m.GetComponent<ModuleObject>().passages[InvertPassage(r)].Count > 0
                        && m.GetComponent<ModuleObject>().totalPassages >= split - (maxDepth - depth - 1) * 9
                        && ((split > 1 && m.GetComponent<ModuleObject>().totalPassages > 1) || split <= 1)  // This is the magic  SEED: -163430234
                        ).ToArray();

                    int modIndex = map.Count;
                    int index = Random.Range(0, possibilities.Length);
                    var mod = MapBranch(System.Array.IndexOf(possibleModules, possibilities[index]), depth + 1, --split, InvertPassage(r));

                    var passe = mod.GetComponent<ModuleObject>().passages[InvertPassage(r)].Where(p => !p.connected).ToArray()[0];
                    int passID = mod.GetComponent<ModuleObject>().passages[InvertPassage(r)].IndexOf(passe);

                    passage.Prepare(moduleIndex, modIndex, map[modIndex].GetComponent<ModuleObject>().passages[InvertPassage(r)][passID]);
                }
            }
        }

        return module;
    }

    private void OnValidate()
    {
        if (minModules > 3 + (maxDepth - 1) * 9)
            if (maxDepth != prevMaxDepth)
                minModules = 3 + (maxDepth - 1) * 9;
            else
                maxDepth = Mathf.CeilToInt((minModules - 3) / 9.0f) + 1;
        prevMaxDepth = maxDepth;
    }
}
