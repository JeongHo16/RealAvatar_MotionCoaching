using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public GameObject[] neck; //목 오브젝트 배열 //구조의 통일성과 CheckAllPartsCollision 함수를 위해 배열로 선언(필요한 오브젝트는 1개)
    public GameObject[] rightArm; //오른팔 오브젝트 배열                       //InitParts, UpdatePartsCollision 함수는 조금 아쉽쓰.
    public GameObject[] leftArm; //왼팔 오브젝트 배열

    public bool[] neckCollision; //오브젝트들의 충돌여부 boolean 배열
    public bool[] rightArmCollision;
    public bool[] leftArmCollision;

    public static bool neckMove = true; // 목 동작 boolean
    public static bool rightArmMove = true; // 오른팔 동작 boolean
    public static bool leftArmMove = true; // 왼팔 동작 boolean

    void Start()
    {
        InitParts();
    }

    void Update()
    {
        UpdatePartsCollision();
        CheckAllPartsCollision();
    }

    private void InitParts() //양팔 오브젝트에 MOOCAPart.cs 컴포넌트 추가, boolean 배열 초기화.
    {
        neckCollision = new bool[neck.Length];
        rightArmCollision = new bool[rightArm.Length];
        leftArmCollision = new bool[leftArm.Length];

        for (int i = 0; i < neck.Length; i++)
        {
            neck[i].AddComponent<MOCCAPart>();
        }
        for (int i = 0; i < rightArm.Length; i++)
        {
            rightArm[i].AddComponent<MOCCAPart>();
            leftArm[i].AddComponent<MOCCAPart>();
        }
    }

    private void UpdatePartsCollision() //부품의 충돌 여부를 갱신
    {
        for (int i = 0; i < neck.Length; i++)
        {
            neckCollision[i] = neck[i].GetComponent<MOCCAPart>().collision;

        }
        for (int i = 0; i < rightArmCollision.Length; i++)
        {
            rightArmCollision[i] = rightArm[i].GetComponent<MOCCAPart>().collision;
            leftArmCollision[i] = leftArm[i].GetComponent<MOCCAPart>().collision;

        }
    }

    private void CheckAllPartsCollision() //충돌 여부에 따라 파트 전체를 컨트롤하는 boolean값 toggle
    {
        CheckPartCollision(ref neckCollision, ref neckMove); //목
        CheckPartCollision(ref rightArmCollision, ref rightArmMove); //오른팔
        CheckPartCollision(ref leftArmCollision, ref leftArmMove); //왼팔
    }

    private void CheckPartCollision(ref bool[] collision, ref bool partMove)
    {
        for (int i = 0; i < collision.Length; i++)
        {
            if (collision[i])
            {
                partMove = false;
                break;
            }
            else if (!collision[i])
            {
                partMove = true;
            }
        }
    }
}
