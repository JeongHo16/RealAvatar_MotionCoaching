using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCubeTest : MonoBehaviour
{
    public float moveSpeed;

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(transform.gameObject);
    }
}
