using System.IO;
using UnityEngine;


public class JsonSerializationManager : MonoBehaviour
{
    public JointOrientationSetter jointSetter;

    public DoubleArray GetMotionDataForSimulator { get { return motionDataForSimulator; } }
    public DoubleArray GetMotionDataForRobot { get { return motionDataForRobot; } }

    private DoubleArray motionDataForSimulator;
    private DoubleArray motionDataForRobot;

    private double realTimeFrameDuration = 0.2;

    public void UpdateMotionDataForSimulator(float recordTime)//파일 생성 전 현재 조인트 값을 DoubleArray에 저장. 시간은 recordTime과 같아야한다.
    {
        motionDataForSimulator = new DoubleArray();
        motionDataForSimulator.Add(MathUtil.Roundoff(recordTime));
        foreach (Joint joint in jointSetter.joints)
        {
            motionDataForSimulator.Add(MathUtil.Roundoff(MathUtil.ConvertAngle(joint.angle))); //컨버팅 후 저장
            motionDataForSimulator.SetSize();
        }
    }

    public void UpdateMotionDataForRobot() //실시간으로 실물로봇에 각도값 매핑하여 보내기. 이때 시간은 0.2
    {
        motionDataForRobot = new DoubleArray();
        motionDataForRobot.Add(realTimeFrameDuration);

        for (int i = 0; i < 3; i++) // 왼팔(거울)
        {
            float angle = MathUtil.ConvertAngle(jointSetter.joints[i].angle);
            motionDataForRobot.Add(MathUtil.Roundoff(angle));
        }

        for (int i = 3; i < 6; i++) // 오른팔(거울)
        {
            float angle = MathUtil.ConvertAngle(jointSetter.joints[i].angle);
            if (i == 3)
                motionDataForRobot.Add(-MathUtil.Roundoff(angle));
            else
                motionDataForRobot.Add(MathUtil.Roundoff(angle));
        }

        for (int i = 6; i < 8; i++) // 실물 모카 목
        {
            float angle = MathUtil.ConvertAngle(jointSetter.joints[i].angle);
            if (i == 7) //tilt 회전 방향이 반대. 30프로 더 회전.
                motionDataForRobot.Add(-MathUtil.Roundoff(angle) * 1.3);
            else
                motionDataForRobot.Add(-MathUtil.Roundoff(angle));
        }

        //for (int i = 3; i < 6; i++) // 실물 모카 왼팔 (시뮬레이터 오른팔)
        //{
        //    float angle = MathUtil.ConvertAngle(jointSetter.joints[i].angle);
        //    if (i == 3)
        //        motionDataForRobot.Add(MathUtil.Roundoff(angle));
        //    else
        //        motionDataForRobot.Add(-MathUtil.Roundoff(angle));
        //}
        //for (int i = 0; i < 3; i++) // 실물 모카 오른팔 (시뮬레이터 왼팔)
        //{
        //    float angle = MathUtil.ConvertAngle(jointSetter.joints[i].angle);
        //    motionDataForRobot.Add(-MathUtil.Roundoff(angle));
        //}
        //for (int i = 6; i < 8; i++) // 실물 모카 목
        //{
        //    float angle = MathUtil.ConvertAngle(jointSetter.joints[i].angle);
        //    if (i == 7) //tilt 회전 방향이 반대. 30프로 더 회전.
        //        motionDataForRobot.Add(-MathUtil.Roundoff(angle) * 1.3);
        //    else
        //        motionDataForRobot.Add(MathUtil.Roundoff(angle));
        //}

        motionDataForRobot.SetSize();
    }

    public string GetJsonStringMotionDataForRobot()
    {
        string jsonString;

        //JSON 문자열 얻기.
        jsonString = JsonUtility.ToJson(motionDataForRobot);

        return jsonString;
    }
}