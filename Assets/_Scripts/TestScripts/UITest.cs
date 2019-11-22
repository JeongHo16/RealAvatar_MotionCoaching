using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public Button startButton;
    public Button stopButton;
    public Dropdown dropdown;
    public InputField inputField;

    private string filePath = "Assets/JsonData/";

    void Start()
    {
        stopButton.gameObject.SetActive(false);
        SetDropdownOptions();
    }

    public void ClickedAddButton()
    {
        string fileName = filePath + inputField.text + ".json";
        File.WriteAllText(fileName, "testing");
        SetDropdownOptions();
    }

    public void ClickedDeleteButton()
    {
        string fileName = filePath + dropdown.options[dropdown.value].text + ".json";
        File.Delete(fileName);
        SetDropdownOptions();
    }

    public void selectOption()
    {
        Debug.Log(dropdown.options[dropdown.value].text);
    }

    private void SetDropdownOptions()
    {
        DirectoryInfo di = new DirectoryInfo("Assets/JsonData/");
        FileInfo[] fi = di.GetFiles("*.json");

        dropdown.ClearOptions();
        for (int i = 0; i < fi.Length; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = fi[i].Name.Substring(0, fi[i].Name.Length - 5);
            dropdown.options.Add(option);
        }
    }

    public void ToggleRecordButton()
    {
        if (!stopButton.gameObject.activeSelf) //startButton 누를때
        {
            stopButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
        else // stopButton 누를때
        {
            startButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
        }
    }

    public void ClickedStartButton()
    {
        Debug.Log("ClickedStartButton");
    }

    public void ClickedStopButton()
    {
        Debug.Log("ClickedStopButton");
    }

    //private int GetMotionDataCount() //모션데이터 개수 반환
    //{
    //    DirectoryInfo di = new DirectoryInfo("Assets/JsonData/");
    //    FileInfo[] fi = di.GetFiles("*.json");

    //    if (fi.Length == 0) return fi.Length;
    //    else return fi.Length;
    //}
}