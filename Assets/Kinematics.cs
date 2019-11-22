using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kinematics : MonoBehaviour
{
    public Transform[] transforms;
    public Transform[] cdTransforms;
    
    float[] beforeAngles = new float[6];
    float[] afterAngles = new float[6];

    float l_d1 = -0.235f;
    float l_a2 = -0.62f;
    float l_a3 = -0.395f;
    float r_d1 = 0.235f;
    float r_a2 = 0.62f;
    float r_a3 = 0.395f;

    float end_lx, end_ly, end_lz;
    float end_rx, end_ry, end_rz;

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


    void ForwardKinematics(float[] angles)
    {
        end_ly = Mathf.Sin(angles[0]) * (l_a2 * Mathf.Sin(angles[1]) + l_a3 * Mathf.Sin(angles[1] + angles[2]));
        end_lz = -Mathf.Cos(angles[0]) * (l_a2 * Mathf.Sin(angles[1]) + l_a3 * Mathf.Sin(angles[1] + angles[2]));
        end_lx = l_a3 * Mathf.Cos(angles[1] + angles[2]) + l_a2 * Mathf.Cos(angles[1]) + l_d1;


        end_ry = Mathf.Sin(angles[3]) * (r_a2 * Mathf.Sin(angles[4]) + r_a3 * Mathf.Sin(angles[4] + angles[5]));
        end_rz = -Mathf.Cos(angles[3]) * (r_a2 * Mathf.Sin(angles[4]) + r_a3 * Mathf.Sin(angles[4] + angles[5]));
        end_rx = r_a3 * Mathf.Cos(angles[4] + angles[5]) + r_a2 * Mathf.Cos(angles[4]) + r_d1;

    }

    void InverseKinematics()
    {
        CalculateInverse(end_lx, end_ly, end_lz, false);
        CalculateInverse(end_rx, end_ry, end_rz, true);
    }

    void CalculateInverse(float end_x, float end_y, float end_z, bool right)
    {
        int sign;
        if (right)
            sign = -1;
        else
            sign = 1;
        float temp = end_x;

        end_x = end_y + 0.05f;
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
        if (float.IsNaN(cos_th_1))
            cos_th_1 = 0f;
        float test;
        if (end_y == 0)
            test = 0;
        else
            test = end_y / cos_th_1;


        float cos_th_3 = ((test * test) + ((end_z - l_d1 * sign) * (end_z - l_d1 * sign)) - ((l_a2 * sign) * (l_a2 * sign)) - ((l_a3 * sign) * (l_a3 * sign))) / (2 * (l_a2 * sign) * (l_a3 * sign));
        if (float.IsNaN(cos_th_3))
            cos_th_3 = 0f;
        //RoundUnder3(ref cos_th_3);

        float sin_th_3 = sign * Mathf.Sqrt(1 - cos_th_3 * cos_th_3);
        //RoundUnder3(ref sin_th_3);

        float sin_th_2 = (test * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) - (end_z - (l_d1 * sign)) * (l_a3 * sign) * sin_th_3) / ((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3);
        if (((test * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) - (end_z - (l_d1 * sign)) * (l_a3 * sign) * sin_th_3) == 0) || (((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3) == 0))
            sin_th_2 = 0;
        else
            sin_th_2 = (test * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) - (end_z - (l_d1 * sign)) * (l_a3 * sign) * sin_th_3) / ((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3);


        float cos_th_2 = ((end_z - (l_d1 * sign)) * ((l_a3 * sign) * cos_th_3 + (l_a2 * sign)) + test * (l_a3 * sign) * sin_th_3) / ((l_a3 * sign) * (l_a3 * sign) + (l_a2 * sign) * (l_a2 * sign) + 2 * (l_a2 * sign) * (l_a3 * sign) * cos_th_3);
        if (float.IsNaN(cos_th_2))
            cos_th_2 = 0f;
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

            //for (int i = 0; i < cdTransforms.Length - 2; i++)
            //{
            //    Vector3 vector = new Vector3();
            //    MakeVector(ref vector, afterAngles[i], i);
            //    cdTransforms[i].localRotation = Quaternion.Euler(vector);
            //}
        }

        //LimitStretchPose();



    }

    void limitAngle(ref float[] new_angles, int index)
    {
        if (new_angles[index] > 90f)
            new_angles[index] -= 180f;
        else if (new_angles[index] < -90f)
            new_angles[index] += 180f;
    }

    void LimitInverseAngle()
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
                case 6:
                    Mathf.Clamp(afterAngles[i], -40.0f, 40.0f);
                    break;
                case 7:
                    Mathf.Clamp(afterAngles[i], -30.0f, 15.0f);
                    break;

            }
        }
    }





}