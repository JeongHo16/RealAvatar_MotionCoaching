using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotionCoaching : MonoBehaviour
{
    public REEL.PoseAnimation.RobotTransformController robot;
    public MoccaFaceAnniTest face;
    public Kinematics kinematics;

    public PopUpMessege popUpMessege;
    public Text resultText;

    public static float[][] motionDataFile;

    [SerializeField]
    public CDJointOrientationSetter cdJointOrientationSetter;
    private CDJoint[] cdJoints;

    float[][] tempMotionData;
    string[] splitOutput;

    string[] firstSplit;
    string[] secondSplit;
    string[] thirdSplit;

    private int highSpeedLimit = 0;

    bool canMove = true;
    public Coroutine coroutine;

    int facesave = 0;

    char hyphen = '-';
    char slash = '/';
    string[] motionOnly = { "머리고개얼굴목", "오른팔", "왼팔", "팔양팔두팔양쪽팔" };


    private void Awake()
    {
        cdJoints = cdJointOrientationSetter.joints;
    }

    private void Update()
    {
        if (SpeechRecognition.receive == true)
            playResultGesture();
        if (StateUpdater.isSpeakingAgain)
        {
            popUpMessege.MessegePopUp("다시 한번 말해주세요");
            StateUpdater.isSpeakingAgain = false;
        }
        if (!StateUpdater.isCanInverse || !canMove)
        {
            popUpMessege.MessegePopUp("더 이상 이동할 수 없어요");
            StateUpdater.isCanInverse = true;
            canMove = true;
        }
    }
    public void playResultGesture()
    {
        face.Clear();
        string keys = SpeechRecognition.output.result;
        resultText.text = keys;

        if (keys == "NOT_FOUND")
        {
            popUpMessege.MessegePopUp("일치하는 동작이 없어요");
        }

        else
        {
            splitOutput = keys.Split(hyphen);
            switch (splitOutput[0])
            {
                case "몸몸통바퀴":
                    popUpMessege.MessegePopUp("일치하는 동작이 없어요");
                    break;

                case "얼굴표정":
                    faceAni();

                    break;

                case "전신":
                    wholeBody(keys);
                    break;

                case "DYN":
                    if (splitOutput[2] == "고개" || splitOutput[2] == "왼쪽" || splitOutput[2] == "오른쪽" || splitOutput[2] == "머리")
                    {
                        MovingNeck();
                    }
                    else
                    {
                        kinematics.ForwardKinematics();
                        kinematics.InverseKinematics(splitOutput[2], splitOutput[1]);
                    }
                    break;

                case "ADV":
                    StateUpdater.isCallingADV = true;
                    switchFaceAni(facesave);
                    if (splitOutput[1] == "속도강")
                    {
                        if (++highSpeedLimit == 1)
                            MotionSpeedUp();
                        else
                        {
                            StartCoroutine(robot.GestureProcess(motionDataFile));
                            popUpMessege.MessegePopUp("더 빠르게 할 수 없어요");
                        }

                    }
                    else if (splitOutput[1] == "속도약")
                        MotionSpeedDown();
                    else if (splitOutput[1] == "각도강")
                        MotionExpansion();
                    else if (splitOutput[1] == "각도약")
                        MotionReduction();
                    break;

                default:
                    for (int i = 0; i < motionOnly.Length; i++)
                    {
                        if (splitOutput[0] == motionOnly[i])
                        {
                            facesave = 0;
                            motionDataFile = CopyFloatArray(keys);

                            if (motionDataFile != null)
                            {
                                StartCoroutine(robot.GestureProcess(motionDataFile));
                                break;
                            }
                        }
                    }
                    break;
            }
            SpeechRecognition.receive = false;
        }
    }

    float[][] CopyFloatArray(string key)
    {
        float[][] value = robot.keyMotionTable(key);
        float[][] newArray = new float[value.Length][];
        for(int i = 0; i < value.Length; i++)
        {
            float[] val = new float[value[i].Length];
            for(int j = 0; j < value[i].Length; j++)
            {
                val[j] = value[i][j];
            }
            newArray[i] = val;
        }
        return newArray;
    }

    public void faceAni()
    {
        if (splitOutput[1] == "무표정")
            facesave = 0;
        else if (splitOutput[1] == "기쁨")
            facesave = 5;
        else if (splitOutput[1] == "화남" || splitOutput[1] == "혐오싫음")
            facesave = 2;
        else if (splitOutput[1] == "두려움")
            facesave = 3;
        else if (splitOutput[1] == "슬픔")
            facesave = 4;
        //else if (splitOutput[1] == "미소")  // 사전에 smile 표정만 하는 거 없음.
        //    facesave = 1;
        else if (splitOutput[1] == "놀람")
            facesave = 6;
        //if (splitOutput[1] == "")  // 사전에 speak 없음.
        //    facesave = 7;
        else if (splitOutput[1] == "왼쪽윙크")
            facesave = 8;
        else if (splitOutput[1] == "오른쪽윙크")
            facesave = 9;
        //else if (splitOutput[1] == "")
        //    facesave = 10;
        //else if (splitOutput[1] == "")
        //    facesave = 11;
        //else if (splitOutput[1] == "")
        //    facesave = 12;
        //else if (splitOutput[1] == "")
        //    facesave = 13;

        switchFaceAni(facesave);

    }


    void wholeBody(string key)
    {
        if (splitOutput[1] == "(규모 등에)놀람")
            facesave = 6;
        else if (splitOutput[1] == "(소리 등에)놀람")
            facesave = 6;
        else if (splitOutput[1] == "고개숙여인사")
            facesave = 5;
        else if (splitOutput[1] == "긍정")
            facesave = 5;
        else if (splitOutput[1] == "부정")
            facesave = 4;
        else if (splitOutput[1] == "슬픔")
            facesave = 4;
        else if (splitOutput[1] == "악수")
            facesave = 5;
        else if (splitOutput[1] == "졸림")
            facesave = 11;
        else if (splitOutput[1] == "집중")
            facesave = 0;
        else if (splitOutput[1] == "칭찬")
            facesave = 5;
        else if (splitOutput[1] == "기본자세")
            facesave = 0;
        else if (splitOutput[1] == "기쁨")
            facesave = 5;
        else if (splitOutput[1] == "두려움")
            facesave = 3;
        else if (splitOutput[1] == "부끄러움")
            facesave = 11;
        else if (splitOutput[1] == "제안")
            facesave = 0;
        else if (splitOutput[1] == "팔로인사")
            facesave = 9;
        else if (splitOutput[1] == "혐오싫음")
            facesave = 2;
        else if (splitOutput[1] == "화남")
            facesave = 2;
        else if (splitOutput[1] == "회피")
            facesave = 12;
        else if (splitOutput[1] == "허그")
            facesave = 5;
        else if (splitOutput[1] == "만세")
            facesave = 5;
        else if (splitOutput[1] == "생각")
            facesave = 10;

        if (splitOutput[1] == "정지")
        {
            facesave = 0;
            switchFaceAni(facesave);
            StopCoroutine(coroutine);
            face.Clear();
        }

        else
        {
            switchFaceAni(facesave);
            //motionDataFile = robot.keyMotionTable(key);
            motionDataFile=CopyFloatArray(key);
            coroutine = StartCoroutine(robot.GestureProcess(motionDataFile));
        }
        
    }

    void MotionSpeedUp()
    {
        for (int i = 0; i < tempMotionData.Length; i++)
        {
            tempMotionData[i][0] = tempMotionData[i][0] * 0.3f;
        }

        StartCoroutine(robot.GestureProcess(tempMotionData));

    }

    void MotionSpeedDown()
    {
        for (int i = 0; i < tempMotionData.Length; i++)
        {
            tempMotionData[i][0] = tempMotionData[i][0] * 2.5f;
        }

        StartCoroutine(robot.GestureProcess(tempMotionData));
    }

    void MotionExpansion()
    {

        for (int i = 0; i < tempMotionData.Length; i++)
        {
            for (int j = 1; j < tempMotionData[i].Length; j++)
            {
                tempMotionData[i][j] = tempMotionData[i][j] * 1.3f;
                limitMinMax(i, j);
            }

        }
        coroutine = StartCoroutine(robot.GestureProcess(tempMotionData));
    }

    void MotionReduction()
    {

        for (int i = 0; i < tempMotionData.Length; i++)
        {
            for (int j = 1; j < tempMotionData[i].Length; j++)
            {
                tempMotionData[i][j] = tempMotionData[i][j] * 0.7f;
                limitMinMax(i, j);

            }

        }
        coroutine = StartCoroutine(robot.GestureProcess(tempMotionData));
    }
    void limitMinMax(int i, int j)
    {
        switch (j)
        {
            case 0:
            case 3:
                Mathf.Clamp(tempMotionData[i][j], -90.0f, 90.0f);
                break;
            case 1:
                Mathf.Clamp(tempMotionData[i][j], -20.0f, 90.0f);
                break;
            case 2:
                Mathf.Clamp(tempMotionData[i][j], 0f, 90.0f);
                break;
            case 4:
                Mathf.Clamp(tempMotionData[i][j], -90.0f, 20.0f);
                break;
            case 5:
                Mathf.Clamp(tempMotionData[i][j], -90.0f, 0f);
                break;
            case 6:
                Mathf.Clamp(tempMotionData[i][j], -40.0f, 40.0f);
                break;
            case 7:
                Mathf.Clamp(tempMotionData[i][j], -30.0f, 15.0f);
                break;
        }
    }


    public void StopMotion()
    {
        StopCoroutine(coroutine);
        face.Clear();
    }

    public void switchFaceAni(int i)
    {
        switch (i)
        {
            case 0:
                face.normal = true;
                break;
            case 1:
                face.happy = true;
                break;
            case 2:
                face.angry = true;
                break;
            case 3:
                face.fear = true;
                break;
            case 4:
                face.sad = true;
                break;
            case 5:
                face.smile = true;
                break;
            case 6:
                face.surprised = true;
                break;
            case 7:
                face.speak = true;
                break;
            case 8:
                face.winkleft = true;
                break;
            case 9:
                face.winkright = true;
                break;
            case 10:
                face.gazeup = true;
                break;
            case 11:
                face.gazedown = true;
                break;
            case 12:
                face.gazeleft = true;
                break;
            case 13:
                face.gazeright = true;
                break;
        }
    }

    void MovingNeck()
    {
        float angle_y = cdJoints[6].GetCurrentAngle;
        float angle_x = cdJoints[7].GetCurrentAngle;
        float angle = 0;
        int index = 0;
        if (splitOutput[1] == "상")
        {
            index = 7;
            angle = angle_x - 20f;
        }
        else if (splitOutput[1] == "하")
        {
            index = 7;
            angle = angle_x + 5f;
        }
        else if (splitOutput[1] == "좌")
        {
            index = 6;
            angle = angle_y - 20f;
        }
        else if (splitOutput[1] == "우")
        {
            index = 6;
            angle = angle_y + 20f;
        }

        if (angle >= 180)
            angle -= 360;
        CheckNeckAngle(angle, index);

        if (canMove)
            StartCoroutine(cdJoints[index].SetQuatLerp(angle, 0.3f));
    }

    void CheckNeckAngle(float angle, int index)
    {
        switch (index)
        {
            case 6:
                if (angle > 50 || angle < -50)
                    canMove = false;
                break;
            case 7:
                if (angle > 13 || angle < -60)
                    canMove = false;
                break;
        }
    }

    

}



