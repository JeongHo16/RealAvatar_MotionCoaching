using System.Collections.Generic;
using UnityEngine;

public class DictionaryTest : MonoBehaviour
{
    int key = 32;
    int value = 101;

    Dictionary<int, Test> test_Dic = new Dictionary<int, Test>();

    void Start()
    {
        Test test = new Test();
        test.a = value;
        test_Dic.Add(key, test);

        Debug.Log("출력 테스트 1 : key로 접근하여 값 출력 a = " + test_Dic[32].a);

        foreach (KeyValuePair<int, Test> temp in test_Dic)
        {
            Debug.Log("출력 테스트 2 : for문을 사용하여 값 출력 key 값 " + temp.Key + ", a = " + temp.Value.a);
        }
    }
}

public class Test
{
    public int a;
}