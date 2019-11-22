using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    public GameObject targetObject;

    private MOCCAPart moccaPartScript;
    private bool collision;

    private void Start()
    {
        targetObject.AddComponent<MOCCAPart>();
        moccaPartScript = targetObject.GetComponent<MOCCAPart>();
    }

    private void OnTriggerEnter(Collider other)
    {
        collision = moccaPartScript.collision;
    }
}
