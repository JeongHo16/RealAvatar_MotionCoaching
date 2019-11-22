using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTest : MonoBehaviour
{
    Vector3 test = new Vector3(30f, 30f, 300f);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("<color=red><size=20>" + test + "</size></color>");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
