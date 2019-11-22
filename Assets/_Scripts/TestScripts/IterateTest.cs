using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterateTest : MonoBehaviour
{
    public static bool val = false;

    private void Start()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    Debug.Log("Current val : " + val.ToString());
        //    if (i == 4)
        //    {
        //        val = true;
        //        Debug.Log("Change the val : " + val.ToString());
        //        break;
        //    }
        //}
        //Debug.Log("Finish!");

        Changebool();
        Debug.Log(val.ToString());
    }

    //private void Update()
    //{
    //    Debug.Log(val.ToString());
    //}

    private void Changebool()
    {
        val = true;
    }
}
