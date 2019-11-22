using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckMoveTest : MonoBehaviour
{
    public Transform head;

    private FacetrackingManager facetrackingManager;

    //private Vector3 HeadInitialPosition;
    private Quaternion HeadInitialRotation;

    void Start()
    {
        if (head != null)
        {
            //HeadInitialPosition = head.localPosition;
            //HeadInitialPosition.z = 0;
            HeadInitialRotation = head.localRotation;
        }
    }

    void Update()
    {
        if (facetrackingManager == null)
        {
            facetrackingManager = FacetrackingManager.Instance;
        }

        if (facetrackingManager && facetrackingManager.IsTrackingFace())
        {
            // set head position & rotation
            if (head != null)
            {
                //Vector3 newPosition = HeadInitialPosition + manager.GetHeadPosition();
                //HeadTransform.localPosition = Vector3.Lerp(HeadTransform.localPosition, newPosition, 3 * Time.deltaTime);

                //Quaternion newRotation = HeadInitialRotation * facetrackingManager.GetHeadRotation();
                //head.localRotation = Quaternion.Slerp(head.localRotation, newRotation, 3 * Time.deltaTime);
                //head.localRotation = HeadInitialRotation * facetrackingManager.GetHeadRotation();
                Vector3 targetOrientation = Vector3.zero;
                targetOrientation.x = facetrackingManager.GetHeadRotation().eulerAngles.x;
                targetOrientation.y = facetrackingManager.GetHeadRotation().eulerAngles.y;

                head.localRotation = Quaternion.Euler(targetOrientation);

            }
        }
    }
}
