using UnityEngine;

public class AngleMessenger : MonoBehaviour
{
    public PopUpMessege popUp;
    public Transform[] moccaTransforms;
    public MotionCoaching motionCoaching;
    public REEL.PoseAnimation.RobotTransformController robot;
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
        //if (StateUpdater.isCallingADV)
        //{
        //    if (!CollisionManager.neckMove || !CollisionManager.leftArmMove || !CollisionManager.rightArmMove)
        //    {
        //        popUp.MessegePopUp("충돌이 일어나서 더 이상 움직일 수 없어요");
        //        motionCoaching.StopMotion();
        //        //for (int i = 0; i < joints.Length; i++)
        //        //    cdJoints[i].RotateJoint(0);
        //        StartCoroutine(robot.SetBasePos());
        //    }
                
        //    else
        //        SendAngle();
        //}
        //else
        //    SendAngle();

        if (!CollisionManager.neckMove || !CollisionManager.leftArmMove || !CollisionManager.rightArmMove)
        {
            popUp.MessegePopUp("충돌이 일어나서 더 이상 움직일 수 없어요");
            motionCoaching.StopMotion();
            if (StateUpdater.isCallingADV || MotionCoaching.degBase)
            {
                StartCoroutine(robot.SetBasePos());
                MotionCoaching.degBase = false;
                
            }

        }
        else
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