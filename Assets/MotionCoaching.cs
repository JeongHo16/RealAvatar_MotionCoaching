using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCoaching : MonoBehaviour
{
    public REEL.PoseAnimation.RobotTransformController robot;
    float[][] motionDataFile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
        if (SpeechRecognition.receive == true)
            playResultGesture();
        
    }
    public void playResultGesture()
    {
        
        string keys = SpeechRecognition.output.result;
        Debug.Log(keys);
        motionDataFile = robot.keyMotionTable(keys);
        
        if (motionDataFile != null)
        {
            Debug.Log("in if");
            StartCoroutine(robot.GestureProcess(motionDataFile));
            SpeechRecognition.receive = false;
        }


    }
}
