using System;
using UnityEngine;
using JointIndex = KinectWrapper.NuiSkeletonPositionIndex;

public class OrientationTester : MonoBehaviour
{
    public JointIndex jointIndex;

    public Transform shoulder1;
    public Transform shoulder2;
    public Transform elbow;

    private KinectManager manager;

    private Matrix4x4 hipCenter;

    private Vector3 hipCenterX;
    private Vector3 hipCenterY;
    private Vector3 hipCenterZ;

    private Vector3 shoulderLeftX;
    private Vector3 elbowLeftX;

    private void Awake()
    {
        manager = KinectManager.Instance;
    }

    private void Update()
    {
        GetJointInfo();

        Shoulder1Rotation(hipCenterY, shoulderLeftX);
        Shoulder2Rotation(hipCenterX, shoulderLeftX);
        ElbowRotation(shoulderLeftX, elbowLeftX);
    }

    private void GetJointInfo()
    {
        hipCenter = manager.GetJointOrientationMatrix((int)JointIndex.HipCenter);//모든 조인트의 기준점

        hipCenterX = new Vector3(hipCenter.GetColumn(0).x, hipCenter.GetColumn(0).y, hipCenter.GetColumn(0).z);
        hipCenterY = new Vector3(hipCenter.GetColumn(1).x, hipCenter.GetColumn(1).y, hipCenter.GetColumn(1).z);
        hipCenterZ = new Vector3(hipCenter.GetColumn(2).x, hipCenter.GetColumn(2).y, hipCenter.GetColumn(2).z);

        shoulderLeftX = GetVectorBetween(JointIndex.ElbowLeft, JointIndex.ShoulderLeft);
        elbowLeftX = GetVectorBetween(JointIndex.WristLeft, JointIndex.ElbowLeft);
    }

    private void GetJointRot()
    {

    }

    private void Shoulder1Rotation(Vector3 vector1, Vector3 vector2)
    {
        float angle;
        angle = -(Dot(vector1, vector2) - 90f);
        Vector3 shoulderOri = Vector3.zero;
        shoulderOri.x = angle;
        shoulder1.localRotation = Quaternion.Euler(shoulderOri);
    }

    private void Shoulder2Rotation(Vector3 vector1, Vector3 vector2)
    {
        float angle;
        angle = -Dot(vector1, vector2);
        Vector3 shoulder2Ori = Vector3.zero;
        shoulder2Ori.y = angle;
        shoulder2.localRotation = Quaternion.Euler(shoulder2Ori);
    }

    private void ElbowRotation(Vector3 vector1, Vector3 vector2)
    {
        float angle;
        angle = Dot(vector1, vector2);
        Vector3 ori = Vector3.zero;
        ori.z = angle;
        elbow.localRotation = Quaternion.Euler(ori);
    }

    Vector3 GetVectorBetween(JointIndex joint1, JointIndex joint2)//순서 중요
    {
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;

        Vector3 pos1 = manager.GetJointPosition(playerID, (int)joint1);
        Vector3 pos2 = manager.GetJointPosition(playerID, (int)joint2);

        return pos2 - pos1;
    }

    private float Dot(Vector3 start, Vector3 end)
    {
        start.Normalize(); end.Normalize();
        float theta = Mathf.Acos(Vector3.Dot(start, end));

        return theta * Mathf.Rad2Deg;
    }

    //private float GetLeftElbowAngle()
    //{
    //    Vector3 vx1 = GetPositionBetween(JointIndex.ElbowLeft, JointIndex.ShoulderLeft);
    //    Vector3 vx2 = GetPositionBetween(JointIndex.WristLeft, JointIndex.ElbowLeft);

    //    return Dot(vx1, vx2);
    //}
}