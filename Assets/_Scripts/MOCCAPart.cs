using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOCCAPart : MonoBehaviour //충돌이 일어나는 오브젝트들의 컴포넌트
{
    public bool collision = false;

    private void OnTriggerEnter(Collider other)
    {
        collision = true;
    }

    private void OnTriggerStay(Collider other)
    {
        collision = true;
    }

    private void OnTriggerExit(Collider other)
    {
        collision = false;
    }
}
