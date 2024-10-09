using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Map<string, int> stats;
    [SerializeField] Level level;
    void Start()
    {
        stats.Init();
        level.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log(stats["test"]);
        }
    }

    void Print(int test)
    {
        Debug.Log("level : " + test);
    }
}
