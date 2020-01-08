using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

public class SpeechRecognition : Singleton<SpeechRecognition>
{
    private GCSpeechRecognition _speechRecognition;
    public Sprite[] micImages;
    public Image mic;
    public InputField inputField;
    public Text Speechinput;
    public static bool receive = false;
    public static AI_MESSAGE output;

    public void OnButtonRecognize()
    {
        if (!_isRecognizing)
        {
            StartRecord();
        }
        else
        {
            StopRecord();
        }
        _isRecognizing = !_isRecognizing;
    }



    void StartRecord()
    {
        Speechinput.text = null;
        Debug.Log("[SpeechRec] StartRecord");
        _result = "";
        _speechRecognition.StartRecord(false);
        mic.sprite = micImages[1];
    }

    void StopRecord()
    {
        Debug.Log("[SpeechRec] StopRecord");
        _speechRecognition.StopRecord();
        mic.sprite = micImages[0];
    }

    private bool _isRecognizing = false;
    public bool IsRecognizing
    {
        get
        {
            return _isRecognizing;
        }
    }

    private string _result = "";
    public string Result
    {
        get
        {
            return _result;
        }
    }

    public void Clear()
    {
        _result = "";
    }

    public void OnRecognized(string words)
    {

    }

    // Use this for initialization
    void Start()
    {
        // Google Cloud Speech Recognition
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.SetLanguage(Enumerators.LanguageCode.ko_KR);
        _speechRecognition.RecognitionSuccessEvent += RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent += SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent += LongRecognitionSuccessEventHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (_result.Length > 0)
        {
            YesNoClient.Instance.Open();
            AI_MESSAGE request = new AI_MESSAGE();
            request.method = "movements";
            request.input = _result;
            string jsonString = JsonUtility.ToJson(request, prettyPrint: false);
            Debug.Log("[Coaching]" + jsonString);
            YesNoClient.Instance.Write(jsonString);
            Speechinput.text = _result;
            _result = "";
        }
        else
        {
            string coaching_result = YesNoClient.Instance.Read();
            if (coaching_result.Length > 0)
            {
                receive = true;
                YesNoClient.Instance.Close();
                //Debug.Log("[Player] Got YesNo result " + result);
                AI_MESSAGE response = JsonUtility.FromJson<AI_MESSAGE>(coaching_result);
                output = response;
                Debug.Log("[Coaching] Result: " + response.result);
                if (REEL.PoseAnimation.RobotTransformController.isPlaying)
                {
                    if (output.result.Contains("전신-정지"))
                        StateUpdater.isCanDoGesture = true;
                    else
                        StateUpdater.isCanDoGesture = false;
                }
                    
                else
                    StateUpdater.isCanDoGesture = true;
                
            }
            
        }

        if (StateUpdater.isInputEntered)
        {
            Speechinput.text = null;
            YesNoClient.Instance.Open();
            AI_MESSAGE request = new AI_MESSAGE();
            request.method = "movements";
            request.input = inputField.text;
            string jsonString = JsonUtility.ToJson(request, prettyPrint: false);
            Debug.Log("[Coaching]" + jsonString);
            YesNoClient.Instance.Write(jsonString);

            inputField.text = "";

            StateUpdater.isInputEntered = false;
        }
    }

    public void InputFiledEnteredButton()
    {
        StateUpdater.isInputEntered = true;
    }

    private void OnDestroy()
    {
        _speechRecognition.RecognitionSuccessEvent -= RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent -= SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent -= LongRecognitionSuccessEventHandler;
    }

    private void SpeechRecognizedFailedEventHandler(string obj, long requestIndex)
    {
        Debug.Log("SpeechRecognizedFailedEventHandler: " + obj);
        StateUpdater.isSpeakingAgain = true;
    }

    private void RecognitionSuccessEventHandler(RecognitionResponse obj, long requestIndex)
    {
        Debug.Log("RecognitionSuccessEventHandler: " + obj);
        if (obj != null && obj.results.Length > 0)
        {
            Debug.Log("Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript);
            _result = obj.results[0].alternatives[0].transcript;

            //SendSTT2Server(obj.results[0].alternatives[0].transcript);
        }
        else
        {
            Debug.Log("Speech Recognition succeeded! Words are no detected.");
        }
    }

    private void LongRecognitionSuccessEventHandler(OperationResponse operation, long index)
    {
        Debug.Log("LongRecognitionSuccessEventHandler: " + operation);
        if (operation != null && operation.response.results.Length > 0)
        {
            Debug.Log("Long Speech Recognition succeeded! Detected Most useful: " + operation.response.results[0].alternatives[0].transcript);

            string other = "\nDetected alternative: ";

            foreach (var result in operation.response.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (operation.response.results[0].alternatives[0] != alternative)
                        other += alternative.transcript + ", ";
                }
            }

            Debug.Log(other);
        }
        else
        {
            Debug.Log("Speech Recognition succeeded! Words are no detected.");
        }
    }
}
