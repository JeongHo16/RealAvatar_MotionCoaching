using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
//using REEL.D2E;
//using REEL.D2EEditor;
using System.Collections.Generic;

public class SpeechRenderrer : Singleton<SpeechRenderrer>
{
    public enum SpeakerType
    {
        jinho, mijin, matt, clara
    }

    public bool useLipsync = true;

    public bool testTTS = false;
    public string testText = "long long 반갑습니다";

    public string[] ttsTestList = { "안녕하세요? 만나서 반갑습니다. 저는 모카입니다.",
                                    "오늘 날씨 참 좋네요",
                                    "가나다라마바사아자차카타파하",
                                    "아에이오우",
                                    "오빤강남스타일"};


    private AudioSource audioSource;
    private WaitForSeconds waitOneSec = null;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        Init();
    }


	public static string LastTTS = "";

    int testCount = 0;

    public void TestTTS()
    {
        testCount = testCount % ttsTestList.Length;
        Play(ttsTestList[testCount]);

        testCount++;
    }

    void Start()
    {
    }

    void Update()
    {
        if (testTTS == true)
        {
            testTTS = false;
            Play(testText);

        }
    }

    public void Init()
    {
        waitOneSec = new WaitForSeconds(1f);
    }

    private bool IsSpeaking
    {
        get
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            return audioSource.isPlaying;
        }
    }

    private bool IsFinished
    {
        get
        {
            return !IsSpeaking;
        }
    }

    public void Play(string speech)
    {
        if (speech == "_STOP_")
        {
            StopCoroutine(Say(speech));
            Stop();
        }
        else
        {
            LastTTS = speech;

            StartCoroutine(Say(speech));
            //if (LogWindow.Instance != null)
            //{
            //    LogWindow.Instance.PrintLog("MOCCA", speech);
            //}
        }
    }

    void OnDisable()
    {
        Stop();
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public bool IsRunning()
    {
        return IsSpeaking;
    }

    public static bool Validator(object sender, X509Certificate certificate, X509Chain chain,
                                  SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }


	public static AudioClip GetAudioClipFromMP3ByteArray(byte[] in_aMP3Data)
	{
		AudioClip l_oAudioClip = null;
		Stream l_oByteStream = new MemoryStream(in_aMP3Data);
		MP3Sharp.MP3Stream l_oMP3Stream = new MP3Sharp.MP3Stream(l_oByteStream);

		try
		{
			//Get the converted stream data
			MemoryStream l_oConvertedAudioData = new MemoryStream();
			byte[] l_aBuffer = new byte[2048];
			int l_nBytesReturned = -1;
			int l_nTotalBytesReturned = 0;

			while (l_nBytesReturned != 0)
			{
				l_nBytesReturned = l_oMP3Stream.Read(l_aBuffer, 0, l_aBuffer.Length);
				l_oConvertedAudioData.Write(l_aBuffer, 0, l_nBytesReturned);
				l_nTotalBytesReturned += l_nBytesReturned;
			}

			//Debug.Log("MP3 file has " + l_oMP3Stream.ChannelCount + " channels with a frequency of " + l_oMP3Stream.Frequency);

			byte[] l_aConvertedAudioData = l_oConvertedAudioData.ToArray();
			Debug.Log("Converted Data has " + l_aConvertedAudioData.Length + " bytes of data");

			//Convert the byte converted byte data into float form in the range of 0.0-1.0
			float[] l_aFloatArray = new float[l_aConvertedAudioData.Length / 2];

			for (int i = 0; i < l_aFloatArray.Length; i++)
			{
				if (BitConverter.IsLittleEndian)
				{
					//Evaluate earlier when pulling from server and/or local filesystem - not needed here
					//Array.Reverse( l_aConvertedAudioData, i * 2, 2 );
				}

				//Yikes, remember that it is SIGNED Int16, not unsigned (spent a bit of time before realizing I screwed this up...)
				l_aFloatArray[i] = (float)(BitConverter.ToInt16(l_aConvertedAudioData, i * 2) / 32768.0f);
			}

			//For some reason the MP3 header is readin as single channel despite it containing 2 channels of data (investigate later)
			if (l_oMP3Stream.ChannelCount == 1)
				l_oAudioClip = AudioClip.Create("MySound", l_aFloatArray.Length / 2, 2, l_oMP3Stream.Frequency, false);
			else
				l_oAudioClip = AudioClip.Create("MySound", l_aFloatArray.Length, 2, l_oMP3Stream.Frequency, false);
			l_oAudioClip.SetData(l_aFloatArray, 0);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		return l_oAudioClip;
	}


	IEnumerator Say(string speech, SpeakerType speaker = SpeakerType.mijin)
    {
        Debug.Log("[SpeechRenderrer::Say]" + speech);
        //#if UNITY_ANDROID
        //        string uriSpeech = Application.persistentDataPath + "/tts.mp3";
        //#else
        //        string uriSpeech = Application.dataPath + "/tts.mp3";
        //#endif
        //        File.Delete(uriSpeech);

        //ServicePointManager.ServerCertificateValidationCallback = Validator;

        string url = "https://naveropenapi.apigw.ntruss.com/voice/v1/tts";
        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "4lk8cmcq67");
        //request.Headers.Add("X-NCP-APIGW-API-KEY", "Dnv1bksb2Trwh7DIbahih3QxFR9FOtAEdN1fPZz2");
        //request.Method = "POST";
        //byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=jinho&speed=0&text=" + speech);
        //request.ContentType = "application/x-www-form-urlencoded";
        //request.ContentLength = byteDataParams.Length;
        //Stream st = request.GetRequestStream();
        //st.Write(byteDataParams, 0, byteDataParams.Length);
        //st.Close();
        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //string status = response.StatusCode.ToString();
        ////Console.WriteLine("status=" + status);
        //using (Stream output = File.OpenWrite(uriSpeech))
        //using (Stream input = response.GetResponseStream())
        //{
        //    input.CopyTo(output);
        //}

        //WWW mp3Open = new WWW(uriSpeech);
        //while (mp3Open.isDone)
        //{
        //    yield return null;
        //}

        //byte[] mp3bytes = File.ReadAllBytes(uriSpeech);
        //audioSource.clip = Utils.GetAudioClipFromMP3ByteArray(mp3bytes);
        //audioSource.Play();

        //StopCoroutine("Feedback");
        //StartCoroutine("Feedback");

        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("X-NCP-APIGW-API-KEY-ID", "4lk8cmcq67");
        headers.Add("X-NCP-APIGW-API-KEY", "Dnv1bksb2Trwh7DIbahih3QxFR9FOtAEdN1fPZz2");
        //form.AddField("speaker", "jinho");
        form.AddField("speaker", speaker.ToString());
        form.AddField("speed", "0");
        form.AddField("text", speech);

        byte[] rawData = form.data;
        using (WWW ttsRequest = new WWW(url, rawData, headers))
        {
            yield return ttsRequest;

            if (ttsRequest.error != null)
            {
                Debug.Log(ttsRequest.error);
            }

            audioSource.clip = GetAudioClipFromMP3ByteArray(ttsRequest.bytes);
            audioSource.Play();

            StopCoroutine("Feedback");
            StartCoroutine("Feedback");
        }

        yield return null;
    }

    IEnumerator Feedback()
    {
        while (audioSource.isPlaying)
        {
            yield return waitOneSec;

            if (Mqtt.Instance != null)
            {
                Mqtt.Instance.Send(Utils.TopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK, "playing");

            }
        }
        if (Mqtt.Instance != null)
        {
            Mqtt.Instance.Send(Utils.TopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK, "done");
        }
    }
}