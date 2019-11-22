using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kinematics : MonoBehaviour
{
   
    public Transform[] cdTransforms;

    float[] beforeAngles = new float[8];
    float[] afterAngles = new float[8];

    float l_d1 = -0.235f;
    float l_a2 = -0.62f;
    float l_a3 = -0.395f;
    float r_d1 = 0.235f;
    float r_a2 = 0.62f;
    float r_a3 = 0.395f;

    float end_lx, end_ly, end_lz;
    float end_rx, end_ry, end_rz;

    string moccaPart;
    string direction;


    public void MakeVector(ref Vector3 targetEuler, double jointAngle, int index)
    {
        switch (index)
        {
            case 0:
            case 3:
            case 7:
                targetEuler.x = (float)jointAngle;

                break;
            case 1:
            case 2:
            case 4:
            case 5:
            case 6:
                targetEuler.y = (float)jointAngle;
                break;

        }
    }

    public void ForwardKinematics()
    {
        for (int i = 0; i < cdTransforms.Length; i++)
            GetAngle(cdTransforms[i], i);
        end_ly = Mathf.Sin(beforeAngles[0]) * (l_a2 * Mathf.Sin(beforeAngles[1]) + l_a3 * Mathf.Sin(beforeAngles[1] + beforeAngles[2]));
        end_lz = -Mathf.Cos(beforeAngles[0]) * (l_a2 * Mathf.Sin(beforeAngles[1]) + l_a3 * Mathf.Sin(beforeAngles[1] + beforeAngles[2]));
        end_lx = l_a3 * Mathf.Cos(beforeAngles[1] + beforeAngles[2]) + l_a2 * Mathf.Cos(beforeAngles[1]) + l_d1;

        end_ry = Mathf.Sin(beforeAngles[3]) * (r_a2 * Mathf.Sin(beforeAngles[4]) + r_a3 * Mathf.Sin(beforeAngles[4] + beforeAngles[5]));
        end_rz = -Mathf.Cos(beforeAngles[3]) * (r_a2 * Mathf.Sin(beforeAngles[4]) + r_a3 * Mathf.Sin(beforeAngles[4] + beforeAngles[5]));
        end_rx = r_a3 * Mathf.Cos(beforeAngles[4] + beforeAngles[5]) + r_a2 * Mathf.Cos(beforeAngles[4]) + r_d1;

    }

    public void InverseKinematics(string part, string dir)
    {
        moccaPart = part;
        direction = dir;
        if (part == "왼팔")
            CalculateInverse(end_lx, end_ly, end_lz, false);
        else if (part == "오른팔")
            CalculateInverse(end_rx, end_ry, end_rz, true);
        else
        {
            CalculateInverse(end_lx, end_ly, end_lz, false);
            CalculateInverse(end_rx, end_ry, end_rz, true);
        }

        //float[][] sendAngles = new float[1][];
        //sendAngles[0][0] = 0.2f;
        //for (int i =0; i< afterAngles.Length; i++)
        //{
        //    sendAngles[0][i+1] = afterAngles[i];
        //}
        //return sendAngles;
    }

    public void CalculateInverse(float end_x, float end_y, float end_z, bool right)
    {
        LimitStretchPose();
        Debug.Log("1");
        if (StateUpdater.isCanInverse)
        {
            int sign;
            if (right)
                sign = -1;
            else
                sign = 1;


            if (direction == "상")
                end_y += 0.2f;
            else if (direction == "하")
                end_y -= 0.2f;
            else if (direction == "좌")
                end_x -= 0.2f;
            else
                end_x += 0.2f;

            float temp = end_x;
            end_x = end_y;
            end_y = end_z;
            end_z = temp;


            float sin_th_1;
            if (end_x == 0)
                sin_th_1 = 0;
            else
                sin_th_1 = end_x / Mathf.Sqrt(end_x * end_x + end_y * end_y);

            float cos_th_1;
            if (end_y == 0)
                cos_th_1 = 0;
            else
                cos_th_1 = -end_y / Mathf.Sqrt(end_x * end_x + end_y * end_y);

            float test;
            if (end_y == 0)
                test = 0;
            else
                test = end_y / cos_th_1;

            float cos_th_3 = ((test * test) + ((end_z - l_d1 * sign) * (end_z - l_d1 * sign)) - ((l_a2 * sign) * (l_a2 * sign)) - ((l_a3 * sign) * (l_a3 * sign))) / (2 * (l_a2 * sign) * (l_a3 * sign));

            float sin_th_3 = sign * Mathf.Sqrt(1 - cos_th_3 * cos_th_3);

            float sin_th_2 = (test * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) - (end_z - (l_d1 * sign)) * (l_a3 * sign) * sin_th_3) / ((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3);
            if (((test * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) - (end_z - (l_d1 * sign)) * (l_a3 * sign) * sin_th_3) == 0) || (((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3) == 0))
                sin_th_2 = 0;
            else
                sin_th_2 = (test * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) - (end_z - (l_d1 * sign)) * (l_a3 * sign) * sin_th_3) / ((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3);

            float cos_th_2 = ((end_z - (l_d1 * sign)) * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) + test * (l_a3 * sign) * sin_th_3) / ((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3);

            CheckNaN();

            if (cos_th_1 == 0 || cos_th_2 == 0 || cos_th_3 == 0)
                StateUpdater.isCanInverse = false;

            if (StateUpdater.isCanInverse)
            {
                if (!right)
                {

                    afterAngles[0] = Mathf.Atan2(sin_th_1, cos_th_1) * Mathf.Rad2Deg;
                    afterAngles[1] = Mathf.Atan2(sin_th_2, cos_th_2) * Mathf.Rad2Deg;
                    afterAngles[2] = Mathf.Atan2(sin_th_3, cos_th_3) * Mathf.Rad2Deg;
                }
                else
                {

                    afterAngles[3] = Mathf.Atan2(sin_th_1, cos_th_1) * Mathf.Rad2Deg;
                    afterAngles[4] = Mathf.Atan2(sin_th_2, cos_th_2) * Mathf.Rad2Deg;
                    afterAngles[5] = Mathf.Atan2(sin_th_3, cos_th_3) * Mathf.Rad2Deg;

                }

                for (int i = 0; i < afterAngles.Length; i++)
                {
                    if (float.IsNaN(afterAngles[i]))
                        afterAngles[i] = 0;
                }

                limitAngle(ref afterAngles, 0);
                limitAngle(ref afterAngles, 3);
                LimitInverseAngle();

                
                LimitStretchPose();
                Debug.Log("2");

                if (StateUpdater.isCanInverse)
                {
                    for (int i = 0; i < cdTransforms.Length; i++)
                        StartCoroutine(MovingInverse(cdTransforms[i], i));
                }
            }
        }
    }

    IEnumerator MovingInverse(Transform transforms, int index)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        Quaternion startQuat = transforms.localRotation;

        Vector3 vec = Vector3.zero;
        MakeVector(ref vec, afterAngles[index], index);
        Quaternion targetQuat = Quaternion.Euler(vec);

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            float normalTime = elapsedTime / duration;
            normalTime = float.IsInfinity(normalTime) ? 0f : normalTime;
            Quaternion currentQuat = Quaternion.Lerp(startQuat, targetQuat, elapsedTime / duration);
            transforms.localRotation = currentQuat;

            yield return null;

        }
    }
        public void limitAngle(ref float[] new_angles, int index)
    {
        if (new_angles[index] > 90f)
            new_angles[index] -= 180f;
        else if (new_angles[index] < -90f)
            new_angles[index] += 180f;
    }


    public void LimitInverseAngle()
    {
        for (int i = 0; i < afterAngles.Length; i++)
        {
            switch (i)
            {
                case 0:
                case 3:
                    Mathf.Clamp(afterAngles[i], -90.0f, 90.0f);
                    break;
                case 1:
                    Mathf.Clamp(afterAngles[i], -20.0f, 90.0f);
                    break;
                case 2:
                    Mathf.Clamp(afterAngles[i], 0f, 90.0f);
                    break;
                case 4:
                    Mathf.Clamp(afterAngles[i], -90.0f, 20.0f);
                    break;
                case 5:
                    Mathf.Clamp(afterAngles[i], -90.0f, 0f);
                    break;

            }
        }
    }

    public void CheckNaN()
    {
        for (int i = 0; i < afterAngles.Length; i++)
        {
            if (float.IsNaN(afterAngles[i]))
                afterAngles[i] = 0;
        }
    }

    public void LimitStretchPose()
    {
        float val0 = 0;
        float val1 = 0;
        float val2 = 0;

        if (moccaPart != "팔양팔두팔양쪽팔")
        {
            if (moccaPart == "오른팔")
            {
                val0 = afterAngles[3];
                val1 = afterAngles[4];
                val2 = afterAngles[5];
                Debug.Log(val0+"  "+val1+"  "+val2);
 

            }

            else
            {
                val0 = afterAngles[0];
                val1 = afterAngles[1];
                val2 = afterAngles[2];
            }

            if (((Mathf.Abs(val0) < 5.0f && Mathf.Abs(val1) < 5.0f) || (Mathf.Abs(val1) < 5.0f && Mathf.Abs(val2) < 5.0f)))
            {
                StateUpdater.isCanInverse = false;
            }
                
            else
                StateUpdater.isCanInverse = true;
        }

        else
        {
            
            if (((Mathf.Abs(afterAngles[3]) < 5.0f && Mathf.Abs(afterAngles[4]) < 5.0f) || (Mathf.Abs(afterAngles[4]) < 5.0f && Mathf.Abs(afterAngles[5]) < 5.0f))
                || (Mathf.Abs(afterAngles[0]) < 5.0f && Mathf.Abs(afterAngles[1]) < 5.0f) || (Mathf.Abs(afterAngles[1]) < 5.0f && Mathf.Abs(afterAngles[2]) < 5.0f))
                StateUpdater.isCanInverse = false;
            else
                StateUpdater.isCanInverse = true;
        }

    }


    public void GetAngle(Transform transform, int index)
    {
        float angle = 0.0f;
        switch (index)
        {
            case 0:
            case 3:
            case 7:
                angle = transform.localRotation.eulerAngles.x;
                break;
            case 1:
            case 2:
            case 4:
            case 5:
            case 6:
                angle = transform.localRotation.eulerAngles.y;
                break;
        }

        if (angle > 180.0f)
            angle -= 360.0f;

        beforeAngles[index] = angle * Mathf.Deg2Rad;
        afterAngles[index] = angle;
    }

}