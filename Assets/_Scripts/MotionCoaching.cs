﻿using System.Collections;
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
    public static bool repeat = false;

    [SerializeField]
    public CDJointOrientationSetter cdJointOrientationSetter;
    private CDJoint[] cdJoints;

    float[][] tempMotionData;
    string[][] splitOutput;
    float[][] tempDataFile = new float[1][];
    float time;
    string whole;
    bool coroutine_running = false;
    bool canADV = true;
    bool canMove = true;
    bool onlyFace = false;
    bool isStop = false;
    public Coroutine coroutine;
    public static bool degBase = false;

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
        {
            if (StateUpdater.isCanDoGesture)
                MakeUpMotionDataFile();
            else
            {
                SpeechRecognition.receive = false;
                popUpMessege.MessegePopUp("동작이 끝난 후 다시 명령해주세요.");
            }
                
        }
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
    public void MakeUpMotionDataFile()
    {
        SpeechRecognition.receive = false;
        face.Clear();
        string keys = SpeechRecognition.output.result;
        resultText.text = keys;

        if (keys == "NOT_FOUND")
        {
            popUpMessege.MessegePopUp("일치하는 동작이 없어요");
            StateUpdater.isCanDoGesture = true;
        }

        else
        {
            splitOutput = SplitKeys(keys);
            if (keys.Contains("DYN") && splitOutput[0].Length == 3)
            {
                if (splitOutput[0][2] == "머리")
                    tempDataFile[0] = MovingNeck();
                else
                {
                    kinematics.ForwardKinematics();
                    tempDataFile[0] = kinematics.InverseKinematics(splitOutput[0][2], splitOutput[0][1]);

                }
                motionDataFile = tempDataFile;
                for (int i = 2; i < motionDataFile[0].Length; i++)
                    motionDataFile[0][i] *= -1f;


            }
            else if (keys.Contains("DEG"))
            {
                facesave = 0;
                degBase = true;
                string degree = splitOutput[splitOutput.Length - 1][1];
                string parts = splitOutput[0][0];

                switch (degree)
                {
                    case "0도":
                    case "360도":
                    case "12시":
                        degree = "0도";
                        break;

                    case "30도":
                    case "1시":
                        degree = "30도";
                        break;

                    case "60도":
                    case "2시":
                        degree = "60도";
                        break;

                    case "90도":
                    case "3시":
                        degree = "90도";
                        break;

                    case "330도":
                    case "11시":
                        degree = "-30도";
                        break;

                    case "300도":
                    case "10시":
                        degree = "-60도";
                        break;

                    case "270도":
                    case "9시":
                        degree = "-90도";
                        break;
                    default:
                        popUpMessege.MessegePopUp("할 수 없는 각도예요");
                        degree = "x";
                        motionDataFile = robot.baseList;
                        break;

                }
                if (parts == "전신")
                {
                    switch (degree)
                    {
                        case "0도":
                        case "30도":
                        case "60도":
                        case "90도":
                            parts = "오른팔";
                            break;

                        case "-30도":
                        case "-60도":
                        case "-90도":
                            parts = "왼팔";
                            break;
                    }
                }

                else if (parts == "얼굴표정")
                    parts = "머리고개얼굴목";
                string ctrldeg = parts + "-DEG-" + degree;
                if (!degree.Equals("x"))
                    motionDataFile = CopyFloatArray(ctrldeg);
            }
            else
            {
                string sendKey = "";
                for (int j = 0; j < splitOutput[0].Length; j++)
                {
                    sendKey += splitOutput[0][j];
                    if (j == splitOutput[0].Length - 1)
                        break;
                    sendKey += "-";
                }

                switch (splitOutput[0][0])
                {
                    case "몸몸통바퀴":
                        isStop = true;
                        popUpMessege.MessegePopUp("일치하는 동작이 없어요");
                        StateUpdater.isCanDoGesture = true;
                        break;

                    case "얼굴표정":
                        onlyFace = true;
                        faceAni();
                        break;

                    case "전신":
                        whole = "전신";
                        wholeBody(sendKey);
                        break;

                    default:
                        for (int i = 0; i < motionOnly.Length; i++)
                        {
                            if (splitOutput[0][0] == motionOnly[i])
                            {
                                if (keys.Contains("ADV") && keys.Contains("전신"))
                                    whole = "전신";
                                else
                                    whole = motionOnly[i];
                                facesave = 0;
                                motionDataFile = CopyFloatArray(sendKey);
                            }
                        }
                        break;
                }
            }


            if (!isStop)
            {
                if (keys.Contains("ADV"))
                {
                    StateUpdater.isCallingADV = true;
                    int speed = 0;
                    int angle = 0;
                    int power = 0;
                    if (keys.Contains("속도"))
                        speed = keys.Contains("속도강") ? 1 : -1;
                    if (keys.Contains("각도"))
                        angle = keys.Contains("각도강") ? 1 : -1;
                    if (keys.Contains("강도"))
                        power = keys.Contains("강도강") ? 1 : -1;

                    if (speed == 0 && angle == 0 && power != 0)
                    {
                        speed = power;
                        power = 0;
                    }


                    int[] parts = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    if (angle != 0)
                    {
                        if (whole == "왼팔")
                        {
                            for (int i = 0; i < 3; i++)
                                parts[i] = 1;
                        }
                        else if (whole == "오른팔")
                            for (int i = 3; i < 6; i++)
                                parts[i] = 1;
                        else if (whole == "머리고개얼굴목")
                            for (int i = 6; i < 8; i++)
                                parts[i] = 1;
                        else if (whole == "팔양팔두팔양쪽팔")
                            for (int i = 0; i < 6; i++)
                                parts[i] = 1;
                        else
                        {
                            for (int i = 0; i < 8; i++)
                                parts[i] = 1;
                        }
                    }

                    if (!((keys.Contains("DYN") || keys.Contains("DEG")) && (angle != 0)))
                    {
                        if (speed == 1)
                            MotionSpeedUp(power);
                        else if (speed == -1)
                            MotionSpeedDown(power);
                        if (angle == 1)
                            MotionExpansion(parts, power);
                        else if (angle == -1)
                            MotionReduction(parts, power);

                    }


                }


                if (canADV && StateUpdater.isCanInverse && canMove)
                {
                    if (keys.Contains("DUR"))
                    {
                        GetDurTime();
                        if (time != 0f)
                        {
                            if (motionDataFile.Length == 1)
                            {
                                StartCoroutine("PlayAndWait");
                            }
                            else
                            {
                                coroutine = StartCoroutine(RepeatMotion());
                                StartCoroutine(CountTime(time));
                            }
                        }
                    }
                    else
                    {
                        if (motionDataFile == null && !onlyFace)
                        {
                            popUpMessege.MessegePopUp("동작을 실행할 수 없어요");
                            StateUpdater.isCanDoGesture = true;
                        }
                        else
                        {
                            switchFaceAni(facesave);
                            if (!onlyFace)
                            {
                                coroutine = StartCoroutine(robot.GestureProcess(motionDataFile));
                            }
                            onlyFace = false;
                        }


                    }
                }

                else
                {
                    popUpMessege.MessegePopUp("더 이상 빨라질 수 없어요");
                    canADV = true;

                }
            }
            else
            {
                isStop = false;
            }
        }
        


    }

    IEnumerator PlayAndWait()
    {
        switchFaceAni(facesave);
        yield return StartCoroutine(robot.GestureProcess(motionDataFile));
        StartCoroutine(CountTime(time));
    }

    float[][] CopyFloatArray(string key)
    {
        float[][] value = robot.keyMotionTable(key);
        float[][] newArray = new float[value.Length][];
        for (int i = 0; i < value.Length; i++)
        {
            float[] val = new float[value[i].Length];
            for (int j = 0; j < value[i].Length; j++)
            {
                val[j] = value[i][j];
            }
            newArray[i] = val;
        }
        return newArray;
    }

    public void faceAni()
    {
        switch (splitOutput[0][1])
        {
            case "무표정":
                facesave = 0;
                break;
            case "기쁨":
                facesave = 5;
                break;
            case "화남":
            case "혐오싫음":
                facesave = 2;
                break;
            case "두려움":
                facesave = 3;
                break;
            case "슬픔":
                facesave = 4;
                break;
            case "놀람":
                facesave = 6;
                break;
            case "왼쪽윙크":
                facesave = 8;
                break;
            case "오른쪽윙크":
                facesave = 9;
                break;

        }

    }


    void wholeBody(string sendKey)
    {
        switch (splitOutput[0][1])
        {
            case "집중":
            case "기본자세":
            case "제안":
                facesave = 0; break;

            case "혐오싫음":
            case "화남":
                facesave = 2; break;

            case "두려움":
                facesave = 3; break;

            case "부정":
            case "슬픔":
                facesave = 4; break;

            case "고개숙여인사":
            case "긍정":
            case "악수":
            case "칭찬":
            case "기쁨":
            case "허그":
            case "만세":
                facesave = 5; break;

            case "(규모 등에)놀람":
            case "(소리 등에)놀람":
                facesave = 6; break;

            case "팔로인사":
                facesave = 9; break;

            case "생각":
                facesave = 10; break;

            
            case "졸림":
                facesave = 11; break;

            case "회피":
                facesave = 12; break;

            case "부끄러움":
                facesave = 13; break;


        }

        if (splitOutput[0][1] == "정지")
        {
            //facesave = 0;
            //switchFaceAni(facesave);
            StopAllCoroutines();
            face.Clear();
            isStop = true;
            StateUpdater.isCanDoGesture = true;
            REEL.PoseAnimation.RobotTransformController.isPlaying = false;
        }

        else
        {
            //motionDataFile = robot.keyMotionTable(key);
            motionDataFile = CopyFloatArray(sendKey);
        }

    }

    void MotionSpeedUp(int power)
    {
        float divideNum = 2f;

        if (power == 1)
            divideNum = 2.4f;
        else if (power == -1)
            divideNum = 1.6f;

        for (int i = 0; i < motionDataFile.Length; i++)
        {
            if (motionDataFile[i][0] / divideNum >= 0.2f)
            {
                motionDataFile[i][0] /= divideNum;
            }
            else
            {
                canADV = false;
                break;
            }
        }

    }

    void MotionSpeedDown(int power)
    {
        float divideNum = 2f;

        if (power == 1)
            divideNum = 2.4f;
        else if (power == -1)
            divideNum = 1.6f;

        for (int i = 0; i < motionDataFile.Length; i++)
        {
            motionDataFile[i][0] *= divideNum;
        }

    }

    void MotionExpansion(int[] parts, int power)
    {
        float multipleNum = 1.2f;

        if (power == 1)
            multipleNum = 1.3f;
        else if (power == -1)
            multipleNum = 1.1f;

        for (int i = 0; i < motionDataFile.Length; i++)
        {
            for (int j = 1; j < motionDataFile[i].Length; j++)
            {
                if (parts[j - 1] == 1)
                {
                    motionDataFile[i][j] = motionDataFile[i][j] * multipleNum;
                    limitMinMax(i, j);
                }

            }

        }
    }

    void MotionReduction(int[] parts, int power)
    {
        float multipleNum = 0.7f;

        if (power == 1)
            multipleNum = 0.6f;
        else if (power == -1)
            multipleNum = 0.8f;

        for (int i = 0; i < motionDataFile.Length; i++)
        {
            for (int j = 1; j < motionDataFile[i].Length; j++)
            {
                if (parts[j - 1] == 1)
                {
                    motionDataFile[i][j] = motionDataFile[i][j] * multipleNum;
                    limitMinMax(i, j);
                }


            }

        }
    }
    void limitMinMax(int i, int j)
    {
        switch (j)
        {
            case 0:
            case 3:
                Mathf.Clamp(motionDataFile[i][j], -90.0f, 90.0f);
                break;
            case 1:
                Mathf.Clamp(motionDataFile[i][j], -20.0f, 90.0f);
                break;
            case 2:
                Mathf.Clamp(motionDataFile[i][j], 0f, 90.0f);
                break;
            case 4:
                Mathf.Clamp(motionDataFile[i][j], -90.0f, 20.0f);
                break;
            case 5:
                Mathf.Clamp(motionDataFile[i][j], -90.0f, 0f);
                break;
            case 6:
                Mathf.Clamp(motionDataFile[i][j], -40.0f, 40.0f);
                break;
            case 7:
                Mathf.Clamp(motionDataFile[i][j], -30.0f, 15.0f);
                break;
        }
    }


    public void StopMotion()
    {
        if (!(coroutine == null))
        {
            StopCoroutine(coroutine);
            face.Clear();
        }

    }

    public void switchFaceAni(int i)
    {
        if(motionDataFile != robot.baseList)
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
        
    }

    float[] MovingNeck()
    {
        float[] temp = new float[9];
        temp[0] = 0.4f;
        for (int i = 1; i < 9; i++)
            temp[i] = cdJoints[i - 1].GetCurrentAngle;

        switch (splitOutput[0][1])
        {
            case "상":
                temp[8] -= 20f;
                break;
            case "하":
                temp[8] += 5f;
                Debug.Log(temp[8]);
                break;
            case "좌":
                temp[7] -= 20f;
                break;
            case "우":
                temp[7] += 20f; ;
                break;
            default:
                canMove = false;
                break;
        }

        CheckNeckAngle(ref temp[7], 6);
        CheckNeckAngle(ref temp[8], 7);
        return temp;

    }

    void CheckNeckAngle(ref float angle, int index)
    {
        if (angle >= 180f)
            angle -= 360f;
        LimitNeckAngle(index, angle);

    }

    void LimitNeckAngle(int index, float angle)
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

    void CheckNeckAngle()
    {
        for (int i = 6; i < 8; i++)
        {
            if (tempDataFile[0][i] >= 180f)
                tempDataFile[0][i] -= 360f;
            LimitNeckAngle(i, tempDataFile[0][i]);
        }

    }


    string[][] SplitKeys(string key)
    {
        if (key.Contains("/"))
        {
            if (!key.Contains("전신"))
                whole = null;
            string[] splitOutput_slash = key.Split(slash);
            string[][] result = new string[splitOutput_slash.Length][];
            for (int i = 0; i < splitOutput_slash.Length; i++)
            {
                result[i] = splitOutput_slash[i].Split(hyphen);
            }
            return result;
        }
        else
        {
            string[][] result = new string[1][];
            result[0] = key.Split(hyphen);
            return result;
        }

    }

    void basicPlay(float[][] motionDataFile)
    {
        if (motionDataFile == null)
        {
            popUpMessege.MessegePopUp("일치하는 동작이 없어요");
        }

        else
        {
            switchFaceAni(facesave);
            coroutine = StartCoroutine(robot.GestureProcess(motionDataFile));
        }

    }

    IEnumerator CountTime(float time)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return Time.deltaTime;
        }
        if (coroutine_running)
        {
            StopAllCoroutines();
            coroutine_running = false;
            repeat = false;
            
        }
        face.Clear();
        StartCoroutine(robot.SetBasePos());
        REEL.PoseAnimation.RobotTransformController.isPlaying = false;
        StateUpdater.isCanDoGesture = true;
    }

    void GetDurTime()
    {
        int index = 0;
        for (int i = 0; i < splitOutput.Length; i++)
        {
            if (splitOutput[i][0] == "DUR")
            {
                index = i;
                break;
            }
        }
        char[] dur_time = splitOutput[index][1].ToCharArray();
        string timestr = "";
        for (int i = 0; i < dur_time.Length - 1; i++)
        {
            timestr += dur_time[i];
        }
        
        switch (dur_time[dur_time.Length - 1])
        {
            case '초':
                time = float.Parse(timestr);
                break;
            case '분':
                time = float.Parse(timestr)*60f;
                break;
            //case '간':
            //    if (dur_time[dur_time.Length - 2].Equals('시'))
            //    {
            //        popUpMessege.MessegePopUp("너무 길어요");
            //        time = 0f;
            //    }
            //    break;
            default:
                popUpMessege.MessegePopUp("너무 길어요");
                time = 0f;
                motionDataFile = robot.baseList;
                break;
        }
    }

    IEnumerator RepeatMotion()
    {
        switchFaceAni(facesave);
        float playTime = 0;
        for (int i = 0; i < motionDataFile.Length; i++)
        {
            playTime += motionDataFile[i][0];
        }
        while (true)
        {
            coroutine_running = true;
            yield return StartCoroutine(robot.GestureProcess(motionDataFile));
            repeat = true;
        }

    }



}


