using System;
using UnityEngine;

[Serializable]
public class Joint
{
    public JointName jointName;
    public TargetAxis rotationAxis;
    public Transform targetTransform;

    public float direction = 1f;
    public float offset = 0f;
    public float angle { get; set; }

    public void UpdateRotation()
    {
        RotateJoint(angle);
    }

    private void RotateJoint(float angle)
    {
        Vector3 targetOrientation = Vector3.zero;

        switch (rotationAxis)
        {
            case TargetAxis.X: targetOrientation.x = angle; break;
            case TargetAxis.Y: targetOrientation.y = angle; break;
            case TargetAxis.Z: targetOrientation.z = angle; break;
            default: break;
        }

        targetTransform.localRotation = Quaternion.Euler(targetOrientation);
    }
}