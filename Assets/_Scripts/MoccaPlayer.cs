using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MoccaPlayer : MonoBehaviour
{
    public CDJointOrientationSetter cdJointSetter;
    public JsonSerializationManager jsonManager;
    public MotionCustomizer motionCustomizer;
    public RecordManager recordManager;
    public PopUpMessege popUpManager;

    public SSH ssh;
    public Toggle realTimeModeToggle;
    public InputField inputField;

    private MotionDataFile motionFileForSimulator;
    private MotionDataFile motionFileForRobot;
    private MotionDataFile zeroPos;
    private CDJoint[] cdJoints;

    //private string filePath = "JsonData/"; //ver. build 
    private string filePath = "Assets/JsonData/"; //ver. not build 

    private float fps = 5f;
    private float targetFrameTime = 0f;
    private float elapsedTime = 0f;

   

    void Start()
    {
        targetFrameTime = 1f / fps;
        cdJoints = cdJointSetter.joints;
        CreateZeroPosData();
    }

    void Update()
    {
        if (StateUpdater.isRealTimeMode || StateUpdater.isMotionPlayingRobot)
            SendAngleRealTimeToRobot(); // 저장된 파일 or 실시간으로 실물로봇으로 보낼때 
    }

    private void CreateZeroPosData()
    {
        zeroPos = new MotionDataFile();
        DoubleArray zero = new DoubleArray();
        zero.Add(0.5);
        for (int i = 0; i < 8; i++)
        {
            zero.Add(0.0);
        }
        zero.SetSize();
        zeroPos.Add(zero);
    }

    public void CustomizedMotionFileAdd() //저장된 모션 데이터(O), 방금 녹화된 모션 데이터(X)
    {
        string fileName = filePath + inputField.text + ".json";
        if (!StateUpdater.isRealTimeMode)
        {
            if (motionFileForSimulator != null)
            {
                if (!File.Exists(fileName))
                {
                    recordManager.CreateMotionJsonFile(fileName, motionFileForSimulator);
                }
                else
                {
                    popUpManager.MessegePopUp("이미 저장된 이름입니다");
                }
            }
            else
            {
                popUpManager.MessegePopUp("먼저 저장된 모션파일을 실행해 주세요");
            }
            recordManager.SetDropdownOptions();
            inputField.text = string.Empty;
        }
        else
        {
            popUpManager.MessegePopUp("실시간 모드가 진행 중 입니다");
        }
    }

    private void SendAngleRealTimeToRobot()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= targetFrameTime)
        {
            SendMotionDataWithSSH();
            elapsedTime = 0f;
        }
    }

    private void SendMotionDataWithSSH()
    {
        jsonManager.UpdateMotionDataForRobot();
        Send("mot:raw(" + jsonManager.GetJsonStringMotionDataForRobot() + ")\n"); //실시간으로 실물에 보낼때 포맷
    }

    private void Send(string rawMotion)
    {
        byte[] data = new byte[1024];
        data = Encoding.UTF8.GetBytes(rawMotion);
        ssh.udpClient.Send(data, data.Length);
    }

    public void PlayMotionFileForSimulator() //시뮬레이터에서 저장된 모션 실행하기 
    {
        if (!StateUpdater.isRealTimeMode)
        {
            if (!StateUpdater.isMotionPlayingSimulator)
            {
                if (inputField.text != string.Empty)
                {
                    LoadMotionFileForSimulator();
                    motionCustomizer.CustomizeMotionData(motionCustomizer.speedSlider.value, motionCustomizer.angleSlider.value, motionFileForSimulator);
                    StartCoroutine(PalyMotionFile(motionFileForSimulator));
                }
                else
                {
                    popUpManager.MessegePopUp("실행할 모션파일을 선택해 주세요");
                }
            }
            else
            {
                popUpManager.MessegePopUp("현재 모션이 실행 중 입니다");
            }
        }
        else
        {
            popUpManager.MessegePopUp("실시간 모드가 진행 중 입니다");
        }
    }

    IEnumerator PalyMotionFile(MotionDataFile motionFileData) //저장된 모션 보간하기.
    {
        StateUpdater.isMotionPlayingSimulator = true;
        WaitForSeconds lerfTime = new WaitForSeconds((float)motionFileData[0][0]);

        if (motionFileForRobot != null) // 파라메타(motionFileData) 로 검사 안하는 이유 : 파라메타가 시뮬레이터 일수도 로봇일수도 있기때문.
            StateUpdater.isMotionPlayingRobot = true;

        for (int i = 0; i < motionFileData.Length; i++)
        {
            float rotDuration = (float)motionFileData[i][0];
            for (int j = 0; j < cdJoints.Length; j++)
            {
                StartCoroutine(cdJoints[j].SetQuatLerp((float)motionFileData[i][j + 1], rotDuration));
            }

            yield return lerfTime;
        }

        yield return StartCoroutine(SetZeroPos());

        StateUpdater.isMotionPlayingSimulator = false;

        if (StateUpdater.isMotionPlayingRobot == true)
            StateUpdater.isMotionPlayingRobot = false;
    }

    IEnumerator SetZeroPos() //보간하며 기본자세 취함.
    {
        float rotDuration = GetrotDuration();
        WaitForSeconds rotDurationSec = new WaitForSeconds(rotDuration + 0.5f); //실물 로봇이 완벽하게 T 자세를 취하기 전에 멈춰버렸기 때문에.

        for (int i = 0; i < cdJoints.Length; i++)
        {
            StartCoroutine(cdJoints[i].SetQuatLerp(0f, rotDuration));
        }

        yield return rotDurationSec;
    }

    private float GetrotDuration()
    {
        float maxDegree = GetMaxDegreeToZeroPos();
        float rotDuration = maxDegree / (360f * 0.8f);

        return rotDuration;
    }

    private float GetMaxDegreeToZeroPos() //현재 자세에서 zeropos까지 가장 큰 각을 구함.
    {
        float maxDegree = 0f;
        for (int i = 0; i < cdJoints.Length; i++)
        {
            float degree = MathUtil.ConvertAngle(cdJoints[i].GetCurrentAngle);
            maxDegree = Mathf.Max(maxDegree, degree);
        }
        return maxDegree;
    }

    public void PlayMotionFileForRobot() //로봇에서 저장된 모션 실행하기 
    {
        if (!StateUpdater.isRealTimeMode)
        {
            if (!StateUpdater.isMotionPlayingSimulator)
            {
                if (inputField.text != string.Empty)
                {
                    LoadMotionFileForRobot();
                    StartCoroutine(PalyMotionFile(motionFileForRobot));
                    motionFileForRobot = null;
                }
                else
                {
                    popUpManager.MessegePopUp("실행할 모션파일을 선택해 주세요");
                }
            }
            else
            {
                popUpManager.MessegePopUp("현재 모션이 실행 중 입니다");
            }
        }
        else if (StateUpdater.isRealTimeMode)
        {
            popUpManager.MessegePopUp("실시간 모드가 진행 중 입니다");
        }
    }

    //private void ChangeAngleForRobot(MotionDataFile motionFileData) //모션파일 각도값 실물 로봇으로 전송전 로봇에 맞게 매핑
    //{
    //    double tempAngle;
    //    for (int i = 0; i < motionFileData.Length; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            tempAngle = motionFileData[i][j + 1]; //시뮬레이터 왼팔 각도들을 변수에 저장.

    //            if (j == 0)
    //                motionFileData[i][j + 1] = motionFileData[i][j + 4]; //시뮬레이터 오른팔 각도들을 왼팔에 저장.
    //            else
    //                motionFileData[i][j + 1] = -motionFileData[i][j + 4];

    //            motionFileData[i][j + 4] = -tempAngle; //변수에 있는 왼팔 각도들을 오른팔에 저장.
    //        }

    //        motionFileData[i][8] = MathUtil.Roundoff((float)(-motionFileData[i][8] * 1.3)); //tilt 회전 방향이 반대. 30프로 더 회전. //소수점 길게 늘어져서 잘라줌.
    //    }
    //}

    //private IEnumerator SendMotionFileDataWithSSH(MotionDataFile motionFileData) //모션 파일 데이터 실물 로봇으로 전송
    //{
    //    StateUpdater.isMotionPlayingRobot = true;
    //    WaitForSeconds SSHTime = new WaitForSeconds((float)motionFileData[0][0]);

    //    for (int i = 0; i < motionFileData.Length; i++)
    //    {
    //        Send("mot:raw(" + JsonUtility.ToJson(motionFileData[i]) + ")\n");
    //        yield return SSHTime;
    //    }

    //    Send("mot:raw(" + JsonUtility.ToJson(zeroPos[0]) + ")\n");
    //    yield return SSHTime;

    //    StateUpdater.isMotionPlayingRobot = false;
    //}

    private void LoadMotionFileForSimulator()
    {
        string fileName = filePath + inputField.text + ".json";
        string jsonString = File.ReadAllText(fileName);
        motionFileForSimulator = JsonUtility.FromJson<MotionDataFile>(jsonString);
    }

    private void LoadMotionFileForRobot()
    {
        string fileName = filePath + inputField.text + ".json";
        string jsonString = File.ReadAllText(fileName);
        motionFileForRobot = JsonUtility.FromJson<MotionDataFile>(jsonString);
    }

    public void RealTimeModeToggle() //실시간 모드 토글
    {
        if (!StateUpdater.isRecording)
        {
            if (realTimeModeToggle.isOn)
                StateUpdater.isRealTimeMode = true;
            else
                StateUpdater.isRealTimeMode = false;
        }
    }
}