using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpFloat : MonoBehaviour
{
    public 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float targetTime = 0.2f;
    float elapsedTime = 0f;
    bool timerStart = false;

    // Update is called once per frame
    void Update()
    {
        if (timerStart)
        {
            if (elapsedTime < targetTime)
            {
                elapsedTime += Time.deltaTime;
                float number = Mathf.Lerp(0f, 10f, elapsedTime / targetTime);
                Debug.Log(number);
            }

            else
            {
                elapsedTime = 0f;
                timerStart = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //float number = Mathf.Lerp(3f, 10f, 0.0f);
            //Debug.Log(number);

            timerStart = true;
        }
    }
}
