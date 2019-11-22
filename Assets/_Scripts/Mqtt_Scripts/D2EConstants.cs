using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D2EConstants
{
    public static readonly string TOPIC_TTS = "/tts";
    public static readonly string TOPIC_TTS_FEEDBACK = "/tts/feedback";
    public static readonly string TOPIC_MOTION = "/motion";
    public static readonly string TOPIC_MOTION_FEEDBACK = "/motion/feedback";
    public static readonly string TOPIC_MOBILITY = "/mobility";
    public static readonly string TOPIC_FACIAL = "/facial";
    public static readonly string TOPIC_FACIAL_FEEDBACK = "/facial/feedback";
    public static readonly string TOPIC_STATUS = "/status";

    public static readonly string currentUserIDKey = "LoginID";
    public static readonly string currentProjectKey = "CurrentProject";
    public static string BaseProjectPath
    {
        get { return Application.dataPath + "/Data/"; }
    }
}

[System.Serializable]
public class LoginResult
{
    public bool success;
    public int code;
    public string message;
    public string id;

    public override string ToString()
    {
        return success.ToString() + " : " + code.ToString() + " : " + message + " : " + id;
    }
}

[System.Serializable]
public class LogoutResult
{
    public bool success;
    public int code;
    public string message;

    public override string ToString()
    {
        return success.ToString() + " : " + code.ToString() + " : " + message;
    }
}