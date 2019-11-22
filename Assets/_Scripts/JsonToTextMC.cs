using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class JsonToTextMC : MonoBehaviour
{

    private readonly string inputPath = "Assets/MotionData";
    private readonly string outputPath = "Assets/MotionDataForMC";
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
        string outputFilePath = Path.Combine(outputPath, fileName + ".txt");
        string output = "float[][] " + fileName + " = {\n";
        MotionDataFile motionData = JsonUtility.FromJson<MotionDataFile>(json);
        for (int ix = 0; ix < motionData.Length; ++ix)
        {
            for (int jx = 0; jx < motionData[ix].Length; ++jx)
            {
                if (jx == 0)
                {
                    output += "    new float[9] { ";
                }
                if (jx == motionData[ix].Length - 1)
                {
                    output += ((float)motionData[ix][jx]).ToString() + "f },\n";
                    continue;
                }
                output += ((float)motionData[ix][jx]).ToString() + "f, ";
            }
            if (ix == motionData.Length - 1)
            {
                output += " };";
            }
        }
        Debug.Log($"Filepath: {outputFilePath} / output: {output}");
        File.WriteAllText(outputFilePath, output);
    }
}

