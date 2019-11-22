using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonToMotionArray : MonoBehaviour
{
    private readonly string inputPath = "Assets/JsonData";
    private readonly string outputPath = "Assets/MotionData";

    // Start is called before the first frame update
    void Start()
    {
        string[] paths = Directory.GetFiles(inputPath);
        foreach (var file in paths)
        {
            if (file.Contains("meta"))
            {
                continue;
            }

            string jsonData = File.ReadAllText(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            ParseToMotion(jsonData, fileName);
        }
    }

    private void ParseToMotion(string json, string fileName)
    {
        string outputFilePath = Path.Combine(outputPath, fileName + ".json");
        string output = "{" + "\r\n" + "\"DoubleArrays\":[ ";
        MotionDataFile motionData = JsonUtility.FromJson<MotionDataFile>(json);
        for (int ix = 0; ix < motionData.Length; ++ix)
        {
            for (int jx = 0; jx < motionData[ix].Length; ++jx)
            {
                if (jx == 0)
                {
                    output += "\r\n" + "{\r\n" + "\"size\": 0, " + "\r\n" + "\"array\": [" + "\r\n";
                }

                if (ix != (motionData.Length - 1))
                {
                    if (jx == motionData[ix].Length - 1)
                    {
                        output += (-1*(Mathf.Round((float)motionData[ix][jx] * 10) / 10)).ToString() + "]" + "\r\n" + "}," + "\r\n";
                        continue;
                    }

                }

                else if (ix == (motionData.Length - 1))
                {
                    if (jx == motionData[ix].Length - 1)
                    {
                        output += (-1*(Mathf.Round((float)motionData[ix][jx] * 10) / 10)).ToString() + "]" + "\r\n" + "}" + "\r\n";
                        continue;
                    }
                }
                

                switch (jx)
                {
                    case 0:
                    case 1:
                        output += (Mathf.Round((float)motionData[ix][jx] * 10) / 10).ToString() + ","+"\r\n";
                        break;
                    
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        output += (((-1*(Mathf.Round((float)motionData[ix][jx] * 10) / 10)))).ToString() + "," + "\r\n";
                        break;

                }

            }

            if (ix == motionData.Length - 1)
            {
                output += "]" + "\r\n" +" }";
            }
        }

        Debug.Log($"Filepath: {outputFilePath} / output: {output}");
        File.WriteAllText(outputFilePath, output);
    }
}
