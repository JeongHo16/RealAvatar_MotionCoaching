using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoccaFaceAnniTest : MonoBehaviour
{
    public bool normal;     //기본
    public bool happy;      //행복
    public bool angry;      //화남
    public bool fear;       //두려움
    public bool sad;        //슬픔
    public bool smile;      //미소
    public bool surprised;  //놀람
    public bool speak;      //말하기

    public bool winkleft;   //윙크(좌)
    public bool winkright;  //윙크(우)

    public bool gazeup;     //쳐다보기(상)
    public bool gazedown;   //쳐다보기(하)
    public bool gazeleft;   //쳐다보기(좌)
    public bool gazeright;  //쳐다보기(우)



    Animator animator;


    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        Clear();

    }

    public void Clear()
    {
        normal = false;
        happy = false;
        angry = false;
        fear = false;
        sad = false;
        smile = false;
        surprised = false;
        speak = false;

        winkleft = false;
        winkright = false;

        gazeup = false;
        gazedown = false;
        gazeleft = false;
        gazeright = false;
    }

    public void SetAnimation(string ani)
    {
        animator.CrossFade(ani, 0.0f);
        //Clear();
    }


    // Update is called once per frame
    void Update ()
    {
        if (normal)     SetAnimation("normal");  //0
        if (happy)      SetAnimation("happy");  //1
        if (angry)      SetAnimation("angry");  //2
        if (fear)       SetAnimation("fear");  //3
        if (sad)        SetAnimation("sad");  //4
        if (smile)      SetAnimation("smile");  //5 
        if (surprised)  SetAnimation("surprised");  //6
        if (speak)      SetAnimation("speak");  //7
        if (winkleft)   SetAnimation("winkleft");  //8
        if (winkright)  SetAnimation("winkright");  //9
        if (gazeup)     SetAnimation("gazeup");  //10
        if (gazedown)   SetAnimation("gazedown");  //11
        if (gazeleft)   SetAnimation("gazeleft");  //12
        if (gazeright)  SetAnimation("gazeright");  //13

    }

}
