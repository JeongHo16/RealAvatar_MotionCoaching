using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Reference
{
    public int a;
}

public class ValueRefTest : MonoBehaviour
{
    private Reference refTest;

    private int valueTest;

    public void ValueTest()
    {
        SetValue();
        Debug.Log("First Value Test: <size=15>" + valueTest + "</size>");

        ChangeValue(valueTest);
        Debug.Log("Second Value Test: <size=15>" + valueTest + "</size>");
    }

    private void SetValue()
    {
        valueTest = 3;
    }

    private void ChangeValue(int num)
    {
        num = 0;
    }

    public void ReferenceTest()
    {
        SetReference();
        Debug.Log("First Reference Test: <size=15>" + refTest.a + "</size>");

        ChangeReference(refTest);
        Debug.Log("Second Reference Test: <size=15>" + refTest.a + "</size>");
    }

    private void SetReference()
    {
        refTest = new Reference();
        refTest.a = 3;
    }

    private void ChangeReference(Reference test)
    {
        test.a = 0;
    }
}


