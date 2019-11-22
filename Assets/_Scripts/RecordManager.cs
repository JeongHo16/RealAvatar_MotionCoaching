using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class RecordManager : MonoBehaviour
{
    public JsonSerializationManager jsonManager;
    public PopUpMessege popUpManager;

    public Button recStartButton;
    public Button recStopButton;
    public InputField inputField;
    public Dropdown dropdown;
    public Image recordImage;

    private MotionDataFile motionFileData;
    private DirectoryInfo directoryInfo;
    private FileInfo[] fileInfo;

    private WaitForSeconds delayRecordTime;
    private WaitForSeconds flickTime;

    private float fps = 5f;
    private float recordTime = 0f;

    //private string filePath = "JsonData/"; //ver. build 
    private string filePath = "Assets/JsonData/"; //ver. not build 

    private void Start()
    {
        recordTime = 1 / fps;
        directoryInfo = new DirectoryInfo(filePath);

        delayRecordTime = new WaitForSeconds(recordTime);
        flickTime = new WaitForSeconds(1.5f);

        recStopButton.interactable = false;
        recordImage.gameObject.SetActive(false);

        SetDropdownOptions();
    }

    public void SetDropdownOptions() //드롭다운 목록 초기화
    {
        dropdown.ClearOptions();
        fileInfo = directoryInfo.GetFiles("*.json");
        for (int i = 0; i < fileInfo.Length; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = fileInfo[i].Name.Substring(0, fileInfo[i].Name.Length - 5);
            dropdown.options.Add(option);
        }
        dropdown.RefreshShownValue();
    }

    public void ChangedDropdownOption() //드롭다운 선택 옵션 바뀌었을 때 
    {
        inputField.text = dropdown.options[dropdown.value].text;
    }

    public void ClickedAddButton() //파일이름 중복 체크하여 모션 데이터 추가
    {
        string fileName = filePath + inputField.text + ".json";
        if (!StateUpdater.isRecording)
        {
            if (motionFileData != null)
            {
                if (!File.Exists(fileName))
                {
                    CreateMotionJsonFile(fileName, motionFileData);
                }
                else
                {
                    popUpManager.MessegePopUp("이미 저장된 이름입니다");
                }
            }
            else
            {
                popUpManager.MessegePopUp("녹화된 파일이 없습니다");
            }
            SetDropdownOptions();
            inputField.text = string.Empty;
        }
        else
        {
            popUpManager.MessegePopUp("녹화중 입니다");
        }
    }

    public void ClickedDeleteButton() //존재하는 파일인지 체크 후 모션 데이터 삭제
    {
        string fileName = filePath + inputField.text + ".json";
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
            popUpManager.MessegePopUp("파일을 삭제합니다");
        }
        else
        {
            popUpManager.MessegePopUp("존재하지 않는 파일입니다");
        }
        SetDropdownOptions();
        inputField.text = string.Empty;
    }

    public void ClickedStartButton() //녹화시작 버튼
    {
        //if (StateUpdater.isConnectingKinect)
        {
            if (StateUpdater.isRealTimeMode)
            {
                if (inputField.text != string.Empty)
                {
                    ToggleRecordButton();
                    StateUpdater.isRecording = true;
                    StartCoroutine("Recording");

                    recordImage.gameObject.SetActive(true);
                    StartCoroutine("Flicker");

                    popUpManager.MessegePopUp("녹화를 시작합니다");
                }
                else
                {
                    popUpManager.MessegePopUp("모션파일의 이름을 정해주세요");
                }
            }
            else
            {
                popUpManager.MessegePopUp("실시간 모드를 실행 해주세요");
            }
        }
        //else
        //{
        //    popUpManager.MessegePopUp("Kinect가 연결되어 있지 않습니다.");
        //}
    }

    public void ClickedStopButton() //녹화 끝 버튼
    {
        if (StateUpdater.isRecording)
        {
            ToggleRecordButton();
            StateUpdater.isRecording = false;
            StopCoroutine("Recording");

            recordImage.gameObject.SetActive(false);
            StopCoroutine("Flicker");

            popUpManager.MessegePopUp("녹화를 종료합니다");
        }
    }

    public void ToggleRecordButton() //녹화버튼 토글
    {
        if (StateUpdater.isRecording)
        {
            recStopButton.interactable = false;
            recStartButton.interactable = true;
        }
        else
        {
            recStopButton.interactable = true;
            recStartButton.interactable = false;
        }
    }

    public void CreateMotionJsonFile(string fileName, MotionDataFile motionFileData) //모션 파일 생성
    {
        if (inputField.text != string.Empty)
        {
            string jsonString = JsonUtility.ToJson(motionFileData, true);
            File.WriteAllText(fileName, jsonString);
            motionFileData = null;//필요 없을 수 도...
            popUpManager.MessegePopUp("모션이 저장되었습니다");
        }
        else
        {
            popUpManager.MessegePopUp("모션파일의 이름을 정해주세요");
        }
    }

    private void CreateOrAddMotionData(DoubleArray motionData) //모션 파일에 들어갈 데이터 생성
    {
        if (motionFileData == null)
            motionFileData = new MotionDataFile();

        motionFileData.Add(motionData);
    }

    IEnumerator Flicker() //rec이미지 깜박이기
    {
        while (StateUpdater.isRecording)
        {
            recordImage.CrossFadeAlpha(0, 1.5f, true);
            yield return flickTime;
            recordImage.CrossFadeAlpha(1f, 1.5f, true);
            yield return flickTime;
        }
    }

    IEnumerator Recording() //녹화하기
    {
        if (motionFileData != null)
            motionFileData = null;

        while (StateUpdater.isRecording)
        {
            yield return delayRecordTime;
            jsonManager.UpdateMotionDataForSimulator(recordTime);
            CreateOrAddMotionData(jsonManager.GetMotionDataForSimulator);
        }
    }

    //private void TimerCounter(float recodeTime)
    //{
    //    elapsedTime += Time.deltaTime;
    //    if (elapsedTime >= recodeTime)
    //    {
    //        jsonManager.UpdateMotionDataForSimulator();
    //        CreateOrAddMotionData(jsonManager.GetMotionDataForSimulator);
    //        elapsedTime = 0f;
    //    }
    //}
}
