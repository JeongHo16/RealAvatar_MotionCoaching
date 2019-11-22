using UnityEngine;

public class AngleMessenger : MonoBehaviour
{
    [SerializeField]
    private CDJointOrientationSetter cdJointOrientationSetter;

    [SerializeField]
    private JointOrientationSetter jointOrientationSetter;

    private CDJoint[] cdJoints;
    private Joint[] joints;

    private void Awake()
    {
        cdJoints = cdJointOrientationSetter.joints;
        joints = jointOrientationSetter.joints;
    }

    private void Update()
    {
        SendAngle();
    }

    void SendAngle()
    {
        SendAngleToNeck();
        SendAngleToRightArm();
        SendAngleToLeftArm();
    }

    void SendAngleToLeftArm()
    {
        if (CollisionManager.leftArmMove) //얘는 시뮬레이터 왼팔임. 사람의 오른팔 
        {
            for (int i = 0; i < 3; i++)
            {
                joints[i].angle = cdJoints[i].GetCurrentAngle;
            }
        }
    }

    void SendAngleToRightArm()
    {
        if (CollisionManager.rightArmMove) //얘는 시뮬레이터 오른팔임. 사람의 왼팔 
        {
            for (int i = 3; i < 6; i++)
            {
                joints[i].angle = cdJoints[i].GetCurrentAngle;
            }
        }
    }

    void SendAngleToNeck()
    {
        if (CollisionManager.neckMove)
        {
            for (int i = 6; i < 8; i++)
            {
                joints[i].angle = cdJoints[i].GetCurrentAngle;
            }
        }
    }
}